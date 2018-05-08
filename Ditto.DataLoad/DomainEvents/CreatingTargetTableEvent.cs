#region Copyright (c) all rights reserved.
// <copyright file="CreatingTargetTableEvent.cs">
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
    /// Creating Target Table Event.
    /// </summary>
    public class CreatingTargetTableEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the operation identifier.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        public Guid OperationId { get; set; }

        /// <summary>
        /// Gets or sets the name of the target.
        /// </summary>
        /// <value>
        /// The name of the target.
        /// </value>
        public string TargetName { get; set; }
    }
}
