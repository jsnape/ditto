#region Copyright (c) all rights reserved.
// <copyright file="SqlDataTarget.cs">
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
    using System.Globalization;
    using System.Linq;
    using Ditto.Core;
    using Ditto.DataLoad.Properties;

    /// <summary>
    /// SQLServer Data Target.
    /// </summary>
    public class SqlDataTarget : IDataTarget
    {
        /// <summary>
        /// The connection factory.
        /// </summary>
        private readonly IConnectionFactory connectionFactory;

        /// <summary>
        /// The bulk copy factory.
        /// </summary>
        private readonly IBulkCopyFactory bulkCopyFactory;

        /// <summary>
        /// The watermark service.
        /// </summary>
        private readonly IHighWatermarkService watermarkService;

        /// <summary>
        /// The table name.
        /// </summary>
        private readonly string tableName;

        /// <summary>
        /// The high watermark column.
        /// </summary>
        private readonly string highWatermarkColumn;

        /// <summary>
        /// The schema.
        /// </summary>
        private Lazy<DataTable> targetSchema;

        /// <summary>
        /// Indicated whether to truncate before load or not.
        /// </summary>
        private bool truncateBeforeLoad;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataTarget" /> class.
        /// </summary>
        /// <param name="connectionFactory">The connection.</param>
        /// <param name="bulkCopyFactory">The bulk copy factory.</param>
        /// <param name="watermarkService">The watermark service.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="highWatermarkColumn">The high watermark column.</param>
        /// <param name="truncateBeforeLoad">If set to <c>true</c> [truncate before load].</param>
        /// <exception cref="System.ArgumentNullException">If any parameters are null.</exception>
        public SqlDataTarget(IConnectionFactory connectionFactory, IBulkCopyFactory bulkCopyFactory, IHighWatermarkService watermarkService, string tableName, string highWatermarkColumn, bool truncateBeforeLoad)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException("connectionFactory");
            }

            this.connectionFactory = connectionFactory;

            if (bulkCopyFactory == null)
            {
                throw new ArgumentNullException("bulkCopyFactory");
            }

            this.bulkCopyFactory = bulkCopyFactory;

            if (tableName == null)
            {
                throw new ArgumentNullException("tableName");
            }

            this.tableName = tableName;

            if (watermarkService == null)
            {
                throw new ArgumentNullException("watermarkService");
            }

            this.watermarkService = watermarkService;

            this.highWatermarkColumn = highWatermarkColumn;
            this.truncateBeforeLoad = truncateBeforeLoad;

            this.targetSchema = new Lazy<DataTable>(() => this.GetSchemaTable());
        }

        /// <summary>
        /// Gets a value indicating whether [truncate before load].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [truncate before load]; otherwise, <c>false</c>.
        /// </value>
        public bool TruncateBeforeLoad
        {
            get { return this.truncateBeforeLoad; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IStagingTarget" /> is exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if exists; otherwise, <c>false</c>.
        /// </value>
        public bool Exists
        {
            get
            {
                var sql = string.Format(CultureInfo.CurrentCulture, "select count(*) from sys.tables where object_id = object_id(N'{0}');", this.tableName);

                var result = (int)this.connectionFactory.ExecuteScalar(sql, this.OperationId);

                return result == 1;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name of the target.
        /// </value>
        public string Name
        {
            get { return this.tableName; }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public IDataOperationInfo Parent { get; set; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public IEnumerable<string> Columns
        {
            get
            {
                return this.targetSchema.Value.Rows
                    .Cast<DataRow>()
                    .Select(r => r["ColumnName"].ToString())
                    .ToList();
            }
        }

        /// <summary>
        /// Gets the previous high watermark.
        /// </summary>
        /// <value>
        /// The previous high watermark.
        /// </value>
        public Watermark PreviousHighWatermark
        {
            get { return this.watermarkService.GetHighWatermark(this.tableName, this.highWatermarkColumn); }
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
        /// Creates the table.
        /// </summary>
        /// <param name="schema">The schema.</param>
        public void CreateTable(DataTable schema)
        {
            var generator = this.connectionFactory.CreateTableGenerator();

            var sql = generator.GenerateTable(this.tableName, schema);

            this.connectionFactory.ExecuteNonQuery(sql, this.OperationId);
        }

        /// <summary>
        /// Truncates this instance.
        /// </summary>
        public void Truncate()
        {
            var sql = string.Format(CultureInfo.CurrentCulture, "truncate table {0};", this.tableName);

            this.connectionFactory.ExecuteNonQuery(sql, this.OperationId);
        }

        /// <summary>
        /// Updates the high watermark using the current value from the staging table.
        /// </summary>
        /// <returns>
        /// A watermark instance.
        /// </returns>
        public Watermark UpdateHighWatermark()
        {
            if (this.highWatermarkColumn == null)
            {
                return null;
            }

            var sql = string.Format(CultureInfo.CurrentCulture, "select max({0}) from {1};", this.highWatermarkColumn, this.tableName);

            var result = this.connectionFactory.ExecuteScalar(sql, this.OperationId);

            if (result == null || result == DBNull.Value)
            {
                return null;
            }

            var newHighwaterMark = new Watermark(this.highWatermarkColumn, (DateTime)result);

            this.watermarkService.UpdateHighWatermark(this.tableName, newHighwaterMark);

            return newHighwaterMark;
        }

        /// <summary>
        /// Creates a bulk copy instance.
        /// </summary>
        /// <returns>
        /// A bulk copy instance.
        /// </returns>
        public IBulkCopy CreateBulkCopy()
        {
            var bulkCopy = this.bulkCopyFactory.CreateBulkCopy();
            bulkCopy.DestinationTableName = this.tableName;

            return bulkCopy;
        }

        /// <summary>
        /// Initializes the batch.
        /// </summary>
        public void InitializeBatch()
        {
            if (this.TruncateBeforeLoad)
            {
                Console.WriteLine(Resources.TruncatingTargetTableMessageFormat, this.Name);
                this.Truncate();
            }
        }

        /// <summary>
        /// Initializes the chunk.
        /// </summary>
        public void InitializeChunk()
        {
        }

        /// <summary>
        /// Notifies that a chunk is complete and should be processed.
        /// </summary>
        public void ChunkComplete()
        {
            this.UpdateHighWatermark();
        }

        /// <summary>
        /// Notified the batch is complete and should be processed.
        /// </summary>
        public void BatchComplete()
        {
        }

        /// <summary>
        /// Lists the columns.
        /// </summary>
        /// <returns>A sequence of column names.</returns>
        private DataTable GetSchemaTable()
        {
            return this.connectionFactory.GetTableSchema(this.tableName);
        }
    }
}
