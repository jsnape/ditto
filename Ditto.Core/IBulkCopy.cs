#region Copyright (c) all rights reserved.
// <copyright file="IBulkCopy.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// IBulkCopy interface definition.
    /// </summary>
    public interface IBulkCopy : IDisposable
    {
        /// <summary>
        /// Gets or sets the name of the destination table.
        /// </summary>
        /// <value>
        /// The name of the destination table.
        /// </value>
        string DestinationTableName { get; set; }

        /// <summary>
        /// Adds the column mapping.
        /// </summary>
        /// <param name="sourceColumn">The source column.</param>
        /// <param name="targetColumn">The target column.</param>
        /// <returns>This instance for a fluent interface.</returns>
        IBulkCopy AddColumnMapping(string sourceColumn, string targetColumn);

        /// <summary>
        /// Copies all rows in the supplied System.Data.IDataReader to a destination table.
        /// </summary>
        /// <param name="reader">A <see cref="System.Data.IDataReader"/> whose rows will be copied to the destination table.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WriteToServerAsync(IDataReader reader, CancellationToken cancellationToken);
    }
}
