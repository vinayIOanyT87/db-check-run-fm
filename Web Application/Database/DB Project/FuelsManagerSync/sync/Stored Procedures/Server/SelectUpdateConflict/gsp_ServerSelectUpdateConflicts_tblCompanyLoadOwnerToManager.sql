-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyLoadOwnerToManager
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCompanyLoadOwnerToManager]
@CompanyLoadOwnerToManagerGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyLoadOwnerToManager].[CompanyLoadOwnerToManagerGuid],[map].[tblCompanyLoadOwnerToManager].[CompanyGuid],[map].[tblCompanyLoadOwnerToManager].[AssignedToCompanyGuid],[map].[tblCompanyLoadOwnerToManager].[SiteGuid],[map].[tblCompanyLoadOwnerToManager].[ID],[map].[tblCompanyLoadOwnerToManager].[CreatedDate],[map].[tblCompanyLoadOwnerToManager].[CreatedBy],[map].[tblCompanyLoadOwnerToManager].[UpdatedDate],[map].[tblCompanyLoadOwnerToManager].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyLoadOwnerToManager]
            INNER JOIN [track].[tblCompanyLoadOwnerToManager] CT
                ON CT.PK_CompanyLoadOwnerToManagerGuid = [map].[tblCompanyLoadOwnerToManager].[CompanyLoadOwnerToManagerGuid]
        WHERE CT.PK_CompanyLoadOwnerToManagerGuid = @CompanyLoadOwnerToManagerGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
