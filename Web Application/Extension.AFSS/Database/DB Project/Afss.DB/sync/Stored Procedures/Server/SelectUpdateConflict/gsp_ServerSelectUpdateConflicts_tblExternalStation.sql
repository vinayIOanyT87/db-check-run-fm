-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExternalStation
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalStation]
@ExternalStationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblExternalStation].[ExternalStationGuid],[dbo].[tblExternalStation].[SiteGuid],[dbo].[tblExternalStation].[ID],[dbo].[tblExternalStation].[LookupExternalStationTypeIndex],[dbo].[tblExternalStation].[BillingID],[dbo].[tblExternalStation].[DownloadTransactionsAutomatically],[dbo].[tblExternalStation].[LookupExternalStationStatusIndex],[dbo].[tblExternalStation].[LastSuccessfulConnection],[dbo].[tblExternalStation].[LastConnectionAttempt],[dbo].[tblExternalStation].[LastTransactionID],[dbo].[tblExternalStation].[LastDeviceCount],[dbo].[tblExternalStation].[CreatedBy],[dbo].[tblExternalStation].[CreatedDate],[dbo].[tblExternalStation].[UpdatedBy],[dbo].[tblExternalStation].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExternalStation]
            INNER JOIN [track].[tblExternalStation] CT
                ON CT.PK_ExternalStationGuid = [dbo].[tblExternalStation].[ExternalStationGuid]
        WHERE CT.PK_ExternalStationGuid = @ExternalStationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END