#region Copyright (c) all rights reserved.
// <copyright file="IHandles.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DomainEvents
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Handles interface definition.
    /// </summary>
    /// <typeparam name="T">Type of event to be handled.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Handles", Justification = "Domain event pattern [JS]")]
    [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "I", Justification = "Domain event pattern [JS]")]
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Domain event pattern [JS]")]
    public interface IHandles<T> where T : IDomainEvent
    {
        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        void Handle(T args);
    }
}
