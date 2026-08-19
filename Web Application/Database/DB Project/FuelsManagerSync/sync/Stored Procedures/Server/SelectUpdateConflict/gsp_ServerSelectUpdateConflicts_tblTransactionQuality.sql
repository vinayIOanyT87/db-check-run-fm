-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblTransactionQuality
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTransactionQuality]
@TransactionQualityIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblTransactionQuality].[TransactionQualityIndex],[lookup].[tblTransactionQuality].[TransactionQualityCode],[lookup].[tblTransactionQuality].[TransactionQualityName],[lookup].[tblTransactionQuality].[TransactionQualityGuid],[lookup].[tblTransactionQuality].[CreatedDate],[lookup].[tblTransactionQuality].[CreatedBy],[lookup].[tblTransactionQuality].[UpdatedDate],[lookup].[tblTransactionQuality].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblTransactionQuality]
            INNER JOIN [track].[tblTransactionQuality] CT
                ON CT.PK_TransactionQualityIndex = [lookup].[tblTransactionQuality].[TransactionQualityIndex]
        WHERE CT.PK_TransactionQualityIndex = @TransactionQualityIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
