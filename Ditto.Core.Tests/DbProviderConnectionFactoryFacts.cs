#region Copyright (c) all rights reserved.
// <copyright file="DbProviderConnectionFactoryFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core.Tests
{
    using System;
    using Xunit;

    /// <summary>
    /// DbProviderConnectionFactory Facts.
    /// </summary>
    [CLSCompliant(false)]
    public class DbProviderConnectionFactoryFacts
    {
        /// <summary>
        /// The target.
        /// </summary>
        private DbProviderConnectionFactory target;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbProviderConnectionFactoryFacts"/> class.
        /// </summary>
        public DbProviderConnectionFactoryFacts()
        {
            this.target = new DbProviderConnectionFactory("wibble", "System.Data.SqlClient", "Data Source=NOWHERE;Initial Catalog=NOTADB;Integrated Security = SSPI;");
        }

        /// <summary>
        /// Should throw when nulls or blanks passed to constructor.
        /// </summary>
        /// <param name="name">The name of the factory.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionString">The connection string.</param>
        [Theory]
        [InlineData(null, "System.Data.SqlClient", "test string")]
        [InlineData("", "System.Data.SqlClient", "test string")]
        [InlineData("test name", null, "test string")]
        [InlineData("test name", "", "test string")]
        [InlineData("test name", "System.Data.SqlClient", null)]
        [InlineData("test name", "System.Data.SqlClient", "")]
        public static void ShouldThrowWhenNullsOrBlanksPassedToConstructor(string name, string provider, string connectionString)
        {
            Assert.Throws<ArgumentNullException>(
                () => new DbProviderConnectionFactory(name, provider, connectionString));
        }

        /// <summary>
        /// Should throw when invalid provider passed to constructor.
        /// </summary>
        [Fact]
        public static void ShouldThrowWhenInvalidProviderPassedToConstructor()
        {
            Assert.Throws<ArgumentException>(
                () => new DbProviderConnectionFactory("test name", "invalid provider", "test string"));
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
        /// Table Generator should be valid.
        /// </summary>
        [Fact]
        public void TableGeneratorShouldBeValid()
        {
            Assert.NotNull(this.target.CreateTableGenerator());
        }

        /// <summary>
        /// Connections should be valid.
        /// </summary>
        [Fact]
        public void ConnectionsShouldBeValid()
        {
            Assert.NotNull(this.target.CreateConnection());
        }
    }
}
