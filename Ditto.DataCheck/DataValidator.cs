#region Copyright (c) all rights reserved.
// <copyright file="DataValidator.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Threading.Tasks;
    using Ditto.Core;

    /// <summary>
    /// Data Validator.
    /// </summary>
    public abstract class DataValidator : IDataValidator
    {
        /// <summary>
        /// The validator name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The connection factory.
        /// </summary>
        private readonly IConnectionFactory connectionFactory;

        /// <summary>
        /// The metadata.
        /// </summary>
        private readonly CheckMetadata metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidator" /> class.
        /// </summary>
        /// <param name="name">The validator name.</param>
        /// <param name="checkType">Type of the check.</param>
        /// <param name="metadata">The metadata.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        /// <exception cref="System.ArgumentException">If <c>metadata.Connection</c> is null.</exception>
        protected DataValidator(string name, string checkType, CheckMetadata metadata)
        {
            this.name = name;

            if (checkType == null)
            {
                throw new ArgumentNullException("checkType");
            }

            this.CheckType = checkType;

            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            this.metadata = metadata;

            if (this.metadata.Connection == null)
            {
                throw new ArgumentException("checkInfo.Connection is null");
            }

            this.connectionFactory = this.metadata.Connection.CreateConnectionFactory();
        }

        /// <summary>
        /// Gets the validator name.
        /// </summary>
        /// <value>
        /// The validator name.
        /// </value>
        public string Name
        {
            get { return this.name ?? this.GenerateName(); }
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
        /// Gets or sets the type of the check.
        /// </summary>
        /// <value>
        /// The type of the check.
        /// </value>
        public string CheckType { get; set; }

        /// <summary>
        /// Gets the connection factory.
        /// </summary>
        /// <value>
        /// The connection factory.
        /// </value>
        protected IConnectionFactory ConnectionFactory
        {
            get { return this.connectionFactory; }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The resulting validation.
        /// </returns>
        public abstract ValidationResult Validate(ValidationContext context);

        /// <summary>
        /// Generates a name from properties.
        /// </summary>
        /// <returns>A string which should be a unique but stable name for the instance.</returns>
        protected abstract string GenerateName();
    }
}
