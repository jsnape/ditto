﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ditto.DataLoad.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ditto.DataLoad.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Executing Ditto data stage process..
        /// </summary>
        internal static string BatchWelcomeMessage {
            get {
                return ResourceManager.GetString("BatchWelcomeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cancellation requested, please wait..
        /// </summary>
        internal static string CancellationRequestedMessage {
            get {
                return ResourceManager.GetString("CancellationRequestedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}: chunk complete in {1}.
        /// </summary>
        internal static string ChunkCompleteMessageFormat {
            get {
                return ResourceManager.GetString("ChunkCompleteMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating target table &apos;{0}&apos;.
        /// </summary>
        internal static string CreatingTargetTableMessageFormat {
            get {
                return ResourceManager.GetString("CreatingTargetTableMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CsvDataReader.{0} field index too large ({1}). Check source file &apos;{2}&apos; for delimiter characters within fields..
        /// </summary>
        internal static string CsvDataReaderIndexOutOfRangeMessageFormat {
            get {
                return ResourceManager.GetString("CsvDataReaderIndexOutOfRangeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CsvDataReader.{0} field index must be at least {1}.
        /// </summary>
        internal static string CsvDataReaderInvalidIndexMessageFormat {
            get {
                return ResourceManager.GetString("CsvDataReaderInvalidIndexMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Data script does not exist..
        /// </summary>
        internal static string DataScriptMissingMessage {
            get {
                return ResourceManager.GetString("DataScriptMissingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Warning no arguments supplied, using defaults..
        /// </summary>
        internal static string DefaultArgumentsUsedMessage {
            get {
                return ResourceManager.GetString("DefaultArgumentsUsedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Source file found: &apos;{0}&apos;, last written &apos;{1}&apos;.
        /// </summary>
        internal static string FoundFileFormat {
            get {
                return ResourceManager.GetString("FoundFileFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}: getting data.
        /// </summary>
        internal static string GettingDataMessageFormat {
            get {
                return ResourceManager.GetString("GettingDataMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error loading {0}, {1}.
        /// </summary>
        internal static string LoadingErrorMessageFormat {
            get {
                return ResourceManager.GetString("LoadingErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} = &apos;{1}&apos;.
        /// </summary>
        internal static string NameValuePairFormat {
            get {
                return ResourceManager.GetString("NameValuePairFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Warning no matching columns in source {0} [Columns: &apos;{1}&apos;].
        /// </summary>
        internal static string NoMatchingColumnsInSourceMessageFormat {
            get {
                return ResourceManager.GetString("NoMatchingColumnsInSourceMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No new data found in source &apos;{0}&apos; above watermark &apos;{1}&apos;.
        /// </summary>
        internal static string NoNewDataMessageFormat {
            get {
                return ResourceManager.GetString("NoNewDataMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}: copy from &apos;{0}&apos; complete in {2}.
        /// </summary>
        internal static string OperationCompletedMessageFormat {
            get {
                return ResourceManager.GetString("OperationCompletedMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}: starting copy.
        /// </summary>
        internal static string StartingCopyMessageFormat {
            get {
                return ResourceManager.GetString("StartingCopyMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Total execution time: {0}.
        /// </summary>
        internal static string TotalExecutionTimeMessageFormat {
            get {
                return ResourceManager.GetString("TotalExecutionTimeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Truncating target table &apos;{0}&apos;.
        /// </summary>
        internal static string TruncatingTargetTableMessageFormat {
            get {
                return ResourceManager.GetString("TruncatingTargetTableMessageFormat", resourceCulture);
            }
        }
    }
}
