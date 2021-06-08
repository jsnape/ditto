#region Copyright (c) all rights reserved.
// <copyright file="CsvDataReaderFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.Tests
{
    using System;
    using System.Data;
    using System.IO;
    using Xunit;
    using Csv = CsvHelper.Configuration;

    /// <summary>
    /// CsvDataReader Facts.
    /// </summary>
    public sealed class CsvDataReaderFacts : IDisposable
    {
        /// <summary>
        /// Simple CSV test data.
        /// </summary>
        private const string SimpleCsv = 
@"stringval,intval,dateval,boolval,byteval,charval,decimalval,doubleval,floatval,guidval,
""wibb,le"",2,2015-01-31,true,1,x,-10.4321,1.456E5,-4E2,""{11A59F73-6905-4299-8E54-5E749B2ACD68}"",
,,,,,,,,,,
";

        /// <summary>
        /// The test target.
        /// </summary>
        private readonly IDataReader target;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataReaderFacts"/> class.
        /// </summary>
        public CsvDataReaderFacts()
        {
            TextReader source = null;

            try
            {
                source = new StringReader(SimpleCsv);
                this.target = new CsvDataReader(source, "simple.csv", new Csv.Configuration());
                source = null;
            }
            finally
            {
                if (source != null)
                {
                    source.Dispose();
                }
            }
        }

        /// <summary>
        /// Should throw when null source passed to constructor.
        /// </summary>
        [Fact]
        public static void ShouldThrowWhenNullSourcePassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvDataReader(null, "not checked", new Csv.Configuration()));
        }

        /// <summary>
        /// Should throw when null filename passed to constructor.
        /// </summary>
        [Fact]
        public static void ShouldThrowWhenNullFileNamePassedToConstructor()
        {
            TextReader source = null;

            try
            {
                source = new StringReader(SimpleCsv);
                Assert.Throws<ArgumentNullException>(() => new CsvDataReader(source, null, new Csv.Configuration()));
                source = null;
            }
            finally
            {
                if (source != null)
                {
                    source.Dispose();
                }
            }
        }

        /// <summary>
        /// Determines whether this instance [can read correct number of rows].
        /// </summary>
        [Fact]
        public void CanReadCorrectNumberOfRows()
        {
            int rows = 0;
            while (this.target.Read())
            {
                rows++;
            }

            Assert.Equal(2, rows);
        }

        /// <summary>
        /// Should be closed once all rows read.
        /// </summary>
        [Fact]
        public void ShouldBeClosedOnceAllRowsRead()
        {
            while (this.target.Read())
            {
            }

            Assert.True(this.target.IsClosed);
        }

        /// <summary>
        /// The field count should be correct.
        /// </summary>
        [Fact]
        public void FieldCountIsCorrect()
        {
            Assert.Equal(14, this.target.FieldCount);
        }

        /// <summary>
        /// Should return the correct field values.
        /// </summary>
        [Fact]
        public void ShouldReturnTheCorrectFieldValues()
        {
            this.target.Read();

            Assert.Equal(1, this.target.GetInt32(0));
            Assert.Equal(1, this.target.GetValue(0));

            Assert.Equal("simple.csv", this.target.GetString(1));
            Assert.Equal("wibb,le", this.target.GetString(3));
            
            Assert.Equal(2, this.target.GetInt16(4));
            Assert.Equal(2, this.target.GetInt32(4));
            Assert.Equal(2, this.target.GetInt64(4));
            
            Assert.Equal(new DateTime(2015, 1, 31), this.target.GetDateTime(5));
            Assert.Equal(new DateTime(1601, 1, 1), this.target.GetDateTime(2));

            Assert.True(this.target.GetBoolean(6));
            Assert.Equal(1, this.target.GetByte(7));
            Assert.Equal('x', this.target.GetChar(8));
            Assert.Equal(-10.4321M, this.target.GetDecimal(9));
            Assert.Equal(1.456E5, this.target.GetDouble(10));
            Assert.Equal(-4E2, this.target.GetFloat(11));
            Assert.Equal(new Guid("{11A59F73-6905-4299-8E54-5E749B2ACD68}"), this.target.GetGuid(12));
        }

        /// <summary>
        /// Should read null field and return is database null.
        /// </summary>
        [Fact]
        public void ShouldReadNullFieldAndReturnIsDBNull()
        {
            this.target.Read();
            this.target.Read();

            Assert.True(this.target.IsDBNull(3));
        }

        /// <summary>
        /// Should return correct column ordinals.
        /// </summary>
        [Fact]
        public void ShouldReturnCorrectColumnOrdinals()
        {
            Assert.Equal(0, this.target.GetOrdinal("_LineNumber"));
            Assert.Equal(3, this.target.GetOrdinal("stringval"));
            Assert.Equal(-1, this.target.GetOrdinal("thiscolumndoesntexist"));
        }

        /// <summary>
        /// GetColumnOrdinal() should throw when passed null.
        /// </summary>
        [Fact]
        public void GetColumnOrdinalShouldThrowWhenPassedNull()
        {
            Assert.Throws<ArgumentNullException>(() => this.target.GetOrdinal(null));
        }

        /// <summary>
        /// Should return correct column names.
        /// </summary>
        [Fact]
        public void ShouldReturnCorrectColumnNames()
        {
            Assert.Equal("_SourceFile", this.target.GetName(1));
            Assert.Equal("intval", this.target.GetName(4));
        }

        /// <summary>
        /// Name indexer should return same as get value.
        /// </summary>
        [Fact]
        public void NameIndexerShouldReturnSameAsGetValue()
        {
            var column = "dateval";
            Assert.Equal(this.target.GetValue(this.target.GetOrdinal(column)), this.target[column]);
        }

        /// <summary>
        /// Ordinal indexer should return same as get value.
        /// </summary>
        [Fact]
        public void OrdinalIndexerShouldReturnSameAsGetValue()
        {
            var index = 4;
            Assert.Equal(this.target.GetValue(index), this.target[index]);
        }

        /// <summary>
        /// Hard coded values should be correct.
        /// </summary>
        [Fact]
        public void HardcodedValuesShouldBeCorrect()
        {
            Assert.Equal(0, this.target.Depth);
            Assert.Equal(-1, this.target.RecordsAffected);
            Assert.False(this.target.NextResult());
        }

        /// <summary>
        /// Should throw when fixed field index is passed.
        /// </summary>
        [Fact]
        public void ShouldThrowWhenFixedFieldIndexIsPassed()
        {
            Assert.Throws<InvalidOperationException>(() => this.target.GetDecimal(0));
            Assert.Throws<InvalidOperationException>(() => this.target.GetBoolean(0));
            Assert.Throws<InvalidOperationException>(() => this.target.GetByte(0));
            Assert.Throws<InvalidOperationException>(() => this.target.GetChar(0));
            Assert.Throws<InvalidOperationException>(() => this.target.GetDouble(0));
            Assert.Throws<InvalidOperationException>(() => this.target.GetFloat(0));
            Assert.Throws<InvalidOperationException>(() => this.target.GetGuid(0));
            Assert.Throws<InvalidOperationException>(() => this.target.GetInt16(0));
            Assert.Throws<InvalidOperationException>(() => this.target.GetInt64(0));
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.target.Close();   
        }
    }
}
