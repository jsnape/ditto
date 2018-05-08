#region Copyright (c) all rights reserved.
// <copyright file="DeviceContextInitializer.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    using System;
    using System.Globalization;

    /// <summary>
    /// A telemetry context initializer that will gather device context information.
    /// </summary>
    public class DeviceContextInitializer : IContextInitializer
    {
        /// <summary>
        /// Initializes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Initialize(TelemetryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var device = context.Device;

            device.DeviceType = "Server";
            device.Id = Environment.MachineName;
            device.Language = CultureInfo.CurrentCulture.Name;
            device.OperatingSystem = Environment.OSVersion.VersionString;
        }
    }
}
