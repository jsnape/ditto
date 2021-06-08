#region Copyright (c) all rights reserved.
// <copyright file="StatisticsDataReader.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System.Data;
    using System.Threading;

    /// <summary>
    /// <see cref="IDataReader"/> implementation that maintains statistics.
    /// </summary>
    public class StatisticsDataReader : PassThroughDataReader
    {
        /// <summary>
        /// The row count.
        /// </summary>
        private int rowCount;

        /// <summary>
        /// The result count.
        /// </summary>
        private int resultCount = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsDataReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public StatisticsDataReader(IDataReader reader)
            : base(reader)
        {
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <value>
        /// The row count.
        /// </value>
        public int RowCount
        {
            get { return this.rowCount; }
        }

        /// <summary>
        /// Gets the result count.
        /// </summary>
        /// <value>
        /// The result count.
        /// </value>
        public int ResultCount
        {
            get { return this.resultCount; }
        }

        /// <summary>
        /// Advances the <see cref="System.Data.IDataReader" /> to the next record.
        /// </summary>
        /// <returns>
        /// True if there are more rows; otherwise, false.
        /// </returns>
        public override bool Read()
        {
            bool read = base.Read();
            
            if (read)
            {
                Interlocked.Increment(ref this.rowCount);
            }

            return read;
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// True if there are more result sets; otherwise, false.
        /// </returns>
        public override bool NextResult()
        {
            bool nextResult = base.NextResult();

            if (nextResult)
            {
                Interlocked.Increment(ref this.resultCount);
            }

            return nextResult;
        }
    }
}
