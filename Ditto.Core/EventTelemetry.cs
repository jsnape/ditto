// <copyright file="EventTelemetry.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace Ditto.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Ditto.Telemetry;

    /// <summary>
    /// Event Telemetry.
    /// </summary>
    public class EventTelemetry : IDisposable
    {
        /// <summary>
        /// The telemetry client.
        /// </summary>
        private readonly TelemetryClient telemetry;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly TelemetryConfiguration configuration;

        /// <summary>
        /// The channel.
        /// </summary>
        private readonly ITelemetryChannel channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTelemetry" /> class.
        /// </summary>
        /// <param name="telemetryConnectionString">The telemetry connection string.</param>
        /// <param name="telemetryTable">The telemetry table.</param>
        /// <param name="globalProperties">The global properties.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "JS: Ownership is transferred to the TelemetryConfiguration")]
        public EventTelemetry(
            string telemetryConnectionString, 
            string telemetryTable, 
            IDictionary<string, string> globalProperties, 
            Action<Exception> exceptionHandler)
        {
            try
            {
                this.channel = new SqlTelemetryChannel(telemetryConnectionString, telemetryTable, exceptionHandler);

                this.configuration = new TelemetryConfiguration();

                this.configuration
                    .InitializeWithDefaults()
                    .InitializeWith(new GlobalPropertyInitializer(globalProperties))
                    .SetInstrumentationKey("Ditto.DataLoad")
                    .SetChannel(this.channel);

                this.channel = null;

                this.telemetry = new TelemetryClient(this.configuration);
            }
            finally
            {
                var disposableChannel = this.channel as IDisposable;

                if (disposableChannel != null)
                {
                    disposableChannel.Dispose();
                }
            }
        }

        /// <summary>
        /// Tracks the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="metrics">The metrics.</param>
        public void TrackEvent(string eventName, IDictionary<string, string> properties, IDictionary<string, double> metrics)
        {
            this.telemetry.TrackEvent(eventName, properties, metrics);
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void LogException(Exception exception)
        {
            this.telemetry.TrackException(exception);
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="metrics">The metrics.</param>
        public void LogException(Exception exception, IDictionary<string, string> properties, IDictionary<string, double> metrics)
        {
            this.telemetry.TrackException(exception, properties, metrics);
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
                this.configuration.Dispose();

                var disposableChannel = this.channel as IDisposable;

                if (disposableChannel != null)
                {
                    disposableChannel.Dispose();
                }
            }
        }
    }
}
