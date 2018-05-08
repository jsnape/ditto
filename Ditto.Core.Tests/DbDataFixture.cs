#region Copyright (c) all rights reserved.
// <copyright file="DbDataFixture.cs">
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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NSubstitute;

    /// <summary>
    /// Database Fixture.
    /// </summary>
    public class DbDataFixture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbDataFixture"/> class.
        /// </summary>
        public DbDataFixture()
        {
            this.DataConnection = Substitute.For<DataConnection>("FakeDataConnection", "FakeDataConnection", "FakeDataConnection", "TEST");
            this.ConnectionFactory = Substitute.For<IConnectionFactory>();
            this.Connection = Substitute.For<IDbConnection>();
            this.Command = Substitute.For<IDbCommand>();
            this.Reader = Substitute.For<IDataReader>();

            this.DataConnection.CreateConnectionFactory().Returns(this.ConnectionFactory);

            this.ConnectionFactory.CreateConnection().Returns(this.Connection);
            this.Connection.CreateCommand().Returns(this.Command);
            this.Command.ExecuteReader().Returns(this.Reader);
            this.Command.ExecuteReader(Arg.Any<CommandBehavior>()).Returns(this.Reader);
            this.ConnectionFactory.CreateTableGenerator().Returns(new TableGenerator());
        }

        /// <summary>
        /// Gets or sets the data connection.
        /// </summary>
        /// <value>
        /// The data connection.
        /// </value>
        public DataConnection DataConnection { get; protected set; }

        /// <summary>
        /// Gets or sets the connection factory.
        /// </summary>
        /// <value>
        /// The connection factory.
        /// </value>
        public IConnectionFactory ConnectionFactory { get; protected set; }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public IDbConnection Connection { get; protected set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public IDbCommand Command { get; protected set; }

        /// <summary>
        /// Gets or sets the reader.
        /// </summary>
        /// <value>
        /// The reader.
        /// </value>
        public IDataReader Reader { get; protected set; }
    }
}
