#region Copyright (c) all rights reserved.
// <copyright file="DataOperation.cs">
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
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Ditto.DomainEvents;
    using Ditto.Core;
    using Ditto.DataLoad.DomainEvents;

    /// <summary>
    /// Data Operation.
    /// </summary>
    public class DataOperation : IDataOperationInfo
    {
        /// <summary>
        /// The source.
        /// </summary>
        private readonly IDataSource source;

        /// <summary>
        /// The target.
        /// </summary>
        private readonly IDataTarget target;

        /// <summary>
        /// The operation timer.
        /// </summary>
        private readonly Stopwatch timer;

        /// <summary>
        /// The operation identifier.
        /// </summary>
        private readonly Guid operationId = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataOperation"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public DataOperation(IDataSource source, IDataTarget target)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            this.source = source;
            this.source.Parent = this;

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            this.target = target;
            this.target.Parent = this;

            this.timer = new Stopwatch();
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public IDataSource Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public IDataTarget Target
        {
            get { return this.target; }
        }

        /// <summary>
        /// Gets the operation identifier.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        public Guid OperationId
        {
            get { return this.operationId; }
        }

        /// <summary>
        /// Runs the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A completion task.</returns>
        public async Task Run(CancellationToken cancellationToken)
        {
            var highWatermark = this.target.PreviousHighWatermark;

            if (this.source.HasData(highWatermark) && !this.target.Exists)
            {
                EventPublisher.Raise(new CreatingTargetTableEvent { TargetName = this.target.Name, OperationId = this.OperationId });

                using (var sourceSchema = this.source.GetSchemaTable())
                {
                    if (sourceSchema.Rows.Count == 0)
                    {
                        // We can't create a table with no columns.
                        EventPublisher.Raise(
                            new NoMatchingColumnsInSourceEvent
                            {
                                Operation = this,
                                SourceColumns = Enumerable.Empty<string>()
                            });

                        return;
                    }

                    this.target.CreateTable(sourceSchema);
                }
            }

            if (Program.Options.CreateOnly)
            {
                return;
            }

            // We want to truncate the source even if there is no source data.
            this.target.InitializeBatch();

            if (!this.source.HasData(highWatermark))
            {
                EventPublisher.Raise(
                    new NoNewDataEvent
                    {
                        Operation = this,
                        HighWatermark = highWatermark
                    });

                return;
            }

            var targetColumns = this.target.Columns;
            var sourceColumns = this.source.GetColumns(highWatermark);
            var commonColumns = targetColumns.Intersect(sourceColumns);

            if (commonColumns.Count() == 0)
            {
                EventPublisher.Raise(
                    new NoMatchingColumnsInSourceEvent
                    {
                        Operation = this,
                        SourceColumns = sourceColumns
                    });

                return;
            }

            EventPublisher.Raise(
                new QueryingSourceEvent 
                {
                    SourceName = this.source.Name,
                    ConnectionName = this.source.ConnectionName,
                    OperationId = this.operationId 
                });

            this.timer.Start();

            try
            {
                using (var sourceData = this.source.GetData(commonColumns, highWatermark))
                using (var bulkCopy = this.target.CreateBulkCopy())
                {
                    EventPublisher.Raise(
                        new SourceQueryEvent
                        {
                            SourceName = this.source.Name,
                            ConnectionName = this.source.ConnectionName,
                            Duration = this.timer.Elapsed,
                            OperationId = this.operationId
                        });

                    foreach (var column in commonColumns)
                    {
                        bulkCopy.AddColumnMapping(column, column);
                    }

                    // Sometimes there are no new files to read so the best we can do it return
                    // null from source.GetData().
                    if (sourceData != null)
                    {
                        await this.CopyData(sourceData, bulkCopy, cancellationToken);
                    }
                }
            }
            catch (IOException ex)
            {
                // Ignore file locking and just stop here.
                if (!ex.IsFileLocked())
                {
                    EventPublisher.Raise(new SourceFileLockedEvent { Exception = ex });
                    throw;
                }
            }

            this.timer.Stop();
        }

        /// <summary>
        /// Extracts the error properties.
        /// </summary>
        /// <param name="statisticalReader">The statistical reader.</param>
        /// <returns>A set of name value pair properties to help with diagnostics.</returns>
        private static IDictionary<string, string> ExtractErrorProperties(StatisticsDataReader statisticalReader)
        {
            var properties = new Dictionary<string, string>();

            properties["_RowCount"] = statisticalReader.RowCount.ToString(CultureInfo.CurrentCulture);
            properties["_ResultCount"] = statisticalReader.ResultCount.ToString(CultureInfo.CurrentCulture);

            for (int i = 0; i < statisticalReader.FieldCount; ++i)
            {
                properties[statisticalReader.GetName(i)] = statisticalReader.GetValue(i).ToString();
            }

            return properties;
        }

        /// <summary>
        /// Copies the data.
        /// </summary>
        /// <param name="sourceData">The source data.</param>
        /// <param name="bulkCopy">The bulk copy target.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous completion token.</returns>
        private async Task CopyData(IDataReader sourceData, IBulkCopy bulkCopy, CancellationToken cancellationToken)
        {
            var statisticalReader = new StatisticsDataReader(sourceData);

            do
            {
                try
                {
                    await this.CopyChunk(statisticalReader, bulkCopy, cancellationToken);
                }
                catch (Exception ex)
                {
                    var properties = ExtractErrorProperties(statisticalReader);

                    EventPublisher.Raise(new OperationErrorEvent(this, ex, properties));

                    throw;
                }
            }
            while (!cancellationToken.IsCancellationRequested && statisticalReader.NextResult());

            this.target.BatchComplete();

            EventPublisher.Raise(new BatchCompleteEvent
            {
                Operation = this,
                Duration = this.timer.Elapsed,
                RowCount = statisticalReader.RowCount,
                ResultCount = statisticalReader.ResultCount
            });
        }

        /// <summary>
        /// Copies the chunk.
        /// </summary>
        /// <param name="reader">The statistical reader.</param>
        /// <param name="bulkCopy">The bulk copy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous completion token.</returns>
        private async Task CopyChunk(StatisticsDataReader reader, IBulkCopy bulkCopy, CancellationToken cancellationToken)
        {
            EventPublisher.Raise(new ChunkCopyingEvent
            {
                TargetName = this.target.Name,
                RowCount = reader.RowCount,
                ResultCount = reader.ResultCount,
                OperationId = this.operationId
            });

            var chunkTimer = new Stopwatch();
            chunkTimer.Start();

            this.target.InitializeChunk();

            await bulkCopy.WriteToServerAsync(reader, cancellationToken);

            chunkTimer.Stop();

            this.target.ChunkComplete();

            EventPublisher.Raise(new ChunkCopiedEvent
            {
                TargetName = this.target.Name,
                Duration = chunkTimer.Elapsed,
                RowCount = reader.RowCount,
                ResultCount = reader.ResultCount,
                OperationId = this.operationId
            });
        }
    }
}
