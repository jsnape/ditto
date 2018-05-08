#region Copyright (c) all rights reserved.
// <copyright file="TelemetryExtensions.cs">
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
    /// Telemetry Extensions.
    /// </summary>
    public static class TelemetryExtensions
    {
        /// <summary>
        /// Applies the telemetry context.
        /// </summary>
        /// <param name="telemetry">The telemetry.</param>
        /// <param name="initializers">The initializers.</param>
        /// <returns>
        /// The context for a fluent interface.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public static ITelemetry ApplyTelemetryContext(this ITelemetry telemetry, IEnumerable<IContextInitializer> initializers)
        {
            if (telemetry == null)
            {
                throw new ArgumentNullException("telemetry");
            }

            if (initializers == null)
            {
                throw new ArgumentNullException("initializers");
            }

            foreach (var initializer in initializers)
            {
                initializer.Initialize(telemetry.Context);
            }

            return telemetry;
        }

        /// <summary>
        /// Applies the telemetry initializers.
        /// </summary>
        /// <param name="telemetry">The telemetry.</param>
        /// <param name="initializers">The initializers.</param>
        /// <returns>
        /// The context for a fluent interface.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public static ITelemetry ApplyTelemetryInitializers(this ITelemetry telemetry, IEnumerable<ITelemetryInitializer> initializers)
        {
            if (telemetry == null)
            {
                throw new ArgumentNullException("telemetry");
            }

            if (initializers == null)
            {
                throw new ArgumentNullException("initializers");
            }

            foreach (var initializer in initializers)
            {
                initializer.Initialize(telemetry);
            }

            return telemetry;
        }
    }
}
