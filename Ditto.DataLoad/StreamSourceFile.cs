#region Copyright (c) all rights reserved.
// <copyright file="StreamSourceFile.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System.IO;

    /// <summary>
    /// String Source File.
    /// </summary>
    public class StreamSourceFile : ISourceFile
    {
        /// <summary>
        /// The name of the source file.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamSourceFile"/> class.
        /// </summary>
        /// <param name="name">The name of the source file.</param>
        public StreamSourceFile(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get { return this.name; }
        }

        /// <summary>
        /// Creates the reader.
        /// </summary>
        /// <returns>
        /// A new TextReader instance.
        /// </returns>
        public TextReader CreateReader()
        {
            return new StreamReader(this.name);
        }
    }
}
