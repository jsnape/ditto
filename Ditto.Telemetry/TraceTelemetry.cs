#region Copyright (c) all rights reserved.
// <copyright file="TraceTelemetry.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    using System;

    /// <summary>
    /// Trace Telemetry.
    /// </summary>
    public class TraceTelemetry : TelemetryBase, ISqlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceTelemetry" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public TraceTelemetry(TelemetryContext context, string message)
            : base(context)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

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
            writer.WriteTelemetryType("trace");
            writer.WriteTelemetryName("message");
            writer.WriteTimestamp(this.Timestamp);
            writer.WriteContext(this.Context);
            writer.WriteProperties(this.Context.Properties);
            writer.WriteMessage(this.Message);
            writer.WriteEndInsert();
        }
    }
}
