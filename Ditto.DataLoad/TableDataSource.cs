#region Copyright (c) all rights reserved.
// <copyright file="TableDataSource.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Ditto.Core;

    /// <summary>
    /// Table Source Info.
    /// </summary>
    public class TableDataSource : DbDataSource
    {
        /// <summary>
        /// The table name.
        /// </summary>
        private readonly string tableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDataSource"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection.</param>
        /// <param name="tableName">Name of the table.</param>
        public TableDataSource(IConnectionFactory connectionFactory, string tableName)
            : base(connectionFactory)
        {
            this.tableName = tableName ?? throw new ArgumentNullException("tableName");
        }

        /// <summary>
        /// Gets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the source.
        /// </value>
        public override string Name 
        {
            get { return this.TableName; }
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName
        {
            get { return this.tableName; }
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
            var query = new StringBuilder();

            query
                .Append("select ")
                .Append(columns.Aggregate(string.Empty, (a, v) => a + (a.Length == 0 ? v : ", " + v)))
                .Append(columns.Count() == 0 ? "*" : string.Empty)
                .AppendFormat(CultureInfo.InvariantCulture, " from {0}", this.tableName);

            if (previousHighWatermark != null)
            {
                query.AppendFormat(CultureInfo.InvariantCulture, " where {0} > @highWatermark", previousHighWatermark.WatermarkColumn);
            }

            return query.ToString();
        }
    }
}
