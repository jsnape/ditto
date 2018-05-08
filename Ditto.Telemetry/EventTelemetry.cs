#region Copyright (c) all rights reserved.
// <copyright file="EventTelemetry.cs">
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
    /// Event Telemetry.
    /// </summary>
    public class EventTelemetry : TelemetryBase, ISqlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTelemetry" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="metrics">The metrics.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public EventTelemetry(TelemetryContext context, string eventName, IDictionary<string, double> metrics)
            : base(context)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException("metrics");
            }

            this.Metrics = metrics;

            this.Name = eventName;
        }

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        /// <value>
        /// The event name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the event metrics.
        /// </summary>
        /// <value>
        /// The event metrics.
        /// </value>
        public IDictionary<string, double> Metrics { get; private set; }

        /// <summary>
        /// Serializes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Serialize(ISqlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            writer.WriteBeginInsert();
            writer.WriteTelemetryType("event");
            writer.WriteTelemetryName(this.Name);
            writer.WriteTimestamp(this.Timestamp);
            writer.WriteContext(this.Context);
            writer.WriteProperties(this.Context.Properties);
            writer.WriteMetrics(this.Metrics);
            writer.WriteEndInsert();
        }
    }
}
