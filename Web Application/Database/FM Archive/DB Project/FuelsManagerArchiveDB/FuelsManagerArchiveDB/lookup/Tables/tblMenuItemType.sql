/*

	DROP TABLE [lookup].[tblMenuItemType]

*/
CREATE TABLE [lookup].[tblMenuItemType] (
    [MenuItemTypeIndex] INT                NOT NULL,
    [MenuItemTypeCode]  NVARCHAR (100)     NOT NULL,
    [MenuItemTypeName]  NVARCHAR (100)     NULL,
    [MenuItemTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]       DATETIMEOFFSET (7) NULL,
    [CreatedBy]         [dbo].[udtUserID]  NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
    [_ClusterIdx]       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblMenuItemType] PRIMARY KEY NONCLUSTERED ([MenuItemTypeIndex] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_lookup_tblMenuItemType_MenuItemTypeGuid]
    ON [lookup].[tblMenuItemType]([MenuItemTypeGuid] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMenuItemType_ClusterIdx]
    ON [lookup].[tblMenuItemType]([_ClusterIdx] ASC);