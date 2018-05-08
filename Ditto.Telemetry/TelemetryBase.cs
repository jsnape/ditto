#region Copyright (c) all rights reserved.
// <copyright file="TelemetryBase.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    using System;

    /// <summary>
    /// Telemetry base class.
    /// </summary>
    public abstract class TelemetryBase : ITelemetry
    {
        /// <summary>
        /// The telemetry context.
        /// </summary>
        private readonly TelemetryContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryBase"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected TelemetryBase(TelemetryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.context = context;
        }

        /// <summary>
        /// Gets the context associated with this telemetry instance.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public TelemetryContext Context
        {
            get { return this.context; }
        }

        /// <summary>
        /// Gets or sets date and time when telemetry was recorded.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTimeOffset Timestamp { get; set; }
    }
}
