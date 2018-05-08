#region Copyright (c) all rights reserved.
// <copyright file="DataScript.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataForge
{
    using System.Collections.Generic;
    using System.Threading;
    using Core;

    /// <summary>
    /// Data Script.
    /// </summary>
    public class ForgeScript : LoaderScript
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForgeScript" /> class.
        /// </summary>
        /// <param name="connections">The connections.</param>
        /// <param name="operations">The operations.</param>
        public ForgeScript(IEnumerable<DataConnection> connections)
            : base(connections)
        {
        }

        /// <summary>
        /// Runs the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public override void Run(CancellationToken cancellationToken)
        {
        }
    }
}
