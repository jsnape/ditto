#region Copyright (c) all rights reserved.
// <copyright file="TelemetryContext.cs">
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
    /// Telemetry Context.
    /// </summary>
    public class TelemetryContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryContext" /> class.
        /// </summary>
        /// <param name="instrumentationKey">The instrumentation key.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public TelemetryContext(string instrumentationKey, IDictionary<string, string> properties)
        {
            if (instrumentationKey == null)
            {
                throw new ArgumentNullException("instrumentationKey");
            }

            this.InstrumentationKey = instrumentationKey;

            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            this.Properties = properties;

            this.Component = new ComponentContext();
            this.Device = new DeviceContext();
            this.Operation = new OperationContext();
            this.Session = new SessionContext();
            this.User = new UserContext();
        }

        /// <summary>
        /// Gets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        public ComponentContext Component { get; private set; }

        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        public DeviceContext Device { get; private set; }

        /// <summary>
        /// Gets or sets the instrumentation key.
        /// </summary>
        /// <value>
        /// The instrumentation key.
        /// </value>
        public string InstrumentationKey { get; set; }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public OperationContext Operation { get; private set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public IDictionary<string, string> Properties { get; private set; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public SessionContext Session { get; private set; }

        /// <summary>
        /// Gets the user context.
        /// </summary>
        /// <value>
        /// The user context.
        /// </value>
        public UserContext User { get; private set; }
    }
}
