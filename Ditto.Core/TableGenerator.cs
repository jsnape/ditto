#region Copyright (c) all rights reserved.
// <copyright file="TableGenerator.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Table Generator.
    /// </summary>
    public class TableGenerator : ITableGenerator
    {
        /// <summary>
        /// Generates the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        /// <returns>
        /// The relevant SQL for the equivalent table.
        /// </returns>
        public string GenerateTable(string tableName, DataTable schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException("schema");
            }

            var sql = new StringBuilder();

            sql
                .AppendFormat(CultureInfo.InvariantCulture, "create table {0} (", tableName)
                .AppendLine();

            foreach (DataRow row in schema.Rows)
            {
                // BIFI-1255: Some CSV files are generated from Excel files and have blank columns in.
                if (string.IsNullOrWhiteSpace(row["ColumnName"].ToString()))
                {
                    continue;
                }

                var dataType = this.GetDataType(row);

                sql
                    .AppendFormat(CultureInfo.InvariantCulture, "    [{0}] {1} null,", row["ColumnName"], dataType)
                    .AppendLine();
            }

            sql
                .AppendLine()
                .AppendLine(");");

            return sql.ToString();
        }

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <param name="schemaRow">The schema row.</param>
        /// <returns>The SQL equivalent.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "JS: This needs refactoring but its just a big switch statement")]
        protected virtual string GetDataType(DataRow schemaRow)
        {
            if (schemaRow == null)
            {
                throw new ArgumentNullException("schemaRow");
            }

            var dataType = (Type)schemaRow["DataType"];

            // TODO: better datatype handling - especially around decimal and money.
            switch (dataType.Name)
            {
                case "Decimal":
                    return string.Format(CultureInfo.CurrentCulture, "decimal({0}, {1})", schemaRow["NumericPrecision"], 4);
                case "String":
                    var columnSize = (int)schemaRow["ColumnSize"];
                    var sqlType = columnSize < 5 ? "nchar" : "nvarchar";
                    return string.Format(CultureInfo.CurrentCulture, "{0}({1})", sqlType, schemaRow["ColumnSize"]);
                case "Int64":
                    return "bigint";
                case "Boolean":
                    return "bit";
                case "DateTime":
                    return "datetime2(7)";
                case "DateTimeOffset":
                    return "datetimeoffset";
                case "Double":
                    return "float";
                case "Int32":
                    return "int";
                case "Int16":
                    return "smallint";
                case "Byte":
                    return "tinyint";
                case "Guid":
                    return "uniqueidentifier";
                case "SqlHierarchyId":
                    return "hierarchyid";
                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Unrecognized data type '{0}'.", dataType.Name));
            }
        }
    }
}
