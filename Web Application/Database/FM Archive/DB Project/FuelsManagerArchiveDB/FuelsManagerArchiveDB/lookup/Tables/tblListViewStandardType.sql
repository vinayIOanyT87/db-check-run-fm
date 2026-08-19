/*

	DROP TABLE [lookup].[tblListViewStandardType] 

*/
CREATE TABLE [lookup].[tblListViewStandardType] (
    [ListViewStandardTypeIndex] INT                NOT NULL,
    [ListViewStandardTypeCode]  NVARCHAR (100)     NOT NULL,
    [ListViewStandardTypeName]  NVARCHAR (100)     NULL,
    [ListViewStandardTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]               DATETIMEOFFSET (7) NULL,
    [CreatedBy]                 [dbo].[udtUserID]  NULL,
    [UpdatedDate]               DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                 [dbo].[udtUserID]  NULL,
    [_RowVersion]               ROWVERSION         NOT NULL,
    [_ClusterIdx]               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblListViewStandardType] PRIMARY KEY NONCLUSTERED ([ListViewStandardTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblListViewStandardType_ListViewStandardTypeGuid]
    ON [lookup].[tblListViewStandardType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblListViewStandardType_ClusterIdx]
    ON [lookup].[tblListViewStandardType]([_ClusterIdx] ASC);