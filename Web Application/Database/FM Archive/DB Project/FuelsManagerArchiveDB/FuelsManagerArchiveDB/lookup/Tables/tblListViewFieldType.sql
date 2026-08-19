/*

	DROP TABLE [lookup].[tblListViewFieldType]

*/
CREATE TABLE [lookup].[tblListViewFieldType] (
    [ListViewFieldTypeIndex] INT                NOT NULL,
    [ListViewFieldTypeCode]  NVARCHAR (100)     NOT NULL,
    [ListViewFieldTypeName]  NVARCHAR (100)     NULL,
    [ListViewFieldTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblListViewFieldType] PRIMARY KEY NONCLUSTERED ([ListViewFieldTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblListViewFieldType_ListViewFieldTypeIndexGuid]
    ON [lookup].[tblListViewFieldType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblListViewFieldType_ClusterIdx]
    ON [lookup].[tblListViewFieldType]([_ClusterIdx] ASC);