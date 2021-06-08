#region Copyright (c) all rights reserved.
// <copyright file="CsvDataSource.cs">
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
    using System.IO;
    using System.Linq;
    using Ditto.DomainEvents;
    using Ditto.Core;
    using Ditto.DataLoad.DomainEvents;
    using Csv = CsvHelper.Configuration;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// FolderMode enumeration.
    /// </summary>
    public enum FolderMode
    {
        /// <summary>
        /// Loads multiple files in a single batch.
        /// </summary>
        MultipleFile = 0,

        /// <summary>
        /// Loads a single file per batch.
        /// </summary>
        SingleFile,

        /// <summary>
        /// Loads only the last file (assumes that each file is a full extract).
        /// </summary>
        LastFileOnly,
    }

    /// <summary>
    /// Csv Data Source.
    /// </summary>
    [CLSCompliant(false)]
    public class CsvDataSource : IDataSource
    {
        /// <summary>
        /// The source name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The connection.
        /// </summary>
        private readonly DataConnection connection;

        /// <summary>
        /// The file pattern.
        /// </summary>
        private readonly string filePattern;

        /// <summary>
        /// The file system.
        /// </summary>
        private readonly IFileSystemService fileSystem;

        /// <summary>
        /// Folder handling mode.
        /// </summary>
        private readonly FolderMode mode;

        /// <summary>
        /// The CSV configuration.
        /// </summary>
        private readonly Csv.Configuration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataSource" /> class.
        /// </summary>
        /// <param name="name">The name of the data source.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="filePattern">The file pattern.</param>
        /// <param name="mode">The file mode.</param>
        /// <param name="config">The CSV configuration.</param>
        public CsvDataSource(string name, DataConnection connection, string filePattern, FolderMode mode, Csv.Configuration config)
            : this(name, connection, filePattern, mode, config, new FileSystemService())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataSource" /> class.
        /// </summary>
        /// <param name="name">The source name.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="filePattern">The file pattern.</param>
        /// <param name="mode">The file mode.</param>
        /// <param name="config">The CSV configuration.</param>
        /// <param name="fileSystem">The file system.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public CsvDataSource(string name, DataConnection connection, string filePattern, FolderMode mode, Csv.Configuration config, IFileSystemService fileSystem)
        {
            this.name = name ?? throw new ArgumentNullException("name");
            this.connection = connection ?? throw new ArgumentNullException("connection");
            this.filePattern = filePattern ?? throw new ArgumentNullException("filePattern");
            this.config = config ?? throw new ArgumentNullException("config");
            this.fileSystem = fileSystem ?? throw new ArgumentNullException("fileSystem");

            this.mode = mode;
        }

        /// <summary>
        /// Gets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the data source.
        /// </value>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        /// <value>
        /// The name of the connection.
        /// </value>
        public string ConnectionName 
        {
            get { return this.connection.Name; }
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
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>
        /// A sequence of columns.
        /// </returns>
        public IEnumerable<string> GetColumns(Watermark previousHighWatermark)
        {
            // The CSV source is a little complex because we derive the column list from real
            // source data. If there are files we need to use the actual one that is going to be
            // imported. If not then any file will do - even ones below the watermark.
            using (var latestData = this.GetData(Enumerable.Empty<string>(), previousHighWatermark))
            using (var sampleData = this.GetData(Enumerable.Empty<string>(), null))
            {
                var columnData = latestData ?? sampleData;

                if (columnData == null)
                {
                    yield break;
                }

                for (int i = 0; i < columnData.FieldCount; ++i)
                {
                    yield return columnData.GetName(i);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "JS: This follows the correct transfer of ownership pattern")]
        public DataTable GetSchemaTable()
        {
            DataTable tempTable = null;
            DataTable schemaTable = null;

            try
            {
                tempTable = new DataTable
                {
                    Locale = CultureInfo.CurrentCulture
                };

                var columns = new DataColumn[] 
                {
                    new DataColumn("ColumnOrdinal", typeof(int)),
                    new DataColumn("ColumnName", typeof(string)),
                    new DataColumn("ColumnSize", typeof(int)),
                    new DataColumn("DataType", typeof(Type)),
                    new DataColumn("AllowDBNull", typeof(bool)),
                };

                tempTable.Columns.AddRange(columns);

                var columnNames = this.GetColumns(null).ToArray();

                // If the file has no columns (or there are no files) then don't add any more
                // metadata columns.
                if (columnNames.Count() != 0)
                {
                    // Fixed columns get real metadata (until I've implemented the schema functionality)
                    tempTable.Rows.Add(0, "_LineNumber", 0, typeof(int), false);
                    tempTable.Rows.Add(0, "_SourceFile", 260, typeof(string), false);

                    // In particular this one or the high watermarking won't work.
                    tempTable.Rows.Add(0, "_LastWriteTimeUtc", 0, typeof(DateTime), false);

                    for (int i = tempTable.Rows.Count; i < columnNames.Length; ++i)
                    {
                        tempTable.Rows.Add(i, columnNames[i], 255, typeof(string), true);
                    }
                }

                schemaTable = tempTable;
                tempTable = null;
            }
            finally
            {
                if (tempTable != null)
                {
                    tempTable.Dispose();
                }
            }

            return schemaTable;
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
            return this.FindFiles(previousHighWatermark).Any();
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>
        /// A streaming reader for the data.
        /// </returns>
        public IDataReader GetData(IEnumerable<string> columns, Watermark previousHighWatermark)
        {
            try
            {
                IEnumerable<IFileInfo> files = this.FindFiles(previousHighWatermark);

                if (files.Count() == 0)
                {
                    return null;
                }

                EventPublisher.Raise(
                    new SourceFileFoundEvent
                    {
                        OperationId = this.Parent.OperationId,
                        SourceName = this.name,
                        ConnectionName = this.ConnectionName,
                        FoundFiles = files
                    });

                return this.fileSystem.CreateDataReader(files, this.config);
            }
            catch (IOException ex)
            {
                if (ex.IsFileLocked())
                {
                    EventPublisher.Raise(
                        new SourceFileLockedEvent
                        {
                            OperationId = this.Parent.OperationId,
                            SourceName = this.Name,
                            ConnectionName = this.ConnectionName,
                            Exception = ex 
                        });

                    // Back off and wait till next time.
                    return null;
                }

                EventPublisher.Raise(
                    new SourceErrorEvent
                    {
                        OperationId = this.Parent.OperationId,
                        SourceName = this.Name,
                        ConnectionName = this.ConnectionName,
                        Exception = ex
                    });

                throw;
            }
        }

        /// <summary>
        /// Finds the files.
        /// </summary>
        /// <param name="previousHighWatermark">The previous high watermark.</param>
        /// <returns>A sequence of files to import.</returns>
        private IEnumerable<IFileInfo> FindFiles(Watermark previousHighWatermark)
        {
            var allFiles = this.fileSystem.EnumerateFiles(this.connection.ConnectionString, this.filePattern)
                .Where(f => previousHighWatermark == null || f.LastWriteTimeUtc > previousHighWatermark.WatermarkValue);

            IEnumerable<IFileInfo> importFiles = null;

            switch (this.mode)
            {
                case FolderMode.MultipleFile:
                    importFiles = allFiles.OrderBy(f => f.LastWriteTimeUtc);
                    break;
                case FolderMode.SingleFile:
                    importFiles = allFiles.OrderBy(f => f.LastWriteTimeUtc).Take(1);
                    break;
                case FolderMode.LastFileOnly:
                    importFiles = allFiles.OrderByDescending(f => f.LastWriteTimeUtc).Take(1);
                    break;
            }

            return importFiles;
        }
    }
}
