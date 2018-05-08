#region Copyright (c) all rights reserved.
// <copyright file="CheckScriptParserFacts.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Ditto.Core;
    using NSubstitute;
    using Xunit;

    /// <summary>
    /// Check script parser facts.
    /// </summary>
    public class CheckScriptParserFacts
    {
        /// <summary>
        /// The XML test data.
        /// </summary>
        private const string Xml = @"
<DataCheck>
  <Connection Name=""conn1""
              Provider=""System.Data.SqlClient""
              ConnectionString=""Data Source=(local);Initial Catalog=DittoEdw;Integrated Security=SSPI;"" />

  <Owner Name=""owner1"">
    <Contact>owner1@ditto.com</Contact>
  </Owner>

  <Feature Name=""Feature1"">
    <Entity Connection=""conn1"" Name=""dbo.Entity1"" Owner=""owner1"">
    </Entity>
  </Feature>

  <Feature Connection=""conn1"" Name=""Feature2"" Owner=""owner1"">
    <Entity Match=""dw[\.].*"">
    </Entity>
  </Feature>

  <Entity Connection=""conn1"" Name=""dbo.Entity3"" Owner=""owner1"">
  </Entity>

</DataCheck>
";

        /// <summary>
        /// The xml document.
        /// </summary>
        private readonly XDocument document;

        /// <summary>
        /// The metadata.
        /// </summary>
        private readonly IDbMetadata metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckScriptParserFacts"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DittoEdw", Justification = "Test data")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ConnectionString", Justification = "Test data")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ditto", Justification = "Test data")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SqlClient", Justification = "Test data")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DataCheck", Justification = "Test data")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "dbo", Justification = "Test data")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "dw", Justification = "Test data")]
        public CheckScriptParserFacts()
        {
            this.metadata = Substitute.For<IDbMetadata>();
            
            this.metadata.AllTables().Returns(
                new string[] { "dw.TableA", "dw.TableB", "dw.TableC", "dbo.TableC" });

            this.document = XDocument.Parse(Xml, LoadOptions.SetLineInfo);
        }

        /// <summary>
        /// Should load all features.
        /// </summary>
        [Fact]
        public void ShouldLoadAllFeatures()
        {
            var features = this.document
                .Descendants("Feature");

            Assert.Equal(2, features.Count());
        }

        /// <summary>
        /// Should load all entities.
        /// </summary>
        [Fact]
        public void ShouldLoadAllEntities()
        {
            var entities = this.document
                .Descendants("Entity");

            Assert.Equal(3, entities.Count());
        }

        /// <summary>
        /// All entities should have connections.
        /// </summary>
        [Fact]
        public void AllEntitiesShouldHaveConnections()
        {
            var entities = this.document
                .Descendants("Entity")
                .Where(e => e.FindAncestorAttribute("Connection") == null);

            Assert.Empty(entities);
        }

        /// <summary>
        /// All entities should have owners.
        /// </summary>
        [Fact]
        public void AllEntitiesShouldHaveOwners()
        {
            var entities = this.document
                .Descendants("Entity")
                .Where(e => e.FindAncestorAttribute("Owner") == null);

            Assert.Empty(entities);
        }

        /// <summary>
        /// Entities should have owners.
        /// </summary>
        [Fact]
        public void EntitiesWithoutFeaturesShouldNotHaveProductFeatures()
        {
            var entities = this.document
                .Descendants("Entity")
                .Where(e => e.FindAncestorElementAttribute("Feature", "Name") == null);

            Assert.Single(entities);
        }

        /// <summary>
        /// Should expand entities with match statements.
        /// </summary>
        [Fact]
        public void ShouldExpandEntitiesWithMatchStatements()
        {
            var entities = this.document
                .Descendants("Entity")
                .SelectMany(e => ExpandEntity(e, this.metadata));

            Assert.Equal(5, entities.Count());
        }

        /// <summary>
        /// Expands the entities.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>
        /// A sequence of expanded entities.
        /// </returns>
        private static IEnumerable<XElement> ExpandEntity(XElement element, IDbMetadata metadata)
        {
            if (element.Attribute("Match") == null)
            {
                yield return element;
            }
            else
            {
                Regex match = new Regex(element.Attribute("Match").Value, RegexOptions.IgnoreCase);

                var matchedTables = metadata
                    .AllTables()
                    .Where(t => match.IsMatch(t));

                foreach (var table in matchedTables)
                {
                    var expandedElement = new XElement(element);
                    expandedElement.SetAttributeValue("Name", table);

                    yield return expandedElement;
                }
            }
        }
    }
}
