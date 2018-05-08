#region Copyright (c) all rights reserved.
// <copyright file="DbCommandExtensions.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// IDbCommand extensions.
    /// </summary>
    public static class DbCommandExtensions
    {
        /// <summary>
        /// An asynchronous version of <c>System.Data.SqlClient.SqlCommand.ExecuteReader()</c>,
        /// which sends the <c>System.Data.SqlClient.SqlCommand.CommandText</c> to the <c>System.Data.SqlClient.SqlCommand.Connection</c>
        /// and builds a <c>System.Data.SqlClient.SqlDataReader</c>. The cancellation token can
        /// be used to request that the operation be abandoned before the command timeout
        /// elapses. Exceptions will be reported via the returned Task object.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public static async Task<IDataReader> ExecuteReaderAsync(this IDbCommand command, CancellationToken cancellationToken)
        {
            var sqlCommand = command as SqlCommand;

            if (sqlCommand != null)
            {
                return await sqlCommand.ExecuteReaderAsync(cancellationToken);
            }
            else
            {
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Adds a parameter to the command.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>
        /// The newly created parameter.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If command is null.</exception>
        public static IDbDataParameter AddParameter<T>(this IDbCommand command, string name, T value)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            var parameter = command.CreateParameter();

            parameter.ParameterName = name;

            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }

            command.Parameters.Add(parameter);

            return parameter;
        }
    }
}
