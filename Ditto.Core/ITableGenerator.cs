#region Copyright (c) all rights reserved.
// <copyright file="ITableGenerator.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System.Data;

    /// <summary>
    /// ITableGenerator interface definition.
    /// </summary>
    public interface ITableGenerator
    {
        /// <summary>
        /// Generates the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <returns>The SQL string for creating the table.</returns>
        string GenerateTable(string tableName, DataTable schema);
    }
}
