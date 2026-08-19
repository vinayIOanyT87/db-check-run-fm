-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldCompany
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblUserDataFieldCompany]
@UserDataFieldCompanyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataFieldCompany].[UserDataFieldCompanyGuid],[dbo].[tblUserDataFieldCompany].[TransactionAliasGuid],[dbo].[tblUserDataFieldCompany].[SiteGuid],[dbo].[tblUserDataFieldCompany].[Number],[dbo].[tblUserDataFieldCompany].[DisplayOrder],[dbo].[tblUserDataFieldCompany].[DisplayName],[dbo].[tblUserDataFieldCompany].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldCompany].[Required],[dbo].[tblUserDataFieldCompany].[UserGroupGuid],[dbo].[tblUserDataFieldCompany].[CreatedDate],[dbo].[tblUserDataFieldCompany].[CreatedBy],[dbo].[tblUserDataFieldCompany].[UpdatedDate],[dbo].[tblUserDataFieldCompany].[UpdatedBy],[dbo].[tblUserDataFieldCompany].[DispatchField],[dbo].[tblUserDataFieldCompany].[ClearOnNew],[dbo].[tblUserDataFieldCompany].[ReadOnly],[dbo].[tblUserDataFieldCompany].[Visibility],[dbo].[tblUserDataFieldCompany].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataFieldCompany]
            INNER JOIN [track].[tblUserDataFieldCompany] CT
                ON CT.PK_UserDataFieldCompanyGuid = [dbo].[tblUserDataFieldCompany].[UserDataFieldCompanyGuid]
        WHERE CT.PK_UserDataFieldCompanyGuid = @UserDataFieldCompanyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
