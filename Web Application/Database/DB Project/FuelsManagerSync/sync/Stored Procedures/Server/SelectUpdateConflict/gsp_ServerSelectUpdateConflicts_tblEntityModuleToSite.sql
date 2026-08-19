-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityModuleToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityModuleToSite]
@ModuleToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityModuleToSite].[ModuleToSiteGuid],[map].[tblEntityModuleToSite].[ModuleGuid],[map].[tblEntityModuleToSite].[SiteGuid],[map].[tblEntityModuleToSite].[CreatedDate],[map].[tblEntityModuleToSite].[CreatedBy],[map].[tblEntityModuleToSite].[UpdatedDate],[map].[tblEntityModuleToSite].[UpdatedBy],[map].[tblEntityModuleToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityModuleToSite]
            INNER JOIN [track].[tblEntityModuleToSite] CT
                ON CT.PK_ModuleToSiteGuid = [map].[tblEntityModuleToSite].[ModuleToSiteGuid]
        WHERE CT.PK_ModuleToSiteGuid = @ModuleToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
