#region Copyright (c) all rights reserved.
// <copyright file="DataGenerator.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataForge.Tests
{
    using System;
    using System.Threading;
    using DataForge;
    using Xunit;

    /// <summary>
    /// Date Time Generator Facts
    /// </summary>
    public class DateTimeGeneratorFacts : IDisposable
    {
        /// <summary>
        /// The test iterations
        /// </summary>
        private const int TestIterations = 10000;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        private CancellationTokenSource cts;

        /// <summary>
        /// The context
        /// </summary>
        private GenerationContext context;

        /// <summary>
        /// The target
        /// </summary>
        private DateTimeGenerator target;

        /// <summary>
        /// True if this instance has been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeGeneratorFacts"/> class.
        /// </summary>
        public DateTimeGeneratorFacts()
        {
            this.cts = new CancellationTokenSource();
            this.context = new GenerationContext(this.cts.Token);
            this.target = new DateTimeGenerator();
        }

        /// <summary>
        /// Next Value Is a <c>DateTime</c>.
        /// </summary>
        [Fact]
        public void NextValueIsDateTime()
        {
            var value = this.target.NextValue(this.context);

            Assert.IsType<DateTime>(value);
        }

        /// <summary>
        /// Values are between minimum and maximum.
        /// </summary>
        [Fact]
        public void ValuesAreBetweenMinAndMax()
        {
            this.target.MinDate = new DateTime(2016, 10, 10);
            this.target.MaxDate = new DateTime(2016, 10, 13);

            for (int i = 0; i < TestIterations; ++i)
            {
                var value = (DateTime)this.target.NextValue(this.context);

                Assert.InRange(value, this.target.MinDate, this.target.MaxDate);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            this.isDisposed = true;

            if (disposing)
            {
                this.cts.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
