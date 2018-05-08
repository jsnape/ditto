#region Copyright (c) all rights reserved.
// <copyright file="ISqlWriter.cs">
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
    /// <c>ISqlWriter</c> interface definition.
    /// </summary>
    public interface ISqlWriter
    {
        /// <summary>
        /// Writes the begin insert.
        /// </summary>
        void WriteBeginInsert();

        /// <summary>
        /// Writes the type of the telemetry.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteTelemetryType(string value);

        /// <summary>
        /// Writes the name of the telemetry.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteTelemetryName(string value);

        /// <summary>
        /// Writes the timestamp.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteTimestamp(DateTimeOffset value);

        /// <summary>
        /// Writes the context.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteContext(TelemetryContext value);

        /// <summary>
        /// Writes the properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        void WriteProperties(IDictionary<string, string> properties);

        /// <summary>
        /// Writes the metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        void WriteMetrics(IDictionary<string, double> metrics);

        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteValue(double value);

        /// <summary>
        /// Writes the message.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteMessage(string value);

        /// <summary>
        /// Writes the end insert.
        /// </summary>
        void WriteEndInsert();
    }
}
