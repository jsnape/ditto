#region Copyright (c) all rights reserved.
// <copyright file="TelemetryConfiguration.cs">
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
    /// Gets the active configuration profile loaded.
    /// </summary>
    public class TelemetryConfiguration : IDisposable
    {
        /// <summary>
        /// The active configuration.
        /// </summary>
        private static Lazy<TelemetryConfiguration> activeConfiguration = new Lazy<TelemetryConfiguration>(() => new TelemetryConfiguration());

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryConfiguration"/> class.
        /// </summary>
        public TelemetryConfiguration()
        {
            this.ContextInitializers = new List<IContextInitializer>();
            this.TelemetryInitializers = new List<ITelemetryInitializer>();
        }

        /// <summary>
        /// Gets the active.
        /// </summary>
        /// <value>
        /// The active.
        /// </value>
        public static TelemetryConfiguration Active
        {
            get { return activeConfiguration.Value; }
        }

        /// <summary>
        /// Gets or sets the default instrumentation key for the application.
        /// </summary>
        /// <value>
        /// The instrumentation key.
        /// </value>
        public string InstrumentationKey { get; set; }

        /// <summary>
        /// Gets or sets the telemetry channel.
        /// </summary>
        /// <value>
        /// The telemetry channel.
        /// </value>
        public ITelemetryChannel TelemetryChannel { get; set; }

        /// <summary>
        /// Gets the list of <see cref="IContextInitializer"/> objects that supply additional information about application.
        /// </summary>
        /// <value>
        /// The context initializers.
        /// </value>
        public IList<IContextInitializer> ContextInitializers { get; private set; }

        /// <summary>
        /// Gets the list of <see cref="ITelemetryInitializer"/> objects that supply additional information about telemetry.
        /// </summary>
        /// <value>
        /// The telemetry initializers.
        /// </value>
        public IList<ITelemetryInitializer> TelemetryInitializers { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposableChannel = this.TelemetryChannel as IDisposable;

                if (disposableChannel != null)
                {
                    disposableChannel.Dispose();
                }
            }
        }
    }
}
