#region Copyright (c) all rights reserved.
// <copyright file="GlobalSuppressions.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "Internal tool not going in the GAC")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Ditto.DataLoad.Tests.DomainEvents", Justification = "JS: Test code mirroring implementation")]
