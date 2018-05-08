#region Copyright (c) all rights reserved.
// <copyright file="SqlTelemetryChannel.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Telemetry
{
    using System;
    using System.Collections.Concurrent;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// <c>SqlServer</c> Telemetry Channel.
    /// </summary>
    public class SqlTelemetryChannel : ITelemetryChannel, IDisposable
    {
        /// <summary>
        /// The transmission queue.
        /// </summary>
        private readonly ConcurrentQueue<ITelemetry> transmissionQueue;

        /// <summary>
        /// The transmission timer.
        /// </summary>
        private readonly Timer transmissionTimer;

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The telemetry table.
        /// </summary>
        private readonly string telemetryTable;

        /// <summary>
        /// The exception handler.
        /// </summary>
        private readonly Action<SqlException> exceptionHandler;

        /// <summary>
        /// The transmission lock.
        /// </summary>
        private readonly object transmissionLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTelemetryChannel" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="telemetryTable">The telemetry table.</param>
        public SqlTelemetryChannel(string connectionString, string telemetryTable)
            : this(connectionString, telemetryTable, ex => { })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTelemetryChannel" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="telemetryTable">The telemetry table.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public SqlTelemetryChannel(string connectionString, string telemetryTable, Action<SqlException> exceptionHandler)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            this.connectionString = connectionString;
            this.telemetryTable = telemetryTable ?? throw new ArgumentNullException("telemetryTable");
            this.exceptionHandler = exceptionHandler ?? throw new ArgumentNullException("exceptionHandler");
            this.transmissionQueue = new ConcurrentQueue<ITelemetry>();
            this.transmissionTimer = new Timer(this.TransmitQueue, null, new TimeSpan(0, 0, 10), new TimeSpan(0, 1, 0));
        }

        /// <summary>
        /// Sends the specified item to the channel.
        /// </summary>
        /// <param name="item">The item to send to the channel.</param>
        public void Send(ITelemetry item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            this.transmissionQueue.Enqueue(item);
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
                this.transmissionTimer.Dispose();

                // Send the final set of messages.
                this.TransmitQueue(null);
            }
        }

        /// <summary>
        /// Transmits the queue.
        /// </summary>
        /// <param name="stateInfo">The state information.</param>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "JS: Internally generated code")]
        private void TransmitQueue(object stateInfo)
        {
            ITelemetry item = null;

            lock (this.transmissionLock)
            {
                using (StringWriter writer = new StringWriter(CultureInfo.CurrentCulture))
                {
                    var sqlWriter = new SqlWriter(writer, this.telemetryTable);

                    while (this.transmissionQueue.TryDequeue(out item))
                    {
                        var serializable = (ISqlSerializable)item;
                        serializable.Serialize(sqlWriter);
                    }

                    var sql = writer.ToString();

                    if (sql.Length > 0)
                    {
                        try
                        {
                            using (var connection = new SqlConnection(this.connectionString))
                            {
                                connection.Open();

                                var command = connection.CreateCommand();
                                command.CommandText = sql;
                                command.CommandTimeout = 0;

                                command.ExecuteNonQuery();
                            }
                        }
                        catch (SqlException ex)
                        {
                            this.exceptionHandler(ex);
                        }
                    }
                }
            }
        }
    }
}
