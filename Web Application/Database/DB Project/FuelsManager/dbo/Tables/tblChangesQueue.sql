CREATE TABLE [dbo].[tblChangesQueue] (
    [EventIndex]                       BIGINT             IDENTITY (1, 1) NOT NULL,
    [EventType]                        CHAR (1)           NOT NULL,
    [Completed]                        BIT                CONSTRAINT [DF_tblChangesQueue_Completed] DEFAULT ((0)) NOT NULL,
    [RecordID]                         NVARCHAR (64)      NOT NULL,
    [CreatedDate]                      DATETIMEOFFSET (7) CONSTRAINT [DF_tblChangesQueue_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                        [dbo].[udtUserID]  CONSTRAINT [DF_tblChangesQueue_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                      DATETIMEOFFSET (7) CONSTRAINT [DF_tblChangesQueue_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                        [dbo].[udtUserID]  CONSTRAINT [DF_tblChangesQueue_UpdatedBy] DEFAULT ('') NOT NULL,
    [ChangesQueueGuid]                 UNIQUEIDENTIFIER   CONSTRAINT [DF_tblChangesQueue_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                      ROWVERSION         NOT NULL,
    [SiteGuid]                         UNIQUEIDENTIFIER   NOT NULL,
    [LookupChangeQueueRecordTypeIndex] INT                NOT NULL,
    [RecordGuid]                       UNIQUEIDENTIFIER   NULL,
    CONSTRAINT [PK_tblChangesQueue_GUID] PRIMARY KEY NONCLUSTERED ([ChangesQueueGuid] ASC),
    CONSTRAINT [FK_tblChangesQueue_LookupChangeQueueRecordTypeIndex] FOREIGN KEY ([LookupChangeQueueRecordTypeIndex]) REFERENCES [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex]),
    CONSTRAINT [FK_tblChangesQueue_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);






GO
CREATE NONCLUSTERED INDEX [IX_tblChangesQueue_CreatedDate]
    ON [dbo].[tblChangesQueue]([CreatedDate] ASC);


GO





GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblChangesQueue_EventIndex]
    ON [dbo].[tblChangesQueue]([EventIndex] ASC);

