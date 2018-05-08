// <copyright file="Program.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace Ditto.DataCheck
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using Autofac;
    using Ditto.DomainEvents;
    using Ditto.Core;
    using Ditto.DataCheck.Properties;

    /// <summary>
    /// Main program entry class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The help requested console return code.
        /// </summary>
        public const int HelpRequested = 1;

        /// <summary>
        /// The success console return code.
        /// </summary>
        public const int Success = 0;

        /// <summary>
        /// The check errors console return code.
        /// </summary>
        public const int CheckErrors = 2;

        /// <summary>
        /// The program exception console return code.
        /// </summary>
        public const int ProgramException = 3;

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        public static IContainer Container { get; private set; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public static Options Options { get; private set; }

        /// <summary>
        /// Parses the arguments.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// A return code for the program.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If <c>args</c> is null.</exception>
        public static int ParseArguments(TextWriter writer, params string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            Program.Options = Options.Parse(args);

            if (args.Length == 0 || Program.Options.ShowHelp)
            {
                Options.WriteHelp(writer, WindowWidth());
                return Program.HelpRequested;
            }

            return Program.Success;
        }

        /// <summary>
        /// Main program entry point.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A code to indicate success or failure.</returns>
        internal static int Main(string[] args)
        {
            int exitCode = ParseArguments(Console.Out, args);

            if (exitCode == Program.HelpRequested)
            {
                return exitCode;
            }

            var originalConsoleColor = Console.ForegroundColor;

            AppDomain.CurrentDomain.UnhandledException += (s, a) =>
            {
                // This will nearly always be an exception instance but in the few cases
                // it isn't we won't know what to do with it. The best we can do is log
                // that some sort of exception occured.
                var exception = a.ExceptionObject as Exception;
                LogException(exception ?? new AggregateException());
            };

            using (Program.Container = RegisterDependencies())
            {
                Program.Container
                    .Resolve<ExceptionLog>()
                    .ReserveSpace();

                Program.Container
                    .Resolve<EventListener>();

                Program.Container
                    .Resolve<EventTelemetry>();

                try
                {
                    Options.Log();

                    Console.WriteLine(Resources.BatchWelcomeMessage);
                    DittoEventSource.Log.ApplicationStart();

                    var errorCount = Run(Options.CheckScript, Options.Environment, Options.TargetConnectionString);

                    if (errorCount > 0)
                    {
                        Console.WriteLine(Resources.CheckFailuresMessageFormat, errorCount);
                        exitCode = Program.CheckErrors;
                    }

                    DittoEventSource.Log.ApplicationComplete();
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    exitCode = Program.ProgramException;
                }
            }

            Console.ForegroundColor = originalConsoleColor;

            return exitCode;
        }

        /// <summary>
        /// Windows the width.
        /// </summary>
        /// <returns>The current console window width or a default if there is no console.</returns>
        private static int WindowWidth()
        {
            try
            {
                return Console.WindowWidth;
            }
            catch (IOException)
            {
                return 80;
            }
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <remarks>
        /// This logs to three different places for three separate reasons:
        /// 1. The console so the exception gets into Control-M log
        /// 2. A file so later support procedures
        /// 3. The trace for diagnostics.
        /// </remarks>
        private static void LogException(Exception exception)
        {
            var aggregateException = exception as AggregateException;

            if (aggregateException != null)
            {
                foreach (var inner in aggregateException.Flatten().InnerExceptions)
                {
                    LogException(inner);
                }
            }

            Program.Container
                .Resolve<ExceptionLog>()
                .LogException(exception);

            Program.Container
                .Resolve<EventTelemetry>()
                .LogException(exception);

            DittoEventSource.ExceptionRaised(exception);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.Message);
            Console.ResetColor();
        }

        /// <summary>
        /// Loads the script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="targetConnectionString">The target connection string.</param>
        /// <returns>
        /// A data script to execute.
        /// </returns>
        private static CheckScript LoadScript(string script, string environment, string targetConnectionString)
        {
            DittoEventSource.Log.ParsingScriptFile(script, environment);

            using (var config = new StreamReader(script))
            {
                var parser = new CheckScriptParser(environment, targetConnectionString);
                return parser.Parse(config);
            }
        }

        /// <summary>
        /// Registers the dependencies.
        /// </summary>
        /// <returns>A DI container.</returns>
        private static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder
                .Register<ExceptionLog>(c => new ExceptionLog(Options.ErrorFile))
                .SingleInstance();

            builder
                .Register<EventListener>(c => new EventListener(Options.LogFile, Settings.Default.LogLevel))
                .SingleInstance();

            var globalProperties = new Dictionary<string, string>
            {
                { "ExecutionId", Guid.NewGuid().ToString() },
                { "DataScript", Options.CheckScript },
                { "Environment", Options.Environment }
            };

            builder
                .Register(c => new EventTelemetry(
                    Options.TargetConnectionString, 
                    Settings.Default.TelemetryTable,
                    globalProperties,
                    ex => Program.LogException(ex)))
                .SingleInstance();

            builder
                .RegisterType<ConsoleHandler>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<TelemetryHandler>()
                .AsImplementedInterfaces();

            return builder.Build();
        }

        /// <summary>
        /// Runs the specified container.
        /// </summary>
        /// <param name="dataScript">The data script.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="targetConnectionString">The target connection string.</param>
        /// <returns>Returns the number of check errors.</returns>
        private static int Run(string dataScript, string environment, string targetConnectionString)
        {
            EventPublisher.Container = Program.Container;

            using (var scope = Program.Container.BeginLifetimeScope())
            {
                using (var cts = new CancellationTokenSource())
                {
                    Console.CancelKeyPress += (s, e) =>
                    {
                        Console.WriteLine(Resources.CancellationRequestedMessage);
                        cts.Cancel();
                    };

                    CheckScript script = LoadScript(dataScript, environment, targetConnectionString);

                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    
                    script.Run(cts.Token);
                    
                    timer.Stop();
                    Console.WriteLine(Resources.TotalExecutionTimeMessageFormat, timer.Elapsed);

                    return script.CheckErrorCount;
                }
            }
        }
    }
}
