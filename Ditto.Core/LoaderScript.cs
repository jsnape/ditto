#region Copyright (c) all rights reserved.
// <copyright file="LoaderScript.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Loader Script.
    /// </summary>
    public abstract class LoaderScript
    {
        /// <summary>
        /// The connections.
        /// </summary>
        private readonly Dictionary<string, DataConnection> connections;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoaderScript" /> class.
        /// </summary>
        /// <param name="connections">The connections.</param>
        protected LoaderScript(IEnumerable<DataConnection> connections)
        {
            if (connections == null)
            {
                throw new ArgumentNullException("connections");
            }

            this.connections = new Dictionary<string, DataConnection>(connections.ToDictionary(c => c.Name));
        }

        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <value>
        /// The connections.
        /// </value>
        public IReadOnlyDictionary<string, DataConnection> Connections
        {
            get { return this.connections; }
        }

        /// <summary>
        /// Runs the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public abstract void Run(CancellationToken cancellationToken);
    }
}
