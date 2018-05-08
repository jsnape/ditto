#region Copyright (c) all rights reserved.
// <copyright file="NullColumnValidator.cs">
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
    using Ditto.Core;

    /// <summary>
    /// Null Column Validator.
    /// </summary>
    public class NullColumnValidator : SingleColumnValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullColumnValidator" /> class.
        /// </summary>
        /// <param name="name">The validator name.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="metadata">The metadata.</param>
        public NullColumnValidator(string name, string entityName, string columnName, CheckMetadata metadata)
            : base(name, entityName, columnName, "NotNull", metadata)
        {
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

                    var invalid = (int)reader["NullRecords"];
                    var total = (int)reader["TotalRecords"];

                    var additionalInformation =
                        new XElement(
                            "NullColumnInfo", 
                            new XElement("ColumnName", this.ColumnName),
                            new XElement("Filter", new XCData(this.Metadata.Filter ?? string.Empty)),
                            new XElement("NullRecords", invalid),
                            new XElement("TotalRecords", total));

                    var value = total != 0 ? (100.0 * invalid) / total : 0;
                    
                    var status = 0;

                    if (total != 0)
                    {
                        status = value <= this.Metadata.Goal ? 1 : -1;
                    }

                    return new ValidationResult
                        {
                            Value = value,
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
select
    isnull(sum(case when {0} is null then 1 else 0 end), 0) as NullRecords,
    count(*) as TotalRecords
from {1} with (nolock)
where {2}
";

            return string.Format(CultureInfo.CurrentCulture, Template, this.ColumnName, this.EntityName, this.FilterOrDefault);
        }
    }
}
