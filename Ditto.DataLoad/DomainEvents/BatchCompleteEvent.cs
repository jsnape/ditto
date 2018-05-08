#region Copyright (c) all rights reserved.
// <copyright file="BatchCompleteEvent.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.DomainEvents
{
    using System;
    using Ditto.DomainEvents;

    /// <summary>
    /// Batch Complete Event.
    /// </summary>
    public class BatchCompleteEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public DataOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        /// <value>
        /// The row count.
        /// </value>
        public int RowCount { get; set; }

        /// <summary>
        /// Gets or sets the result count.
        /// </summary>
        /// <value>
        /// The result count.
        /// </value>
        public int ResultCount { get; set; }
    }
}
