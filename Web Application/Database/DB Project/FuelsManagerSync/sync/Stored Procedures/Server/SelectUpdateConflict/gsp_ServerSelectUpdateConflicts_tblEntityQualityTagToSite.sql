-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityQualityTagToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityQualityTagToSite]
@QualityTagToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityQualityTagToSite].[QualityTagToSiteGuid],[map].[tblEntityQualityTagToSite].[QualityTagGuid],[map].[tblEntityQualityTagToSite].[SiteGuid],[map].[tblEntityQualityTagToSite].[CreatedDate],[map].[tblEntityQualityTagToSite].[CreatedBy],[map].[tblEntityQualityTagToSite].[UpdatedDate],[map].[tblEntityQualityTagToSite].[UpdatedBy],[map].[tblEntityQualityTagToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityQualityTagToSite]
            INNER JOIN [track].[tblEntityQualityTagToSite] CT
                ON CT.PK_QualityTagToSiteGuid = [map].[tblEntityQualityTagToSite].[QualityTagToSiteGuid]
        WHERE CT.PK_QualityTagToSiteGuid = @QualityTagToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
