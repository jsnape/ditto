// <copyright file="DataScriptParserFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace Ditto.DataLoad.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using NSubstitute;
    using Xunit;

    /// <summary>
    /// DataScriptParser Facts.
    /// </summary>
    public class DataScriptParserFacts
    {
        /// <summary>
        /// The target connection string.
        /// </summary>
        private const string TargetConnectionString = "Data Source= (local);Initial Catalog=tempdb;Integrated Security=SSPI;";

        /// <summary>
        /// The watermark service.
        /// </summary>
        private readonly IHighWatermarkService watermarkService;

        /// <summary>
        /// The test target.
        /// </summary>
        private readonly DataScriptParser target;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataScriptParserFacts"/> class.
        /// </summary>
        public DataScriptParserFacts()
        {
            this.watermarkService = Substitute.For<IHighWatermarkService>();
            this.target = new DataScriptParser(this.watermarkService, "UnitTest", TargetConnectionString);
        }

        /// <summary>
        /// Should throw when null watermark service passed to constructor.
        /// </summary>
        [Fact]
        public static void ShouldThrowWhenNullWatermarkServicePassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new DataScriptParser(null, "not checked", TargetConnectionString));
        }

        /// <summary>
        /// Should throw when null environment passed to constructor.
        /// </summary>
        [Fact]
        public void ShouldThrowWhenNullEnvironmentPassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new DataScriptParser(this.watermarkService, null, TargetConnectionString));
        }

        /// <summary>
        /// Should throw when blank environment passed to constructor.
        /// </summary>
        [Fact]
        public void ShouldThrowWhenBlankEnvironmentPassedToConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new DataScriptParser(this.watermarkService, string.Empty, TargetConnectionString));
        }

        /// <summary>
        /// Should throw when null reader is passed to Parse().
        /// </summary>
        [Fact]
        public void ShouldThrowWhenNullReaderParsed()
        {
            Assert.Throws<ArgumentNullException>(() => this.target.Parse(null));
        }

        /// <summary>
        /// Should parse empty configuration.
        /// </summary>
        [Fact]
        public void ShouldParseEmptyConfig()
        {
            var script = this.Parse("<DataLoad/>");

            Assert.NotNull(script);
            Assert.Equal("target", script.Connections.Single().Key);
            Assert.Empty(script.Operations);
        }

        /// <summary>
        /// Should parse a single connection.
        /// </summary>
        [Fact]
        public void ShouldParseSingleConnection()
        {
            var config = @"
<DataLoad>
  <Connection Name=""apollo"" Provider=""FileSystem"" ConnectionString=""\\londata002\ditto\EDW\Apollo\Samples\"" />
</DataLoad>
";
            
            var script = this.Parse(config);

            var connection = script.Connections.Where(c => c.Key == "apollo").Single();

            Assert.Equal("apollo", connection.Key);
            Assert.Equal(connection.Key, connection.Value.Name);
            
            Assert.Equal("FileSystem", connection.Value.Provider);
            Assert.Equal(@"\\londata002\ditto\EDW\Apollo\Samples\", connection.Value.ConnectionString);
        }

        /// <summary>
        /// Should parse a single connection.
        /// </summary>
        [Fact]
        public void ShouldParseInheritedConnection()
        {
            var config = @"
<DataLoad>
  <Connection Name=""apollo"" Provider=""FileSystem"" ConnectionString=""\\londata002\ditto\EDW\Apollo\Samples\"">
    <Connection Environment=""UnitTest"" ConnectionString=""\\londata002\ditto\EDW\Apollo\UnitTest\"" />
  </Connection>
</DataLoad>
";

            var script = this.Parse(config);

            var connection = script.Connections.Where(c => c.Key == "apollo").Single();

            Assert.Equal("apollo", connection.Key);
            Assert.Equal(connection.Key, connection.Value.Name);

            Assert.Equal("FileSystem", connection.Value.Provider);
            Assert.Equal(@"\\londata002\ditto\EDW\Apollo\UnitTest\", connection.Value.ConnectionString);
        }

        /// <summary>
        /// Should parse a single connection.
        /// </summary>
        [Fact]
        public void ShouldParseSingleOperation()
        {
            var config = @"
<DataLoad>
  <Connection Name=""apollo"" Provider=""FileSystem"" ConnectionString=""\\londata002\ditto\EDW\Apollo\Samples\"" />
  <Connection Name=""ditto.etl"" Provider=""System.Data.SqlClient"" ConnectionString=""Data Source=(local);Initial Catalog=DittoDv;Integrated Security=SSPI;""/>
  <Operation>
    <Source Name=""apollo.ClientContribution"" Type=""CsvFile"" Connection=""apollo"" FilePattern=""apollo-cc-*-v2.csv"" />
    <Target Name=""apollo.ClientContribution"" Connection=""ditto.etl"" HighWatermarkColumn=""_LastWriteTimeUtc"" />
  </Operation>
</DataLoad>
";

            var script = this.Parse(config);
            Assert.Single(script.Operations);

            var operation = script.Operations.Single();

            var dataSource = operation.Source;
            Assert.Equal("apollo.ClientContribution", dataSource.Name);
            Assert.IsAssignableFrom<CsvDataSource>(dataSource);

            var dataTarget = operation.Target;
            Assert.Equal("apollo.ClientContribution", dataTarget.Name);
        }

        /// <summary>
        /// Should parse a truncate before load attribute.
        /// </summary>
        [Fact]
        public void ShouldParseTruncateOnLoad()
        {
            var config = @"
<DataLoad>
  <Connection Name=""apollo"" Provider=""FileSystem"" ConnectionString=""\\londata002\ditto\EDW\Apollo\Samples\"" />
  <Connection Name=""ditto.etl"" Provider=""System.Data.SqlClient"" ConnectionString=""Data Source=(local);Initial Catalog=DittoDv;Integrated Security=SSPI;""/>
  <Operation>
    <Source Name=""apollo.ClientContribution"" Type=""CsvFile"" Connection=""apollo"" FilePattern=""apollo-cc-*-v2.csv"" />
    <Target Name=""apollo.ClientContribution"" Connection=""ditto.etl"" HighWatermarkColumn=""_LastWriteTimeUtc"" TruncateBeforeLoad=""True"" />
  </Operation>
</DataLoad>
";

            var script = this.Parse(config);

            var dataTarget = (SqlDataTarget)script.Operations.Single().Target;
            Assert.True(dataTarget.TruncateBeforeLoad);
        }

        /// <summary>
        /// Should parse a table source.
        /// </summary>
        [Fact]
        public void ShouldParseTableSource()
        {
            var config = @"
<DataLoad>
  <Connection Name=""apollo"" Provider=""FileSystem"" ConnectionString=""\\londata002\ditto\EDW\Apollo\Samples\"" />
  <Connection Name=""ditto.etl"" Provider=""System.Data.SqlClient"" ConnectionString=""Data Source=(local);Initial Catalog=DittoDv;Integrated Security=SSPI;""/>
  <Operation>
    <Source Name=""apollo.ClientContribution1"" Type=""Table"" Connection=""ditto.etl""/>
    <Target Name=""apollo.ClientContribution2"" Connection=""ditto.etl"" HighWatermarkColumn=""_LastWriteTimeUtc"" />
  </Operation>
</DataLoad>
";

            var script = this.Parse(config);
            Assert.Single(script.Operations);

            var operation = script.Operations.Single();

            var dataSource = operation.Source;
            Assert.IsAssignableFrom<TableDataSource>(dataSource);
        }

        /// <summary>
        /// Should parse a query source.
        /// </summary>
        [Fact]
        public void ShouldParseQuerySource()
        {
            var config = @"
<DataLoad>
  <Connection Name=""apollo"" Provider=""FileSystem"" ConnectionString=""\\londata002\ditto\EDW\Apollo\Samples\"" />
  <Connection Name=""ditto.etl"" Provider=""System.Data.SqlClient"" ConnectionString=""Data Source=(local);Initial Catalog=DittoDv;Integrated Security=SSPI;""/>
  <Operation>
    <Source Name=""apollo.ClientContribution1"" Type=""Query"" Connection=""ditto.etl"">
        select * from dbo.foo;
    </Source>
    <Target Name=""apollo.ClientContribution2"" Connection=""ditto.etl"" HighWatermarkColumn=""_LastWriteTimeUtc"" />
  </Operation>
</DataLoad>
";

            var script = this.Parse(config);
            Assert.Single(script.Operations);

            var operation = script.Operations.Single();

            var dataSource = operation.Source;
            Assert.IsAssignableFrom<QueryDataSource>(dataSource);

            Assert.Equal("select * from dbo.foo;", dataSource.ToString().Trim());
        }

        /// <summary>
        /// Should error on missing data source.
        /// </summary>
        [Fact]
        public void ShouldErrorOnMissingDataSource()
        {
            var config = @"
<DataLoad>
  <Connection Name=""apollo"" Provider=""FileSystem"" ConnectionString=""\\londata002\ditto\EDW\Apollo\Samples\"" />
  <Connection Name=""ditto.etl"" Provider=""System.Data.SqlClient"" ConnectionString=""Data Source=(local);Initial Catalog=DittoDv;Integrated Security=SSPI;""/>
  <Operation>
    <Target Name=""apollo.ClientContribution"" Connection=""ditto.etl"" HighWatermarkColumn=""_LastWriteTimeUtc"" />
  </Operation>
</DataLoad>
";

            Assert.Throws<DataScriptException>(() => this.Parse(config));
        }

        /// <summary>
        /// Should error on invalid data source type.
        /// </summary>
        [Fact]
        public void ShouldErrorOnInvalidDataSourceType()
        {
            var config = @"
<DataLoad>
  <Connection Name=""apollo"" Provider=""FileSystem"" ConnectionString=""\\londata002\ditto\EDW\Apollo\Samples\"" />
  <Connection Name=""ditto.etl"" Provider=""System.Data.SqlClient"" ConnectionString=""Data Source=(local);Initial Catalog=DittoDv;Integrated Security=SSPI;""/>
  <Operation>
    <Source Name=""apollo.ClientContribution1"" Type=""ThisIsNotValid"" Connection=""ditto.etl""/>
    <Target Name=""apollo.ClientContribution"" Connection=""ditto.etl"" HighWatermarkColumn=""_LastWriteTimeUtc"" />
  </Operation>
</DataLoad>
";

            Assert.Throws<DataScriptException>(() => this.Parse(config));
        }

        /// <summary>
        /// Should error on missing data target.
        /// </summary>
        [Fact]
        public void ShouldErrorOnMissingDataTarget()
        {
            var config = @"
<DataLoad>
  <Connection Name=""apollo"" Provider=""FileSystem"" ConnectionString=""\\londata002\ditto\EDW\Apollo\Samples\"" />
  <Connection Name=""ditto.etl"" Provider=""System.Data.SqlClient"" ConnectionString=""Data Source=(local);Initial Catalog=DittoDv;Integrated Security=SSPI;""/>
  <Operation>
    <Source Name=""apollo.ClientContribution"" Type=""CsvFile"" Connection=""apollo"" FilePattern=""apollo-cc-*-v2.csv"" />
  </Operation>
</DataLoad>
";

            Assert.Throws<DataScriptException>(() => this.Parse(config));
        }

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML to parse.</param>
        /// <returns>
        /// The parsed script.
        /// </returns>
        private DataScript Parse(string xml)
        {
            using (var reader = new StringReader(xml))
            {
                return this.target.Parse(reader);
            }
        }
    }
}
