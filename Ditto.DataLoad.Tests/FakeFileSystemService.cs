// <copyright file="FakeFileSystemService.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace Ditto.DataLoad.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Csv = CsvHelper.Configuration;

    /// <summary>
    /// Fake FileSystemService.
    /// </summary>
    [CLSCompliant(false)]
    public class FakeFileSystemService : IFileSystemService
    {
        /// <summary>
        /// The files.
        /// </summary>
        private readonly IEnumerable<IFileInfo> fakeFiles;

        /// <summary>
        /// The file contents.
        /// </summary>
        private readonly IEnumerable<string> fileContents;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeFileSystemService" /> class.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="fileContents">The file contents.</param>
        /// <exception cref="System.ArgumentNullException">If any parameters are null.</exception>
        public FakeFileSystemService(IEnumerable<IFileInfo> files, IEnumerable<string> fileContents)
        {
            this.fakeFiles = files ?? throw new ArgumentNullException("files");
            this.fileContents = fileContents ?? throw new ArgumentNullException("fileContents");
        }

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
            return this.fakeFiles;
        }

        /// <summary>
        /// Creates a CSV data reader from the files.
        /// </summary>
        /// <param name="files">The files to load.</param>
        /// <param name="config">The CSV configuration.</param>
        /// <returns>
        /// An IDataReader instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If files is null.</exception>
        public IDataReader CreateDataReader(IEnumerable<IFileInfo> files, Csv.Configuration config)
        {
            if (files == null)
            {
                throw new ArgumentNullException("files");
            }

            var sourceFiles = 
                files.Zip(this.fileContents, (l, r) => new StringSourceFile(l.FullName, r));

            return new MultipleCsvDataReader(sourceFiles, config);
        }
    }
}
