-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueEquipment
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataListValueEquipment]
@UserDataListValueEquipmentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataListValueEquipment].[UserDataListValueEquipmentGuid],[dbo].[tblUserDataListValueEquipment].[UserDataFieldEquipmentGuid],[dbo].[tblUserDataListValueEquipment].[Value],[dbo].[tblUserDataListValueEquipment].[CreatedDate],[dbo].[tblUserDataListValueEquipment].[CreatedBy],[dbo].[tblUserDataListValueEquipment].[UpdatedDate],[dbo].[tblUserDataListValueEquipment].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataListValueEquipment]
            INNER JOIN [track].[tblUserDataListValueEquipment] CT
                ON CT.PK_UserDataListValueEquipmentGuid = [dbo].[tblUserDataListValueEquipment].[UserDataListValueEquipmentGuid]
        WHERE CT.PK_UserDataListValueEquipmentGuid = @UserDataListValueEquipmentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
