#region Copyright (c) all rights reserved.
// <copyright file="DataConnectionFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// DataConnection Facts.
    /// </summary>
    [CLSCompliant(false)]
    public static class DataConnectionFacts
    {
        /// <summary>
        /// Should throw when nulls or blanks passed to constructor.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="environmentName">Name of the environment.</param>
        [Theory]
        [InlineData(null, "System.Data.SqlClient", "test string", "UNIT TEST")]
        [InlineData("", "System.Data.SqlClient", "test string", "UNIT TEST")]
        [InlineData("test name", null, "test string", "UNIT TEST")]
        [InlineData("test name", "", "test string", "UNIT TEST")]
        [InlineData("test name", "System.Data.SqlClient", null, "UNIT TEST")]
        [InlineData("test name", "System.Data.SqlClient", "", "UNIT TEST")]
        [InlineData("test name", "System.Data.SqlClient", "test string", null)]
        [InlineData("test name", "System.Data.SqlClient", "test string", "")]
        public static void ShouldThrowWhenNullsOrBlanksPassedToConstructor(string name, string provider, string connectionString, string environmentName)
        {
            Assert.Throws<ArgumentNullException>(
                () => new DataConnection(name, provider, connectionString, environmentName));
        }

        /// <summary>
        /// Creating a connection factory should throw when provider is file system.
        /// </summary>
        [Fact]
        public static void ConnectionFactoryShouldThrowWhenProviderIsFileSystem()
        {
            var target = new DataConnection("wibble", "FileSystem", @"\\server\share\folder", "UNIT TEST");

            Assert.Throws<InvalidOperationException>(() => target.CreateConnectionFactory());
        }

        /// <summary>
        /// Creating a bulk copy factory should throw when provider is file system.
        /// </summary>
        [Fact]
        public static void BulkCopyFactoryShouldThrowWhenProviderIsNotSql()
        {
            var target = new DataConnection("wibble", "FileSystem", @"\\server\share\folder", "UNIT TEST");

            Assert.Throws<InvalidOperationException>(() => target.CreateBulkCopyFactory());
        }
    }
}
