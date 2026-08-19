-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueCompany
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblUserDataListValueCompany]
@UserDataListValueCompanyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataListValueCompany].[UserDataListValueCompanyGuid],[dbo].[tblUserDataListValueCompany].[UserDataFieldCompanyGuid],[dbo].[tblUserDataListValueCompany].[Value],[dbo].[tblUserDataListValueCompany].[CreatedDate],[dbo].[tblUserDataListValueCompany].[CreatedBy],[dbo].[tblUserDataListValueCompany].[UpdatedDate],[dbo].[tblUserDataListValueCompany].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataListValueCompany]
            INNER JOIN [track].[tblUserDataListValueCompany] CT
                ON CT.PK_UserDataListValueCompanyGuid = [dbo].[tblUserDataListValueCompany].[UserDataListValueCompanyGuid]
        WHERE CT.PK_UserDataListValueCompanyGuid = @UserDataListValueCompanyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
