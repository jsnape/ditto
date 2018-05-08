#region Copyright (c) all rights reserved.
// <copyright file="SqlMetadata.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Ditto.Core;

    /// <summary>
    /// SQL metadata.
    /// </summary>
    public class SqlMetadata : IDbMetadata
    {
        /// <summary>
        /// The connection factory.
        /// </summary>
        private readonly IConnectionFactory connectionFactory;

        /// <summary>
        /// The tables.
        /// </summary>
        private readonly string[] tables;

        /// <summary>
        /// The table columns.
        /// </summary>
        private readonly Dictionary<string, string[]> tableColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlMetadata"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        public SqlMetadata(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException("connectionFactory");
            }

            this.connectionFactory = connectionFactory;

            this.tables = this.ListTables().ToArray();

            this.tableColumns = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Gets all the tables.
        /// </summary>
        /// <returns>
        /// A sequence of table names in the form {schema}.{table}.
        /// </returns>
        public IEnumerable<string> AllTables()
        {
            return this.tables;
        }

        /// <summary>
        /// Gets all the columns for a particular table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        /// A sequence of column names for the table.
        /// </returns>
        public IEnumerable<string> AllColumns(string tableName)
        {
            if (!this.tableColumns.ContainsKey(tableName))
            {
                this.tableColumns[tableName] = this.ListColumns(tableName).ToArray();
            }

            return this.tableColumns[tableName];
        }

        /// <summary>
        /// All the tables.
        /// </summary>
        /// <returns>
        /// A sequence of table names in the form {schema}.{table}.
        /// </returns>
        private IEnumerable<string> ListTables()
        {
            const string Query = @"
select s.name + N'.' + t.name
from sys.tables t
join sys.schemas s on s.schema_id = t.schema_id
";

            using (IDbConnection connection = this.connectionFactory.CreateConnection())
            {
                connection.Open();
                
                var command = connection.CreateCommand();
                command.CommandText = Query;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        /// <summary>
        /// All the columns.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        /// A sequence of column names for the table.
        /// </returns>
        private IEnumerable<string> ListColumns(string tableName)
        {
            const string Query = @"
select name
from sys.columns c
where c.object_id = object_id(@tableName)
";

            using (IDbConnection connection = this.connectionFactory.CreateConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = Query;
                command.AddParameter("@tableName", tableName);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }
        }
    }
}
