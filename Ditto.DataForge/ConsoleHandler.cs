#region Copyright (c) all rights reserved.
// <copyright file="ConsoleHandler.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataForge
{
    using System;
    using Ditto.DomainEvents;
    using DomainEvents;
    using Properties;

    /// <summary>
    /// Console Event Handler.
    /// </summary>
    public class ConsoleHandler :
        IHandles<BatchCompleteEvent>
    {
        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(BatchCompleteEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            Console.WriteLine(Resources.OperationCompletedMessageFormat, args.Duration);
        }
    }
}
