#region Copyright (c) all rights reserved.
// <copyright file="SqlWriter.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Writes <c>SQL</c>
    /// </summary>
    public class SqlWriter : ISqlWriter
    {
        /// <summary>
        /// The writer.
        /// </summary>
        private readonly TextWriter writer;

        /// <summary>
        /// The telemetry table.
        /// </summary>
        private readonly string telemetryTable;

        /// <summary>
        /// The telemetry type.
        /// </summary>
        private string telemetryType;

        /// <summary>
        /// The telemetry name.
        /// </summary>
        private string telemetryName;

        /// <summary>
        /// The instrumentation key.
        /// </summary>
        private string instrumentationKey;

        /// <summary>
        /// The timestamp.
        /// </summary>
        private DateTimeOffset timestamp;

        /// <summary>
        /// The context XML.
        /// </summary>
        private string contextXml;

        /// <summary>
        /// The properties XML.
        /// </summary>
        private string propertiesXml;

        /// <summary>
        /// The metrics XML.
        /// </summary>
        private string metricsXml;

        /// <summary>
        /// The value.
        /// </summary>
        private double? telemetryValue;

        /// <summary>
        /// The message.
        /// </summary>
        private string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlWriter" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="telemetryTable">The telemetry table.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public SqlWriter(TextWriter writer, string telemetryTable)
        {
            this.writer = writer ?? throw new ArgumentNullException("writer");
            this.telemetryTable = telemetryTable ?? throw new ArgumentNullException("telemetryTable");
        }

        /// <summary>
        /// Writes the begin insert.
        /// </summary>
        public void WriteBeginInsert()
        {
            this.writer.WriteLine("insert into {0} (", this.telemetryTable);
        }

        /// <summary>
        /// Writes the type of the telemetry.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteTelemetryType(string value)
        {
            this.telemetryType = value;
        }

        /// <summary>
        /// Writes the name of the telemetry.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteTelemetryName(string value)
        {
            this.telemetryName = value;
        }

        /// <summary>
        /// Writes the timestamp.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteTimestamp(DateTimeOffset value)
        {
            this.timestamp = value;
        }

        /// <summary>
        /// Writes the context.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteContext(TelemetryContext value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.instrumentationKey = value.InstrumentationKey;

            var deviceElement = new XElement(
                "Device",
                new XElement("DeviceType", value.Device.DeviceType),
                new XElement("Id", value.Device.Id),
                new XElement("Language", value.Device.Language),
                new XElement("OperatingSystem", value.Device.OperatingSystem));

            var operationElement = new XElement(
                    "Operation",
                    new XElement("Id", value.Operation.Id),
                    new XElement("Name", value.Operation.Name));

            var userElement = new XElement(
                "User",
                new XElement("AccountId", value.User.AccountId),
                new XElement("Id", value.User.Id));

            var contextElement = new XElement(
                "Context",
                new XElement("Component", new XElement("Version", value.Component.Version)),
                deviceElement,
                operationElement,
                new XElement("Session", new XElement("Id", value.Session.Id)),
                userElement);

            this.contextXml = contextElement.ToString();
        }

        /// <summary>
        /// Writes the properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public void WriteProperties(IDictionary<string, string> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            try
            {
                var propertyElements = properties
                    .Select(p => new XElement("Property", new XAttribute("Name", p.Key), new XAttribute("Value", p.Value)));

                XElement propertiesElement =
                    new XElement("Properties", propertyElements);

                this.propertiesXml = propertiesElement.ToString();
            }
            catch (ArgumentNullException ex)
            {
                var values = properties
                    .Select(p => string.Format(CultureInfo.CurrentCulture, "{0}={1}", p.Key, p.Value));

                var exceptionMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    "A key or value from the dictionary is null: {0}",
                    string.Join(";", values));

                throw new ArgumentNullException(exceptionMessage, ex);
            }
        }

        /// <summary>
        /// Writes the metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        public void WriteMetrics(IDictionary<string, double> metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException("metrics");
            }

            var metricsElements = metrics
                .Select(p => new XElement("Metric", new XAttribute("Name", p.Key), new XAttribute("Value", p.Value)));

            var metricsElement =
                new XElement("Metrics", metricsElements);

            this.metricsXml = metricsElement.ToString();
        }

        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteValue(double value)
        {
            this.telemetryValue = value;
        }

        /// <summary>
        /// Writes the message.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteMessage(string value)
        {
            this.message = value;
        }

        /// <summary>
        /// Writes the end insert.
        /// </summary>
        public void WriteEndInsert()
        {
            this.WriteUsedColumns();

            this.writer.WriteLine(") values (");

            this.WriteUsedValues();

            this.writer.WriteLine(");");
        }

        /// <summary>
        /// Escapes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The escaped string.</returns>
        private static string Escape(string input)
        {
            return input.Replace("'", "''");
        }

        /// <summary>
        /// Writes the values used.
        /// </summary>
        private void WriteUsedValues()
        {
            this.writer.Write("    '{0}'", this.instrumentationKey);
            this.writer.Write(", '{0}'", this.telemetryType);
            this.writer.Write(", '{0}'", this.telemetryName);
            this.writer.Write(", '{0}'", this.timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffffzzz", CultureInfo.CurrentCulture));
            this.writer.Write(", '{0}'", Escape(this.contextXml));

            if (this.propertiesXml != null)
            {
                this.writer.Write(", '{0}'", Escape(this.propertiesXml));
            }

            if (this.metricsXml != null)
            {
                this.writer.Write(", '{0}'", Escape(this.metricsXml));
            }

            if (this.telemetryValue.HasValue)
            {
                this.writer.Write(", {0}", this.telemetryValue);
            }

            if (!string.IsNullOrWhiteSpace(this.message))
            {
                this.writer.Write(", '{0}'", Escape(this.message));
            }

            this.writer.WriteLine();
        }

        /// <summary>
        /// Writes the columns used.
        /// </summary>
        private void WriteUsedColumns()
        {
            this.writer.Write("    InstrumentationKey, TelemetryType, TelemetryName, Timestamp, Context");

            if (this.propertiesXml != null)
            {
                this.writer.Write(", Properties");
            }

            if (this.metricsXml != null)
            {
                this.writer.Write(", Metrics");
            }

            if (this.telemetryValue.HasValue)
            {
                this.writer.Write(", Value");
            }

            if (!string.IsNullOrWhiteSpace(this.message))
            {
                this.writer.Write(", Message");
            }

            this.writer.WriteLine();
        }
    }
}
