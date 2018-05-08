#region Copyright (c) all rights reserved.
// <copyright file="MultipleCsvDataReader.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System;
    using System.Collections.Generic;
    using Ditto.Core;
    using Csv = CsvHelper.Configuration;

    /// <summary>
    /// Multiple Csv Data Reader.
    /// </summary>
    [CLSCompliant(false)]
    public class MultipleCsvDataReader : PassThroughDataReader
    {
        /// <summary>
        /// The source files.
        /// </summary>
        private readonly Queue<ISourceFile> sourceFiles;

        /// <summary>
        /// The CSV configuration.
        /// </summary>
        private Csv.Configuration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleCsvDataReader" /> class.
        /// </summary>
        /// <param name="sourceFiles">The source files.</param>
        /// <param name="config">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public MultipleCsvDataReader(IEnumerable<ISourceFile> sourceFiles, Csv.Configuration config)
        {
            if (sourceFiles == null)
            {
                throw new ArgumentNullException("sourceFiles");
            }

            this.sourceFiles = new Queue<ISourceFile>(sourceFiles);
            this.config = config ?? throw new ArgumentNullException("config");

            this.NextResultImpl();
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public override bool IsClosed
        {
            get { return this.sourceFiles.Count == 0 && this.Reader.IsClosed; }
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// True if there are more rows; otherwise, false.
        /// </returns>
        public override bool NextResult()
        {
            return this.NextResultImpl();
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <remarks>Added to avoid a virtual call in the constructor.</remarks>
        /// <returns>
        /// True if there are more rows; otherwise, false.
        /// </returns>
        private bool NextResultImpl()
        {
            if (this.sourceFiles.Count == 0)
            {
                return false;
            }

            if (this.Reader != null)
            {
                this.Reader.Close();
                this.Reader = null;
            }

            var nextFile = this.sourceFiles.Dequeue();
            var reader = nextFile.CreateReader();

            try
            {
                this.Reader = new CsvDataReader(reader, nextFile.FileName, this.config);

                // At this point the CsvDataReader owns the supplied reader so we must ensure
                // that is is not disposed of twice.
                reader = null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return true;
        }
    }
}
