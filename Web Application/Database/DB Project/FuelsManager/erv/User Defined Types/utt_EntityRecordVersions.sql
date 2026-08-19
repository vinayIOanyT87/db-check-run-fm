CREATE TYPE [erv].[utt_EntityRecordVersions] AS TABLE (
    [EntityTypeId]              NVARCHAR (100)   NULL,
    [SiteGuid]                  UNIQUEIDENTIFIER NULL,
    [MasterRecordGuid]          UNIQUEIDENTIFIER NULL,
    [EntityGuid]                UNIQUEIDENTIFIER NULL,
    [EntitySegmentTemplateGuid] UNIQUEIDENTIFIER NULL,
    [FilterFieldName]           NVARCHAR (100)   NULL,
    [FilterValueGuid]           UNIQUEIDENTIFIER NULL,
    [FilterValueName]           NVARCHAR (100)   NULL);

