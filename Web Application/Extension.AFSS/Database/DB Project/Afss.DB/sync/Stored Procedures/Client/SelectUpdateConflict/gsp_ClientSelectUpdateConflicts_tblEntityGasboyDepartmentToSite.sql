-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityGasboyDepartmentToSite
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityGasboyDepartmentToSite]
@GasboyDepartmentToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentToSiteGuid],[map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentGuid],[map].[tblEntityGasboyDepartmentToSite].[SiteGuid],[map].[tblEntityGasboyDepartmentToSite].[AssignedFromSiteGuid],[map].[tblEntityGasboyDepartmentToSite].[CreatedBy],[map].[tblEntityGasboyDepartmentToSite].[CreatedDate],[map].[tblEntityGasboyDepartmentToSite].[UpdatedBy],[map].[tblEntityGasboyDepartmentToSite].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityGasboyDepartmentToSite]
            INNER JOIN [track].[tblEntityGasboyDepartmentToSite] CT
                ON CT.PK_GasboyDepartmentToSiteGuid = [map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentToSiteGuid]
        WHERE CT.PK_GasboyDepartmentToSiteGuid = @GasboyDepartmentToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END