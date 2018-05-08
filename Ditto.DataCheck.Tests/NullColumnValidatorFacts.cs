#region Copyright (c) all rights reserved.
// <copyright file="NullColumnValidatorFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Ditto.Core.Tests;
    using NSubstitute;
    using Xunit;

    /// <summary>
    /// Null column validator facts.
    /// </summary>
    public sealed class NullColumnValidatorFacts : DbDataFixture, IDisposable
    {
        /// <summary>
        /// The cancellation token source.
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// The context.
        /// </summary>
        private ValidationContext context;

        /// <summary>
        /// The target.
        /// </summary>
        private NullColumnValidator target;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullColumnValidatorFacts"/> class.
        /// </summary>
        public NullColumnValidatorFacts()
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.context = new ValidationContext(this.cancellationTokenSource.Token);

            var metadata = new CheckMetadata { Connection = this.DataConnection };
            this.target = new NullColumnValidator("TEST", "dbo.foo", "acolumn", metadata);

            this.Reader.Read().Returns(true, false);
            this.Reader["TotalRecords"].Returns(3338117);
        }

        /// <summary>
        /// Validates a column with no nulls.
        /// </summary>
        [Fact]
        public void ValidatesColumnWithNoNulls()
        {
            this.Reader["NullRecords"].Returns(0);

            var result = this.target.Validate(this.context);

            Assert.Equal(1.0, result.Status);
        }

        /// <summary>
        /// Validates a column with nulls.
        /// </summary>
        [Fact]
        public void ValidatesColumnWithNulls()
        {
            this.Reader["NullRecords"].Returns(2540320);

            var result = this.target.Validate(this.context);

            Assert.Equal(-1.0, result.Status);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.cancellationTokenSource.Dispose();
        }
    }
}
