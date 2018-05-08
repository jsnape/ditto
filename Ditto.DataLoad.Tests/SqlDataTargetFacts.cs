#region Copyright (c) all rights reserved.
// <copyright file="SqlDataTargetFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Ditto.Core;
    using Ditto.Core.Tests;
    using NSubstitute;
    using Xunit;

    /// <summary>
    /// <c>SqlDataTarget</c> Facts.
    /// </summary>
    [CLSCompliant(false)]
    public class SqlDataTargetFacts : DbDataFixture
    {
        /// <summary>
        /// The target table name.
        /// </summary>
        private const string TargetTableName = "TEST TABLE NAME";
        
        /// <summary>
        /// The bulk copy factory.
        /// </summary>
        private IBulkCopyFactory bulkCopyFactory;

        /// <summary>
        /// The watermark service.
        /// </summary>
        private IHighWatermarkService watermarkService;

        /// <summary>
        /// The target.
        /// </summary>
        private SqlDataTarget nonWatermarkTarget;

        /// <summary>
        /// The target.
        /// </summary>
        private SqlDataTarget watermarkedTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataTargetFacts"/> class.
        /// </summary>
        public SqlDataTargetFacts()
        {
            this.bulkCopyFactory = Substitute.For<IBulkCopyFactory>();
            this.watermarkService = Substitute.For<IHighWatermarkService>();

            this.nonWatermarkTarget = new SqlDataTarget(this.ConnectionFactory, this.bulkCopyFactory, this.watermarkService, TargetTableName, string.Empty, true);
            this.watermarkedTarget = new SqlDataTarget(this.ConnectionFactory, this.bulkCopyFactory, this.watermarkService, TargetTableName, "WATERMARK COLUMN", true);
        }

        /// <summary>
        /// Enumerates the database nulls.
        /// </summary>
        /// <returns>A sequence of nulls that come from the database.</returns>
        public static IEnumerable<object[]> EnumerateDatabaseNulls()
        {
            yield return new object[] { null };
            yield return new object[] { DBNull.Value };
        }

        /// <summary>
        /// Exists the should return true when table exists.
        /// </summary>
        /// <param name="commandResponse">The command response.</param>
        /// <param name="exists">If set to <c>true</c> [exists].</param>
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void ExistsShouldCheckForTableExisting(int commandResponse, bool exists)
        {
            this.Command.ExecuteScalar().ReturnsForAnyArgs(commandResponse);
            Assert.Equal(exists, this.nonWatermarkTarget.Exists);
        }

        /// <summary>
        /// Should return null when updating non watermarked watermark.
        /// </summary>
        [Fact]
        public void ShouldReturnNullWhenUpdatingNonWatermarkedWatermark()
        {
            Assert.Null(this.nonWatermarkTarget.UpdateHighWatermark());
        }

        /// <summary>
        /// Should return null when updating watermark with no data in the table.
        /// </summary>
        /// <param name="commandResponse">The command response.</param>
        [Theory]
        [MemberData(nameof(EnumerateDatabaseNulls))]
        public void ShouldReturnNullWhenUpdatingWatermarkWithNoData(object commandResponse)
        {
            this.Command.ExecuteScalar().ReturnsForAnyArgs(commandResponse);
            Assert.Null(this.watermarkedTarget.UpdateHighWatermark());
        }

        /// <summary>
        /// Should update watermark with correct date.
        /// </summary>
        [Fact]
        public void ShouldUpdateWatermarkWithCorrectDate()
        {
            var watermark = new DateTime(2015, 06, 02, 14, 15, 16);

            this.Command.ExecuteScalar().ReturnsForAnyArgs(watermark);

            this.watermarkService.UpdateHighWatermark(
                Arg.Any<string>(), 
                Arg.Do<Watermark>(w => Assert.Equal(watermark, w.WatermarkValue)));

            this.watermarkedTarget.UpdateHighWatermark();
        }

        /// <summary>
        /// Should truncate when initialize batch called.
        /// </summary>
        [Fact]
        public void ShouldTruncateWhenInitializeBatchCalled()
        {
            this.nonWatermarkTarget.InitializeBatch();

            this.Command.Received().ExecuteNonQuery();

            Assert.Contains("truncate", this.Command.CommandText, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// BulkCopy should have correct target table name.
        /// </summary>
        [Fact]
        public void BulkCopyShouldHaveCorrectTargetTableName()
        {
            using (var bulkCopy = this.nonWatermarkTarget.CreateBulkCopy())
            {
                Assert.Equal(TargetTableName, bulkCopy.DestinationTableName);
            }
        }

        /// <summary>
        /// Column property should return correct column count.
        /// </summary>
        [Fact]
        public void ColumnsPropertyShouldReturnCorrectColumnCount()
        {
            using (var schema = FakeSchemaFactory.CreateDefaultTableSchema())
            {
                this.Reader.GetSchemaTable().ReturnsForAnyArgs(schema);

                Assert.Equal(schema.Rows.Count, this.nonWatermarkTarget.Columns.Count());
            }
        }

        /// <summary>
        /// Creating the table causes SQL to be executed.
        /// </summary>
        /// <remarks>NB: This is a behavior test and not very good.</remarks>
        [Fact]
        public void CreatingTableCausesSqlToBeExecuted()
        {
            using (var schema = FakeSchemaFactory.CreateDefaultTableSchema())
            {
                this.nonWatermarkTarget.CreateTable(schema);

                Assert.Contains(TargetTableName, this.Command.CommandText, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Should throw when null connection factory passed.
        /// </summary>
        [Fact]
        public void ShouldThrowWhenNullConnectionFactoryPassed()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SqlDataTarget(null, this.bulkCopyFactory, this.watermarkService, string.Empty, string.Empty, false));
        }

        /// <summary>
        /// Should throw when null bulk copy factory passed.
        /// </summary>
        [Fact]
        public void ShouldThrowWhenNullBulkCopyFactoryPassed()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SqlDataTarget(this.ConnectionFactory, null, this.watermarkService, string.Empty, string.Empty, false));
        }

        /// <summary>
        /// Should throw when null watermark service passed.
        /// </summary>
        [Fact]
        public void ShouldThrowWhenNullWatermarkServicePassed()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SqlDataTarget(this.ConnectionFactory, this.bulkCopyFactory, null, string.Empty, string.Empty, false));
        }

        /// <summary>
        /// Should throw when null table name passed.
        /// </summary>
        [Fact]
        public void ShouldThrowWhenNullTableNamePassed()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SqlDataTarget(this.ConnectionFactory, this.bulkCopyFactory, this.watermarkService, null, string.Empty, false));
        }
    }
}
