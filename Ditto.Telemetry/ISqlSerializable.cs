#region Copyright (c) all rights reserved.
// <copyright file="ISqlSerializable.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    /// <summary>
    /// <c>ISqlSerializable</c> interface definition.
    /// </summary>
    public interface ISqlSerializable
    {
        /// <summary>
        /// Serializes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void Serialize(ISqlWriter writer);
    }
}
