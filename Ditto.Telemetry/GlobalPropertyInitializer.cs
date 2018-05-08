#region Copyright (c) all rights reserved.
// <copyright file="GlobalPropertyInitializer.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Global Property Initializer.
    /// </summary>
    public class GlobalPropertyInitializer : ITelemetryInitializer
    {
        /// <summary>
        /// The global properties.
        /// </summary>
        private readonly IDictionary<string, string> globalProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalPropertyInitializer"/> class.
        /// </summary>
        /// <param name="globalProperties">The global properties.</param>
        public GlobalPropertyInitializer(IDictionary<string, string> globalProperties)
        {
            if (globalProperties == null)
            {
                throw new ArgumentNullException("globalProperties");
            }

            this.globalProperties = globalProperties;
        }

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

            foreach (var item in this.globalProperties)
            {
                telemetry.Context.Properties[item.Key] = item.Value;
            }
        }
    }
}
