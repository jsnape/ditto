#region Copyright (c) all rights reserved.
// <copyright file="LoaderScriptParser.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Loader script parser.
    /// </summary>
    public abstract class LoaderScriptParser
    {
        /// <summary>
        /// The current environment.
        /// </summary>
        private readonly string currentEnvironment;

        /// <summary>
        /// The target connection string.
        /// </summary>
        private readonly string targetConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoaderScriptParser"/> class.
        /// </summary>
        /// <param name="currentEnvironment">The current environment.</param>
        /// <param name="targetConnectionString">The target connection string.</param>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        protected LoaderScriptParser(string currentEnvironment, string targetConnectionString)
        {
            if (string.IsNullOrEmpty(currentEnvironment))
            {
                throw new ArgumentNullException("currentEnvironment");
            }

            this.currentEnvironment = currentEnvironment;

            if (string.IsNullOrEmpty(targetConnectionString))
            {
                throw new ArgumentNullException("targetConnectionString");
            }

            this.targetConnectionString = targetConnectionString;
        }

        /// <summary>
        /// Gets the current environment.
        /// </summary>
        /// <value>
        /// The current environment.
        /// </value>
        protected string CurrentEnvironment
        {
            get { return this.currentEnvironment; }
        }

        /// <summary>
        /// Gets the target connection string.
        /// </summary>
        /// <value>
        /// The target connection string.
        /// </value>
        protected string TargetConnectionString
        {
            get { return this.targetConnectionString; }
        }

        /// <summary>
        /// Finds the element.
        /// </summary>
        /// <param name="item">The item to search.</param>
        /// <param name="name">The ancestor name to find.</param>
        /// <returns>
        /// An element if found else null.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If item is null.</exception>
        protected static XObject FindAncestorByName(XObject item, XName name)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var parent = item.Parent;

            while (parent != null && parent.Name != name)
            {
                parent = parent.Parent;
            }

            return parent;
        }

        /// <summary>
        /// Searches up the hierarchy for a particular attribute.
        /// </summary>
        /// <remarks> This algorithm only searches up until the element name changes.</remarks>
        /// <param name="element">The element.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The attribute value or null if not found.</returns>
        protected static string FindAttribute(XElement element, XName attribute)
        {
            XAttribute value = null;

            while (value == null || string.IsNullOrEmpty(value.Value))
            {
                value = element.Attribute(attribute);

                if (element.Parent == null || element.Parent.Name != element.Name)
                {
                    break;
                }

                element = element.Parent;
            }

            return value == null ? null : value.Value;
        }

        /// <summary>
        /// Finds the element text.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="element">The element.</param>
        /// <returns>The element value or null if not found.</returns>
        protected static string FindElementText(XContainer parent, XName element)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            var child = parent.Element(element);
            return child == null ? null : child.Value;
        }

        /// <summary>
        /// Parses the specified attribute.
        /// </summary>
        /// <typeparam name="T">Attribute type to parse.</typeparam>
        /// <param name="value">The attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The parsed value.</returns>
        protected static T ParseAttribute<T>(string value, T defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Loads the connections.
        /// </summary>
        /// <param name="xml">The XML config.</param>
        /// <returns>
        /// A sequence of data connection instances.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If any arguments are null.</exception>
        protected IDictionary<string, DataConnection> LoadConnections(XContainer xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            var connections =
                //// Starting at the root
                xml
                //// Find all the connections (even the ones that are children of other connection elements)
                .Descendants("Connection")
                //// Load the attributes
                .Select(
                    e => new
                    {
                        Name = FindAttribute(e, "Name"),
                        Provider = FindAttribute(e, "Provider"),
                        Environment = FindAttribute(e, "Environment"),
                        ConnectionString = FindAttribute(e, "ConnectionString")
                    })
                //// Filter those to the default or current environment
                .Where(e => e.Environment == this.CurrentEnvironment || e.Environment == null)
                //// Select a single connection for each name but prefer a specific environment over a defaulted one
                .GroupBy(e => e.Name, (k, s) => s.OrderBy(x => x.Environment == null ? 1 : 0).First())
                //// Convert to the internal connection type
                .Select(e => new DataConnection(e.Name, e.Provider, e.ConnectionString, e.Environment ?? this.CurrentEnvironment));

            if (!connections.Any(c => c.Name == "target"))
            {
                DittoEventSource.Log.TargetConnectionGenerated(this.TargetConnectionString);
                connections = connections.Union(this.CreateTargetConnection());
            }

            return connections
                //// Force the enumeration to be materialized.
                .ToDictionary(c => c.Name);
        }

        /// <summary>
        /// Creates the target connection.
        /// </summary>
        /// <returns>A single sequence of one connection.</returns>
        protected IEnumerable<DataConnection> CreateTargetConnection()
        {
            var targetConnection = new DataConnection(
                "target",
                "System.Data.SqlClient",
                this.TargetConnectionString,
                this.CurrentEnvironment);

            return new DataConnection[] { targetConnection };
        }
    }
}
