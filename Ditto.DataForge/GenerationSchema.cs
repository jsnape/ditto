#region Copyright (c) all rights reserved.
// <copyright file="ValidationContext.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataForge
{
    using System.Collections.Generic;

    /// <summary>
    /// Generation Schema
    /// </summary>
    public class GenerationSchema
    {
        /// <summary>
        /// Gets the generators.
        /// </summary>
        /// <value>
        /// The generators.
        /// </value>
        public IEnumerable<IDataGenerator> Generators { get; }
    }
}