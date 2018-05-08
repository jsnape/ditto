#region Copyright (c) all rights reserved.
// <copyright file="ProgramFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.Tests
{
    using System;
    using System.Globalization;
    using System.IO;
    using Xunit;

    /// <summary>
    /// Program Facts.
    /// </summary>
    [CLSCompliant(false)]
    public static class ProgramFacts
    {
        /// <summary>
        /// Should return help requested code for help argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="exitCode">The exit code.</param>
        [Theory]
        [InlineData(new[] { "-h", "-Script", "C:\\windows\\win.ini" }, Program.HelpRequested)]
        [InlineData(new[] { "-?", "-Script", "C:\\windows\\win.ini" }, Program.HelpRequested)]
        [InlineData(new[] { "-Script", "C:\\windows\\win.ini" }, Program.Success)]
        public static void ShouldReturnHelpRequestedCodeForHelpArgument(string[] args, int exitCode)
        {
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                Assert.Equal(exitCode, Program.ParseArguments(writer, args));
            }
        }

        /// <summary>
        /// Should throw when null arguments passed.
        /// </summary>
        [Fact]
        public static void ShouldThrowWhenNullArgsPassed()
        {
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                Assert.Throws<ArgumentNullException>(() => Program.ParseArguments(writer, null));
            }
        }
    }
}
