// <copyright file="FileExtensions.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace Ditto.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// File extensions.
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// The process cannot access the file because it is being used by another process.
        /// </summary>
        private const int ErrorSharingViolation = 32;

        /// <summary>
        /// The process cannot access the file because another process has locked a portion of the file.
        /// </summary>
        private const int ErrorLockViolation = 32;

        /// <summary>
        /// Determines whether a file [is locked] from the exception thrown.
        /// </summary>
        /// <param name="exception">The exception to check.</param>
        /// <returns>True if the file is locked.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "JS: This only works with IOExceptions")]
        public static bool IsFileLocked(this IOException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            var hresult = Marshal.GetHRForException(exception);
            var code = hresult & ((1 << 16) - 1);

            return code == ErrorSharingViolation || code == ErrorLockViolation;
        }
    }
}
