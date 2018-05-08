#region Copyright (c) all rights reserved.
// <copyright file="CheckStartedEvent.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck.DomainEvents
{
    using Ditto.DomainEvents;

    /// <summary>
    /// Check started event.
    /// </summary>
    public class CheckStartedEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the check name.
        /// </summary>
        /// <value>
        /// The check name.
        /// </value>
        public string Name { get; set; }
    }
}
