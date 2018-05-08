#region Copyright (c) all rights reserved.
// <copyright file="ExpandingElementEvent.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck.DomainEvents
{
    using Ditto.DomainEvents;

    /// <summary>
    /// Expanding element event.
    /// </summary>
    public class ExpandingElementEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the match.
        /// </summary>
        /// <value>
        /// The match.
        /// </value>
        public string Match { get; set; }

        /// <summary>
        /// Gets or sets the expansion.
        /// </summary>
        /// <value>
        /// The expansion.
        /// </value>
        public string Expansion { get; set; }
    }
}
