#region Copyright (c) all rights reserved.
// <copyright file="DbDataSource.cs">
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
    using System.Data.Common;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Ditto.DomainEvents;
    using Ditto.Core;
    using Ditto.DataLoad.DomainEvents;
    using Ditto.DataLoad.Properties;

    /// <summary>
    /// Source Info base class.
    /// </summary>
    public abstract class DbDataSource : IDataSource
    {
        /// <summary>
        /// The connection factory.
        /// </summary>
        private readonly IConnectionFactory connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbDataSource"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        protected DbDataSource(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException("connectionFactory");
            }

            this.connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Gets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the source.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        /// <value>
        /// The name of the connection.
        /// </value>
        public string ConnectionName
        {
            get { return this.connectionFactory.Name; }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public IDataOperationInfo Parent { get; set; }

        /// <summary>
        /// Gets the connection factory.
        /// </summary>
        /// <value>
        /// The connection factory.
        /// </value>
        protected IConnectionFactory ConnectionFactory
        {
            get { return this.connectionFactory; }
        }

        /// <summary>
        /// Gets the operation identifier.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        private Guid OperationId
        {
            get { return this.Parent == null ? Guid.Empty : this.Parent.OperationId; }
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>
        /// A sequence of columns.
        /// </returns>
        public IEnumerable<string> GetColumns(Watermark previousHighWatermark)
        {
            using (var schema = this.GetSchemaTable())
            {
                return schema.Rows
                    .Cast<DataRow>()
                    .Select(r => r["ColumnName"].ToString())
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.GetCommandText(Enumerable.Empty<string>(), null);
        }

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        /// </returns>
        public virtual DataTable GetSchemaTable()
        {
            try
            {
                var sql = this.GetCommandText(Enumerable.Empty<string>(), null);

                return this.connectionFactory.GetQuerySchema(sql);
            }
            catch (DbException ex)
            {
                EventPublisher.Raise(
                    new SourceErrorEvent
                    {
                        OperationId = this.Parent.OperationId,
                        SourceName = this.Name,
                        ConnectionName = this.connectionFactory.Name,
                        Exception = ex
                    });

                throw;
            }
        }

        /// <summary>
        /// Determines whether the specified previous high watermark has data.
        /// </summary>
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>
        /// True if there is data.
        /// </returns>
        /// <remarks>
        /// This allows a shortcut for sources that may not exist e.g. files.
        /// </remarks>
        public bool HasData(Watermark previousHighWatermark)
        {
            // Databases always "have" data even if there are no rows.
            return true;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>
        /// A streaming reader for the data.
        /// </returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input in SQL string")]
        public IDataReader GetData(IEnumerable<string> columns, Watermark previousHighWatermark)
        {
            try
            {
                var connection = this.connectionFactory.CreateConnection();
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = this.GetCommandText(columns, previousHighWatermark);
                command.CommandTimeout = 0;

                var highWatermarkParameterName = this.connectionFactory.MakeParameterName("highWatermark");

                if (command.CommandText.Contains(highWatermarkParameterName))
                {
                    var watermarkParameter = command.CreateParameter();
                    watermarkParameter.ParameterName = highWatermarkParameterName;
                    watermarkParameter.Value = previousHighWatermark == null ? Settings.Default.DefaultHighWatermark : previousHighWatermark.WatermarkValue;
                    command.Parameters.Add(watermarkParameter);
                }

                DittoEventSource.Log.DatabaseQuery(this.connectionFactory.Name, command.CommandText, this.OperationId);

                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (DbException ex)
            {
                EventPublisher.Raise(
                    new SourceErrorEvent 
                    {
                        OperationId = this.Parent.OperationId,
                        SourceName = this.Name, 
                        ConnectionName = this.connectionFactory.Name,
                        Exception = ex
                    });

                throw;
            }
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <param name="columns">The target columns.</param>
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>
        /// A SQL query.
        /// </returns>
        protected abstract string GetCommandText(IEnumerable<string> columns, Watermark previousHighWatermark);
    }
}
