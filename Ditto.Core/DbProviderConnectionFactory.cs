#region Copyright (c) all rights reserved.
// <copyright file="DbProviderConnectionFactory.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;

    /// <summary>
    /// DbProvider Connection Factory.
    /// </summary>
    public class DbProviderConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// The name of he connection factory.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The provider.
        /// </summary>
        private readonly string provider;

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The factory.
        /// </summary>
        private readonly DbProviderFactory factory;

        /// <summary>
        /// The command builder.
        /// </summary>
        private readonly DbCommandBuilder commandBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbProviderConnectionFactory" /> class.
        /// </summary>
        /// <param name="name">The name of the connection factory.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public DbProviderConnectionFactory(string name, string provider, string connectionString)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;

            if (string.IsNullOrEmpty(provider))
            {
                throw new ArgumentNullException("provider");
            }

            this.provider = provider;

            this.factory = DbProviderFactories.GetFactory(this.provider);
            Debug.Assert(this.factory != null, "The factory throws if invalid provider passed.");

            this.commandBuilder = this.factory.CreateCommandBuilder();

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            this.connectionString = connectionString;
        }

        /// <summary>
        /// Gets the name of the connection factory.
        /// </summary>
        /// <value>
        /// The name of the connection factory.
        /// </value>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Quotes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A quoted string.
        /// </returns>
        public string QuoteIdentifier(string value)
        {
            return this.commandBuilder.QuoteIdentifier(value);
        }

        /// <summary>
        /// Makes the name of the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>
        /// A parameter name with the correct prefix.
        /// </returns>
        public string MakeParameterName(string parameterName)
        {
            switch (this.provider)
            {
                case "Oracle.ManagedDataAccess.Client":
                    return ":" + parameterName;
                default:
                    return "@" + parameterName;
            }
        }

        /// <summary>
        /// Creates the table generator.
        /// </summary>
        /// <returns>A new table generator instance.</returns>
        public ITableGenerator CreateTableGenerator()
        {
            return new TableGenerator();
        }

        /// <summary>
        /// Creates a connection.
        /// </summary>
        /// <returns>A connection instance.</returns>
        public IDbConnection CreateConnection()
        {
            var connection = this.factory.CreateConnection();
            connection.ConnectionString = this.connectionString;

            return connection;
        }
    }
}
