-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldEquipment
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataFieldEquipment]
@UserDataFieldEquipmentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataFieldEquipment].[UserDataFieldEquipmentGuid],[dbo].[tblUserDataFieldEquipment].[TransactionAliasGuid],[dbo].[tblUserDataFieldEquipment].[SiteGuid],[dbo].[tblUserDataFieldEquipment].[Number],[dbo].[tblUserDataFieldEquipment].[DisplayOrder],[dbo].[tblUserDataFieldEquipment].[DisplayName],[dbo].[tblUserDataFieldEquipment].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldEquipment].[Required],[dbo].[tblUserDataFieldEquipment].[UserGroupGuid],[dbo].[tblUserDataFieldEquipment].[CreatedDate],[dbo].[tblUserDataFieldEquipment].[CreatedBy],[dbo].[tblUserDataFieldEquipment].[UpdatedDate],[dbo].[tblUserDataFieldEquipment].[UpdatedBy],[dbo].[tblUserDataFieldEquipment].[DispatchField],[dbo].[tblUserDataFieldEquipment].[ClearOnNew],[dbo].[tblUserDataFieldEquipment].[ReadOnly],[dbo].[tblUserDataFieldEquipment].[Visibility],[dbo].[tblUserDataFieldEquipment].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataFieldEquipment]
            INNER JOIN [track].[tblUserDataFieldEquipment] CT
                ON CT.PK_UserDataFieldEquipmentGuid = [dbo].[tblUserDataFieldEquipment].[UserDataFieldEquipmentGuid]
        WHERE CT.PK_UserDataFieldEquipmentGuid = @UserDataFieldEquipmentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
