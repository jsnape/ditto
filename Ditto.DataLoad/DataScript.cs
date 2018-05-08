#region Copyright (c) all rights reserved.
// <copyright file="DataScript.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Ditto.Core;

    /// <summary>
    /// Data Script.
    /// </summary>
    public class DataScript : LoaderScript
    {
        /// <summary>
        /// The operations.
        /// </summary>
        private readonly IEnumerable<DataOperation> operations;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataScript" /> class.
        /// </summary>
        /// <param name="connections">The connections.</param>
        /// <param name="operations">The operations.</param>
        public DataScript(IEnumerable<DataConnection> connections, IEnumerable<DataOperation> operations)
            : base(connections)
        {
            if (operations == null)
            {
                throw new ArgumentNullException("operations");
            }

            this.operations = operations.ToArray();
        }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        public IEnumerable<DataOperation> Operations
        {
            get { return this.operations; }
        }

        /// <summary>
        /// Runs the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public override void Run(CancellationToken cancellationToken)
        {
            Task.WhenAll(
                this.operations.Select(
                    op => Task.Run(
                        () => op.Run(cancellationToken))))
            .Wait(cancellationToken);
        }
    }
}
