-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityDotHazardousMessagesToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityDotHazardousMessagesToSite]
@DotHazardousMessagesToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityDotHazardousMessagesToSite].[DotHazardousMessagesToSiteGuid],[map].[tblEntityDotHazardousMessagesToSite].[ApplicationStringGuid],[map].[tblEntityDotHazardousMessagesToSite].[SiteGuid],[map].[tblEntityDotHazardousMessagesToSite].[CreatedDate],[map].[tblEntityDotHazardousMessagesToSite].[CreatedBy],[map].[tblEntityDotHazardousMessagesToSite].[UpdatedDate],[map].[tblEntityDotHazardousMessagesToSite].[UpdatedBy],[map].[tblEntityDotHazardousMessagesToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityDotHazardousMessagesToSite]
            INNER JOIN [track].[tblEntityDotHazardousMessagesToSite] CT
                ON CT.PK_DotHazardousMessagesToSiteGuid = [map].[tblEntityDotHazardousMessagesToSite].[DotHazardousMessagesToSiteGuid]
        WHERE CT.PK_DotHazardousMessagesToSiteGuid = @DotHazardousMessagesToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
