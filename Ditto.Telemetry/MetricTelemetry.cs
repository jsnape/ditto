#region Copyright (c) all rights reserved.
// <copyright file="MetricTelemetry.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    using System;

    /// <summary>
    /// Metric Telemetry.
    /// </summary>
    public class MetricTelemetry : TelemetryBase, ISqlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricTelemetry" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="metricName">Name of the metric.</param>
        /// <param name="value">The metric value.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public MetricTelemetry(TelemetryContext context, string metricName, double value)
            : base(context)
        {
            this.Name = metricName;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the metric name.
        /// </summary>
        /// <value>
        /// The metric name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the metric value.
        /// </summary>
        /// <value>
        /// The metric value.
        /// </value>
        public double Value { get; set; }

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
            writer.WriteTelemetryType("metric");
            writer.WriteTelemetryName(this.Name);
            writer.WriteTimestamp(this.Timestamp);
            writer.WriteContext(this.Context);
            writer.WriteProperties(this.Context.Properties);
            writer.WriteValue(this.Value);
            writer.WriteEndInsert();
        }
    }
}
