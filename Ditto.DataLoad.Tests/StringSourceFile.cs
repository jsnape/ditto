#region Copyright (c) all rights reserved.
// <copyright file="StringSourceFile.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.Tests
{
    using System.IO;

    /// <summary>
    /// String Source File.
    /// </summary>
    public class StringSourceFile : ISourceFile
    {
        /// <summary>
        /// The name of the source.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The contents.
        /// </summary>
        private readonly string contents;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSourceFile"/> class.
        /// </summary>
        /// <param name="name">The name of the source file.</param>
        /// <param name="contents">The contents.</param>
        public StringSourceFile(string name, string contents)
        {
            this.name = name;
            this.contents = contents;
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
            return new StringReader(this.contents);
        }
    }
}
