-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblSitesAncillaryData
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblSitesAncillaryData]
@SiteAncillaryDataGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblSitesAncillaryData].[SiteAncillaryDataGuid],[dbo].[tblSitesAncillaryData].[SiteGuid],[dbo].[tblSitesAncillaryData].[AdjustmentTransactionAliasGuid],[dbo].[tblSitesAncillaryData].[IATAGuid],[dbo].[tblSitesAncillaryData].[InventoryTransactionAliasGuid],[dbo].[tblSitesAncillaryData].[NoteGuid],[dbo].[tblSitesAncillaryData].[CreatedDate],[dbo].[tblSitesAncillaryData].[CreatedBy],[dbo].[tblSitesAncillaryData].[UpdatedDate],[dbo].[tblSitesAncillaryData].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblSitesAncillaryData]
            INNER JOIN [track].[tblSitesAncillaryData] CT
                ON CT.PK_SiteAncillaryDataGuid = [dbo].[tblSitesAncillaryData].[SiteAncillaryDataGuid]
        WHERE CT.PK_SiteAncillaryDataGuid = @SiteAncillaryDataGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
