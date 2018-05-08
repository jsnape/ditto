#region Copyright (c) all rights reserved.
// <copyright file="ISourceFile.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System.IO;

    /// <summary>
    /// ISourceFile interface definition.
    /// </summary>
    public interface ISourceFile
    {
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        string FileName { get; }

        /// <summary>
        /// Creates the reader.
        /// </summary>
        /// <returns>A new TextReader instance.</returns>
        TextReader CreateReader();
    }
}
