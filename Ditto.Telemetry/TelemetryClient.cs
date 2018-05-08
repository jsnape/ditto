#region Copyright (c) all rights reserved.
// <copyright file="TelemetryClient.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// ITelemetryService interface definition.
    /// </summary>
    public class TelemetryClient
    {
        /// <summary>
        /// An empty properties constant.
        /// </summary>
        private static readonly IDictionary<string, string> EmptyProperties = new Dictionary<string, string>();

        /// <summary>
        /// An empty metrics constant.
        /// </summary>
        private static readonly IDictionary<string, double> EmptyMetrics = new Dictionary<string, double>();

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly TelemetryConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryClient"/> class.
        /// </summary>
        public TelemetryClient()
            : this(TelemetryConfiguration.Active)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryClient" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public TelemetryClient(TelemetryConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.configuration = configuration;
        }

        /// <summary>
        /// Tracks a telemetry event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public void TrackEvent(string eventName)
        {
            this.TrackEvent(eventName, EmptyProperties, EmptyMetrics);
        }

        /// <summary>
        /// Tracks a telemetry event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="properties">The properties associated with the event.</param>
        /// <param name="metrics">The metrics associated with the event.</param>
        public void TrackEvent(string eventName, IDictionary<string, string> properties, IDictionary<string, double> metrics)
        {
            this.CreateAndSendTelemetry(c => new EventTelemetry(c, eventName, metrics), properties);
        }

        /// <summary>
        /// Tracks a telemetry metric.
        /// </summary>
        /// <param name="name">The metric name.</param>
        /// <param name="value">The metric value.</param>
        public void TrackMetric(string name, double value)
        {
            this.TrackMetric(name, value, EmptyProperties);
        }

        /// <summary>
        /// Tracks a telemetry metric.
        /// </summary>
        /// <param name="name">The metric name.</param>
        /// <param name="value">The metric value.</param>
        /// <param name="properties">The metric properties.</param>
        public void TrackMetric(string name, double value, IDictionary<string, string> properties)
        {
            this.CreateAndSendTelemetry(c => new MetricTelemetry(c, name, value), properties);
        }

        /// <summary>
        /// Tracks an exception with the telemetry service.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void TrackException(Exception exception)
        {
            this.TrackException(exception, EmptyProperties, EmptyMetrics);
        }

        /// <summary>
        /// Tracks an exception with the telemetry service.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="properties">The exception properties.</param>
        /// <param name="metrics">The exception metrics.</param>
        public void TrackException(Exception exception, IDictionary<string, string> properties, IDictionary<string, double> metrics)
        {
            this.CreateAndSendTelemetry(c => new ExceptionTelemetry(c, exception, metrics), properties);
        }

        /// <summary>
        /// Tracks a telemetry trace.
        /// </summary>
        /// <param name="message">The message.</param>
        public void TrackTrace(string message)
        {
            this.TrackTrace(message, EmptyProperties);
        }

        /// <summary>
        /// Tracks a telemetry trace.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="properties">The trace properties.</param>
        public void TrackTrace(string message, IDictionary<string, string> properties)
        {
            this.CreateAndSendTelemetry(c => new TraceTelemetry(c, message), properties);
        }

        /// <summary>
        /// Creates the and send telemetry.
        /// </summary>
        /// <param name="createTelemetry">The create telemetry.</param>
        /// <param name="properties">The properties.</param>
        private void CreateAndSendTelemetry(Func<TelemetryContext, ITelemetry> createTelemetry, IDictionary<string, string> properties)
        {
            var context = this.configuration.CreateContext(properties);

            var telemetry = createTelemetry(context);

            this.configuration.InitializeTelemetry(telemetry);

            this.configuration.TelemetryChannel.Send(telemetry);
        }
    }
}
