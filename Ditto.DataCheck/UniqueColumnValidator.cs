#region Copyright (c) all rights reserved.
// <copyright file="UniqueColumnValidator.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Ditto.Core;

    /// <summary>
    /// Unique Column Validator.
    /// </summary>
    public class UniqueColumnValidator : MultipleColumnValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueColumnValidator" /> class.
        /// </summary>
        /// <param name="name">The validator name.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="columnNames">A comma separated list of column names.</param>
        /// <param name="metadata">The metadata.</param>
        public UniqueColumnValidator(string name, string entityName, string columnNames, CheckMetadata metadata)
            : base(name, entityName, columnNames, "UniqueColumn", metadata)
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
                command.CommandTimeout = 60;

                using (var reader = command.ExecuteReader())
                {
                    var duplicates = new List<XElement>();

                    if (!string.IsNullOrEmpty(this.Metadata.Filter))
                    {
                        duplicates.Add(new XElement("Filter", new XCData(this.Metadata.Filter)));
                    }

                    while (reader.Read())
                    {
                        var columns =
                            this.ColumnNames
                            .Select(n => new XElement(n, reader[n].ToString()));

                        var duplicateCount = new XElement("DuplicateCount", (int)reader["DuplicateCount"]);

                        duplicates.Add(new XElement("Duplicate", columns, duplicateCount));
                    }

                    var additionalInformation = new XElement("UniqueColumnInfo", duplicates);

                    return new ValidationResult
                    {
                        Value = duplicates.Count,
                        Metadata = this.Metadata,
                        Status = duplicates.Count <= this.Metadata.Goal ? 1 : -1,
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
select {0}, count(*) as DuplicateCount
from {1} with (nolock)
where {2}
group by {0}
having count(*) > 1
";

            return string.Format(CultureInfo.CurrentCulture, Template, this.ColumnList, this.EntityName, this.FilterOrDefault);
        }
    }
}
