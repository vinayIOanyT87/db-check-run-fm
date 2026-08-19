-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblMenuItemType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMenuItemType]
@MenuItemTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblMenuItemType].[MenuItemTypeIndex],[lookup].[tblMenuItemType].[MenuItemTypeCode],[lookup].[tblMenuItemType].[MenuItemTypeName],[lookup].[tblMenuItemType].[MenuItemTypeGuid],[lookup].[tblMenuItemType].[CreatedDate],[lookup].[tblMenuItemType].[CreatedBy],[lookup].[tblMenuItemType].[UpdatedDate],[lookup].[tblMenuItemType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblMenuItemType]
            INNER JOIN [track].[tblMenuItemType] CT
                ON CT.PK_MenuItemTypeIndex = [lookup].[tblMenuItemType].[MenuItemTypeIndex]
        WHERE CT.PK_MenuItemTypeIndex = @MenuItemTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
