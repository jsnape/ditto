#region Copyright (c) all rights reserved.
// <copyright file="DataConnection.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System;

    /// <summary>
    /// Data Connection.
    /// </summary>
    public class DataConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConnection"/> class.
        /// </summary>
        /// <param name="name">The connection name.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="environmentName">The environment.</param>
        public DataConnection(string name, string provider, string connectionString, string environmentName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.Name = name;

            if (string.IsNullOrEmpty(provider))
            {
                throw new ArgumentNullException("provider");
            }

            this.Provider = provider;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            this.ConnectionString = connectionString;

            if (string.IsNullOrEmpty(environmentName))
            {
                throw new ArgumentNullException("environmentName");
            }

            this.EnvironmentName = environmentName;
        }

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        /// <value>
        /// The name of the connection.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        public string Provider { get; private set; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the environment name.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        public string EnvironmentName { get; private set; }

        /// <summary>
        /// Creates the connection factory.
        /// </summary>
        /// <returns>A connection factory instance.</returns>
        public virtual IConnectionFactory CreateConnectionFactory()
        {
            if (this.Provider == "FileSystem")
            {
                throw new InvalidOperationException("This data connection does not support connection factories.");
            }

            return new DbProviderConnectionFactory(this.Name, this.Provider, this.ConnectionString);
        }

        /// <summary>
        /// Creates the bulk copy factory.
        /// </summary>
        /// <returns>A bulk copy factory instance.</returns>
        public virtual IBulkCopyFactory CreateBulkCopyFactory()
        {
            if (this.Provider != "System.Data.SqlClient")
            {
                throw new InvalidOperationException("This data connection does not support bulk copy.");
            }

            return new SqlBulkCopyFactory(this.ConnectionString);
        }
    }
}
