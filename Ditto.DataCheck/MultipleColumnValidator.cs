#region Copyright (c) all rights reserved.
// <copyright file="MultipleColumnValidator.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ditto.Core;

    /// <summary>
    /// Multiple Column Validator.
    /// </summary>
    public abstract class MultipleColumnValidator : EntityValidator
    {
        /// <summary>
        /// The columns.
        /// </summary>
        private readonly string[] columnNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleColumnValidator" /> class.
        /// </summary>
        /// <param name="name">The validator name.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="columnNames">A comma separated list of column names.</param>
        /// <param name="checkType">Type of the check.</param>
        /// <param name="metadata">The metadata.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        protected MultipleColumnValidator(string name, string entityName, string columnNames, string checkType, CheckMetadata metadata)
            : base(name, entityName, checkType, metadata)
        {
            if (columnNames == null)
            {
                throw new ArgumentNullException("columnNames");
            }

            this.columnNames = columnNames
                .Split(',')
                .Select(n => n.Trim())
                .ToArray();
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        protected IEnumerable<string> ColumnNames
        {
            get { return this.columnNames; }
        }

        /// <summary>
        /// Gets the column list.
        /// </summary>
        /// <value>
        /// The column list.
        /// </value>
        protected string ColumnList
        {
            get { return string.Join(",", this.columnNames); }
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <value>
        /// The column count.
        /// </value>
        protected int ColumnCount
        {
            get { return this.columnNames.Length; }
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

            return connectionName + this.EntityName + ":" + this.ColumnList + ":" + this.CheckType;
        }
    }
}
