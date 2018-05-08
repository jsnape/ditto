#region Copyright (c) all rights reserved.
// <copyright file="ITelemetryChannel.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    /// <summary>
    /// ITelemetryChannel interface definition.
    /// </summary>
    public interface ITelemetryChannel
    {
        /// <summary>
        /// Sends the specified item to the channel.
        /// </summary>
        /// <param name="item">The item to send to the channel.</param>
        void Send(ITelemetry item);
    }
}
