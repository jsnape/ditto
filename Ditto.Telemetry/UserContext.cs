#region Copyright (c) all rights reserved.
// <copyright file="UserContext.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    /// <summary>
    /// Encapsulates information about a user using an application.
    /// </summary>
    public class UserContext
    {
        /// <summary>
        /// Gets or sets the ID of an application-defined account associated with the user.
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the ID of user accessing the application.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }
    }
}
