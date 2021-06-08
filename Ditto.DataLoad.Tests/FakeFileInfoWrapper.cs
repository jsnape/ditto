// <copyright file="FakeFileInfoWrapper.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace Ditto.DataLoad.Tests
{
    using System;

    /// <summary>
    /// FileInfo Wrapper.
    /// </summary>
    internal class FakeFileInfoWrapper : IFileInfo
    {
        /// <summary>
        /// The full name.
        /// </summary>
        private readonly string fullName;

        /// <summary>
        /// The last write time UTC.
        /// </summary>
        private readonly DateTime lastWriteTimeUtc;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeFileInfoWrapper"/> class.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="lastWriteTimeUtc">The last write time UTC.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public FakeFileInfoWrapper(string fullName, DateTime lastWriteTimeUtc)
        {
            this.fullName = fullName ?? throw new ArgumentNullException("fullName");
            this.lastWriteTimeUtc = lastWriteTimeUtc;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>
        /// Gets the last write time UTC.
        /// </summary>
        /// <value>
        /// The last write time UTC.
        /// </value>
        public DateTime LastWriteTimeUtc
        {
            get { return this.lastWriteTimeUtc; }
        }
    }
}
