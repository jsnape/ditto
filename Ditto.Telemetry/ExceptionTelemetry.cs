#region Copyright (c) all rights reserved.
// <copyright file="ExceptionTelemetry.cs">
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
    /// Exception Telemetry.
    /// </summary>
    public class ExceptionTelemetry : TelemetryBase, ISqlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionTelemetry" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="metrics">The metrics.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public ExceptionTelemetry(TelemetryContext context, Exception exception, IDictionary<string, double> metrics)
            : base(context)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            this.Exception = exception;

            if (metrics == null)
            {
                throw new ArgumentNullException("metrics");
            }

            this.Metrics = metrics;
        }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets the metrics.
        /// </summary>
        /// <value>
        /// The metrics.
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
            writer.WriteTelemetryType("exception");
            writer.WriteTelemetryName(this.Exception.Source);
            writer.WriteMessage(this.Exception.Message);
            writer.WriteTimestamp(this.Timestamp);
            writer.WriteContext(this.Context);
            writer.WriteProperties(this.Context.Properties);
            writer.WriteMetrics(this.Metrics);
            writer.WriteEndInsert();
        }
    }
}
