#region Copyright (c) all rights reserved.
// <copyright file="UnknownCheckEvent.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck.DomainEvents
{
    using Ditto.DomainEvents;

    /// <summary>
    /// Unknown check event.
    /// </summary>
    public class UnknownCheckEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the name of the check.
        /// </summary>
        /// <value>
        /// The name of the check.
        /// </value>
        public string CheckName { get; set; }
    }
}
