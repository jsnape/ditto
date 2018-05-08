#region Copyright (c) all rights reserved.
// <copyright file="ValidationContext.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataForge
{
    using System;
    using System.Threading;

    /// <summary>
    /// Validation Context.
    /// </summary>
    public class GenerationContext
    {
        /// <summary>
        /// The cancellation token.
        /// </summary>
        private readonly CancellationToken cancellationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationContext"/> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public GenerationContext(CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
            {
                throw new ArgumentNullException("cancellationToken");
            }

            this.cancellationToken = cancellationToken;

            // In order to generate the same outputs each time we should use a fixed
            // seed value. However, given the generators might use multiple threads there is
            // still an element of randomness so I've left this at the default for now.
            this.Random = new Random();
        }

        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        /// <value>
        /// The cancellation token.
        /// </value>
        public CancellationToken CancellationToken
        {
            get { return this.cancellationToken; }
        }

        /// <summary>
        /// Gets the random number generator.
        /// </summary>
        /// <value>
        /// A random number generator.
        /// </value>
        public Random Random { get; }
    }
}
