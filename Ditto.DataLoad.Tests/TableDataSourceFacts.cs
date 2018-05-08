#region Copyright (c) all rights reserved.
// <copyright file="TableDataSourceFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.Tests
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Ditto.DomainEvents;
    using Ditto.Core.Tests;
    using Ditto.DataLoad.DomainEvents;
    using NSubstitute;
    using Xunit;

    /// <summary>
    /// TableDataSource Facts.
    /// </summary>
    [CLSCompliant(false)]
    public class TableDataSourceFacts : DbDataFixture
    {
        /// <summary>
        /// The target.
        /// </summary>
        private readonly TableDataSource target;

        /// <summary>
        /// The operation.
        /// </summary>
        private readonly IDataOperationInfo operation;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDataSourceFacts"/> class.
        /// </summary>
        public TableDataSourceFacts()
        {
            this.operation = Substitute.For<IDataOperationInfo>();
            this.operation.OperationId.ReturnsForAnyArgs(Guid.NewGuid());

            this.target = new TableDataSource(this.ConnectionFactory, "dbo.wibble");
            this.target.Parent = this.operation;
        }

        /// <summary>
        /// Should throw when nulls passed to constructor.
        /// </summary>
        [Fact]
        public void ShouldThrowWhenNullsPassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new TableDataSource(this.ConnectionFactory, null));
        }

        /// <summary>
        /// Name property should be correct.
        /// </summary>
        [Fact]
        public void NamePropertyShouldBeCorrect()
        {
            Assert.Equal("dbo.wibble", this.target.Name);
        }

        /// <summary>
        /// Query should be correct.
        /// </summary>
        [Fact]
        public void CommandTextShouldBeCorrect()
        {
            using (var data = this.target.GetData(Enumerable.Empty<string>(), null))
            {
                Assert.Contains("select * from dbo.wibble", this.Command.CommandText, StringComparison.OrdinalIgnoreCase);
            }

            this.Command.ExecuteReader().Received();
        }

        /// <summary>
        /// Failed source query should raise domain event.
        /// </summary>
        [Fact]
        public void FailedSourceQueryShouldRaiseDomainEvent()
        {
            this.Command
                .When(x => x.ExecuteReader(CommandBehavior.CloseConnection))
                .Do(x => { throw new FakeDbException(); });

            bool eventRaised = false;
            EventPublisher.Register<SourceErrorEvent>(e => eventRaised = true);

            Assert.Throws<FakeDbException>(() => this.target.GetData(Enumerable.Empty<string>(), null));

            Assert.True(eventRaised);
        }
    }
}
