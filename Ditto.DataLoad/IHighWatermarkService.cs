#region Copyright (c) all rights reserved.
// <copyright file="IHighWatermarkService.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    /// <summary>
    /// IHighWatermarkService interface definition.
    /// </summary>
    public interface IHighWatermarkService
    {
        /// <summary>
        /// Gets the high watermark.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="highWatermarkColumn">The high watermark column.</param>
        /// <returns>
        /// The current high watermark.
        /// </returns>
        Watermark GetHighWatermark(string table, string highWatermarkColumn);

        /// <summary>
        /// Updates the high watermark.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="newValue">The new value.</param>
        void UpdateHighWatermark(string table, Watermark newValue);
    }
}
