#region Copyright (c) all rights reserved.
// <copyright file="TelemetryHandler.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataLoad
{
    using System;
    using System.Collections.Generic;
    using Ditto.DomainEvents;
    using Ditto.Core;
    using Ditto.DataLoad.DomainEvents;

    /// <summary>
    /// Telemetry Handler.
    /// </summary>
    public class TelemetryHandler : 
        IHandles<BatchCompleteEvent>, 
        IHandles<SourceErrorEvent>,
        IHandles<SourceQueryEvent>,
        IHandles<OperationErrorEvent>,
        IHandles<ChunkCopyingEvent>,
        IHandles<ChunkCopiedEvent>,
        IHandles<NoMatchingColumnsInSourceEvent>,
        IHandles<SourceFileLockedEvent>
    {
        /// <summary>
        /// The no-metrics empty dictionary.
        /// </summary>
        private static readonly IDictionary<string, double> NoMetrics = new Dictionary<string, double>();

        /// <summary>
        /// The telemetry.
        /// </summary>
        private readonly EventTelemetry telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryHandler"/> class.
        /// </summary>
        /// <param name="telemetry">The telemetry.</param>
        public TelemetryHandler(EventTelemetry telemetry)
        {
            if (telemetry == null)
            {
                throw new ArgumentNullException("telemetry");
            }

            this.telemetry = telemetry;
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(BatchCompleteEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var operation = args.Operation;

            var properties = new Dictionary<string, string> 
            {
                { "OperationId", operation.OperationId.ToString() }, 
                { "Source", operation.Source.Name }, 
                { "Target", operation.Target.Name },
                { "Connection", operation.Source.ConnectionName },
            };

            var metrics = new Dictionary<string, double>
            {
                { "Duration (secs)", args.Duration.TotalSeconds },
                { "Total Rows", args.RowCount },
                { "Total Chunks", args.ResultCount }
            };

            this.telemetry.TrackEvent("Operation Completed", properties, metrics);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(SourceErrorEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var properties = new Dictionary<string, string> 
            {
                { "OperationId", args.OperationId.ToString() }, 
                { "Source", args.SourceName }, 
                { "Connection", args.ConnectionName },
            };

            this.telemetry.LogException(args.Exception, properties, NoMetrics);
            this.telemetry.TrackEvent("Source Error", properties, NoMetrics);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(SourceQueryEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var properties = new Dictionary<string, string> 
            {
                { "OperationId", args.OperationId.ToString() }, 
                { "Source", args.SourceName }, 
                { "Connection", args.ConnectionName },
            };

            var metrics = new Dictionary<string, double>
            {
                { "Duration (secs)", args.Duration.TotalSeconds }
            };

            this.telemetry.TrackEvent("Source Query", properties, metrics);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(OperationErrorEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var properties = new Dictionary<string, string>(args.Properties);

            properties["OperationId"] = args.Operation.OperationId.ToString(); 
            properties["Source"] = args.Operation.Source.Name;
            properties["Connection"] = args.Operation.Source.ConnectionName;
            properties["Target"] = args.Operation.Target.Name;

            this.telemetry.LogException(args.Exception, properties, NoMetrics);
            this.telemetry.TrackEvent("Operation Error", properties, NoMetrics);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(ChunkCopyingEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var properties = new Dictionary<string, string> 
            {
                { "OperationId", args.OperationId.ToString() }, 
                { "Target", args.TargetName }, 
            };

            var metrics = new Dictionary<string, double>
            {
                { "Rows", args.RowCount },
                { "Chunks", args.ResultCount }
            };

            this.telemetry.TrackEvent("Chunk Started", properties, metrics);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(ChunkCopiedEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var properties = new Dictionary<string, string> 
            {
                { "OperationId", args.OperationId.ToString() }, 
                { "Target", args.TargetName }, 
            };

            var metrics = new Dictionary<string, double>
            {
                { "Duration (secs)", args.Duration.TotalSeconds },
                { "Rows", args.RowCount },
                { "Chunks", args.ResultCount }
            };

            this.telemetry.TrackEvent("Chunk Completed", properties, metrics);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(NoMatchingColumnsInSourceEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var properties = new Dictionary<string, string> 
            {
                { "OperationId", args.Operation.OperationId.ToString() }, 
                { "Source", args.Operation.Source.Name }, 
                { "Columns", string.Join(", ", args.SourceColumns) }, 
            };

            this.telemetry.TrackEvent("No Matching Columns", properties, NoMetrics);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(SourceFileLockedEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var properties = new Dictionary<string, string> 
            {
                { "OperationId", args.OperationId.ToString() }, 
                { "Source", args.SourceName }, 
                { "Message", args.Exception.Message }, 
            };

            this.telemetry.TrackEvent("Source File Locked", properties, NoMetrics);
        }
    }
}
