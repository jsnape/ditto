#region Copyright (c) all rights reserved.
// <copyright file="IDataSource.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// IDataSource interface definition.
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Gets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the source.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        /// <value>
        /// The name of the connection.
        /// </value>
        string ConnectionName { get; }

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
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>A sequence of columns.</returns>
        IEnumerable<string> GetColumns(Watermark previousHighWatermark);

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Following the IDataReader pattern")]
        DataTable GetSchemaTable();

        /// <summary>
        /// Determines whether the specified previous high watermark has data.
        /// </summary>
        /// <remarks>This allows a shortcut for sources that may not exist e.g. files.</remarks>
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>True if there is data.</returns>
        bool HasData(Watermark previousHighWatermark);

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>A streaming reader for the data.</returns>
        IDataReader GetData(IEnumerable<string> columns, Watermark previousHighWatermark);
    }
}
