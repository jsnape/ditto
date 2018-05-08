#region Copyright (c) all rights reserved.
// <copyright file="MultipleCsvDataReaderFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.Tests
{
    using System;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Csv = CsvHelper.Configuration;
    using Xunit;

    /// <summary>
    /// MultipleCsvDataReader Facts.
    /// </summary>
    public sealed class MultipleCsvDataReaderFacts : IDisposable
    {
        /// <summary>
        /// Simple CSV test data.
        /// </summary>
        private static Tuple<string, string>[] simpleCsv = new Tuple<string, string>[] 
            {
                Tuple.Create("first file", "stringval,intval,dateval,boolval,byteval,charval,decimalval,doubleval,floatval,guidval\r\n\"wibb,le\",2,2015-01-31,true,1,x,-10.4321,1.456E5,-4E2,\"{11A59F73-6905-4299-8E54-5E749B2ACD68}\""),
                Tuple.Create("second file", "stringval,intval,dateval,boolval,byteval,charval,decimalval,doubleval,floatval,guidval\r\n\"wobb,le\",3,2015-03-31,false,2,y,-11.4321,55.456E5,-4E2,\"{11A59073-6905-4299-8E54-5E749B2ACD68}\"")
            };

        /// <summary>
        /// The test target.
        /// </summary>
        private readonly IDataReader target;

        /// <summary>
        /// The test shadow.
        /// </summary>
        private IDataReader shadow;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleCsvDataReaderFacts"/> class.
        /// </summary>
        public MultipleCsvDataReaderFacts()
        {
            var sources = simpleCsv.Select(f => new StringSourceFile(f.Item1, f.Item2));
            this.target = new MultipleCsvDataReader(sources, new Csv.Configuration());

            // Setup a shadow - this version allows up to test all the passthrough functions.
            var source = simpleCsv.First();
            TextReader reader = new StringReader(source.Item2);

            try
            {
                this.shadow = new CsvDataReader(reader, source.Item1, new Csv.Configuration());
                reader = null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Should throw when null source passed to constructor.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code")]
        [Fact]
        public static void ShouldThrowWhenNullSourcesPassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new MultipleCsvDataReader(null, new Csv.Configuration()));
        }

        /// <summary>
        /// Determines whether [is closed should pass through].
        /// </summary>
        [Fact]
        public void IsClosedShouldPassThrough()
        {
            Assert.Equal(this.shadow.IsClosed, this.target.IsClosed);
        }

        /// <summary>
        /// Depth should pass through.
        /// </summary>
        [Fact]
        public void DepthShouldPassThrough()
        {
            Assert.Equal(this.shadow.Depth, this.target.Depth);
        }

        /// <summary>
        /// FieldCount should pass through.
        /// </summary>
        [Fact]
        public void FieldCountShouldPassThrough()
        {
            Assert.Equal(this.shadow.FieldCount, this.target.FieldCount);
        }

        /// <summary>
        /// RecordsAffected should pass through.
        /// </summary>
        [Fact]
        public void RecordsAffectedShouldPassThrough()
        {
            Assert.Equal(this.shadow.RecordsAffected, this.target.RecordsAffected);
        }

        /// <summary>
        /// Name Indexer should pass through.
        /// </summary>
        [Fact]
        public void NameIndexerShouldPassThrough()
        {
            Assert.Equal(this.shadow["_LineNumber"], this.target["_LineNumber"]);
            Assert.Equal(this.shadow["dateval"], this.target["dateval"]);
        }

        /// <summary>
        /// Ordinal Indexer should pass through.
        /// </summary>
        [Fact]
        public void OrdinalIndexerShouldPassThrough()
        {
            Assert.Equal(this.shadow[1], this.target[1]);
            Assert.Equal(this.shadow[3], this.target[3]);
        }

        /// <summary>
        /// IsDBNull should pass through.
        /// </summary>
        [Fact]
        public void IsDBNullShouldPassThrough()
        {
            Assert.Equal(this.shadow.IsDBNull(0), this.target.IsDBNull(0));
        }

        /// <summary>
        /// GetValue should pass through.
        /// </summary>
        [Fact]
        public void GetValueShouldPassThrough()
        {
            Assert.Equal(this.shadow.GetValue(0), this.target.GetValue(0));
        }

        /// <summary>
        /// GetString should pass through.
        /// </summary>
        [Fact]
        public void GetStringShouldPassThrough()
        {
            Assert.Equal(this.shadow.GetString(1), this.target.GetString(1));
        }

        /// <summary>
        /// GetBoolean should pass through.
        /// </summary>
        [Fact]
        public void GetBooleanShouldPassThrough()
        {
            int ordinal = this.target.GetOrdinal("boolval");
            Assert.Equal(this.shadow.GetBoolean(ordinal), this.target.GetBoolean(ordinal));
        }

        /// <summary>
        /// GetByte should pass through.
        /// </summary>
        [Fact]
        public void GetByteShouldPassThrough()
        {
            int ordinal = this.target.GetOrdinal("byteval");
            Assert.Equal(this.shadow.GetByte(ordinal), this.target.GetByte(ordinal));
        }

        /// <summary>
        /// GetChar should pass through.
        /// </summary>
        [Fact]
        public void GetCharShouldPassThrough()
        {
            int ordinal = this.target.GetOrdinal("charval");
            Assert.Equal(this.shadow.GetChar(ordinal), this.target.GetChar(ordinal));
        }

        /// <summary>
        /// GetDecimal should pass through.
        /// </summary>
        [Fact]
        public void GetDecimalShouldPassThrough()
        {
            int ordinal = this.target.GetOrdinal("decimalval");
            Assert.Equal(this.shadow.GetDecimal(ordinal), this.target.GetDecimal(ordinal));
        }

        /// <summary>
        /// GetDouble should pass through.
        /// </summary>
        [Fact]
        public void GetDoubleShouldPassThrough()
        {
            int ordinal = this.target.GetOrdinal("doubleval");
            Assert.Equal(this.shadow.GetDouble(ordinal), this.target.GetDouble(ordinal));
        }

        /// <summary>
        /// GetFloat should pass through.
        /// </summary>
        [Fact]
        public void GetFloatShouldPassThrough()
        {
            int ordinal = this.target.GetOrdinal("floatval");
            Assert.Equal(this.shadow.GetFloat(ordinal), this.target.GetFloat(ordinal));
        }

        /// <summary>
        /// <c>GetGuid</c> should pass through.
        /// </summary>
        [Fact]
        public void GetGuidShouldPassThrough()
        {
            int ordinal = this.target.GetOrdinal("guidval");
            Assert.Equal(this.shadow.GetGuid(ordinal), this.target.GetGuid(ordinal));
        }

        /// <summary>
        /// <c>GetInt16</c> should pass through.
        /// </summary>
        [Fact]
        public void GetInt16ShouldPassThrough()
        {
            int ordinal = this.target.GetOrdinal("intval");
            Assert.Equal(this.shadow.GetInt16(ordinal), this.target.GetInt16(ordinal));
        }

        /// <summary>
        /// <c>GetInt32</c> should pass through.
        /// </summary>
        [Fact]
        public void GetInt32ShouldPassThrough()
        {
            int ordinal = this.target.GetOrdinal("intval");
            Assert.Equal(this.shadow.GetInt32(ordinal), this.target.GetInt32(ordinal));
        }

        /// <summary>
        /// <c>GetInt64</c> should pass through.
        /// </summary>
        [Fact]
        public void GetInt64ShouldPassThrough()
        {
            int ordinal = this.target.GetOrdinal("intval");
            Assert.Equal(this.shadow.GetInt64(ordinal), this.target.GetInt64(ordinal));
        }

        /// <summary>
        /// GetDateTime should pass through.
        /// </summary>
        [Fact]
        public void GetDateTimeShouldPassThrough()
        {
            Assert.Equal(this.shadow.GetDateTime(2), this.target.GetDateTime(2));
        }

        /// <summary>
        /// GetOrdinal should pass through.
        /// </summary>
        [Fact]
        public void GetOrdinalShouldPassThrough()
        {
            Assert.Equal(this.shadow.GetOrdinal("b"), this.target.GetOrdinal("b"));
        }

        /// <summary>
        /// GetName should pass through.
        /// </summary>
        [Fact]
        public void GetNameShouldPassThrough()
        {
            Assert.Equal(this.shadow.GetName(1), this.target.GetName(1));
        }

        /// <summary>
        /// Should not be implemented.
        /// </summary>
        [Fact]
        public void ShouldNotBeImplemented()
        {
            char[] charBuffer = null;
            Assert.Throws<NotImplementedException>(() => this.target.GetChars(0, 0, charBuffer, 0, 0));

            byte[] byteBuffer = null;
            Assert.Throws<NotImplementedException>(() => this.target.GetBytes(0, 0, byteBuffer, 0, 0));

            Assert.Throws<NotImplementedException>(() => this.target.GetSchemaTable());
            Assert.Throws<NotImplementedException>(() => this.target.GetDataTypeName(0));
            Assert.Throws<NotImplementedException>(() => this.target.GetFieldType(0));

            object[] valueBuffer = null;
            Assert.Throws<NotImplementedException>(() => this.target.GetValues(valueBuffer));

            Assert.Throws<NotImplementedException>(() => this.target.GetData(0));
        }

        /// <summary>
        /// Determines whether this instance [can read multiple CSV files].
        /// </summary>
        [Fact]
        public void CanReadMultipleCsvFiles()
        {
            var resultCount = 0;
            var recordCount = 0;

            do
            {
                while (this.target.Read())
                {
                    recordCount++;
                }

                resultCount++;
            }
            while (this.target.NextResult());

            Assert.Equal(2, resultCount);
            Assert.Equal(2, recordCount);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.shadow.Close();
            this.target.Close();
        }
    }
}
