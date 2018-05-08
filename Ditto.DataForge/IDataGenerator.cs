#region Copyright (c) all rights reserved.
// <copyright file="IDataGenerator.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataForge
{
    /// <summary>
    /// IDataGenerator interface definition.
    /// </summary>
    public interface IDataGenerator
    {
        /// <summary>
        /// Gets the validator name.
        /// </summary>
        /// <value>
        /// The validator name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Returns the next value for the data element.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The next value</returns>
        object NextValue(GenerationContext context);
    }
}
