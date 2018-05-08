#region Copyright (c) all rights reserved.
// <copyright file="ComponentContext.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    /// <summary>
    /// Encapsulates information describing an telemetry component.
    /// </summary>
    public class ComponentContext
    {
        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }
    }
}
