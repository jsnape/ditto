#region Copyright (c) all rights reserved.
// <copyright file="DittoEventSource.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Tracing;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Ditto Event Source.
    /// </summary>
    [EventSource(Name = "Snape-Ditto")]
    public sealed partial class DittoEventSource : EventSource
    {
        /// <summary>
        /// The log instance.
        /// </summary>
        private static readonly Lazy<DittoEventSource> Instance = new Lazy<DittoEventSource>(() => new DittoEventSource());

        /// <summary>
        /// Prevents a default instance of the <see cref="DittoEventSource"/> class from being created.
        /// </summary>
        private DittoEventSource()
        {
        }

        /// <summary>
        /// Gets the log instance.
        /// </summary>
        /// <value>
        /// The log instance.
        /// </value>
        public static DittoEventSource Log
        {
            get { return DittoEventSource.Instance.Value; }
        }
        
        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public static void ExceptionRaised(Exception ex)
        {
            if (ex is AggregateException aggregateException)
            {
                foreach (var inner in aggregateException.Flatten().InnerExceptions)
                {
                    ExceptionRaised(inner);
                }
            }
            else
            {
                if (ex != null)
                {
                    var exceptionData = new StringBuilder();

                    foreach (DictionaryEntry entry in ex.Data)
                    {
                        exceptionData.AppendFormat(CultureInfo.CurrentCulture, "{0}={1}|", entry.Key, entry.Value);
                    }

                    Log.ExceptionRaised(ex.Message, ex.HelpLink, exceptionData.ToString());
                }
                else
                {
                    Log.UnknownExceptionRaised();
                }
            }
        }

        /// <summary>
        /// Application startup log event.
        /// </summary>
        [Event(1, Message = "Application starting", Keywords = Keywords.Performance)]
        public void ApplicationStart()
        {
            this.WriteEvent(1);
        }

        /// <summary>
        /// Application complete log event.
        /// </summary>
        [Event(2, Message = "Application complete", Keywords = Keywords.Performance)]
        public void ApplicationComplete()
        {
            this.WriteEvent(2);
        }

        /// <summary>
        /// Exception thrown log event.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="helpLink">The help link.</param>
        /// <param name="data">The exception data.</param>
        [Event(3, Message = "Exception: {0} help='{1}' [{2}]", Keywords = Keywords.Diagnostic, Level = EventLevel.Error)]
        public void ExceptionRaised(string message, string helpLink, string data)
        {
            this.WriteEvent(3, message, helpLink, data);
        }

        /// <summary>
        /// Log a database query.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="query">The query.</param>
        /// <param name="operationId">The operation identifier.</param>
        [Event(10, Message = "Database {0} query {1} for operation {2}", Keywords = Keywords.Database)]
        public void DatabaseQuery(string database, string query, Guid operationId)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(10, database, "\n" + query + "\n", operationId);
            }
        }

        /// <summary>
        /// Exception thrown log event.
        /// </summary>
        [Event(10001, Message = "Unknown exception", Keywords = Keywords.Diagnostic, Level = EventLevel.Error)]
        public void UnknownExceptionRaised()
        {
            this.WriteEvent(10001);
        }

        /// <summary>
        /// Parsing the script file.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="environment">The environment.</param>
        [Event(10002, Message = "Parsing script file '{0}' for '{1}'", Keywords = Keywords.Diagnostic, Level = EventLevel.Informational)]
        public void ParsingScriptFile(string script, string environment)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(10002, script, environment);
            }
        }

        /// <summary>
        /// Bulk copy progress.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="rows">The row count copied.</param>
        [Event(10003, Message = "'{0}': '{1}' rows copied", Keywords = Keywords.Diagnostic, Level = EventLevel.Verbose)]
        public void BulkCopyProgress(string target, long rows)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(10003, target, rows);
            }
        }

        /// <summary>
        /// Logs a program argument.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <param name="value">The argument value.</param>
        [Event(10004, Message = "Arg '{0}': '{1}'", Keywords = Keywords.Diagnostic, Level = EventLevel.Informational)]
        public void ProgramArgument(string name, string value)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(10004, name, value);
            }
        }

        /// <summary>
        /// Logs the fact that the special target connection was generated.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        [Event(10005, Message = "Target connection generated to '{0}'", Keywords = Keywords.Diagnostic, Level = EventLevel.Informational)]
        public void TargetConnectionGenerated(string connectionString)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(10005, connectionString);
            }
        }

        /// <summary>
        /// Logs the fact that a file is being read.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        [Event(10006, Message = "Loading file '{0}'", Keywords = Keywords.Source, Level = EventLevel.Informational)]
        public void LoadingFile(string fileName)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(10006, fileName);
            }
        }

        /// <summary>
        /// Keywords constants.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Standard event source pattern [JS]")]
        public static class Keywords
        {
            /// <summary>
            /// The database keyword.
            /// </summary>
            public const EventKeywords Database = (EventKeywords)1;

            /// <summary>
            /// The diagnostic keyword.
            /// </summary>
            public const EventKeywords Diagnostic = (EventKeywords)2;

            /// <summary>
            /// The performance keyword.
            /// </summary>
            public const EventKeywords Performance = (EventKeywords)4;

            /// <summary>
            /// The source keyword.
            /// </summary>
            public const EventKeywords Source = (EventKeywords)8;
        }

        /// <summary>
        /// Tasks constants.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Standard event source pattern [JS]")]
        public static class Tasks
        {
            /// <summary>
            /// An extract task.
            /// </summary>
            public const EventTask Extract = (EventTask)1;

            /// <summary>
            /// A load task.
            /// </summary>
            public const EventTask Load = (EventTask)2;

            /// <summary>
            /// A database query task.
            /// </summary>
            public const EventTask DataQuery = (EventTask)4;
        }
    }
}
