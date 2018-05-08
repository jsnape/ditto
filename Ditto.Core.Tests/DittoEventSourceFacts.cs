#region Copyright (c) all rights reserved.
// <copyright file="DittoEventSourceFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core.Tests
{
    using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
    using Xunit;

    /// <summary>
    /// DittoEventSource Facts.
    /// </summary>
    public static class DittoEventSourceFacts
    {
        /// <summary>
        /// Logger should validate as an event source.
        /// </summary>
        [Fact]
        public static void LoggerShouldValidateAsAnEventSource()
        {
            EventSourceAnalyzer.InspectAll(DittoEventSource.Log);
        }
    }
}
