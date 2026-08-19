-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueProduct
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataListValueProduct]
@UserDataListValueProductGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataListValueProduct].[UserDataListValueProductGuid],[dbo].[tblUserDataListValueProduct].[UserDataFieldProductGuid],[dbo].[tblUserDataListValueProduct].[Value],[dbo].[tblUserDataListValueProduct].[CreatedDate],[dbo].[tblUserDataListValueProduct].[CreatedBy],[dbo].[tblUserDataListValueProduct].[UpdatedDate],[dbo].[tblUserDataListValueProduct].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataListValueProduct]
            INNER JOIN [track].[tblUserDataListValueProduct] CT
                ON CT.PK_UserDataListValueProductGuid = [dbo].[tblUserDataListValueProduct].[UserDataListValueProductGuid]
        WHERE CT.PK_UserDataListValueProductGuid = @UserDataListValueProductGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
