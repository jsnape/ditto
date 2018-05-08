#region Copyright (c) all rights reserved.
// <copyright file="AssemblyInfoShared.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
// Make it easy to distinguish Debug and Release (i.e. Retail) builds;
// for example, through the file properties window.
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("Debug Release")]
#else
[assembly: AssemblyConfiguration("Retail")]
[assembly: AssemblyDescription("Retail Release")] 
#endif
[assembly: AssemblyCompany("James Snape")]
[assembly: AssemblyProduct("Ditto")]
[assembly: AssemblyCopyright("Copyright © 2014-2015")]
[assembly: AssemblyTrademark("")]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: AssemblyVersion("1.0.0.0")]
