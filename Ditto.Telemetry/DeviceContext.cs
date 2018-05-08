#region Copyright (c) all rights reserved.
// <copyright file="DeviceContext.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    /// <summary>
    /// Encapsulates information about a device where an application is running.
    /// </summary>
    public class DeviceContext
    {
        /// <summary>
        /// Gets or sets a device unique ID.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the current display language of the operating system.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the operating system name.
        /// </summary>
        /// <value>
        /// The operating system.
        /// </value>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// Gets or sets the type for the current device.
        /// </summary>
        /// <value>
        /// The device type.
        /// </value>
        public string DeviceType { get; set; }
    }
}
