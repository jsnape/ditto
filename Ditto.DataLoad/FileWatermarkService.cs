#region Copyright (c) all rights reserved.
// <copyright file="FileWatermarkService.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// File Watermark Service.
    /// </summary>
    public class FileWatermarkService : IHighWatermarkService
    {
        /// <summary>
        /// The mark format.
        /// </summary>
        private const string MarkFormat = "yyyy-MM-ddTHH:mm:ss.ffffff";

        /// <summary>
        /// The watermark path.
        /// </summary>
        private readonly string watermarkPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatermarkService"/> class.
        /// </summary>
        /// <param name="watermarkPath">The watermark path.</param>
        public FileWatermarkService(string watermarkPath)
        {
            if (string.IsNullOrEmpty(watermarkPath))
            {
                throw new ArgumentNullException("watermarkPath");
            }

            if (!Directory.Exists(watermarkPath))
            {
                Directory.CreateDirectory(watermarkPath);
            }

            this.watermarkPath = watermarkPath;
        }

        /// <summary>
        /// Gets the high watermark.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="highWatermarkColumn">The high watermark column.</param>
        /// <returns>
        /// The current high watermark.
        /// </returns>
        /// <exception cref="System.NotImplementedException">If any arguments are null.</exception>
        public Watermark GetHighWatermark(string table, string highWatermarkColumn)
        {
            var watermarkFile = this.FormatWatermarkFileName(table);

            if (!File.Exists(watermarkFile))
            {
                return null;
            }

            using (var file = new StreamReader(watermarkFile))
            {
                var contents = file.ReadToEnd();
                var watermark = JsonConvert.DeserializeObject<Watermark>(contents);
                return watermark;
            }
        }

        /// <summary>
        /// Updates the high watermark.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="newValue">The new value.</param>
        public void UpdateHighWatermark(string table, Watermark newValue)
        {
            var watermarkFile = this.FormatWatermarkFileName(table);

            if (!File.Exists(watermarkFile))
            {
                File.Delete(watermarkFile);
            }

            using (var file = new StreamWriter(watermarkFile))
            {
                var json = JsonConvert.SerializeObject(newValue);
                file.Write(json);
            }
        }

        /// <summary>
        /// Gets the full watermark file name.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The full watermark file name.</returns>
        private string FormatWatermarkFileName(string table)
        {
            return Path.Combine(this.watermarkPath, table + ".json");
        }
    }
}
