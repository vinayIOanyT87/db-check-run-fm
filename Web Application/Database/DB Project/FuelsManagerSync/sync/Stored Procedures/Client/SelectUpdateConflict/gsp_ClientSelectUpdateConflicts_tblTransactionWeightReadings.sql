-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionWeightReadings
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTransactionWeightReadings]
@TransactionWeightReadingGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTransactionWeightReadings].[CompartmentID],[dbo].[tblTransactionWeightReadings].[BeginQuantityValue],[dbo].[tblTransactionWeightReadings].[RequestedQuantityValue],[dbo].[tblTransactionWeightReadings].[FinalQuantityValue],[dbo].[tblTransactionWeightReadings].[CreatedBy],[dbo].[tblTransactionWeightReadings].[CreatedDate],[dbo].[tblTransactionWeightReadings].[UpdatedBy],[dbo].[tblTransactionWeightReadings].[UpdatedDate],[dbo].[tblTransactionWeightReadings].[TransVersion],[dbo].[tblTransactionWeightReadings].[TransactionWeightReadingGuid],[dbo].[tblTransactionWeightReadings].[TransactionGuid],[dbo].[tblTransactionWeightReadings].[FuelsManagerVersionNumber],[dbo].[tblTransactionWeightReadings].[SourceVersionNumber],[dbo].[tblTransactionWeightReadings].[HistoricalFlag],[dbo].[tblTransactionWeightReadings].[VolumetricTopOffFlag], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTransactionWeightReadings]
            INNER JOIN [track].[tblTransactionWeightReadings] CT
                ON CT.PK_TransactionWeightReadingGuid = [dbo].[tblTransactionWeightReadings].[TransactionWeightReadingGuid]
        WHERE CT.PK_TransactionWeightReadingGuid = @TransactionWeightReadingGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
