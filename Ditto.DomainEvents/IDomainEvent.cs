#region Copyright (c) all rights reserved.
// <copyright file="IDomainEvent.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DomainEvents
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// IDomainEvent interface definition.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "This is a domain event pattern [JS]")]
    public interface IDomainEvent
    {
    }
}
