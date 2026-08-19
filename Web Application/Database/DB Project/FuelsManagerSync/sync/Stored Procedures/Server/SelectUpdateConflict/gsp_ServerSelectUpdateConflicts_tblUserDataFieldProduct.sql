-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldProduct
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataFieldProduct]
@UserDataFieldProductGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataFieldProduct].[UserDataFieldProductGuid],[dbo].[tblUserDataFieldProduct].[TransactionAliasGuid],[dbo].[tblUserDataFieldProduct].[SiteGuid],[dbo].[tblUserDataFieldProduct].[Number],[dbo].[tblUserDataFieldProduct].[DisplayOrder],[dbo].[tblUserDataFieldProduct].[DisplayName],[dbo].[tblUserDataFieldProduct].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldProduct].[Required],[dbo].[tblUserDataFieldProduct].[UserGroupGuid],[dbo].[tblUserDataFieldProduct].[CreatedDate],[dbo].[tblUserDataFieldProduct].[CreatedBy],[dbo].[tblUserDataFieldProduct].[UpdatedDate],[dbo].[tblUserDataFieldProduct].[UpdatedBy],[dbo].[tblUserDataFieldProduct].[DispatchField],[dbo].[tblUserDataFieldProduct].[ClearOnNew],[dbo].[tblUserDataFieldProduct].[ReadOnly],[dbo].[tblUserDataFieldProduct].[Visibility],[dbo].[tblUserDataFieldProduct].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataFieldProduct]
            INNER JOIN [track].[tblUserDataFieldProduct] CT
                ON CT.PK_UserDataFieldProductGuid = [dbo].[tblUserDataFieldProduct].[UserDataFieldProductGuid]
        WHERE CT.PK_UserDataFieldProductGuid = @UserDataFieldProductGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
