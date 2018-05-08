// <copyright file="EventPublisherFacts.cs">
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace Ditto.Core.Tests
{
    using Xunit;

    /// <summary>
    /// EventPublisher Facts.
    /// </summary>
    public static class EventPublisherFacts
    {
        /// <summary>
        /// When the callback is registered event fires.
        /// </summary>
        [Fact]
        public static void WhenCallbackIsRegisteredEventFires()
        {
            bool callbackFired = false;
            EventPublisher.Register<FakeDomainEvent>(x => callbackFired = true);
            EventPublisher.Raise(new FakeDomainEvent());

            Assert.True(callbackFired);
        }

        /// <summary>
        /// When the callbacks are cleared event does not fire.
        /// </summary>
        [Fact]
        public static void WhenCallbacksAreClearedEventDoesNotFire()
        {
            bool callbackFired = false;
            EventPublisher.Register<FakeDomainEvent>(x => callbackFired = true);
            EventPublisher.ClearCallbacks();
            EventPublisher.Raise(new FakeDomainEvent());

            Assert.False(callbackFired);
        }

        /// <summary>
        /// Fake DomainEvent.
        /// </summary>
        private class FakeDomainEvent : IDomainEvent
        {
        }
    }
}
