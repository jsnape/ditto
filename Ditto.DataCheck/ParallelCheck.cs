#region Copyright (c) all rights reserved.
// <copyright file="ParallelCheck.cs">
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
    using System.Linq;
    using System.Xml.Linq;
    using Ditto.Core;

    /// <summary>
    /// Parallel check.
    /// </summary>
    public class ParallelCheck : DataValidator
    {
        /// <summary>
        /// The expected values connection.
        /// </summary>
        private readonly IConnectionFactory expectedConnectionFactory;

        /// <summary>
        /// The expected query.
        /// </summary>
        private readonly string expectedQuery;

        /// <summary>
        /// The actuals query.
        /// </summary>
        private readonly string actualsQuery;

        /// <summary>
        /// The entity name.
        /// </summary>
        private readonly string entityName;

        /// <summary>
        /// The number differences.
        /// </summary>
        private int differences;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelCheck" /> class.
        /// </summary>
        /// <param name="name">The check name.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="expectedConnection">The expected connection.</param>
        /// <param name="expectedQuery">The expected query.</param>
        /// <param name="actualsQuery">The actual values query.</param>
        /// <param name="metadata">The metadata.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If any of the arguments are null.
        /// </exception>
        public ParallelCheck(string name, string entityName, DataConnection expectedConnection, string expectedQuery, string actualsQuery, CheckMetadata metadata)
            : base(name, "ParallelQuery", metadata)
        {
            if (string.IsNullOrEmpty(expectedQuery))
            {
                throw new ArgumentNullException("expectedQuery");
            }

            this.expectedQuery = expectedQuery;

            if (string.IsNullOrEmpty(actualsQuery))
            {
                throw new ArgumentNullException("actualsQuery");
            }

            this.actualsQuery = actualsQuery;

            // If an expected values connection is not supplied then use the default.
            this.expectedConnectionFactory = (expectedConnection ?? metadata.Connection).CreateConnectionFactory();

            this.entityName = entityName;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <remarks>This method is too complex and needs refactoring.</remarks>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The resulting validation.
        /// </returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "JS: Admin permissions required to edit source.")]
        public override ValidationResult Validate(ValidationContext context)
        {
            using (IDbConnection expectedConnection = this.expectedConnectionFactory.CreateConnection())
            using (IDbConnection actualsConnection = this.ConnectionFactory.CreateConnection())
            {
                expectedConnection.Open();
                actualsConnection.Open();

                var expectedCommand = expectedConnection.CreateCommand();
                expectedCommand.CommandText = this.expectedQuery;
                expectedCommand.CommandTimeout = 600;

                var actualsCommand = actualsConnection.CreateCommand();
                actualsCommand.CommandText = this.actualsQuery;
                actualsCommand.CommandTimeout = 600;

                var additionalInformation = new List<XElement>();
                
                additionalInformation.Add(
                    new XElement("ExpectedConnection", new XCData(this.expectedConnectionFactory.Name)));
                
                additionalInformation.Add(
                    new XElement("ExpectedQuery", new XCData(this.expectedQuery)));
                
                additionalInformation.Add(
                    new XElement("ActualsQuery", new XCData(this.actualsQuery)));

                using (var expectedResults = new StatisticsDataReader(expectedCommand.ExecuteReader()))
                using (var actualValues = new StatisticsDataReader(actualsCommand.ExecuteReader()))
                {
                    bool checkExtraRows = true;

                    while (expectedResults.Read() && actualValues.Read())
                    {
                        if (expectedResults.FieldCount != actualValues.FieldCount)
                        {
                            // The results are different since they have a differing 
                            // number of columns.
                            additionalInformation.Add(
                                new XElement(
                                    "ColumnCountDifference", 
                                    new XAttribute("Expected", expectedResults.FieldCount),
                                    new XAttribute("Actual", actualValues.FieldCount)));

                            ++this.differences;
                            checkExtraRows = false;
                            break;
                        }

                        var rowDifferences =
                            this.CheckRowDifferences(expectedResults, actualValues)
                            .ToArray();

                        if (rowDifferences.Length > 0)
                        {
                            additionalInformation.Add(new XElement("Row", rowDifferences));
                        }
                    }

                    if (checkExtraRows)
                    {
                        additionalInformation.AddRange(
                            this.CheckRowCountDifferences(expectedResults, actualValues));
                    }
                }

                return new ValidationResult
                {
                    Value = this.differences,
                    Metadata = this.Metadata,
                    Status = this.differences <= this.Metadata.Goal ? 1 : -1,
                    CheckName = this.Name,
                    CheckType = this.CheckType,
                    EntityName = this.entityName,
                    AdditionalInformation = new XElement("AdditionalInformation", additionalInformation)
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

        /// <summary>
        /// Checks the row count differences.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="actualValues">The actual values.</param>
        /// <returns>A sequence of row count difference elements.</returns>
        private IEnumerable<XElement> CheckRowCountDifferences(StatisticsDataReader expectedResults, StatisticsDataReader actualValues)
        {
            while (expectedResults.Read())
            {
            }

            while (actualValues.Read())
            {
            }

            if (expectedResults.RowCount != actualValues.RowCount)
            {
                ++this.differences;

                yield return
                    new XElement(
                        "RowCountDifference",
                        new XAttribute("Expected", expectedResults.RowCount),
                        new XAttribute("Actual", actualValues.RowCount));
            }
        }

        /// <summary>
        /// Checks the row differences.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="actualValues">The actual values.</param>
        /// <returns>A sequence of field differences.</returns>
        private IEnumerable<XElement> CheckRowDifferences(IDataReader expectedResults, IDataReader actualValues)
        {
            var fieldDifferences = new List<XElement>();

            for (int i = 0; i < expectedResults.FieldCount; ++i)
            {
                var expectedValue = expectedResults.GetValue(i);
                var actualValue = actualValues.GetValue(i);

                if (!expectedValue.Equals(actualValue))
                {
                    var expectedColumnName = expectedResults.GetName(i);
                    var actualColumnName = actualValues.GetName(i);

                    ++this.differences;

                    yield return
                        new XElement(
                            "ColumnDifference",
                            new XAttribute("ExpectedRow", expectedResults.GetValue(0)),
                            new XAttribute("ExpectedColumn", expectedColumnName),
                            new XAttribute("ExpectedValue", expectedValue),
                            new XAttribute("ActualRow", actualValues.GetValue(0)),
                            new XAttribute("ActualColumn", actualColumnName),
                            new XAttribute("ActualValue", actualValue));
                }
            }
        }
    }
}
