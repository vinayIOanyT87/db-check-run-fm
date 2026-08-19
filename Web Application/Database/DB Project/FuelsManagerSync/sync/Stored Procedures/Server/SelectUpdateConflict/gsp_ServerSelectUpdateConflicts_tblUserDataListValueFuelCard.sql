-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueFuelCard
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataListValueFuelCard]
@UserDataListValueFuelCardGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataListValueFuelCard].[UserDataListValueFuelCardGuid],[dbo].[tblUserDataListValueFuelCard].[UserDataFieldFuelCardGuid],[dbo].[tblUserDataListValueFuelCard].[Value],[dbo].[tblUserDataListValueFuelCard].[CreatedDate],[dbo].[tblUserDataListValueFuelCard].[CreatedBy],[dbo].[tblUserDataListValueFuelCard].[UpdatedDate],[dbo].[tblUserDataListValueFuelCard].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataListValueFuelCard]
            INNER JOIN [track].[tblUserDataListValueFuelCard] CT
                ON CT.PK_UserDataListValueFuelCardGuid = [dbo].[tblUserDataListValueFuelCard].[UserDataListValueFuelCardGuid]
        WHERE CT.PK_UserDataListValueFuelCardGuid = @UserDataListValueFuelCardGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
