-- This script can be used to create the correct tables in SQL server ready for
-- accepting check data.

-- This is a barebones table - you will need to add your own primary keys, indexes etc.
-- This is a high write table - don't lock it.

create table dbo.ValidationResults (
    [Timestamp] datetimeoffset not null,
    CheckName nvarchar(255) not null,
    ConnectionName nvarchar(50) not null,
    ProductFeature nvarchar(255) null,
    EntityName nvarchar(128) null,
    CheckType nvarchar(50) null,
    [Value] float null,
    Goal float null,
    [Status] float null,
    DurationSeconds float null,
    [Severity] nvarchar(50) null,
    [Owner] nvarchar(255) null,
    OwnerContact nvarchar(255) null,
    NotifyOwner bit,
    AdditionalInformation xml
);

go

create view dbo.NullRecordsValidationResults
as
    select 
        x.ColumnName,
        x.Filter,
        x.NullRecords,
        x.TotalRecords, 
        NullRecords * 100.0 / TotalRecords AS PercentageNull
    FROM (
        SELECT
            [Timestamp],
            CheckName,
            ConnectionName,
            ProductFeature,
            EntityName,
            [Value],
            [Goal],
            [Status],
            [DurationSeconds],
            [Severity],
            [Owner],
            [OwnerContact],
            [NotifyOwner],
            AdditionalInformation.value(N'(/NullColumnInfo/ColumnName)[1]', 'nvarchar(128)') AS ColumnName,
            AdditionalInformation.value(N'(/NullColumnInfo/Filter)[1]', 'nvarchar(256)') AS [Filter],
            AdditionalInformation.value(N'(/NullColumnInfo/NullRecords)[1]', 'int') AS NullRecords,
            AdditionalInformation.value(N'(/NullColumnInfo/TotalRecords)[1]', 'int') AS TotalRecords
        FROM dbo.ValidationResults WITH (NOLOCK)
        WHERE ISNULL(CheckType, N'') = N'NotNull'
        AND ISNULL([Status], 1) <> 1
    ) x;
go

create view dbo.RegexMatchValidationResults
as
    select
        [Timestamp],
        CheckName,
        ConnectionName,
        ProductFeature,
        EntityName,
        [Value],
        [Goal],
        [Status],
        [DurationSeconds],
        [Severity],
        [Owner],
        [OwnerContact],
        [NotifyOwner],
        AdditionalInformation.value(N'(/UnmatchedValueInfo/ColumnName)[1]', 'nvarchar(128)') as ColumnName,
        AdditionalInformation.value(N'(/UnmatchedValueInfo/Filter)[1]', 'nvarchar(256)') as [Filter],
        AdditionalInformation.value(N'(/UnmatchedValueInfo/UnmatchedRecords)[1]', 'int') as UnmatchedValues,
        AdditionalInformation.value(N'(/UnmatchedValueInfo/TotalRecords)[1]', 'int') as DistinctValues,
        AdditionalInformation.value(N'(/UnmatchedValueInfo/Unmatched)[1]', 'nvarchar(256)') as Unmatched
    from dbo.ValidationResults with (nolock)
    where ISNULL(CheckType, N'') = N'RegexMatch'
    and ISNULL([Status], 1) <> 1
GO

