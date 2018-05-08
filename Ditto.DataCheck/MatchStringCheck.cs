#region Copyright (c) all rights reserved.
// <copyright file="MatchStringCheck.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    /// <summary>
    /// Match string check.
    /// </summary>
    public class MatchStringCheck : SingleColumnValidator
    {
        /// <summary>
        /// The regex.
        /// </summary>
        private readonly Regex regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchStringCheck" /> class.
        /// </summary>
        /// <param name="name">The validator name.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="pattern">The regex pattern.</param>
        /// <param name="options">The regex options.</param>
        /// <param name="metadata">The metadata.</param>
        public MatchStringCheck(string name, string entityName, string columnName, string pattern, RegexOptions options, CheckMetadata metadata)
            : base(name, entityName, columnName, "RegexMatch", metadata)
        {
            this.regex = new Regex(pattern, options | RegexOptions.Compiled);
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
                    var totalValues = 0;
                    var unmatchedValues = new List<string>();

                    while (reader.Read())
                    {
                        ++totalValues;

                        if (reader.IsDBNull(0))
                        {
                            continue;
                        }

                        var columnValue = reader.GetString(0);

                        if (!this.regex.IsMatch(columnValue))
                        {
                            unmatchedValues.Add(columnValue);
                        }
                    }

                    var additionalInformation =
                        new XElement(
                            "UnmatchedValueInfo",
                            new XElement("ColumnName", this.ColumnName),
                            new XElement("Filter", new XCData(this.Metadata.Filter ?? string.Empty)),
                            new XElement("UnmatchedRecords", unmatchedValues.Count),
                            new XElement("TotalRecords", totalValues),
                            unmatchedValues.Select(v => new XElement("Unmatched", v)));

                    var value = totalValues != 0 ? (100.0 * unmatchedValues.Count) / totalValues : 0;

                    var status = 0;

                    if (totalValues != 0)
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
select distinct {0} as ColumnValue
from {1} with (nolock)
where {2}
";

            return string.Format(CultureInfo.CurrentCulture, Template, this.ColumnName, this.EntityName, this.FilterOrDefault);
        }
    }
}
