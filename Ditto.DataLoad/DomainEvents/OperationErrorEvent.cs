#region Copyright (c) all rights reserved.
// <copyright file="OperationErrorEvent.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.DomainEvents
{
    using System;
    using System.Collections.Generic;
    using Ditto.DomainEvents;

    /// <summary>
    /// Operation Error Event.
    /// </summary>
    public class OperationErrorEvent : IDomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationErrorEvent"/> class.
        /// </summary>
        public OperationErrorEvent()
        {
            this.Properties = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationErrorEvent" /> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="properties">The properties.</param>
        public OperationErrorEvent(DataOperation operation, Exception exception, IDictionary<string, string> properties)
        {
            this.Operation = operation;
            this.Exception = exception;
            this.Properties = new Dictionary<string, string>(properties);
        }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public DataOperation Operation { get; set; }

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
