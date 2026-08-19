-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityAutoDistributionReasonCodeToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityAutoDistributionReasonCodeToSite]
@AutoDistributionReasonCodeToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityAutoDistributionReasonCodeToSite].[AutoDistributionReasonCodeToSiteGuid],[map].[tblEntityAutoDistributionReasonCodeToSite].[SiteGuid],[map].[tblEntityAutoDistributionReasonCodeToSite].[AutoDistributionReasonCodeGuid],[map].[tblEntityAutoDistributionReasonCodeToSite].[Description],[map].[tblEntityAutoDistributionReasonCodeToSite].[CreatedDate],[map].[tblEntityAutoDistributionReasonCodeToSite].[CreatedBy],[map].[tblEntityAutoDistributionReasonCodeToSite].[UpdatedDate],[map].[tblEntityAutoDistributionReasonCodeToSite].[UpdatedBy],[map].[tblEntityAutoDistributionReasonCodeToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityAutoDistributionReasonCodeToSite]
            INNER JOIN [track].[tblEntityAutoDistributionReasonCodeToSite] CT
                ON CT.PK_AutoDistributionReasonCodeToSiteGuid = [map].[tblEntityAutoDistributionReasonCodeToSite].[AutoDistributionReasonCodeToSiteGuid]
        WHERE CT.PK_AutoDistributionReasonCodeToSiteGuid = @AutoDistributionReasonCodeToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
