-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityLedgerViewToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityLedgerViewToSite]
@LedgerViewToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityLedgerViewToSite].[LedgerViewToSiteGuid],[map].[tblEntityLedgerViewToSite].[ListViewGuid],[map].[tblEntityLedgerViewToSite].[SiteGuid],[map].[tblEntityLedgerViewToSite].[CreatedDate],[map].[tblEntityLedgerViewToSite].[CreatedBy],[map].[tblEntityLedgerViewToSite].[UpdatedDate],[map].[tblEntityLedgerViewToSite].[UpdatedBy],[map].[tblEntityLedgerViewToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityLedgerViewToSite]
            INNER JOIN [track].[tblEntityLedgerViewToSite] CT
                ON CT.PK_LedgerViewToSiteGuid = [map].[tblEntityLedgerViewToSite].[LedgerViewToSiteGuid]
        WHERE CT.PK_LedgerViewToSiteGuid = @LedgerViewToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
