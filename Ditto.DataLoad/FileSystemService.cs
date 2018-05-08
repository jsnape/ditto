#region Copyright (c) all rights reserved.
// <copyright file="FileSystemService.cs">
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
    using System.IO;
    using System.Linq;
    using Csv = CsvHelper.Configuration;

    /// <summary>
    /// File System Service.
    /// </summary>
    [CLSCompliant(false)]
    public class FileSystemService : IFileSystemService
    {
        /// <summary>
        /// Enumerates the files.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <param name="filePattern">The file pattern.</param>
        /// <returns>
        /// A sequence of wrapped FileInfo instances.
        /// </returns>
        public IEnumerable<IFileInfo> EnumerateFiles(string path, string filePattern)
        {
            var directory = new DirectoryInfo(path);
            
            var files = 
                directory.GetFiles(filePattern)
                .Select(f => new FileInfoWrapper(f));
            
            return files;
        }

        /// <summary>
        /// Creates a CSV data reader from the files.
        /// </summary>
        /// <param name="files">A sequence of files.</param>
        /// <param name="config">The CSV configuration.</param>
        /// <returns>
        /// An IDataReader instance.
        /// </returns>
        public IDataReader CreateDataReader(IEnumerable<IFileInfo> files, Csv.Configuration config)
        {
            return new MultipleCsvDataReader(
                files.Select(f => new StreamSourceFile(f.FullName)),
                config);
        }

        /// <summary>
        /// FileInfo Wrapper.
        /// </summary>
        private class FileInfoWrapper : IFileInfo
        {
            /// <summary>
            /// The file being wrapped.
            /// </summary>
            private readonly FileInfo file;

            /// <summary>
            /// Initializes a new instance of the <see cref="FileInfoWrapper"/> class.
            /// </summary>
            /// <param name="file">The file to wrap.</param>
            /// <exception cref="System.ArgumentNullException">If file is null.</exception>
            public FileInfoWrapper(FileInfo file)
            {
                this.file = file ?? throw new ArgumentNullException("file");
            }

            /// <summary>
            /// Gets the full name.
            /// </summary>
            /// <value>
            /// The full name.
            /// </value>
            public string FullName
            {
                get { return this.file.FullName; }
            }

            /// <summary>
            /// Gets the last write time UTC.
            /// </summary>
            /// <value>
            /// The last write time UTC.
            /// </value>
            public DateTime LastWriteTimeUtc
            {
                get { return this.file.LastWriteTimeUtc; }
            }
        }
    }
}
