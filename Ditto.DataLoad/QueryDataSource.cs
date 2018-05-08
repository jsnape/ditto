#region Copyright (c) all rights reserved.
// <copyright file="QueryDataSource.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Ditto.Core;

    /// <summary>
    /// Query Source Info.
    /// </summary>
    public class QueryDataSource : DbDataSource
    {
        /// <summary>
        /// The name of the source.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The query to execute.
        /// </summary>
        private readonly string query;

        /// <summary>
        /// The schema.
        /// </summary>
        private readonly DataTable schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDataSource" /> class.
        /// </summary>
        /// <param name="connectionFactory">The connection.</param>
        /// <param name="name">The name of the source.</param>
        /// <param name="query">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public QueryDataSource(IConnectionFactory connectionFactory, string name, string query, DataTable schema)
            : base(connectionFactory)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;

            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            this.query = query;

            this.schema = schema;
        }

        /// <summary>
        /// Gets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the source.
        /// </value>
        public override string Name 
        {
            get { return this.name; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        /// </returns>
        public override DataTable GetSchemaTable()
        {
            if (this.schema != null)
            {
                return this.schema.Copy();
            }

            return base.GetSchemaTable();
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <param name="columns">The target columns.</param>
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>
        /// A SQL query.
        /// </returns>
        protected override string GetCommandText(IEnumerable<string> columns, Watermark previousHighWatermark)
        {
            return this.query;
        }
    }
}
