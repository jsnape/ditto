#region Copyright (c) all rights reserved.
// <copyright file="DataScriptParser.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Ditto.Core;
    using Csv = CsvHelper.Configuration;

    /// <summary>
    /// Data Script Parser.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "JS: Would result in very unreadable code")] 
    public class DataScriptParser : LoaderScriptParser
    {
        /// <summary>
        /// The high watermark service.
        /// </summary>
        private readonly IHighWatermarkService highWatermarkService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataScriptParser" /> class.
        /// </summary>
        /// <param name="highWatermarkService">The high watermark service.</param>
        /// <param name="currentEnvironment">The current environment.</param>
        /// <param name="targetConnectionString">The target connection string.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public DataScriptParser(IHighWatermarkService highWatermarkService, string currentEnvironment, string targetConnectionString)
            : base(currentEnvironment, targetConnectionString)
        {
            this.highWatermarkService = highWatermarkService ?? throw new ArgumentNullException("highWatermarkService");
        }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// A data script for execution.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If any of the arguments are null or blank.</exception>
        public DataScript Parse(TextReader config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            XDocument xml = XDocument.Load(config);

            var connections = this.LoadConnections(xml);
            var operations = this.LoadOperations(connections.Values, xml);

            return new DataScript(connections.Values, operations);
        }

        /// <summary>
        /// Creates the specified configuration.
        /// </summary>
        /// <param name="sourceElement">The source element.</param>
        /// <param name="connections">The connections.</param>
        /// <returns>
        /// A data source instance.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">If an invalid source type is specified.</exception>
        private static IDataSource CreateSource(XElement sourceElement, IEnumerable<DataConnection> connections)
        {
            var type = sourceElement.Attribute("Type").Value;
            var name = sourceElement.Attribute("Name").Value;
            var connection = connections.Where(c => c.Name == sourceElement.Attribute("Connection").Value).Single();

            var schema = LoadSchema(sourceElement.Element("Schema"));

            switch (type)
            {
                case "CsvFile":
                    return CreateCsvFileSource(name, connection, sourceElement);
                case "Table":
                    return CreateTableSource(name, connection);
                case "Query":
                    return CreateQuerySource(name, connection, schema, sourceElement.Value);
                default:
                    throw new DataScriptException(string.Format(CultureInfo.CurrentCulture, "Source type '{0}' is not valid for {1}", type, name));
            }
        }

        /// <summary>
        /// Loads the schema.
        /// </summary>
        /// <param name="parent">The schema element.</param>
        /// <returns>
        /// A schema table.
        /// </returns>
        private static DataTable LoadSchema(XElement parent)
        {
            if (parent == null)
            {
                return null;
            }

            DataTable schema = new DataTable();
            var disposable = schema;

            try
            {
                schema.Locale = CultureInfo.CurrentCulture;

                var columns = new DataColumn[] 
                {
                    new DataColumn("ColumnName", typeof(string)),
                    new DataColumn("ColumnSize", typeof(int)),
                    new DataColumn("DataType", typeof(Type)),
                    new DataColumn("AllowDBNull", typeof(bool)),
                    new DataColumn("NumericPrecision", typeof(short)),
                    new DataColumn("NumericScale", typeof(short)),
                };

                schema.Columns.AddRange(columns);

                // It's a bit weird naming the variable rows since they are schema columns.
                // However these are DataTable rows.
                var rows = parent.Elements("Column");

                foreach (var row in rows)
                {
                    var dataRow = LoadColumn(schema, row);
                    schema.Rows.Add(dataRow);
                }

                disposable = null;
                return schema;
            }
            finally
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Loads the column.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="parent">The parent.</param>
        /// <returns>
        /// A data row for the xml column.
        /// </returns>
        private static DataRow LoadColumn(DataTable schema, XElement parent)
        {
            var row = schema.NewRow();

            row.SetField("ColumnName", parent.Attribute("ColumnName").Value);
            row.SetField("ColumnSize", ParseAttribute(FindAttribute(parent, "ColumnSize"), 0));
            row.SetField("AllowDBNull", ParseAttribute(FindAttribute(parent, "AllowDBNull"), true));
            row.SetField("NumericPrecision", ParseAttribute(FindAttribute(parent, "NumericPrecision"), 0));
            row.SetField("NumericScale", ParseAttribute(FindAttribute(parent, "NumericScale"), 0));

            var dataType = parent.Attribute("DataType").Value;
            row.SetField("DataType", Type.GetType(dataType));

            return row;
        }

        /// <summary>
        /// Creates the table source.
        /// </summary>
        /// <param name="name">The name of the source.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="query">The query.</param>
        /// <returns>
        /// A new IDataSource instance.
        /// </returns>
        private static IDataSource CreateQuerySource(string name, DataConnection connection, DataTable schema, string query)
        {
            return new QueryDataSource(connection.CreateConnectionFactory(), name, query, schema);
        }

        /// <summary>
        /// Creates the table source.
        /// </summary>
        /// <param name="name">The name of the source.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>
        /// A new IDataSource instance.
        /// </returns>
        private static IDataSource CreateTableSource(string name, DataConnection connection)
        {
            return new TableDataSource(connection.CreateConnectionFactory(), name);
        }

        /// <summary>
        /// Creates the CSV file source.
        /// </summary>
        /// <param name="name">The name of the source.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// A new IDataSource instance.
        /// </returns>
        private static IDataSource CreateCsvFileSource(string name, DataConnection connection, XElement config)
        {
            var filePattern = config.Attribute("FilePattern").Value;
            var mode = (FolderMode)Enum.Parse(typeof(FolderMode), FindAttribute(config, "Mode") ?? "MultipleFile", true);

            // This ties us to the CsvHelper implementation but it makes like a little simpler.
            // If needed this can be wrapped at a later date in a custom class.
            var csvConfig = new Csv.Configuration();

            csvConfig.AllowComments = ParseAttribute(FindElementText(config, "AllowComments"), csvConfig.AllowComments);
            csvConfig.Comment = ParseAttribute(FindElementText(config, "Comment"), csvConfig.Comment);
            csvConfig.Delimiter = ParseAttribute(FindElementText(config, "Delimiter"), csvConfig.Delimiter);
            csvConfig.DetectColumnCountChanges = ParseAttribute(FindElementText(config, "DetectColumnCountChanges"), csvConfig.DetectColumnCountChanges);
            ////csvConfig.HasExcelSeparator = ParseAttribute(FindElementText(config, "HasExcelSeparator"), csvConfig.HasExcelSeparator);
            csvConfig.HasHeaderRecord = ParseAttribute(FindElementText(config, "HasHeaderRecord"), csvConfig.HasHeaderRecord);
            csvConfig.IgnoreBlankLines = ParseAttribute(FindElementText(config, "IgnoreBlankLines"), csvConfig.IgnoreBlankLines);
            csvConfig.IgnoreQuotes = ParseAttribute(FindElementText(config, "IgnoreQuotes"), csvConfig.IgnoreQuotes);
            csvConfig.Quote = ParseAttribute(FindElementText(config, "Quote"), csvConfig.Quote);

            // These are defaulted because its the right way to handle the values.
            // If you absolutely need then set to true/false in the config file.
            if (ParseAttribute(FindElementText(config, "SkipEmptyRecords"), false))
            {
                csvConfig.ShouldSkipRecord = record => record.All(string.IsNullOrEmpty);
            }

            if (ParseAttribute(FindElementText(config, "TrimHeaders"), true))
            {
                csvConfig.PrepareHeaderForMatch = header => header?.Trim();
            }

            if (ParseAttribute(FindElementText(config, "IgnoreHeaderWhiteSpace"), true))
            {
                csvConfig.PrepareHeaderForMatch = header => csvConfig.PrepareHeaderForMatch(header)?.Replace(" ", string.Empty);
            }

            if (!ParseAttribute(FindElementText(config, "IsHeaderCaseSensitive"), false))
            {
                csvConfig.PrepareHeaderForMatch = header => csvConfig.PrepareHeaderForMatch(header)?.ToUpperInvariant();
            }

            csvConfig.TrimOptions = ParseAttribute(FindElementText(config, "TrimFields"), true) ? Csv.TrimOptions.Trim : Csv.TrimOptions.None;

            return new CsvDataSource(name, connection, filePattern, mode, csvConfig);
        }

        /// <summary>
        /// Loads the operations.
        /// </summary>
        /// <param name="connections">The connections.</param>
        /// <param name="xml">The XML config.</param>
        /// <returns>A sequence of data operation instances.</returns>
        private IEnumerable<DataOperation> LoadOperations(IEnumerable<DataConnection> connections, XDocument xml)
        {
            var operations =
                //// Starting at the root
                xml
                //// Find all the enabled operations (enabled is default)
                .Descendants("Operation")
                .Where(x => ParseAttribute(FindAttribute(x, "Enabled"), true))
                //// This second clause allows for bulk disabling
                .Where(x => x.Parent == null || ParseAttribute(FindAttribute(x.Parent, "Enabled"), true))
                //// Create a DataOperation instance
                .Select(x => this.CreateDataOperation(connections, x))
                //// Force the enumeration to be materialized.
                .ToArray();

            return operations;
        }

        /// <summary>
        /// Creates the data operation.
        /// </summary>
        /// <param name="connections">The connections.</param>
        /// <param name="operationElement">The operation element.</param>
        /// <returns>A DataOperation instance.</returns>
        private DataOperation CreateDataOperation(IEnumerable<DataConnection> connections, XElement operationElement)
        {
            var sourceElement = operationElement.Element("Source");

            if (sourceElement == null)
            {
                throw new DataScriptException("Source element missing");
            }

            var source = CreateSource(sourceElement, connections);

            var targetElement = operationElement.Element("Target");

            if (targetElement == null)
            {
                throw new DataScriptException("Target element missing");
            }

            IDataTarget target = this.CreateTarget(targetElement, connections);

            return new DataOperation(source, target);
        }

        /// <summary>
        /// Creates the target.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="connections">The connections.</param>
        /// <returns>A data target instance.</returns>
        private IDataTarget CreateTarget(XElement targetElement, IEnumerable<DataConnection> connections)
        {
            var name = targetElement.Attribute("Name").Value;
            var highWatermarkColumn = FindAttribute(targetElement, "HighWatermarkColumn");
            var connection = connections.Where(c => c.Name == targetElement.Attribute("Connection").Value).Single();

            bool truncateBeforeLoad = ParseAttribute(FindAttribute(targetElement, "TruncateBeforeLoad"), false);

            return new SqlDataTarget(
                connection.CreateConnectionFactory(),
                connection.CreateBulkCopyFactory(),
                this.highWatermarkService,
                name,
                highWatermarkColumn,
                truncateBeforeLoad);
        }
    }
}
