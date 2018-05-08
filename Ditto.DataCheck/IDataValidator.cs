#region Copyright (c) all rights reserved.
// <copyright file="IDataValidator.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    /// <summary>
    /// IDataValidator interface definition.
    /// </summary>
    public interface IDataValidator
    {
        /// <summary>
        /// Gets the validator name.
        /// </summary>
        /// <value>
        /// The validator name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        CheckMetadata Metadata { get; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The resulting validation.
        /// </returns>
        ValidationResult Validate(ValidationContext context);
    }
}
