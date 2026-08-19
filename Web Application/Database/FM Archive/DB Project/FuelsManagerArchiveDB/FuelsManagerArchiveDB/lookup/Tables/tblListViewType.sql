/*

	DROP TABLE [lookup].[tblListViewType]

*/
CREATE TABLE [lookup].[tblListViewType] (
    [ListViewTypeIndex] INT                NOT NULL,
    [ListViewTypeCode]  NVARCHAR (100)     NOT NULL,
    [ListViewTypeName]  NVARCHAR (100)     NULL,
    [ListViewTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]       DATETIMEOFFSET (7) NULL,
    [CreatedBy]         [dbo].[udtUserID]  NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
    [_ClusterIdx]       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblListViewType] PRIMARY KEY NONCLUSTERED ([ListViewTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblListViewType_ListViewTypeGuid]
    ON [lookup].[tblListViewType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblListViewType_ClusterIdx]
    ON [lookup].[tblListViewType]([_ClusterIdx] ASC);