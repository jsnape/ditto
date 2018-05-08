#region Copyright (c) all rights reserved.
// <copyright file="ConsoleHandler.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Ditto.DomainEvents;
    using Ditto.DataCheck.DomainEvents;
    using Ditto.DataCheck.Properties;

    /// <summary>
    /// Console Event Handler.
    /// </summary>
    public class ConsoleHandler :
        IHandles<UnknownCheckEvent>,
        IHandles<ExpandingElementEvent>,
        IHandles<CheckStartedEvent>,
        IHandles<CheckFailedEvent>,
        IHandles<CheckPassedEvent>
    {
        /// <summary>
        /// The console lock.
        /// </summary>
        private object consoleLock = new object();

        /// <summary>
        /// Are we running under team city.
        /// </summary>
        private bool runningUnderTeamCity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleHandler"/> class.
        /// </summary>
        public ConsoleHandler()
        {
            var teamCityVersion = Environment.GetEnvironmentVariable("TEAMCITY_VERSION");
            this.runningUnderTeamCity = !string.IsNullOrEmpty(teamCityVersion);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(UnknownCheckEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            lock (this.consoleLock)
            {
                Console.WriteLine(Resources.UnknownCheckEventMessage, args.CheckName);
            }
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(ExpandingElementEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            lock (this.consoleLock)
            {
                using (Color.Quiet)
                {
                    Console.WriteLine(Resources.ExpandingElementEventMessage, args.EntityName, args.ColumnName, args.Match, args.Expansion);
                }
            }
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(CheckStartedEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (this.runningUnderTeamCity)
            {
                Console.WriteLine(
                    Resources.TeamCityTestStartedMessageFormat,
                    TeamcityEscape(args.Name),
                    Task.CurrentId ?? 0);
            }
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(CheckPassedEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (this.runningUnderTeamCity)
            {
                Console.WriteLine(
                    Resources.TeamCityTestFinishedMessageFormat,
                    TeamcityEscape(args.Name),
                    args.Duration.Milliseconds,
                    Task.CurrentId ?? 0);
            }
            else
            {
                lock (this.consoleLock)
                {
                    using (Color.Success)
                    {
                        Console.WriteLine(
                            Resources.ConsoleCheckFinishedMessageFormat, 
                            args.Name, 
                            "passed", 
                            args.Duration,
                            "Info");
                    }
                }
            }
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(CheckFailedEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (this.runningUnderTeamCity)
            {
                if (args.Severity == CheckSeverity.Error)
                {
                    Console.WriteLine(
                        Resources.TeamCityTestFailedMessageFormat,
                        TeamcityEscape(args.CheckType),
                        TeamcityEscape(args.Name),
                        TeamcityEscape(args.Message),
                        TeamcityEscape(args.Details),
                        args.Goal,
                        args.Value,
                        args.Duration.Milliseconds,
                        Task.CurrentId ?? 0);
                }

                Console.WriteLine(
                    Resources.TeamCityTestFinishedMessageFormat,
                    args.Name,
                    args.Duration.Milliseconds,
                    Task.CurrentId ?? 0);
            }
            else
            {
                lock (this.consoleLock)
                {
                    using (GetColor(args.Severity))
                    {
                        Console.WriteLine(
                            Resources.ConsoleCheckFinishedMessageFormat, 
                            args.Name, 
                            "failed", 
                            args.Duration, 
                            args.Severity);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <param name="checkSeverity">The check severity.</param>
        /// <returns>A color resource.</returns>
        private static IDisposable GetColor(CheckSeverity checkSeverity)
        {
            if (checkSeverity == CheckSeverity.Error)
            {
                return Color.Failed;
            }
            else
            {
                return Color.Warning;
            }
        }

        /// <summary>
        /// Escapes a string ready to pass to team city.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>An escaped string ready for team city.</returns>
        private static string TeamcityEscape(string value)
        {
            return value
                .Replace("\r", "|r")
                .Replace("\n", "|n")
                .Replace("'", "|'")
                .Replace("|", "||")
                .Replace("[", "|[")
                .Replace("]", "|]")
                .Replace(new string((char)0x0085, 1), "|x")
                .Replace(new string((char)0x2028, 1), "|l")
                .Replace(new string((char)0x2029, 1), "|p");
        }

        /// <summary>
        /// Console Color Resource.
        /// </summary>
        private class Color : IDisposable
        {
            /// <summary>
            /// The original foreground color.
            /// </summary>
            private ConsoleColor originalForeground;

            /// <summary>
            /// Initializes a new instance of the <see cref="Color"/> class.
            /// </summary>
            /// <param name="foreground">The foreground.</param>
            public Color(ConsoleColor foreground)
            {
                this.originalForeground = Console.ForegroundColor;

                // Change the console color
                Console.ForegroundColor = foreground;
            }

            /// <summary>
            /// Gets the error color.
            /// </summary>
            /// <value>
            /// The error color.
            /// </value>
            public static Color Failed 
            {
                get { return new Color(ConsoleColor.Red); }
            }

            /// <summary>
            /// Gets the success color.
            /// </summary>
            /// <value>
            /// The success color.
            /// </value>
            public static Color Success 
            {
                get { return new Color(ConsoleColor.Green); } 
            }

            /// <summary>
            /// Gets the quiet color.
            /// </summary>
            /// <value>
            /// The quiet color.
            /// </value>
            public static Color Quiet
            {
                get { return new Color(ConsoleColor.DarkGray); }
            }

            /// <summary>
            /// Gets the warning color.
            /// </summary>
            /// <value>
            /// The warning color.
            /// </value>
            public static Color Warning 
            {
                get { return new Color(ConsoleColor.Yellow); } 
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Console.ForegroundColor = this.originalForeground;
            }
        }
    }
}
