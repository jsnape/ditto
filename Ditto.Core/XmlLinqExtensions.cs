#region Copyright (c) all rights reserved.
// <copyright file="XmlLinqExtensions.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.Core
{
    using System;
    using System.Globalization;
    using System.Xml.Linq;

    /// <summary>
    /// <c>XLinq</c> extensions.
    /// </summary>
    public static class XmlLinqExtensions
    {
        /// <summary>
        /// Finds the attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>The value of the attribute or null if missing.</returns>
        public static string FindAttribute(this XElement element, XName attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            var attribute = element.Attribute(attributeName);

            return attribute != null ? attribute.Value : null;
        }

        /// <summary>
        /// Parses the with default.
        /// </summary>
        /// <typeparam name="T">Type to parse to.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>An instance of T.</returns>
        public static T ParseWithDefault<T>(this string value, T defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Finds the ancestor attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <returns>The value of the nearest ancestor attribute.</returns>
        public static string FindAncestorAttribute(this XElement element, XName attributeName)
        {
            XElement next = element;
            XAttribute attribute = null;

            while (attribute == null && next != null)
            {
                attribute = next.Attribute(attributeName);
                next = next.Parent;
            }

            return attribute != null ? attribute.Value : null;
        }

        /// <summary>
        /// Finds an ancestor element of a specific name and returns the attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element to find.</param>
        /// <param name="attributeName">Name of the attribute to return.</param>
        /// <returns>
        /// The value of the attribute.
        /// </returns>
        public static string FindAncestorElementAttribute(this XElement element, XName elementName, XName attributeName)
        {
            XElement next = element;

            while (next != null)
            {
                var attribute = next.Attribute(attributeName);

                if (attribute != null && next.Name.LocalName == elementName)
                {
                    return attribute.Value;
                }

                next = next.Parent;
            }

            return null;
        }
    }
}
