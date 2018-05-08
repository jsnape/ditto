#region Copyright (c) all rights reserved.
// <copyright file="Watermark.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System;

    /// <summary>
    /// Watermark class definition.
    /// </summary>
    public class Watermark
    {
        /// <summary>
        /// The watermark column.
        /// </summary>
        private readonly string watermarkColumn;

        /// <summary>
        /// The watermark value.
        /// </summary>
        private readonly DateTime watermarkValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Watermark" /> class.
        /// </summary>
        /// <param name="watermarkColumn">The watermark column.</param>
        /// <param name="watermarkValue">The watermark value.</param>
        public Watermark(string watermarkColumn, DateTime watermarkValue)
        {
            this.watermarkColumn = watermarkColumn;
            this.watermarkValue = watermarkValue;
        }

        /// <summary>
        /// Gets the watermark column.
        /// </summary>
        /// <value>
        /// The watermark column.
        /// </value>
        public string WatermarkColumn
        {
            get { return this.watermarkColumn; }
        }

        /// <summary>
        /// Gets the watermark value.
        /// </summary>
        /// <value>
        /// The watermark value.
        /// </value>
        public DateTime WatermarkValue
        {
            get { return this.watermarkValue; }
        }
    }
}
