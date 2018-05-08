#region Copyright (c) all rights reserved.
// <copyright file="EventPublisher.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DomainEvents
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Autofac;

    /// <summary>
    /// Domain Events.
    /// </summary>
    public static class EventPublisher
    {
        /// <summary>
        /// The actions.
        /// </summary>
        /// <remarks>
        /// This is thread specific.
        /// </remarks>
        [ThreadStatic]
        private static List<Delegate> actions;

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        public static IContainer Container { get; set; }

        /// <summary>
        /// Registers a callback for the given domain event.
        /// </summary>
        /// <typeparam name="T">The of event to register.</typeparam>
        /// <param name="callback">The callback.</param>
        public static void Register<T>(Action<T> callback) where T : IDomainEvent
        {
            if (actions == null)
            {
                actions = new List<Delegate>();
            }

            actions.Add(callback);
        }

        /// <summary>
        /// Clears callbacks passed to Register on the current thread.
        /// </summary>
        public static void ClearCallbacks()
        {
            actions = null;
        }

        /// <summary>
        /// Raises the given domain event.
        /// </summary>
        /// <typeparam name="T">Type of event to raise.</typeparam>
        /// <param name="args">The event arguments.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Domain event pattern used instead [JS]")]
        public static void Raise<T>(T args) where T : IDomainEvent
        {
            if (Container != null)
            {
                foreach (var handler in Container.Resolve<IEnumerable<IHandles<T>>>())
                {
                    handler.Handle(args);
                }
            }

            if (actions != null)
            {
                foreach (var action in actions
                    .Where(a => a is Action<T>)
                    .Cast<Action<T>>())
                {
                    action(args);
                }
            }
        }
    }
}
