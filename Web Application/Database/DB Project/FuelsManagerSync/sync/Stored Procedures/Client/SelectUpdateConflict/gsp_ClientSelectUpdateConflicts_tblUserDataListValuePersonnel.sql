-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValuePersonnel
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblUserDataListValuePersonnel]
@UserDataListValuePersonnelGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataListValuePersonnel].[UserDataListValuePersonnelGuid],[dbo].[tblUserDataListValuePersonnel].[UserDataFieldPersonnelGuid],[dbo].[tblUserDataListValuePersonnel].[Value],[dbo].[tblUserDataListValuePersonnel].[CreatedDate],[dbo].[tblUserDataListValuePersonnel].[CreatedBy],[dbo].[tblUserDataListValuePersonnel].[UpdatedDate],[dbo].[tblUserDataListValuePersonnel].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataListValuePersonnel]
            INNER JOIN [track].[tblUserDataListValuePersonnel] CT
                ON CT.PK_UserDataListValuePersonnelGuid = [dbo].[tblUserDataListValuePersonnel].[UserDataListValuePersonnelGuid]
        WHERE CT.PK_UserDataListValuePersonnelGuid = @UserDataListValuePersonnelGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
