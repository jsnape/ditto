#region Copyright (c) all rights reserved.
// <copyright file="IConnectionFactory.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System.Data;

    /// <summary>
    /// IConnectionFactory interface definition.
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Gets the name of the connection factory.
        /// </summary>
        /// <value>
        /// The name of the connection factory.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Quotes the identifier.
        /// </summary>
        /// <param name="value">The value to quote.</param>
        /// <returns>A quoted string.</returns>
        string QuoteIdentifier(string value);

        /// <summary>
        /// Makes the name of the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>
        /// A parameter name with the correct prefix.
        /// </returns>
        string MakeParameterName(string parameterName);

        /// <summary>
        /// Creates the table generator.
        /// </summary>
        /// <returns>A table generator instance.</returns>
        ITableGenerator CreateTableGenerator();

        /// <summary>
        /// Creates a connection.
        /// </summary>
        /// <returns>A connection instance.</returns>
        IDbConnection CreateConnection();
    }
}
