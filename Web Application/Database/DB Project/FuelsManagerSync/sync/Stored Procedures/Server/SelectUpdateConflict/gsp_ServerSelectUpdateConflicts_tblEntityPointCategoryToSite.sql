-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityPointCategoryToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityPointCategoryToSite]
@PointCategoryToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityPointCategoryToSite].[PointCategoryToSiteGuid],[map].[tblEntityPointCategoryToSite].[ApplicationStringGuid],[map].[tblEntityPointCategoryToSite].[SiteGuid],[map].[tblEntityPointCategoryToSite].[CreatedDate],[map].[tblEntityPointCategoryToSite].[CreatedBy],[map].[tblEntityPointCategoryToSite].[UpdatedDate],[map].[tblEntityPointCategoryToSite].[UpdatedBy],[map].[tblEntityPointCategoryToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityPointCategoryToSite]
            INNER JOIN [track].[tblEntityPointCategoryToSite] CT
                ON CT.PK_PointCategoryToSiteGuid = [map].[tblEntityPointCategoryToSite].[PointCategoryToSiteGuid]
        WHERE CT.PK_PointCategoryToSiteGuid = @PointCategoryToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
