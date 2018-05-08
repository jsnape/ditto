#region Copyright (c) all rights reserved.
// <copyright file="CheckMetadata.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using Core;

    /// <summary>
    /// Check Severity enumeration.
    /// </summary>
    public enum CheckSeverity
    {
        /// <summary>
        /// The information member.
        /// </summary>
        Information = 0,

        /// <summary>
        /// The warning member.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// The error member.
        /// </summary>
        Error = 2
    }

    /// <summary>
    /// Check metadata.
    /// </summary>
    public class CheckMetadata
    {
        /// <summary>
        /// Gets or sets the product feature.
        /// </summary>
        /// <value>
        /// The product feature.
        /// </value>
        public string ProductFeature { get; set; }

        /// <summary>
        /// Gets or sets the check goal.
        /// </summary>
        /// <value>
        /// The check goal.
        /// </value>
        public double Goal { get; set; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public CheckSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        public DataOwner Owner { get; set; }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public DataConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public string Filter { get; set; }
    }
}
