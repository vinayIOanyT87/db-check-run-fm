-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldIata
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataFieldIata]
@UserDataFieldIataGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataFieldIata].[UserDataFieldIataGuid],[dbo].[tblUserDataFieldIata].[TransactionAliasGuid],[dbo].[tblUserDataFieldIata].[SiteGuid],[dbo].[tblUserDataFieldIata].[Number],[dbo].[tblUserDataFieldIata].[DisplayOrder],[dbo].[tblUserDataFieldIata].[DisplayName],[dbo].[tblUserDataFieldIata].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldIata].[Required],[dbo].[tblUserDataFieldIata].[UserGroupGuid],[dbo].[tblUserDataFieldIata].[CreatedDate],[dbo].[tblUserDataFieldIata].[CreatedBy],[dbo].[tblUserDataFieldIata].[UpdatedDate],[dbo].[tblUserDataFieldIata].[UpdatedBy],[dbo].[tblUserDataFieldIata].[DispatchField],[dbo].[tblUserDataFieldIata].[ClearOnNew],[dbo].[tblUserDataFieldIata].[ReadOnly],[dbo].[tblUserDataFieldIata].[Visibility],[dbo].[tblUserDataFieldIata].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataFieldIata]
            INNER JOIN [track].[tblUserDataFieldIata] CT
                ON CT.PK_UserDataFieldIataGuid = [dbo].[tblUserDataFieldIata].[UserDataFieldIataGuid]
        WHERE CT.PK_UserDataFieldIataGuid = @UserDataFieldIataGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
