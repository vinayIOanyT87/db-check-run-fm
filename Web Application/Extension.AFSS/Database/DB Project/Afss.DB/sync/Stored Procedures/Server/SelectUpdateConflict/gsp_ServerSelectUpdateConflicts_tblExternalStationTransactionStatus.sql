-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExternalStationTransactionStatus
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalStationTransactionStatus]
@ExternalStationTransactionStatusIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblExternalStationTransactionStatus].[ExternalStationTransactionStatusIndex],[lookup].[tblExternalStationTransactionStatus].[ExternalStationTransactionStatusCode],[lookup].[tblExternalStationTransactionStatus].[ExternalStationTransactionStatusName],[lookup].[tblExternalStationTransactionStatus].[ExternalStationTransactionStatusGuid],[lookup].[tblExternalStationTransactionStatus].[LongDescription],[lookup].[tblExternalStationTransactionStatus].[DisplayOrder],[lookup].[tblExternalStationTransactionStatus].[CreatedBy],[lookup].[tblExternalStationTransactionStatus].[CreatedDate],[lookup].[tblExternalStationTransactionStatus].[UpdatedBy],[lookup].[tblExternalStationTransactionStatus].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblExternalStationTransactionStatus]
            INNER JOIN [track].[tblExternalStationTransactionStatus] CT
                ON CT.PK_ExternalStationTransactionStatusIndex = [lookup].[tblExternalStationTransactionStatus].[ExternalStationTransactionStatusIndex]
        WHERE CT.PK_ExternalStationTransactionStatusIndex = @ExternalStationTransactionStatusIndex
    ORDER BY CT.UpdatedRowVersion ASC
END