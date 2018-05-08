#region Copyright (c) all rights reserved.
// <copyright file="NoMatchingColumnsInSourceEvent.cs">
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
    /// No matching columns in source event.
    /// </summary>
    public class NoMatchingColumnsInSourceEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public DataOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets the source columns.
        /// </summary>
        /// <value>
        /// The source columns.
        /// </value>
        public IEnumerable<string> SourceColumns { get; set; }
    }
}
