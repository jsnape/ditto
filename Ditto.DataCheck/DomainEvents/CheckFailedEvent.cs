#region Copyright (c) all rights reserved.
// <copyright file="CheckFailedEvent.cs">
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
    /// Check failed event.
    /// </summary>
    public class CheckFailedEvent : IDomainEvent
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The check name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the check type.
        /// </summary>
        /// <value>
        /// The check type.
        /// </value>
        public string CheckType { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>
        /// The details.
        /// </value>
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets the goal.
        /// </summary>
        /// <value>
        /// The expected goal.
        /// </value>
        public double Goal { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The actual value.
        /// </value>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public CheckSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public TimeSpan Duration { get; set; }
    }
}
