-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityLedgerAggregateColumnToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityLedgerAggregateColumnToSite]
@LedgerAggregateColumnToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityLedgerAggregateColumnToSite].[LedgerAggregateColumnToSiteGuid],[map].[tblEntityLedgerAggregateColumnToSite].[LedgerAggregateColumnGuid],[map].[tblEntityLedgerAggregateColumnToSite].[SiteGuid],[map].[tblEntityLedgerAggregateColumnToSite].[CreatedDate],[map].[tblEntityLedgerAggregateColumnToSite].[CreatedBy],[map].[tblEntityLedgerAggregateColumnToSite].[UpdatedDate],[map].[tblEntityLedgerAggregateColumnToSite].[UpdatedBy],[map].[tblEntityLedgerAggregateColumnToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityLedgerAggregateColumnToSite]
            INNER JOIN [track].[tblEntityLedgerAggregateColumnToSite] CT
                ON CT.PK_LedgerAggregateColumnToSiteGuid = [map].[tblEntityLedgerAggregateColumnToSite].[LedgerAggregateColumnToSiteGuid]
        WHERE CT.PK_LedgerAggregateColumnToSiteGuid = @LedgerAggregateColumnToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
