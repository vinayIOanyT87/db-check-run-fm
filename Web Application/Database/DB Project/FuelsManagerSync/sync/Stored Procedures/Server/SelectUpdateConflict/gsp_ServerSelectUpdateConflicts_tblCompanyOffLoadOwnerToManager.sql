-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyOffLoadOwnerToManager
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCompanyOffLoadOwnerToManager]
@CompanyOffLoadOwnerToManagerGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyOffLoadOwnerToManager].[CompanyOffLoadOwnerToManagerGuid],[map].[tblCompanyOffLoadOwnerToManager].[CompanyGuid],[map].[tblCompanyOffLoadOwnerToManager].[AssignedToCompanyGuid],[map].[tblCompanyOffLoadOwnerToManager].[SiteGuid],[map].[tblCompanyOffLoadOwnerToManager].[ID],[map].[tblCompanyOffLoadOwnerToManager].[CreatedDate],[map].[tblCompanyOffLoadOwnerToManager].[CreatedBy],[map].[tblCompanyOffLoadOwnerToManager].[UpdatedDate],[map].[tblCompanyOffLoadOwnerToManager].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyOffLoadOwnerToManager]
            INNER JOIN [track].[tblCompanyOffLoadOwnerToManager] CT
                ON CT.PK_CompanyOffLoadOwnerToManagerGuid = [map].[tblCompanyOffLoadOwnerToManager].[CompanyOffLoadOwnerToManagerGuid]
        WHERE CT.PK_CompanyOffLoadOwnerToManagerGuid = @CompanyOffLoadOwnerToManagerGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
