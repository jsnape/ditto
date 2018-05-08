#region Copyright (c) all rights reserved.
// <copyright file="ConsoleHandler.cs">
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
    using Ditto.DataLoad.DomainEvents;
    using Ditto.DataLoad.Properties;

    /// <summary>
    /// Console Event Handler.
    /// </summary>
    public class ConsoleHandler :
        IHandles<BatchCompleteEvent>,
        IHandles<CreatingTargetTableEvent>,
        IHandles<QueryingSourceEvent>,
        IHandles<ChunkCopyingEvent>,
        IHandles<ChunkCopiedEvent>,
        IHandles<OperationErrorEvent>,
        IHandles<SourceFileFoundEvent>,
        IHandles<NoMatchingColumnsInSourceEvent>,
        IHandles<SourceFileLockedEvent>,
        IHandles<NoNewDataEvent>
    {
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

            Console.WriteLine(Resources.OperationCompletedMessageFormat, operation.Source.Name, operation.Target.Name, args.Duration);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(CreatingTargetTableEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            Console.WriteLine(Resources.CreatingTargetTableMessageFormat, args.TargetName);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(QueryingSourceEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            Console.WriteLine(Resources.GettingDataMessageFormat, args.SourceName);
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

            Console.WriteLine(Resources.StartingCopyMessageFormat, args.TargetName);
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

            Console.WriteLine(Resources.ChunkCompleteMessageFormat, args.TargetName, args.Duration);
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

            Console.WriteLine(Resources.LoadingErrorMessageFormat, args.Operation.Source.Name, args.Exception.Message);

            foreach (var property in args.Properties)
            {
                Console.WriteLine(Resources.NameValuePairFormat, property.Key, property.Value);
            }
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(SourceFileFoundEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            foreach (var file in args.FoundFiles)
            {
                Console.WriteLine(Resources.FoundFileFormat, file.FullName, file.LastWriteTimeUtc);
            }
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

            Console.WriteLine(
                Resources.NoMatchingColumnsInSourceMessageFormat, 
                args.Operation.Source.Name, 
                string.Join(", ", args.SourceColumns));
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

            Console.WriteLine(args.Exception.Message);
        }

        /// <summary>
        /// Handles the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Handle(NoNewDataEvent args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (args.HighWatermark == null)
            {
                Console.WriteLine(
                    Resources.NoNewDataMessageFormat,
                    args.Operation.Source.Name,
                    "None");
            }
            else
            {
                Console.WriteLine(
                    Resources.NoNewDataMessageFormat,
                    args.Operation.Source.Name,
                    args.HighWatermark.WatermarkValue);
            }
        }
    }
}
