#region Copyright (c) all rights reserved.
// <copyright file="TelemetryConfigurationExtensions.cs">
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
    /// <see cref="TelemetryConfiguration"/> Extensions.
    /// </summary>
    public static class TelemetryConfigurationExtensions
    {
        /// <summary>
        /// Initializes the configuration with defaults.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The configuration for a fluent interface.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public static TelemetryConfiguration InitializeWithDefaults(this TelemetryConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            configuration
                .InitializeContextWith(new DeviceContextInitializer())
                .InitializeContextWith(new UserContextInitializer())
                .InitializeWith(new TimestampPropertyInitializer());

            return configuration;
        }

        /// <summary>
        /// Sets the instrumentation key.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="instrumentationKey">The instrumentation key.</param>
        /// <returns>The configuration for a fluent interface.</returns>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public static TelemetryConfiguration SetInstrumentationKey(this TelemetryConfiguration configuration, string instrumentationKey)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (instrumentationKey == null)
            {
                throw new ArgumentNullException("instrumentationKey");
            }

            configuration.InstrumentationKey = instrumentationKey;

            return configuration;
        }

        /// <summary>
        /// Sets the channel.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The configuration for a fluent interface.</returns>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public static TelemetryConfiguration SetChannel(this TelemetryConfiguration configuration, ITelemetryChannel channel)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }

            configuration.TelemetryChannel = channel;

            return configuration;
        }

        /// <summary>
        /// Adds a context initializer.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="initializer">The initializer.</param>
        /// <returns>
        /// The configuration for a fluent interface.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public static TelemetryConfiguration InitializeContextWith(this TelemetryConfiguration configuration, IContextInitializer initializer)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }

            configuration.ContextInitializers.Add(initializer);

            return configuration;
        }

        /// <summary>
        /// Adds a telemetry initializer.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="initializer">The initializer.</param>
        /// <returns>
        /// The configuration for a fluent interface.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public static TelemetryConfiguration InitializeWith(this TelemetryConfiguration configuration, ITelemetryInitializer initializer)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }

            configuration.TelemetryInitializers.Add(initializer);

            return configuration;
        }

        /// <summary>
        /// Creates the telemetry context.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>
        /// A new context instance.
        /// </returns>
        public static TelemetryContext CreateContext(this TelemetryConfiguration configuration, IDictionary<string, string> properties)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            var context = new TelemetryContext(configuration.InstrumentationKey, properties);

            foreach (var initializer in configuration.ContextInitializers)
            {
                initializer.Initialize(context);
            }

            return context;
        }

        /// <summary>
        /// Initializes the telemetry.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="telemetry">The telemetry.</param>
        /// <returns>
        /// The supplied telemetry instance.
        /// </returns>
        public static ITelemetry InitializeTelemetry(this TelemetryConfiguration configuration, ITelemetry telemetry)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            foreach (var initializer in configuration.TelemetryInitializers)
            {
                initializer.Initialize(telemetry);
            }

            return telemetry;
        }
    }
}
