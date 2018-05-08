#region Copyright (c) all rights reserved.
// <copyright file="QueryDataSourceFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.Tests
{
    using System;
    using System.Data;
    using System.Linq;
    using Ditto.Core.Tests;
    using NSubstitute;
    using Xunit;

    /// <summary>
    /// QueryDataSource Facts.
    /// </summary>
    [CLSCompliant(false)]
    public class QueryDataSourceFacts : DbDataFixture
    {
        /// <summary>
        /// The query SQL.
        /// </summary>
        private const string QuerySql = "select * from foo";

        /// <summary>
        /// The target.
        /// </summary>
        private readonly QueryDataSource target;

        /// <summary>
        /// The schema.
        /// </summary>
        private readonly DataTable schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDataSourceFacts"/> class.
        /// </summary>
        public QueryDataSourceFacts()
        {
            this.schema = FakeSchemaFactory.CreateDefaultTableSchema();
            this.target = new QueryDataSource(this.ConnectionFactory, "wibble", QuerySql, this.schema);
        }

        /// <summary>
        /// Should throw when null connection factory passed to constructor.
        /// </summary>
        [Fact]
        public static void ShouldThrowWhenNullConnectionFactoryPassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(
                () => new QueryDataSource(null, "wibble", QuerySql, null));
        }

        /// <summary>
        /// Should throw when nulls passed to constructor.
        /// </summary>
        /// <param name="name">The name of the source.</param>
        /// <param name="query">The query to execute.</param>
        [Theory]
        [InlineData(null, QuerySql)]
        [InlineData("wibble", null)]
        public void ShouldThrowWhenNullsPassedToConstructor(string name, string query)
        {
            Assert.Throws<ArgumentNullException>(
                () => new QueryDataSource(this.ConnectionFactory, name, query, null));
        }

        /// <summary>
        /// Should use supplied schema for columns.
        /// </summary>
        [Fact]
        public void ShouldUseSchemaForColumns()
        {
            var expectedColumns = this.schema
                .Rows
                .Cast<DataRow>()
                .Select(r => r["ColumnName"].ToString());

            var actualColumns = this.target.GetColumns(null);

            Assert.Equal(expectedColumns, actualColumns);
        }

        /// <summary>
        /// Name property should be correct.
        /// </summary>
        [Fact]
        public void NamePropertyShouldBeCorrect()
        {
            Assert.Equal("wibble", this.target.Name);
        }

        /// <summary>
        /// Query should be correct.
        /// </summary>
        [Fact]
        public void CommandTextShouldBeCorrect()
        {
            using (var data = this.target.GetData(Enumerable.Empty<string>(), null))
            {
                Assert.Equal(QuerySql, this.Command.CommandText);
            }

            this.Command.ExecuteReader().Received();
        }
    }
}
