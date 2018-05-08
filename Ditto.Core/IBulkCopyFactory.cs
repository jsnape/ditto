#region Copyright (c) all rights reserved.
// <copyright file="IBulkCopyFactory.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    /// <summary>
    /// IBulkCopyFactory interface.
    /// </summary>
    public interface IBulkCopyFactory
    {
        /// <summary>
        /// Creates the bulk copy.
        /// </summary>
        /// <returns>A bulk copy instance.</returns>
        IBulkCopy CreateBulkCopy();
    }
}
