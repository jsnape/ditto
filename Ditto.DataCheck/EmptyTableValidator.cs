#region Copyright (c) all rights reserved.
// <copyright file="EmptyTableValidator.cs">
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
    /// Empty table validator.
    /// </summary>
    public class EmptyTableValidator : EntityValidator
    {
        /// <summary>
        /// The invert.
        /// </summary>
        private bool invert;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyTableValidator" /> class.
        /// </summary>
        /// <param name="name">The validator name.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="invert">If set to <c>true</c> [invert].</param>
        /// <param name="metadata">The metadata.</param>
        public EmptyTableValidator(string name, string entityName, bool invert, CheckMetadata metadata)
            : base(name, entityName, invert ? "NotEmpty" : "Empty", metadata)
        {
            this.invert = invert;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The resulting validation.
        /// </returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "JS: No external strings used")]
        public override ValidationResult Validate(ValidationContext context)
        {
            using (IDbConnection connection = this.ConnectionFactory.CreateConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = this.GenerateCheckSql();

                // Checks should run pretty quickly or else they are stealing resources from real clients.
                command.CommandTimeout = 300;

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException("Execute reader should return at least one row.");
                    }

                    var count = reader.GetInt32(0);

                    var additionalInformation =
                        new XElement(
                            "EmptyTableInfo",
                            new XElement("Filter", new XCData(this.Metadata.Filter ?? string.Empty)),
                            new XElement("RowCount", count));

                    var status = count == 0 ^ this.invert ? 1 : -1;

                    return new ValidationResult
                    {
                        Value = count,
                        Metadata = this.Metadata,
                        Status = status,
                        CheckName = this.Name,
                        CheckType = this.CheckType,
                        EntityName = this.EntityName,
                        AdditionalInformation = additionalInformation
                    };
                }
            }
        }

        /// <summary>
        /// Generates the check SQL.
        /// </summary>
        /// <returns>The SQL used for the check.</returns>
        private string GenerateCheckSql()
        {
            const string Template = @"
select count(*)
from {0} with (nolock)
where {1}
";

            return string.Format(CultureInfo.CurrentCulture, Template, this.EntityName, this.FilterOrDefault);
        }
    }
}
