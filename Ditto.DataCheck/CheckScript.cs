#region Copyright (c) all rights reserved.
// <copyright file="CheckScript.cs">
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
#endregion

namespace Ditto.DataCheck
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Ditto.DomainEvents;
    using Ditto.Core;
    using Ditto.DataCheck.DomainEvents;
    using Ditto.DataCheck.Properties;

    /// <summary>
    /// Check Script.
    /// </summary>
    public class CheckScript : LoaderScript
    {
        /// <summary>
        /// A list of data owners.
        /// </summary>
        private readonly Dictionary<string, DataOwner> owners;

        /// <summary>
        /// A list of data checks.
        /// </summary>
        private readonly List<IDataValidator> checks;

        /// <summary>
        /// The target factory.
        /// </summary>
        private readonly IConnectionFactory targetFactory;

        /// <summary>
        /// The check error count.
        /// </summary>
        private int checkErrorCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckScript" /> class.
        /// </summary>
        /// <param name="connections">The connections.</param>
        /// <param name="owners">A sequence of data owners.</param>
        /// <param name="checks">A sequence of data checks.</param>
        /// <exception cref="System.ArgumentNullException">Is any arguments are null.</exception>
        public CheckScript(IEnumerable<DataConnection> connections, IDictionary<string, DataOwner> owners, IEnumerable<IDataValidator> checks)
            : base(connections)
        {
            if (owners == null)
            {
                throw new ArgumentNullException("owners");
            }

            this.owners = new Dictionary<string, DataOwner>(owners);

            if (checks == null)
            {
                throw new ArgumentNullException("checks");
            }

            this.checks = new List<IDataValidator>(checks);

            // This is the default target.
            this.targetFactory =
                connections
                .Where(c => c.Name == "target")
                .Single()
                .CreateConnectionFactory();
        }

        /// <summary>
        /// Gets the data owners.
        /// </summary>
        /// <value>
        /// The data owners.
        /// </value>
        public IDictionary<string, DataOwner> Owners 
        {
            get { return this.owners; } 
        }

        /// <summary>
        /// Gets the data checks.
        /// </summary>
        /// <value>
        /// The data checks.
        /// </value>
        public IEnumerable<IDataValidator> Checks
        {
            get { return this.checks; }
        }

        /// <summary>
        /// Gets the check error count.
        /// </summary>
        /// <value>
        /// The check error count.
        /// </value>
        public int CheckErrorCount
        {
            get { return this.checkErrorCount; }
        }

        /// <summary>
        /// Runs the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public override void Run(CancellationToken cancellationToken)
        {
            var context = new ValidationContext(cancellationToken);

            var results =
                this.checks
                .AsParallel()
                .WithCancellation(cancellationToken)
                .Select(c => this.Validate(c, context));

            results.ForAll(r => this.WriteResult(r));
        }

        /// <summary>
        /// Validates the specified check.
        /// </summary>
        /// <param name="check">The check.</param>
        /// <param name="context">The context.</param>
        /// <returns>A validation result.</returns>
        private ValidationResult Validate(IDataValidator check, ValidationContext context)
        {
            try
            {
                var timer = new Stopwatch();

                EventPublisher.Raise(new CheckStartedEvent { Name = check.Name });

                timer.Start();
                var result = check.Validate(context);
                timer.Stop();

                result.Duration = timer.Elapsed;

                if (result.Status < 0)
                {
                    if (result.Metadata.Severity == CheckSeverity.Error)
                    {
                        Interlocked.Increment(ref this.checkErrorCount);
                    }

                    EventPublisher.Raise(
                        new CheckFailedEvent 
                        {
                            Name = check.Name, 
                            CheckType = result.CheckType,
                            Goal = result.Metadata.Goal,
                            Value = result.Value,
                            Message = result.Status < 0 ? Resources.CheckFailedMessage : Resources.CheckPassedMessage,
                            Details = result.AdditionalInformation.ToString(),
                            Severity = result.Metadata.Severity,
                            Duration = timer.Elapsed 
                        });
                }
                else
                {
                    EventPublisher.Raise(new CheckPassedEvent { Name = check.Name, Duration = timer.Elapsed });
                }

                return result;
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string>();
                EventPublisher.Raise(new CheckErrorEvent(check, ex, properties));
                throw;
            }
        }

        /// <summary>
        /// Writes the result.
        /// </summary>
        /// <param name="result">The result.</param>
        private void WriteResult(ValidationResult result)
        {
            const string Template = @"
insert into metrics.ValidationResultsStage (
    [Timestamp],
    CheckName,
    ConnectionName,
    ProductFeature,
    EntityName,
    CheckType,
    Value,
    Goal,
    [Status],
    DurationSeconds,
    [Severity],
    [Owner],
    OwnerContact,
    NotifyOwner,
    AdditionalInformation
)
values (
    sysdatetimeoffset(),
    @CheckName,
    @ConnectionName,
    @ProductFeature,
    @EntityName,
    @CheckType,
    @Value,
    @Goal,
    @Status,
    @DurationSeconds,
    @Severity,
    @Owner,
    @OwnerContact,
    @NotifyOwner,
    @AdditionalInformation
);
";

            using (IDbConnection connection = this.targetFactory.CreateConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = Template;

                command.AddParameter("@CheckName", result.CheckName);
                command.AddParameter("@ConnectionName", result.Metadata.Connection.Name);
                command.AddParameter("@ProductFeature", result.Metadata.ProductFeature);
                command.AddParameter("@EntityName", result.EntityName);
                command.AddParameter("@CheckType", result.CheckType);
                command.AddParameter("@Value", result.Value);
                command.AddParameter("@Goal", result.Metadata.Goal);
                command.AddParameter("@Status", result.Status);
                command.AddParameter("@DurationSeconds", result.Duration.TotalSeconds);
                command.AddParameter("@Severity", result.Metadata.Severity.ToString());
                command.AddParameter("@Owner", result.Metadata.Owner != null ? result.Metadata.Owner.Name : null);
                command.AddParameter("@OwnerContact", result.Metadata.Owner != null ? result.Metadata.Owner.Contact : null);
                command.AddParameter("@NotifyOwner", result.Metadata.Owner != null ? result.Metadata.Owner.Notify : false);
                
                if (result.AdditionalInformation != null)
                {
                    command.AddParameter("@AdditionalInformation", result.AdditionalInformation.ToString());
                }
                else
                {
                    command.AddParameter("@AdditionalInformation", DBNull.Value);
                }

                command.ExecuteNonQuery();
            }
        }
    }
}
