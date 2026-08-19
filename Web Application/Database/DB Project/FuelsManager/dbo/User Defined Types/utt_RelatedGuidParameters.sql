CREATE TYPE [dbo].[utt_RelatedGuidParameters] AS TABLE (
    [Section]    INT              NOT NULL,
    [SiteGuid]   UNIQUEIDENTIFIER NOT NULL,
    [TransId]    NVARCHAR (100)   NOT NULL,
    [EntityId]   NVARCHAR (100)   NULL,
    [EntityType] NVARCHAR (100)   NULL,
    [EntityGuid] UNIQUEIDENTIFIER NULL,
    [Identifier] NVARCHAR (100)   NOT NULL);

