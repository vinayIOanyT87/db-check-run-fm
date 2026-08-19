CREATE TABLE [erv].[tblTempEntityRecordVersion] (
    [EntityRecordVersionIndex] INT              IDENTITY (1, 1) NOT NULL,
    [EntityTypeId]             NVARCHAR (100)   NULL,
    [SiteGuid]                 UNIQUEIDENTIFIER NULL,
    [MasterRecordGuid]         UNIQUEIDENTIFIER NULL,
    [EntityGuid]               UNIQUEIDENTIFIER NULL,
    [AssignedFromSiteGuid]     UNIQUEIDENTIFIER NULL,
    [ParentRecordGuid]         UNIQUEIDENTIFIER NULL,
    [_CallingReferenceGuid]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_tblTempEntityRecordVersion] PRIMARY KEY CLUSTERED ([EntityRecordVersionIndex] ASC)
);
GO

/****** Object:  Index [IX_tblTempEntityRecordVersion_EntityGuid]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempEntityRecordVersion_EntityGuid]
    ON [erv].[tblTempEntityRecordVersion]([EntityGuid] ASC);
GO

/****** Object:  Index [IX_tblTempEntityRecordVersion_CallingReferenceGuid]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempEntityRecordVersion_CallingReferenceGuid] ON [erv].[tblTempEntityRecordVersion]
(
    [_CallingReferenceGuid] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)