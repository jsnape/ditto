#region Copyright (c) all rights reserved.
// <copyright file="TimestampPropertyInitializer.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    using System;

    /// <summary>
    /// An <see cref="ITelemetryInitializer"/> that sets Timestamp to <see cref="DateTimeOffset.Now"/>.
    /// </summary>
    public class TimestampPropertyInitializer : ITelemetryInitializer
    {
        /// <summary>
        /// Initializes properties of the specified ITelemetry object.
        /// </summary>
        /// <param name="telemetry">The telemetry.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry == null)
            {
                throw new ArgumentNullException("telemetry");
            }

            telemetry.Timestamp = DateTimeOffset.Now;
        }
    }
}
