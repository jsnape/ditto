#region Copyright (c) all rights reserved.
// <copyright file="IDataTarget.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System.Collections.Generic;
    using System.Data;
    using Ditto.Core;

    /// <summary>
    /// IDataTarget interface definition.
    /// </summary>
    public interface IDataTarget
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IDataTarget"/> exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if exists; otherwise, <c>false</c>.
        /// </value>
        bool Exists { get; }

        /// <summary>
        /// Gets the name of the target.
        /// </summary>
        /// <value>
        /// The name of the target.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        IDataOperationInfo Parent { get; set; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        IEnumerable<string> Columns { get; }

        /// <summary>
        /// Gets the previous high watermark.
        /// </summary>
        /// <value>
        /// The previous high watermark.
        /// </value>
        Watermark PreviousHighWatermark { get; }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        void CreateTable(DataTable schema);

        /// <summary>
        /// Truncates this instance.
        /// </summary>
        void Truncate();

        /// <summary>
        /// Updates the high watermark using the current value from the staging table.
        /// </summary>
        /// <returns>A watermark instance.</returns>
        Watermark UpdateHighWatermark();

        /// <summary>
        /// Creates a bulk copy instance.
        /// </summary>
        /// <returns>A bulk copy instance.</returns>
        IBulkCopy CreateBulkCopy();

        /// <summary>
        /// Initializes the batch.
        /// </summary>
        void InitializeBatch();

        /// <summary>
        /// Initializes the chunk.
        /// </summary>
        void InitializeChunk();

        /// <summary>
        /// Notifies that a chunk is complete and should be processed.
        /// </summary>
        void ChunkComplete();

        /// <summary>
        /// Notified the batch is complete and should be processed.
        /// </summary>
        void BatchComplete();
    }
}
