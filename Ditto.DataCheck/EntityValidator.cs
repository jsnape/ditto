#region Copyright (c) all rights reserved.
// <copyright file="EntityValidator.cs">
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
    /// Entity Validator.
    /// </summary>
    public abstract class EntityValidator : DataValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityValidator" /> class.
        /// </summary>
        /// <param name="name">The validator name.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="checkType">Type of the check.</param>
        /// <param name="metadata">The metadata.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        protected EntityValidator(string name, string entityName, string checkType, CheckMetadata metadata)
            : base(name, checkType, metadata)
        {
            if (entityName == null)
            {
                throw new ArgumentNullException("entityName");
            }

            this.EntityName = entityName;
        }

        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the entity.
        /// </value>
        public string EntityName { get; private set; }

        /// <summary>
        /// Gets the filter or default.
        /// </summary>
        /// <value>
        /// The filter or default.
        /// </value>
        protected string FilterOrDefault
        {
            get { return string.IsNullOrEmpty(this.Metadata.Filter) ? "1 = 1" : this.Metadata.Filter; }
        }

        /// <summary>
        /// Generates a name from properties.
        /// </summary>
        /// <returns>
        /// A string which should be a unique but stable name for the instance.
        /// </returns>
        protected override string GenerateName()
        {
            var connectionName = this.Metadata.Connection.Name == "target" ? string.Empty : this.Metadata.Connection.Name + ":";

            return connectionName + this.EntityName + ":" + this.CheckType;
        }
    }
}
