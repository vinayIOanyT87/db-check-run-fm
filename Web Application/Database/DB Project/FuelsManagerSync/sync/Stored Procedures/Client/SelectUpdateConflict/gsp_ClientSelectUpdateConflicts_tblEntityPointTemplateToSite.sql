-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityPointTemplateToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityPointTemplateToSite]
@PointTemplateToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityPointTemplateToSite].[PointTemplateToSiteGuid],[map].[tblEntityPointTemplateToSite].[PointTemplateGuid],[map].[tblEntityPointTemplateToSite].[SiteGuid],[map].[tblEntityPointTemplateToSite].[CreatedDate],[map].[tblEntityPointTemplateToSite].[CreatedBy],[map].[tblEntityPointTemplateToSite].[UpdatedDate],[map].[tblEntityPointTemplateToSite].[UpdatedBy],[map].[tblEntityPointTemplateToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityPointTemplateToSite]
            INNER JOIN [track].[tblEntityPointTemplateToSite] CT
                ON CT.PK_PointTemplateToSiteGuid = [map].[tblEntityPointTemplateToSite].[PointTemplateToSiteGuid]
        WHERE CT.PK_PointTemplateToSiteGuid = @PointTemplateToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
