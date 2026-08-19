/*

	DROP TABLE [lookup].[tblChangeQueueRecordType]

*/

CREATE TABLE [lookup].[tblChangeQueueRecordType] (
    [ChangeQueueRecordTypeIndex] INT                NOT NULL,
    [ChangeQueueRecordTypeCode]  NVARCHAR (100)     NOT NULL,
    [ChangeQueueRecordTypeName]  NVARCHAR (100)     NULL,
    [ChangeQueueRecordTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]                DATETIMEOFFSET (7) NULL,
    [CreatedBy]                  [dbo].[udtUserID]  NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  NULL,
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