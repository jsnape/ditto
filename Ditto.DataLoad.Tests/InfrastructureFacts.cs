#region Copyright (c) all rights reserved.
// <copyright file="InfrastructureFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.Tests
{
    using Xunit;

    /// <summary>
    /// Infrastructure Facts.
    /// </summary>
    public static class InfrastructureFacts
    {
        /// <summary>
        /// X-Unit is installed.
        /// </summary>
        [Fact]
        public static void XunitIsInstalled()
        {
            Assert.True(true);
        }
    }
}
