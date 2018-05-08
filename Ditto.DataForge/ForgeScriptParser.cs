#region Copyright (c) all rights reserved.
// <copyright file="DataScriptParser.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataForge
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Xml.Linq;
    using Core;

    /// <summary>
    /// Data Script Parser.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "JS: Would result in very unreadable code")] 
    public class ForgeScriptParser : LoaderScriptParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForgeScriptParser" /> class.
        /// </summary>
        /// <param name="currentEnvironment">The current environment.</param>
        public ForgeScriptParser(string currentEnvironment)
            : base(currentEnvironment, null)
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
        public ForgeScript Parse(TextReader config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            XDocument xml = XDocument.Load(config);

            var connections = this.LoadConnections(xml);

            return new ForgeScript(connections.Values);
        }
    }
}
