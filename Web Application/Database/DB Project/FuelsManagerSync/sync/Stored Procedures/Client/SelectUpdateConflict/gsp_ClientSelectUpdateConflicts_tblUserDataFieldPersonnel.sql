-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldPersonnel
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblUserDataFieldPersonnel]
@UserDataFieldPersonnelGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataFieldPersonnel].[UserDataFieldPersonnelGuid],[dbo].[tblUserDataFieldPersonnel].[TransactionAliasGuid],[dbo].[tblUserDataFieldPersonnel].[SiteGuid],[dbo].[tblUserDataFieldPersonnel].[Number],[dbo].[tblUserDataFieldPersonnel].[DisplayOrder],[dbo].[tblUserDataFieldPersonnel].[DisplayName],[dbo].[tblUserDataFieldPersonnel].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldPersonnel].[Required],[dbo].[tblUserDataFieldPersonnel].[UserGroupGuid],[dbo].[tblUserDataFieldPersonnel].[CreatedDate],[dbo].[tblUserDataFieldPersonnel].[CreatedBy],[dbo].[tblUserDataFieldPersonnel].[UpdatedDate],[dbo].[tblUserDataFieldPersonnel].[UpdatedBy],[dbo].[tblUserDataFieldPersonnel].[DispatchField],[dbo].[tblUserDataFieldPersonnel].[ClearOnNew],[dbo].[tblUserDataFieldPersonnel].[ReadOnly],[dbo].[tblUserDataFieldPersonnel].[Visibility],[dbo].[tblUserDataFieldPersonnel].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataFieldPersonnel]
            INNER JOIN [track].[tblUserDataFieldPersonnel] CT
                ON CT.PK_UserDataFieldPersonnelGuid = [dbo].[tblUserDataFieldPersonnel].[UserDataFieldPersonnelGuid]
        WHERE CT.PK_UserDataFieldPersonnelGuid = @UserDataFieldPersonnelGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
