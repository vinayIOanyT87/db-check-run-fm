-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExternalStationTransaction
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalStationTransaction]
@ExternalStationTransactionGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblExternalStationTransaction].[ExternalStationTransactionGuid],[dbo].[tblExternalStationTransaction].[ExternalStationGuid],[dbo].[tblExternalStationTransaction].[SiteGuid],[dbo].[tblExternalStationTransaction].[StationTransactionID],[dbo].[tblExternalStationTransaction].[RawTransactionData],[dbo].[tblExternalStationTransaction].[CreatedBy],[dbo].[tblExternalStationTransaction].[CreatedDate],[dbo].[tblExternalStationTransaction].[UpdatedBy],[dbo].[tblExternalStationTransaction].[UpdatedDate],[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionStatusIndex],[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionFailedStatusIndex], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExternalStationTransaction]
            INNER JOIN [track].[tblExternalStationTransaction] CT
                ON CT.PK_ExternalStationTransactionGuid = [dbo].[tblExternalStationTransaction].[ExternalStationTransactionGuid]
        WHERE CT.PK_ExternalStationTransactionGuid = @ExternalStationTransactionGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
