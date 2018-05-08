#region Copyright (c) all rights reserved.
// <copyright file="GenericCheck.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Xml.Linq;

    /// <summary>
    /// Generic check.
    /// </summary>
    public class GenericCheck : DataValidator
    {
        /// <summary>
        /// The check query.
        /// </summary>
        private readonly string query;

        /// <summary>
        /// The entity name.
        /// </summary>
        private readonly string entityName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCheck" /> class.
        /// </summary>
        /// <param name="name">The check name.</param>
        /// <param name="entityName">Optional name of the entity.</param>
        /// <param name="query">The query.</param>
        /// <param name="metadata">The metadata.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public GenericCheck(string name, string entityName, string query, CheckMetadata metadata)
            : base(name, "GenericQuery", metadata)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException("query");
            }

            this.query = query;

            this.entityName = entityName;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The resulting validation.
        /// </returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "JS: Admin permissions required to edit source.")]
        public override ValidationResult Validate(ValidationContext context)
        {
            using (IDbConnection connection = this.ConnectionFactory.CreateConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = this.query;

                // Checks should run pretty quickly or else they are stealing resources from real clients.
                command.CommandTimeout = 60;

                var result = command.ExecuteScalar();

                var value = result == DBNull.Value ? this.Metadata.Goal : (double)Convert.ChangeType(result, typeof(double), CultureInfo.CurrentCulture);

                return new ValidationResult
                {
                    Value = value,
                    Metadata = this.Metadata,
                    Status = value <= this.Metadata.Goal ? 1 : -1,
                    CheckName = this.Name,
                    CheckType = this.CheckType,
                    EntityName = this.entityName,
                    AdditionalInformation = new XElement("Query", new XCData(this.query))
                };
            }
        }

        /// <summary>
        /// Generates a name from properties.
        /// </summary>
        /// <returns>
        /// A string which should be a unique but stable name for the instance.
        /// </returns>
        protected override string GenerateName()
        {
            // Names should be supplied.
            return Guid.NewGuid().ToString();
        }
    }
}
