-- This script can be used to create the correct tables in SQL server ready for
-- accepting telemetry data.

-- This is a barebones table - you will need to add your own primary keys, indexes etc.
-- This is a high write table - don't lock it.

create table dbo.TelemetryLog (
    TelemetryLogKey int not null identity(1,1),
	InstrumentationKey nvarchar(50) not null,
	TelemetryType nvarchar(50) not null,
	TelemetryName nvarchar(50) not null,
	[Timestamp] datetimeoffset not null,
	[Value] float,
	[Message] nvarchar(max),
	Context xml null,
	Properties xml null,
	Metrics xml null,
)
go

create view dbo.Telemetry
as
    select
        TelemetryLogKey,
        InstrumentationKey,
        TelemetryType,
        TelemetryName,
        [Timestamp],
        [Value],
        [Message],
        Context.value(N'(/Context/Device/Id)[1]', 'nvarchar(100)') as Hostname,
        Context.value(N'(/Context/User/Id)[1]', 'nvarchar(100)') as UserId,
        Properties.value(N'(/Properties/Property[@Name = ''ExecutionId'']/@Value)[1]', 'uniqueidentifier') as ExecutionId,
        Properties.value(N'(/Properties/Property[@Name = ''OperationId'']/@Value)[1]', 'uniqueidentifier') as OperationId,
        Properties.value(N'(/Properties/Property[@Name = ''Source'']/@Value)[1]', 'nvarchar(50)') as SourceName,
        Properties.value(N'(/Properties/Property[@Name = ''Target'']/@Value)[1]', 'nvarchar(50)') as TargetName,
        Properties.value(N'(/Properties/Property[@Name = ''Connection'']/@Value)[1]', 'nvarchar(50)') as ConnectionName,
        Properties.value(N'(/Properties/Property[@Name = ''DataScript'']/@Value)[1]', 'nvarchar(50)') as DataScript,
        Properties.value(N'(/Properties/Property[@Name = ''Environment'']/@Value)[1]', 'nvarchar(50)') as Environment,
        Metrics.value(N'(/Metrics/Metric[@Name = ''Duration (secs)'']/@Value)[1]', 'float') as DurationSecs,
        Metrics.value(N'(/Metrics/Metric[@Name = ''Rows'']/@Value)[1]', 'int') as RecordCount,
        Metrics.value(N'(/Metrics/Metric[@Name = ''Chunks'']/@Value)[1]', 'int') as ChunksCount
    from dbo.TelemetryLog with (nolock);
go
