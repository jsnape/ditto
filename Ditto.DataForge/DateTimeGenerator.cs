#region Copyright (c) all rights reserved.
// <copyright file="DataGenerator.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataForge
{
    using System;

    /// <summary>
    /// Date Time Generator
    /// </summary>
    /// <seealso cref="Ditto.DataForge.IDataGenerator" />
    public class DateTimeGenerator : IDataGenerator
    {
        /// <summary>
        /// Gets the validator name.
        /// </summary>
        /// <value>
        /// The validator name.
        /// </value>
        public string Name => "DateTime Generator";

        /// <summary>
        /// Gets or sets the minimum date.
        /// </summary>
        /// <value>
        /// The minimum date.
        /// </value>
        public DateTime MinDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the maximum date.
        /// </summary>
        /// <value>
        /// The maximum date.
        /// </value>
        public DateTime MaxDate { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// Gets or sets a value indicating whether [date only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [date only]; otherwise, <c>false</c>.
        /// </value>
        public bool DateOnly { get; set; } = false;

        /// <summary>
        /// Returns the next value for the data element.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The next value
        /// </returns>
        public object NextValue(GenerationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var difference = (MaxDate - MinDate).TotalMilliseconds;
            var random = context.Random.NextDouble();
            var offset = random * difference;

            var value = MinDate.AddMilliseconds(offset);

            return this.DateOnly ? value.Date : value;
        }
    }
}