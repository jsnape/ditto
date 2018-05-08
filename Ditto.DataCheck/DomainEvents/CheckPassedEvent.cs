#region Copyright (c) all rights reserved.
// <copyright file="CheckPassedEvent.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck.DomainEvents
{
    using System;
    using Ditto.DomainEvents;

    /// <summary>
    /// Check passed event.
    /// </summary>
    public class CheckPassedEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the check name.
        /// </summary>
        /// <value>
        /// The check name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public TimeSpan Duration { get; set; }
    }
}
