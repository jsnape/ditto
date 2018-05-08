#region Copyright (c) all rights reserved.
// <copyright file="CheckErrorEvent.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck.DomainEvents
{
    using System;
    using System.Collections.Generic;
    using Ditto.DomainEvents;

    /// <summary>
    /// Operation Error Event.
    /// </summary>
    public class CheckErrorEvent : IDomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckErrorEvent"/> class.
        /// </summary>
        public CheckErrorEvent()
        {
            this.Properties = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckErrorEvent" /> class.
        /// </summary>
        /// <param name="check">The data check.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="properties">The properties.</param>
        public CheckErrorEvent(IDataValidator check, Exception exception, IDictionary<string, string> properties)
        {
            this.Check = check;
            this.Exception = exception;
            this.Properties = new Dictionary<string, string>(properties);
        }

        /// <summary>
        /// Gets or sets the check.
        /// </summary>
        /// <value>
        /// The check.
        /// </value>
        public IDataValidator Check { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public IDictionary<string, string> Properties { get; private set; }
    }
}
