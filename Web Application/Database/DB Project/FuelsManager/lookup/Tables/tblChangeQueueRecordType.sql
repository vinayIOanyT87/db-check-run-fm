CREATE TABLE [lookup].[tblChangeQueueRecordType] (
    [ChangeQueueRecordTypeIndex] INT                NOT NULL,
    [ChangeQueueRecordTypeCode]  NVARCHAR (100)     NOT NULL,
    [ChangeQueueRecordTypeName]  NVARCHAR (100)     NULL,
    [ChangeQueueRecordTypeGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblChangeQueueRecordType_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblChangeQueueRecordType_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblChangeQueueRecordType_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblChangeQueueRecordType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblChangeQueueRecordType_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]                ROWVERSION         NOT NULL,
    [_ClusterIdx]                BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblChangeQueueRecordType] PRIMARY KEY NONCLUSTERED ([ChangeQueueRecordTypeIndex] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblChangeQueueRecordType_ChangeQueueRecordTypeGuid]
    ON [lookup].[tblChangeQueueRecordType]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblChangeQueueRecordType_ClusterIdx]
    ON [lookup].[tblChangeQueueRecordType]([_ClusterIdx] ASC);

