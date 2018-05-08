#region Copyright (c) all rights reserved.
// <copyright file="CheckScriptParser.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Ditto.DomainEvents;
    using Ditto.Core;
    using Ditto.DataCheck.DomainEvents;

    /// <summary>
    /// Check Script Parser.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "JS: Would result in very unreadable code")]
    public class CheckScriptParser : LoaderScriptParser
    {
        /// <summary>
        /// The connections.
        /// </summary>
        private IDictionary<string, DataConnection> connections;

        /// <summary>
        /// The owners.
        /// </summary>
        private IDictionary<string, DataOwner> owners;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckScriptParser" /> class.
        /// </summary>
        /// <param name="currentEnvironment">The current environment.</param>
        /// <param name="targetConnectionString">The target connection string.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        public CheckScriptParser(string currentEnvironment, string targetConnectionString)
            : base(currentEnvironment, targetConnectionString)
        {
        }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// A data script for execution.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If any of the arguments are null or blank.</exception>
        public CheckScript Parse(TextReader config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            XDocument xml = XDocument.Load(config);

            this.owners = LoadOwners(xml);
            this.connections = this.LoadConnections(xml);
            var checks = this.LoadChecks(xml);

            return new CheckScript(this.connections.Values, this.owners, checks);
        }

        /// <summary>
        /// Loads the owners.
        /// </summary>
        /// <param name="xml">The XML config.</param>
        /// <returns>A map of owners keyed on owner name.</returns>
        private static IDictionary<string, DataOwner> LoadOwners(XDocument xml)
        {
            return
                //// Starting at the root
                xml
                //// Find all the Owner elements
                .Descendants("Owner")
                //// Create an Owner instance
                .Select(x => new DataOwner
                {
                    Name = FindAttribute(x, "Name"),
                    Contact = x.Element("Contact").Value,
                    Notify = ParseAttribute(FindAttribute(x.Element("Contact"), "Notify"), false)
                })
                //// Convert into a map
                .ToDictionary(x => x.Name);
        }

        /// <summary>
        /// Finds the data owner.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="collection">The collection.</param>
        /// <returns>The found item or default(T) if not found.</returns>
        private static T FindElement<T>(XElement element, XName attributeName, IDictionary<string, T> collection)
        {
            if (element == null)
            {
                return default(T);
            }

            var attributeValue = element.FindAncestorAttribute(attributeName);

            if (attributeValue == null)
            {
                return default(T);
            }

            return collection[attributeValue];
        }

        /// <summary>
        /// Finds the data owner.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="defaultMember">The default member.</param>
        /// <returns>
        /// The found item or default(T) if not found.
        /// </returns>
        private static T FindElement<T>(XElement element, XName attributeName, IDictionary<string, T> collection, string defaultMember)
        {
            if (element == null)
            {
                return collection[defaultMember];
            }

            var attributeValue = element.FindAncestorAttribute(attributeName);

            if (attributeValue == null)
            {
                return collection[defaultMember];
            }

            return collection[attributeValue];
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

                    element.Parent.Add(expandedElement);

                    RaiseExpansionEvent(expandedElement, match.ToString(), table);

                    yield return expandedElement;
                }
            }
        }

        /// <summary>
        /// Raises the expansion event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="match">The match.</param>
        /// <param name="expansion">The expansion.</param>
        private static void RaiseExpansionEvent(XElement element, string match, string expansion)
        {
            EventPublisher.Raise(
                new ExpandingElementEvent
                {
                    EntityName = element.FindAncestorElementAttribute("Entity", "Name"),
                    ColumnName = element.FindAttribute("Field"),
                    Match = match.ToString(),
                    Expansion = expansion
                });
        }

        /// <summary>
        /// Expands the check.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>
        /// A sequence of check elements.
        /// </returns>
        private static IEnumerable<XElement> ExpandCheck(XElement element, IDbMetadata metadata)
        {
            var entityName = element.FindAncestorElementAttribute("Entity", "Name");

            if (element.Attribute("Match") == null || entityName == null)
            {
                yield return element;
            }
            else
            {
                Regex match = new Regex(element.Attribute("Match").Value, RegexOptions.IgnoreCase);

                var matchedTables = metadata
                    .AllColumns(entityName)
                    .Where(t => match.IsMatch(t));

                foreach (var column in matchedTables)
                {
                    var expandedElement = new XElement(element);
                    expandedElement.SetAttributeValue("Field", column);

                    element.Parent.Add(expandedElement);

                    RaiseExpansionEvent(expandedElement, match.ToString(), column);

                    yield return expandedElement;
                }
            }
        }
        /// <summary>
        /// Loads the checks.
        /// </summary>
        /// <param name="doc">The parent.</param>
        /// <returns>
        /// A sequence of data checks.
        /// </returns>
        private IEnumerable<IDataValidator> LoadChecks(XDocument doc)
        {
            var entityCheckElements = doc
                .Descendants("Entity")
                .ToList()
                .SelectMany(e => ExpandEntity(e, this.LoadMetadata(e)))
                .Elements()
                .Where(x => ParseAttribute(x.FindAncestorAttribute("Enabled"), true))
                .ToList()
                .SelectMany(c => ExpandCheck(c, this.LoadMetadata(c)));

            var checkElements = doc
                .Descendants()
                .Where(x => x.Name == "Check" || x.Name == "ParallelCheck")
                .Where(x => x.Parent.Name != "Entity")
                .Where(x => ParseAttribute(x.FindAncestorAttribute("Enabled"), true));

            return entityCheckElements
                .Concat(checkElements)
                .Where(x => this.IsCurrentEnvironment(x.FindAncestorAttribute("Environments")))
                .Select(e => this.CreateCheck(e));
        }

        /// <summary>
        /// Determines whether [is current environment] [the specified environments].
        /// </summary>
        /// <param name="environments">The comma separated environment list.</param>
        /// <returns>True is the current environment exists.</returns>
        private bool IsCurrentEnvironment(string environments)
        {
            if (string.IsNullOrEmpty(environments))
            {
                return true;
            }

            var items = environments.Split(',');

            return items.Contains(this.CurrentEnvironment);
        }

        /// <summary>
        /// Creates the check.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>A check instant.</returns>
        private IDataValidator CreateCheck(XElement element)
        {
            var filterElement = element.Element("Filter");

            var metadata = new CheckMetadata
                {
                    Goal = element.FindAttribute("Goal").ParseWithDefault(0.0),
                    Owner = FindElement(element, "Owner", this.owners),
                    ProductFeature = element.FindAncestorElementAttribute("Feature", "Name"),
                    Connection = FindElement(element, "Connection", this.connections, "target"),
                    Filter = filterElement != null ? filterElement.Value : string.Empty,
                    Severity = (CheckSeverity)Enum.Parse(typeof(CheckSeverity), element.FindAncestorAttribute("Severity") ?? "Warning")
                };

            switch (element.Name.LocalName)
            {
                case "NotNull":
                    return new NullColumnValidator(
                        element.FindAttribute("Name"),
                        element.Parent.FindAttribute("Name"),
                        element.FindAttribute("Field"),
                        metadata);
                case "Unique":
                    return new UniqueColumnValidator(
                        element.FindAttribute("Name"),
                        element.Parent.FindAttribute("Name"),
                        element.FindAttribute("Fields") ?? element.FindAttribute("Field"),
                        metadata);
                case "Check":
                    return new GenericCheck(
                        element.FindAttribute("Name"),
                        element.FindAncestorElementAttribute("Entity", "Name"),
                        element.Value,
                        metadata);
                case "Match":
                    var regexOptionsString = element.FindAttribute("Options");

                    var options = string.IsNullOrEmpty(regexOptionsString) ? RegexOptions.None : (RegexOptions)Enum.Parse(typeof(RegexOptions), regexOptionsString);

                    return new MatchStringCheck(
                        element.FindAttribute("Name"),
                        element.Parent.FindAttribute("Name"),
                        element.FindAttribute("Field"),
                        element.FindAttribute("Expression"),
                        options,
                        metadata);
                case "Empty":
                    return new EmptyTableValidator(
                        element.FindAttribute("Name"),
                        element.Parent.FindAttribute("Name"),
                        false,
                        metadata);
                case "NotEmpty":
                    return new EmptyTableValidator(
                        element.FindAttribute("Name"),
                        element.Parent.FindAttribute("Name"),
                        true,
                        metadata);
                case "ParallelCheck":
                    var expectedElement = element.Element("Expected");
                    var actualElement = element.Element("Actual");

                    return new ParallelCheck(
                        element.FindAttribute("Name"),
                        element.FindAncestorElementAttribute("Entity", "Name"),
                        FindElement(expectedElement, "Connection", this.connections, "target"),
                        expectedElement.Value,
                        actualElement.Value,
                        metadata);
                default:
                    EventPublisher.Raise(new UnknownCheckEvent { CheckName = element.Name.LocalName });
                    return new UnknownCheckTypeValidator(element, metadata);
            }
        }

        /// <summary>
        /// Loads the metadata.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>A metadata instance.</returns>
        private IDbMetadata LoadMetadata(XElement entity)
        {
            var connection = FindElement(entity, "Connection", this.connections, "target");

            return new SqlMetadata(
                connection.CreateConnectionFactory());
        }
    }
}
