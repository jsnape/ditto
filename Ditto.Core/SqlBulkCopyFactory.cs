#region Copyright (c) all rights reserved.
// <copyright file="SqlBulkCopyFactory.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    /// <summary>
    /// SQLServer BulkCopy Factory.
    /// </summary>
    public class SqlBulkCopyFactory : IBulkCopyFactory
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkCopyFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlBulkCopyFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Creates the bulk copy.
        /// </summary>
        /// <returns>
        /// A bulk copy instance.
        /// </returns>
        public IBulkCopy CreateBulkCopy()
        {
            return new SqlBulkCopyWrapper(this.connectionString);
        }
    }
}
