/*

	DROP TABLE [lookup].[tblScheduleType]

*/
CREATE TABLE [lookup].[tblScheduleType] (
    [ScheduleTypeIndex] INT                NOT NULL,
    [ScheduleTypeCode]  NVARCHAR (100)     NOT NULL,
    [ScheduleTypeName]  NVARCHAR (100)     NULL,
    [ScheduleTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]       DATETIMEOFFSET (7) NULL,
    [CreatedBy]         [dbo].[udtUserID]  NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
    [_ClusterIdx]       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblScheduleType] PRIMARY KEY NONCLUSTERED ([ScheduleTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblScheduleType_ScheduleTypeGuid]
    ON [lookup].[tblScheduleType]([CreatedDate] ASC);
GO
CREATE CLUSTERED INDEX [IX_tblScheduleType_ClusterIdx]
    ON [lookup].[tblScheduleType]([_ClusterIdx] ASC);