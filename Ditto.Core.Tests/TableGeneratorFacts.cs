#region Copyright (c) all rights reserved.
// <copyright file="TableGeneratorFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core.Tests
{
    using System;
    using System.Text.RegularExpressions;
    using Xunit;

    /// <summary>
    /// TableGenerator Facts.
    /// </summary>
    [CLSCompliant(false)]
    public class TableGeneratorFacts
    {
        /// <summary>
        /// The target.
        /// </summary>
        private ITableGenerator target;

        /// <summary>
        /// The generated SQL.
        /// </summary>
        private string generatedSql;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableGeneratorFacts"/> class.
        /// </summary>
        public TableGeneratorFacts()
        {
            this.target = new TableGenerator();

            using (var schema = FakeSchemaFactory.CreateDefaultTableSchema())
            {
                this.generatedSql = this.target.GenerateTable("TEST TABLE", schema);
            }
        }

        /// <summary>
        /// Should contain table name.
        /// </summary>
        [Fact]
        public void ShouldContainTableName()
        {
            Assert.Contains("TEST TABLE", this.generatedSql, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Should contain correct columns and types.
        /// </summary>
        /// <param name="match">The match.</param>
        [Theory]
        [InlineData(@"^\s*\[_LineNumber]\s*int\s*null\s*\,\s*$")]
        [InlineData(@"^\s*\[_SourceFile]\s*nvarchar\(260\)\s*null\s*\,\s*$")]
        [InlineData(@"^\s*\[_LastWriteTimeUtc]\s*datetime2\(7\)\s*null\s*\,\s*$")]
        [InlineData(@"^\s*\[adecimal]\s*decimal\(24\s*,\s*4\)\s*null\s*\,\s*$")]
        [InlineData(@"^\s*\[abigint]\s*bigint\s*null\s*\,\s*$")]
        [InlineData(@"^\s*\[abit]\s*bit\s*null\s*\,\s*$")]
        [InlineData(@"^\s*\[adatetimeoffset]\s*datetimeoffset\s*null\s*\,\s*$")]
        [InlineData(@"^\s*\[adouble]\s*float\s*null\s*\,\s*$")]
        [InlineData(@"^\s*\[asmallint]\s*smallint\s*null\s*\,\s*$")]
        [InlineData(@"^\s*\[atinyint]\s*tinyint\s*null\s*\,\s*$")]
        [InlineData(@"^\s*\[aguid]\s*uniqueidentifier\s*null\s*\,\s*$")]
        public void ShouldContainCorrectColumnsAndTypes(string match)
        {
            var options = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline;
            var matched = Regex.IsMatch(this.generatedSql, match, options);
            Assert.True(matched, match);
        }
    }
}
