-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExternalStationGeneralConfiguration
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalStationGeneralConfiguration]
@ExternalStationGeneralConfigurationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblExternalStationGeneralConfiguration].[ExternalStationGeneralConfigurationGuid],[dbo].[tblExternalStationGeneralConfiguration].[SiteGuid],[dbo].[tblExternalStationGeneralConfiguration].[RetailSaleTransactionAliasGuid],[dbo].[tblExternalStationGeneralConfiguration].[DownloadTransactionsIntervalMinutes],[dbo].[tblExternalStationGeneralConfiguration].[DownloadEventsIntervalMinutes],[dbo].[tblExternalStationGeneralConfiguration].[CreatedBy],[dbo].[tblExternalStationGeneralConfiguration].[CreatedDate],[dbo].[tblExternalStationGeneralConfiguration].[UpdatedBy],[dbo].[tblExternalStationGeneralConfiguration].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExternalStationGeneralConfiguration]
            INNER JOIN [track].[tblExternalStationGeneralConfiguration] CT
                ON CT.PK_ExternalStationGeneralConfigurationGuid = [dbo].[tblExternalStationGeneralConfiguration].[ExternalStationGeneralConfigurationGuid]
        WHERE CT.PK_ExternalStationGeneralConfigurationGuid = @ExternalStationGeneralConfigurationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END