#region Copyright (c) all rights reserved.
// <copyright file="NoNewDataEvent.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.DomainEvents
{
    using Ditto.DomainEvents;

    /// <summary>
    /// No new data event.
    /// </summary>
    public class NoNewDataEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public DataOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets the high watermark.
        /// </summary>
        /// <value>
        /// The high watermark.
        /// </value>
        public Watermark HighWatermark { get; set; }
    }
}
