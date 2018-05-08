#region Copyright (c) all rights reserved.
// <copyright file="IFileInfo.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System;

    /// <summary>
    /// IFileInfo interface definition.
    /// </summary>
    public interface IFileInfo
    {
        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        string FullName { get; }

        /// <summary>
        /// Gets the last write time UTC.
        /// </summary>
        /// <value>
        /// The last write time UTC.
        /// </value>
        DateTime LastWriteTimeUtc { get; }
    }
}
