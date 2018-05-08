#region Copyright (c) all rights reserved.
// <copyright file="UnknownCheckTypeValidator.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Xml;
    using System.Xml.Linq;
    using Ditto.Core;

    /// <summary>
    /// Unknown check type validator.
    /// </summary>
    /// <remarks>Used to pass info back about unrecognized check types in the script.</remarks>
    public class UnknownCheckTypeValidator : IDataValidator
    {
        /// <summary>
        /// The check element.
        /// </summary>
        private readonly XElement checkElement;

        /// <summary>
        /// The metadata.
        /// </summary>
        private readonly CheckMetadata metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCheckTypeValidator" /> class.
        /// </summary>
        /// <param name="checkElement">The check element.</param>
        /// <param name="metadata">The metadata.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public UnknownCheckTypeValidator(XElement checkElement, CheckMetadata metadata)
        {
            if (checkElement == null)
            {
                throw new ArgumentNullException("checkElement");
            }

            this.checkElement = checkElement;

            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            this.metadata = metadata;
        }

        /// <summary>
        /// Gets the validator name.
        /// </summary>
        /// <value>
        /// The validator name.
        /// </value>
        public string Name
        {
            get { return this.checkElement.FindAttribute("Name") ?? Guid.NewGuid().ToString(); }
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public CheckMetadata Metadata
        {
            get { return this.metadata; }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The resulting validation.
        /// </returns>
        public ValidationResult Validate(ValidationContext context)
        {
            return new ValidationResult
                {
                    CheckName = this.Name,
                    CheckType = "UnknownCheckType",
                    Metadata = this.Metadata,
                    AdditionalInformation = 
                        new XElement(
                            "UnknownCheck",
                            new XAttribute("Line", ((IXmlLineInfo)this.checkElement).LineNumber),
                            this.checkElement)
                };
        }
    }
}
