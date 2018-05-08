#region Copyright (c) all rights reserved.
// <copyright file="DataOwner.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    /// <summary>
    /// Data Owner.
    /// </summary>
    public class DataOwner
    {
        /// <summary>
        /// Gets or sets the owner name.
        /// </summary>
        /// <value>
        /// The owner name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the contact.
        /// </summary>
        /// <value>
        /// The contact.
        /// </value>
        public string Contact { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DataOwner" /> is notified on check failure.
        /// </summary>
        /// <value>
        ///   <c>true</c> if notify; otherwise, <c>false</c>.
        /// </value>
        public bool Notify { get; set; }
    }
}
