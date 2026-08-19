-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblTransactionFieldType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTransactionFieldType]
@TransactionFieldTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblTransactionFieldType].[TransactionFieldTypeIndex],[lookup].[tblTransactionFieldType].[TransactionFieldTypeCode],[lookup].[tblTransactionFieldType].[TransactionFieldTypeName],[lookup].[tblTransactionFieldType].[TransactionFieldTypeGuid],[lookup].[tblTransactionFieldType].[CreatedDate],[lookup].[tblTransactionFieldType].[CreatedBy],[lookup].[tblTransactionFieldType].[UpdatedDate],[lookup].[tblTransactionFieldType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblTransactionFieldType]
            INNER JOIN [track].[tblTransactionFieldType] CT
                ON CT.PK_TransactionFieldTypeIndex = [lookup].[tblTransactionFieldType].[TransactionFieldTypeIndex]
        WHERE CT.PK_TransactionFieldTypeIndex = @TransactionFieldTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
