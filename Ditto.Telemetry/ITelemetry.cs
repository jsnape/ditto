#region Copyright (c) all rights reserved.
// <copyright file="ITelemetry.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    using System;

    /// <summary>
    /// ITelemetry interface definition.
    /// </summary>
    public interface ITelemetry
    {
        /// <summary>
        /// Gets the context associated with this telemetry instance.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        TelemetryContext Context { get; }

        /// <summary>
        /// Gets or sets date and time when telemetry was recorded.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        DateTimeOffset Timestamp { get; set; }
    }
}
