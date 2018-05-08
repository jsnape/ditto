#region Copyright (c) all rights reserved.
// <copyright file="TelemetryHandler.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Collections.Generic;
    using Ditto.DomainEvents;
    using Ditto.Core;
    using Ditto.DataCheck.DomainEvents;

    /// <summary>
    /// Telemetry Handler.
    /// </summary>
    public class TelemetryHandler :
        IHandles<CheckErrorEvent>
    {
        /// <summary>
        /// The no-metrics empty dictionary.
        /// </summary>
        private static readonly IDictionary<string, double> NoMetrics = new Dictionary<string, double>();

        /// <summary>
        /// The telemetry.
        /// </summary>
        private readonly EventTelemetry telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryHandler"/> class.
        /// </summary>
        /// <param name="telemetry">The telemetry.</param>
        public TelemetryHandler(EventTelemetry telemetry)
        {
            if (telemetry == null)
            {
                throw new ArgumentNullException("telemetry");
            }

            this.telemetry = telemetry;
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(CheckErrorEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var properties = new Dictionary<string, string>(args.Properties);

            properties["CheckName"] = args.Check.Name;

            this.telemetry.LogException(args.Exception, properties, NoMetrics);
            this.telemetry.TrackEvent("Check Error", properties, NoMetrics);
        }
    }
}
