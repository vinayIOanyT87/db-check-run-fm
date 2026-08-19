-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExternalStationTransactionFailedStatus
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblExternalStationTransactionFailedStatus]
@ExternalStationTransactionFailedStatusIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblExternalStationTransactionFailedStatus].[ExternalStationTransactionFailedStatusIndex],[lookup].[tblExternalStationTransactionFailedStatus].[ExternalStationTransactionFailedStatusCode],[lookup].[tblExternalStationTransactionFailedStatus].[ExternalStationTransactionFailedStatusName],[lookup].[tblExternalStationTransactionFailedStatus].[ExternalStationTransactionFailedStatusGuid],[lookup].[tblExternalStationTransactionFailedStatus].[LongDescription],[lookup].[tblExternalStationTransactionFailedStatus].[DisplayOrder],[lookup].[tblExternalStationTransactionFailedStatus].[FinalState],[lookup].[tblExternalStationTransactionFailedStatus].[CreatedBy],[lookup].[tblExternalStationTransactionFailedStatus].[CreatedDate],[lookup].[tblExternalStationTransactionFailedStatus].[UpdatedBy],[lookup].[tblExternalStationTransactionFailedStatus].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblExternalStationTransactionFailedStatus]
            INNER JOIN [track].[tblExternalStationTransactionFailedStatus] CT
                ON CT.PK_ExternalStationTransactionFailedStatusIndex = [lookup].[tblExternalStationTransactionFailedStatus].[ExternalStationTransactionFailedStatusIndex]
        WHERE CT.PK_ExternalStationTransactionFailedStatusIndex = @ExternalStationTransactionFailedStatusIndex
    ORDER BY CT.UpdatedRowVersion ASC
END