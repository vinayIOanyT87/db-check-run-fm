-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueIata
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblUserDataListValueIata]
@UserDataListValueIataGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataListValueIata].[UserDataListValueIataGuid],[dbo].[tblUserDataListValueIata].[UserDataFieldIataGuid],[dbo].[tblUserDataListValueIata].[Value],[dbo].[tblUserDataListValueIata].[CreatedDate],[dbo].[tblUserDataListValueIata].[CreatedBy],[dbo].[tblUserDataListValueIata].[UpdatedDate],[dbo].[tblUserDataListValueIata].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataListValueIata]
            INNER JOIN [track].[tblUserDataListValueIata] CT
                ON CT.PK_UserDataListValueIataGuid = [dbo].[tblUserDataListValueIata].[UserDataListValueIataGuid]
        WHERE CT.PK_UserDataListValueIataGuid = @UserDataListValueIataGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
