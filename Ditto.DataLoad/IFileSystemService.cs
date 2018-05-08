#region Copyright (c) all rights reserved.
// <copyright file="IFileSystemService.cs">
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
    using Csv = CsvHelper.Configuration;

    /// <summary>
    /// IFileSystemService mocking interface.
    /// </summary>
    [CLSCompliant(false)]
    public interface IFileSystemService
    {
        /// <summary>
        /// Enumerates the files.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <param name="filePattern">The file pattern.</param>
        /// <returns>
        /// A sequence of wrapped FileInfo instances.
        /// </returns>
        IEnumerable<IFileInfo> EnumerateFiles(string path, string filePattern);

        /// <summary>
        /// Creates a CSV data reader from the files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="config">The CSV configuration.</param>
        /// <returns>
        /// An IDataReader instance.
        /// </returns>
        IDataReader CreateDataReader(IEnumerable<IFileInfo> files, Csv.Configuration config);
    }
}
