#region Copyright (c) all rights reserved.
// <copyright file="SqlBulkCopyWrapper.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// SQLServer BulkCopy wrapper class.
    /// </summary>
    public class SqlBulkCopyWrapper : IBulkCopy
    {
        /// <summary>
        /// The bulk copy.
        /// </summary>
        private readonly SqlBulkCopy bulkCopy;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkCopyWrapper"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlBulkCopyWrapper(string connectionString)
        {
            this.bulkCopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.TableLock);

            // Changing this to false causes all the data to be loaded into memory before
            // sending to the server which is not a good idea for large data sets.
            this.bulkCopy.EnableStreaming = true;
           
            this.bulkCopy.BulkCopyTimeout = 0;
        }

        /// <summary>
        /// Gets or sets the name of the destination table.
        /// </summary>
        /// <value>
        /// The name of the destination table.
        /// </value>
        public string DestinationTableName
        {
            get { return this.bulkCopy.DestinationTableName; }
            set { this.bulkCopy.DestinationTableName = value; }
        }

        /// <summary>
        /// Adds the column mapping.
        /// </summary>
        /// <param name="sourceColumn">The source column.</param>
        /// <param name="targetColumn">The target column.</param>
        /// <returns>
        /// This instance for a fluent interface.
        /// </returns>
        public IBulkCopy AddColumnMapping(string sourceColumn, string targetColumn)
        {
            this.bulkCopy.ColumnMappings.Add(sourceColumn, targetColumn);
            return this;
        }

        /// <summary>
        /// Copies all rows in the supplied System.Data.IDataReader to a destination table.
        /// </summary>
        /// <param name="reader">A <see cref="System.Data.IDataReader"/> whose rows will be copied to the destination table.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task WriteToServerAsync(IDataReader reader, CancellationToken cancellationToken)
        {
            try
            {
                this.bulkCopy.NotifyAfter = 100000;
                this.bulkCopy.SqlRowsCopied += this.OnSqlRowsCopied;

                await this.bulkCopy.WriteToServerAsync(reader, cancellationToken);
            }
            finally
            {
                this.bulkCopy.SqlRowsCopied -= this.OnSqlRowsCopied;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.bulkCopy.Close();
            }
        }

        /// <summary>
        /// Handles the <c>SqlRowsCopied</c> event of the bulkCopy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SqlRowsCopiedEventArgs"/> instance containing the event data.</param>
        private void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            DittoEventSource.Log.BulkCopyProgress(this.bulkCopy.DestinationTableName, e.RowsCopied);
        }
    }
}
