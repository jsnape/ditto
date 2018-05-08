#region Copyright (c) all rights reserved.
// <copyright file="IDbMetadata.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System.Collections.Generic;

    /// <summary>
    /// IDbMetadata interface definition.
    /// </summary>
    public interface IDbMetadata
    {
        /// <summary>
        /// All the tables.
        /// </summary>
        /// <returns>A sequence of table names in the form {schema}.{table}.</returns>
        IEnumerable<string> AllTables();

        /// <summary>
        /// All the columns.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A sequence of column names for the table.</returns>
        IEnumerable<string> AllColumns(string tableName);
    }
}
