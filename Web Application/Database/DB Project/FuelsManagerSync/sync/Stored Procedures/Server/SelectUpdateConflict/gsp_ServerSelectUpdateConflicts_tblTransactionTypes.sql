-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblTransactionTypes
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTransactionTypes]
@TransactionTypesIndex smallint
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblTransactionTypes].[TransactionTypesIndex],[lookup].[tblTransactionTypes].[TransactionTypesCode],[lookup].[tblTransactionTypes].[TransactionTypesName],[lookup].[tblTransactionTypes].[TransactionTypesGuid],[lookup].[tblTransactionTypes].[CreatedDate],[lookup].[tblTransactionTypes].[CreatedBy],[lookup].[tblTransactionTypes].[UpdatedDate],[lookup].[tblTransactionTypes].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblTransactionTypes]
            INNER JOIN [track].[tblTransactionTypes] CT
                ON CT.PK_TransactionTypesIndex = [lookup].[tblTransactionTypes].[TransactionTypesIndex]
        WHERE CT.PK_TransactionTypesIndex = @TransactionTypesIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
