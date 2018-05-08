#region Copyright (c) all rights reserved.
// <copyright file="CsvDataSourceFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad.Tests
{
    using System;
    using System.Linq;
    using Ditto.Core;
    using Csv = CsvHelper.Configuration;
    using NSubstitute;
    using Xunit;

    /// <summary>
    /// CsvDataSource Facts.
    /// </summary>
    public class CsvDataSourceFacts
    {
        /// <summary>
        /// The fake data connection.
        /// </summary>
        private static readonly DataConnection FakeDataConnection = new DataConnection("fake", "fake", "fake", "fake");

        /// <summary>
        /// Simple CSV test data.
        /// </summary>
        private static Tuple<string, string, DateTime>[] simpleCsv = new Tuple<string, string, DateTime>[] 
            {
                Tuple.Create(
                    "firstfile.csv", 
                    "stringval,intval,dateval,boolval,byteval,charval,decimalval,doubleval,floatval,guidval\r\n\"wibb,le\",2,2015-01-31,true,1,x,-10.4321,1.456E5,-4E2,\"{11A59F73-6905-4299-8E54-5E749B2ACD68}\"",
                    new DateTime(2015, 01, 01)),
                Tuple.Create(
                    "secondfile.csv", 
                    "stringval,intval,dateval,boolval,byteval,charval,decimalval,doubleval,floatval,guidval\r\n\"wobb,le\",3,2015-03-31,false,2,y,-11.4321,55.456E5,-4E2,\"{11A59073-6905-4299-8E54-5E749B2ACD68}\"",
                    new DateTime(2015, 01, 02)),
            };

        /// <summary>
        /// The file system service.
        /// </summary>
        private readonly IFileSystemService fileSystemService;

        /// <summary>
        /// The test target.
        /// </summary>
        private readonly CsvDataSource target;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataSourceFacts"/> class.
        /// </summary>
        public CsvDataSourceFacts()
        {
            var fakeFiles = simpleCsv.Select(f => new FakeFileInfoWrapper(f.Item1, f.Item3));
            var fakeContents = simpleCsv.Select(f => f.Item2);

            this.fileSystemService = new FakeFileSystemService(fakeFiles, fakeContents);

            this.target = new CsvDataSource("test source", FakeDataConnection, "*.csv", FolderMode.MultipleFile, new Csv.Configuration(), this.fileSystemService);

            this.target.Parent = Substitute.For<IDataOperationInfo>();
        }

        /// <summary>
        /// Zero files should return null reader.
        /// </summary>
        [Fact]
        public static void ZeroFilesShouldReturnNullReader()
        {
            var fileSystemService = new FakeFileSystemService(
                Enumerable.Empty<IFileInfo>(), Enumerable.Empty<string>());

            var target = new CsvDataSource("test source", FakeDataConnection, "*.csv", FolderMode.MultipleFile, new Csv.Configuration(), fileSystemService);

            Assert.Null(target.GetData(Enumerable.Empty<string>(), null));
        }

        /// <summary>
        /// Should throw when null name passed to constructor.
        /// </summary>
        [Fact]
        public static void ShouldThrowWhenNullNamePassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvDataSource(null, FakeDataConnection, "not checked", FolderMode.MultipleFile, new Csv.Configuration()));
        }

        /// <summary>
        /// Should throw when null connection passed to constructor.
        /// </summary>
        [Fact]
        public static void ShouldThrowWhenNullConnectionPassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvDataSource("foo", null, "not checked", FolderMode.MultipleFile, new Csv.Configuration()));
        }

        /// <summary>
        /// Should throw when null file pattern passed to constructor.
        /// </summary>
        [Fact]
        public static void ShouldThrowWhenFilePatternPassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvDataSource("foo", FakeDataConnection, null, FolderMode.MultipleFile, new Csv.Configuration()));
        }

        /// <summary>
        /// Should throw when null file system service passed to constructor.
        /// </summary>
        [Fact]
        public static void ShouldThrowWhenFileSystemServicePassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvDataSource("foo", FakeDataConnection, "*.csv", FolderMode.MultipleFile, new Csv.Configuration(), null));
        }

        /// <summary>
        /// The Source Name is correct.
        /// </summary>
        [Fact]
        public void SourceNameIsCorrect()
        {
            Assert.Equal("test source", this.target.Name);
        }

        /// <summary>
        /// The Columns is correct.
        /// </summary>
        [Fact]
        public void ColumnsAreCorrect()
        {
            var columns = new string[]
            {
                "_LineNumber", "_SourceFile", "_LastWriteTimeUtc", "stringval", "intval", "dateval", "boolval", "byteval", "charval", "decimalval", "doubleval", "floatval", "guidval"
            };

            Assert.Equal(columns, this.target.GetColumns(null));
        }

        /// <summary>
        /// Schemas the rows count should equal column count.
        /// </summary>
        [Fact]
        public void SchemaRowsCountShouldEqualColumnCount()
        {
            var schema = this.target.GetSchemaTable();
            Assert.Equal(13, schema.Rows.Count);
        }
    }
}
