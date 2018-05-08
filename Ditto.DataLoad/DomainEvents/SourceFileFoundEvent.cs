#region Copyright (c) all rights reserved.
// <copyright file="SourceFileFoundEvent.cs">
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
    /// Source file found event.
    /// </summary>
    public class SourceFileFoundEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the operation identifier.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        public Guid OperationId { get; set; }

        /// <summary>
        /// Gets or sets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the source.
        /// </value>
        public string SourceName { get; set; }

        /// <summary>
        /// Gets or sets the name of the connection.
        /// </summary>
        /// <value>
        /// The name of the connection.
        /// </value>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>
        /// The found files.
        /// </value>
        public IEnumerable<IFileInfo> FoundFiles { get; set; }
    }
}
