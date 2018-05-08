#region Copyright (c) all rights reserved.
// <copyright file="FakeSchemaFactory.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Fake Schema Factory.
    /// </summary>
    public static class FakeSchemaFactory
    {
        /// <summary>
        /// Creates the default schema table.
        /// </summary>
        /// <returns>A data table.</returns>
        public static DataTable CreateDefaultTableSchema()
        {
            DataTable table = null;

            try
            {
                table = new DataTable();
                table.Locale = CultureInfo.CurrentCulture;

                var columns = new DataColumn[] 
                {
                    new DataColumn("ColumnOrdinal", typeof(int)),
                    new DataColumn("ColumnName", typeof(string)),
                    new DataColumn("ColumnSize", typeof(int)),
                    new DataColumn("DataType", typeof(Type)),
                    new DataColumn("AllowDBNull", typeof(bool)),
                    new DataColumn("NumericPrecision", typeof(short)),
                    new DataColumn("NumericScale", typeof(short)),
                };

                table.Columns.AddRange(columns);

                table.Rows.Add(0, "_LineNumber", 0, typeof(int), false, 0, 0);
                table.Rows.Add(1, "_SourceFile", 260, typeof(string), false, 0, 0);
                table.Rows.Add(2, "_LastWriteTimeUtc", 0, typeof(DateTime), false, 0, 0);
                table.Rows.Add(3, "adecimal", 0, typeof(decimal), false, 24, 4);
                table.Rows.Add(4, "abigint", 0, typeof(long), false, 0, 0);
                table.Rows.Add(5, "abit", 0, typeof(bool), false, 0, 0);
                table.Rows.Add(6, "adatetimeoffset", 0, typeof(DateTimeOffset), false, 0, 0);
                table.Rows.Add(7, "adouble", 0, typeof(double), false, 0, 0);
                table.Rows.Add(8, "asmallint", 0, typeof(short), false, 0, 0);
                table.Rows.Add(9, "atinyint", 0, typeof(byte), false, 0, 0);
                table.Rows.Add(10, "aguid", 0, typeof(Guid), false, 0, 0);

                var schemaTable = table;
                table = null;
                return schemaTable;
            }
            finally
            {
                if (table != null)
                {
                    table.Dispose();
                }
            }
        }
    }
}
