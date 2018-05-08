// <copyright file="EventListener.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace Ditto.Core
{
    using System;
    using System.Diagnostics.Tracing;
    using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
    using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Sinks;

    /// <summary>
    /// Event Listener.
    /// </summary>
    public class EventListener : IDisposable
    {
        /// <summary>
        /// The trace listener.
        /// </summary>
        private readonly ObservableEventListener traceListener;

        /// <summary>
        /// The console listener.
        /// </summary>
        private readonly ObservableEventListener consoleListener;

        /// <summary>
        /// The trace subscription.
        /// </summary>
        private readonly SinkSubscription<RollingFlatFileSink> traceSubscription;

        /// <summary>
        /// The console subscription.
        /// </summary>
        private readonly SinkSubscription<ConsoleSink> consoleSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListener" /> class.
        /// </summary>
        /// <param name="traceLogFile">The trace log file.</param>
        /// <param name="traceLogLevel">The trace log level.</param>
        public EventListener(string traceLogFile, EventLevel traceLogLevel)
        {
            this.traceListener = new ObservableEventListener();
            this.traceListener.EnableEvents(DittoEventSource.Log, traceLogLevel, Keywords.All);

            this.traceSubscription = this.traceListener.LogToRollingFlatFile(
                traceLogFile,
                100 * 1024, // KB
                "yyyyMMdd",
                RollFileExistsBehavior.Increment,
                RollInterval.Day);

            this.consoleListener = new ObservableEventListener();
            this.consoleListener.EnableEvents(DittoEventSource.Log, EventLevel.Error, Keywords.All);

            this.consoleSubscription = this.consoleListener.LogToConsole();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.consoleSubscription.Dispose();
                this.consoleListener.Dispose();

                this.traceSubscription.Dispose();
                this.traceListener.Dispose();
            }
        }
    }
}
