#region Copyright (c) all rights reserved.
// <copyright file="ITelemetryInitializer.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    /// <summary>
    /// ITelemetryInitializer interface definition.
    /// </summary>
    public interface ITelemetryInitializer
    {
        /// <summary>
        /// Initializes properties of the specified ITelemetry object.
        /// </summary>
        /// <param name="telemetry">The telemetry.</param>
        void Initialize(ITelemetry telemetry);
    }
}
