#region Copyright (c) all rights reserved.
// <copyright file="OperationContext.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    /// <summary>
    /// Encapsulates information about a user operation.
    /// </summary>
    public class OperationContext
    {
        /// <summary>
        /// Gets or sets the application-defined operation ID.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the application-defined operation Name.
        /// </summary>
        /// <value>
        /// The operation name.
        /// </value>
        public string Name { get; set; }
    }
}
