#region Copyright (c) all rights reserved.
// <copyright file="Options.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.IO;
    using Args;
    using Args.Help;
    using Args.Help.Formatters;
    using Ditto.Core;
    using Ditto.DataCheck.Properties;

    /// <summary>
    /// Command line options class.
    /// </summary>
    [CLSCompliant(false)]
    [ArgsModel(SwitchDelimiter = "-")]
    public class Options
    {
        /// <summary>
        /// The target connection string.
        /// </summary>
        private readonly SqlConnectionStringBuilder targetConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        public Options()
        {
            this.Environment = "Local";
            this.LogFile = Settings.Default.LogFile;
            this.ErrorFile = Settings.Default.ErrorFile;
            this.targetConnectionString = new SqlConnectionStringBuilder(Settings.Default.TargetConnectionString);
        }

        /// <summary>
        /// Gets or sets the check script.
        /// </summary>
        /// <value>
        /// The check script.
        /// </value>
        [Description("Check script")]
        [ArgsMemberSwitch("c", "cs", "CheckScript", "Script")]
        public string CheckScript { get; set; }

        /// <summary>
        /// Gets or sets the database server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        [Description("Target database server")]
        [ArgsMemberSwitch("S", "Server")]
        public string Server 
        {
            get { return this.targetConnectionString.DataSource; }
            set { this.targetConnectionString.DataSource = value; }
        }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        [Description("Target database name")]
        [ArgsMemberSwitch("db", "Database")]
        public string Database
        {
            get { return this.targetConnectionString.InitialCatalog; }
            set { this.targetConnectionString.InitialCatalog = value; }
        }

        /// <summary>
        /// Gets or sets the log file.
        /// </summary>
        /// <value>
        /// The log file.
        /// </value>
        [Description("Log file")]
        [ArgsMemberSwitch("l", "Log", "LogFile")]
        public string LogFile { get; set; }

        /// <summary>
        /// Gets or sets the error file.
        /// </summary>
        /// <value>
        /// The error file.
        /// </value>
        [Description("Error file")]
        [ArgsMemberSwitch("err", "ErrorFile")]
        public string ErrorFile { get; set; }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        [Description("The execution environment")]
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show help].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show help]; otherwise, <c>false</c>.
        /// </value>
        [ArgsMemberSwitch("?", "h", "hh", "help")]
        [Description("Shows this help text")]
        public bool ShowHelp { get; set; }

        /// <summary>
        /// Gets the target connection string.
        /// </summary>
        /// <value>
        /// The target connection string.
        /// </value>
        public string TargetConnectionString
        {
            get { return this.targetConnectionString.ConnectionString; }
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>A new Options instance.</returns>
        public static Options Parse(string[] args)
        {
            var argumentConfiguration = Configuration.Configure<Options>();
            return argumentConfiguration.CreateAndBind(args);
        }

        /// <summary>
        /// Writes the help.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="windowWidth">Width of the window.</param>
        public static void WriteHelp(TextWriter writer, int windowWidth)
        {
            var argumentConfiguration = Configuration.Configure<Options>();

            var help = new HelpProvider().GenerateModelHelp(argumentConfiguration);
            var helpFormatter = new ConsoleHelpFormatter(windowWidth, 1, 5);

            helpFormatter.WriteHelp(help, writer);
        }

        /// <summary>
        /// Logs this instance.
        /// </summary>
        public void Log()
        {
            DittoEventSource.Log.ProgramArgument("CheckScript", this.CheckScript);
            DittoEventSource.Log.ProgramArgument("Environment", this.Environment);
            DittoEventSource.Log.ProgramArgument("Server", this.Server);
            DittoEventSource.Log.ProgramArgument("Database", this.Database);

            DittoEventSource.Log.ProgramArgument("ErrorFile", this.ErrorFile);
            DittoEventSource.Log.ProgramArgument("LogFile", this.LogFile);
            DittoEventSource.Log.ProgramArgument("TargetConnectionString", this.TargetConnectionString);
        }
    }
}
