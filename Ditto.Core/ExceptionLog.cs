// <copyright file="ExceptionLog.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace Ditto.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Ditto.Core.Properties;

    /// <summary>
    /// Exception Log.
    /// </summary>
    public class ExceptionLog : IDisposable
    {
        /// <summary>
        /// The exception log writer.
        /// </summary>
        private readonly TextWriter logWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLog"/> class.
        /// </summary>
        /// <param name="errorFile">The error file.</param>
        public ExceptionLog(string errorFile)
            : this(errorFile, s => new StreamWriter(s, true))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLog" /> class.
        /// </summary>
        /// <remarks>Extra writer function for testing.</remarks>
        /// <param name="errorFile">The error file.</param>
        /// <param name="createWriter">The create writer function.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If any arguments are null.
        /// </exception>
        public ExceptionLog(string errorFile, Func<string, TextWriter> createWriter)
        {
            if (string.IsNullOrWhiteSpace(errorFile))
            {
                throw new ArgumentNullException("errorFile");
            }

            if (createWriter == null)
            {
                throw new ArgumentNullException("createWriter");
            }

            this.logWriter = createWriter(errorFile);
        }

        /// <summary>
        /// Formats the exception message.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>A formatted exception message.</returns>
        public static string FormatExceptionMessage(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            var message = new StringBuilder();

            message.AppendLine();
            message.AppendLine();

            message.AppendLine(ex.Message);

            if (!string.IsNullOrEmpty(ex.HelpLink))
            {
                message.AppendFormat(CultureInfo.CurrentCulture, Resources.ProcessFailedErrorMessageFormat, ex.HelpLink);
                message.AppendLine();
            }

            foreach (DictionaryEntry entry in ex.Data)
            {
                message.AppendFormat(CultureInfo.CurrentCulture, Resources.KeyValuePairFormat, entry.Key, entry.Value);
                message.AppendLine();
            }

            return message.ToString();
        }

        /// <summary>
        /// Reserves the space for the next log message.
        /// </summary>
        /// <remarks>
        /// This is a no-op at the moment but the intention is that space 
        /// will be reserved so it can log an exception even in a low 
        /// disk space situation.
        /// </remarks>
        public void ReserveSpace()
        {
            this.logWriter.Flush();
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public void LogException(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            var message = FormatExceptionMessage(ex);

            this.logWriter.WriteLine();
            this.logWriter.WriteLine(Resources.ExceptionLogHeaderFormat, DateTime.Now);
            this.logWriter.WriteLine(message);
            this.logWriter.WriteLine(ex.StackTrace);
            this.logWriter.WriteLine();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.logWriter.Close();
            }
        }
    }
}
