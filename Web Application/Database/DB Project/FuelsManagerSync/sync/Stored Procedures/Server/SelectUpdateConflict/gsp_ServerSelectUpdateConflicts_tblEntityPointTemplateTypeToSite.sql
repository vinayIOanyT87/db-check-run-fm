-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityPointTemplateTypeToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityPointTemplateTypeToSite]
@PointTemplateTypeToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityPointTemplateTypeToSite].[PointTemplateTypeToSiteGuid],[map].[tblEntityPointTemplateTypeToSite].[ApplicationStringGuid],[map].[tblEntityPointTemplateTypeToSite].[SiteGuid],[map].[tblEntityPointTemplateTypeToSite].[CreatedDate],[map].[tblEntityPointTemplateTypeToSite].[CreatedBy],[map].[tblEntityPointTemplateTypeToSite].[UpdatedDate],[map].[tblEntityPointTemplateTypeToSite].[UpdatedBy],[map].[tblEntityPointTemplateTypeToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityPointTemplateTypeToSite]
            INNER JOIN [track].[tblEntityPointTemplateTypeToSite] CT
                ON CT.PK_PointTemplateTypeToSiteGuid = [map].[tblEntityPointTemplateTypeToSite].[PointTemplateTypeToSiteGuid]
        WHERE CT.PK_PointTemplateTypeToSiteGuid = @PointTemplateTypeToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
