#region Copyright (c) all rights reserved.
// <copyright file="ValidationContext.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Threading;

    /// <summary>
    /// Validation Context.
    /// </summary>
    public class ValidationContext
    {
        /// <summary>
        /// The cancellation token.
        /// </summary>
        private readonly CancellationToken cancellationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public ValidationContext(CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
            {
                throw new ArgumentNullException("cancellationToken");
            }

            this.cancellationToken = cancellationToken;
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
    }
}
