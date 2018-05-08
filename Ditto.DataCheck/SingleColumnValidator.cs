#region Copyright (c) all rights reserved.
// <copyright file="SingleColumnValidator.cs">
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
    /// Single Column Validator.
    /// </summary>
    public abstract class SingleColumnValidator : EntityValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleColumnValidator" /> class.
        /// </summary>
        /// <param name="name">The validator name.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="checkType">Type of the check.</param>
        /// <param name="metadata">The metadata.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        protected SingleColumnValidator(string name, string entityName, string columnName, string checkType, CheckMetadata metadata)
            : base(name, entityName, checkType, metadata)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException("columnName");
            }

            this.ColumnName = columnName;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        protected string ColumnName { get; private set; }

        /// <summary>
        /// Generates a name from properties.
        /// </summary>
        /// <returns>
        /// A string which should be a unique but stable name for the instance.
        /// </returns>
        protected override string GenerateName()
        {
            var connectionName = this.Metadata.Connection.Name == "target" ? string.Empty : this.Metadata.Connection.Name + ":";

            return connectionName + this.EntityName + ":" + this.ColumnName + ":" + this.CheckType;
        }
    }
}
