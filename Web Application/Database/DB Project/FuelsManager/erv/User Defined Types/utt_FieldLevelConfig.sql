CREATE TYPE [erv].[utt_FieldLevelConfig] AS TABLE (
    [FieldConfigGuid]           UNIQUEIDENTIFIER NULL,
    [EntitySegmentTemplateGuid] UNIQUEIDENTIFIER NULL,
    [EntityTypeId]              NVARCHAR (100)   NULL,
    [SiteGroupGuid]             UNIQUEIDENTIFIER NULL,
    [FilterFieldName]           NVARCHAR (100)   NULL,
    [FilterValueGuid]           UNIQUEIDENTIFIER NULL,
    [FilterValueName]           NVARCHAR (100)   NULL,
    [TargetField]               NVARCHAR (100)   NULL,
    [IsExternalAttribute]       BIT              NULL,
    [InternalFieldName]         NVARCHAR (100)   NULL,
    [InheritedControlMode]      NVARCHAR (100)   NULL,
    [ForwardControlMode]        NVARCHAR (100)   NULL,
    [HierarchyLevel]            INT              NULL);

