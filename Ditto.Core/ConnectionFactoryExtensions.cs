#region Copyright (c) all rights reserved.
// <copyright file="ConnectionFactoryExtensions.cs">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// Connection Extensions.
    /// </summary>
    public static class ConnectionFactoryExtensions
    {
        /// <summary>
        /// Gets the schema table.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        /// A schema for the table.
        /// </returns>
        public static DataTable GetTableSchema(this IConnectionFactory connectionFactory, string tableName)
        {
            return connectionFactory.GetQuerySchema(
                string.Format(CultureInfo.CurrentCulture, "select * from {0} with (nolock);", tableName));
        }

        /// <summary>
        /// Gets the schema table.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="query">The query.</param>
        /// <returns>
        /// A schema for the query.
        /// </returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Does not execute the query, just gets the schema.")]
        public static DataTable GetQuerySchema(this IConnectionFactory connectionFactory, string query)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException("connectionFactory");
            }

            DataTable schema = null;

            using (var connection = connectionFactory.CreateConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = query;

                var highWatermarkParameterName = connectionFactory.MakeParameterName("highWatermark");

                if (command.CommandText.Contains(highWatermarkParameterName))
                {
                    var watermarkParameter = command.CreateParameter();
                    watermarkParameter.ParameterName = highWatermarkParameterName;
                    watermarkParameter.Value = DateTime.Today;
                    command.Parameters.Add(watermarkParameter);
                }

                try
                {
                    using (var reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        schema = reader.GetSchemaTable();
                    }
                }
#pragma warning disable CC0004 // Catch block cannot be empty
                catch (DbException)
                {
                    // SQL Server might throw an 'invalid object' exception if a temp table is used.
                    // Sysbase seems to just return a null schema table.
                }
#pragma warning restore CC0004 // Catch block cannot be empty
            }

            if (schema == null)
            {
                using (var connection = connectionFactory.CreateConnection())
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = query;

                    // Brute force - if we failed to get a schema then we need to re-execute properly.
                    using (var reader = command.ExecuteReader(CommandBehavior.Default))
                    {
                        reader.Read();
                        schema = reader.GetSchemaTable();
                    }
                }
            }

            return schema;
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="query">The query.</param>
        /// <param name="operationId">The operation identifier.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If <c>connectionFactory</c> is null.</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input allowed")]
        public static int ExecuteNonQuery(this IConnectionFactory connectionFactory, string query, Guid operationId)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException("connectionFactory");
            }

            using (var connection = connectionFactory.CreateConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandTimeout = 0;

                DittoEventSource.Log.DatabaseQuery(connectionFactory.Name, command.CommandText, operationId);

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="query">The query.</param>
        /// <param name="operationId">The operation identifier.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If <c>connectionFactory</c> is null.</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input allowed")]
        public static object ExecuteScalar(this IConnectionFactory connectionFactory, string query, Guid operationId)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException("connectionFactory");
            }

            using (var connection = connectionFactory.CreateConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandTimeout = 0;

                DittoEventSource.Log.DatabaseQuery(connectionFactory.Name, command.CommandText, operationId);

                return command.ExecuteScalar();
            }
        }
    }
}
