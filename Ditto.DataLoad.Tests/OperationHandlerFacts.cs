#region Copyright (c) all rights reserved.
// <copyright file="OperationHandlerFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.Tests
{
    using System;
    using Ditto.DataLoad.DomainEvents;
    using NSubstitute;
    using Xunit;

    /// <summary>
    /// OperationHandler Facts.
    /// </summary>
    public class OperationHandlerFacts
    {
        /// <summary>
        /// The target.
        /// </summary>
        private readonly ConsoleHandler target;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationHandlerFacts"/> class.
        /// </summary>
        public OperationHandlerFacts()
        {
            this.target = new ConsoleHandler();
        }

        /// <summary>
        /// Should throw when null passed as event.
        /// </summary>
        [Fact]
        public void ShouldThrowWhenNullPassedAsEvent()
        {
            BatchCompleteEvent args = null;
            Assert.Throws<ArgumentNullException>(() => this.target.Handle(args));
        }

        /// <summary>
        /// Should throw when null passed as event.
        /// </summary>
        [Fact]
        public void ShouldNotThrowWhenValidEventPassed()
        {
            var operation = new DataOperation(Substitute.For<IDataSource>(), Substitute.For<IDataTarget>());
            this.target.Handle(new BatchCompleteEvent { Operation = operation });
        }
    }
}
