-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblControllersLogToTransaction
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblControllersLogToTransaction]
@ControllersLogToTransactionGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblControllersLogToTransaction].[ControllersLogToTransactionGuid],[map].[tblControllersLogToTransaction].[ControllersLogGuid],[map].[tblControllersLogToTransaction].[TransactionGuid],[map].[tblControllersLogToTransaction].[CreatedDate],[map].[tblControllersLogToTransaction].[CreatedBy],[map].[tblControllersLogToTransaction].[UpdatedDate],[map].[tblControllersLogToTransaction].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblControllersLogToTransaction]
            INNER JOIN [track].[tblControllersLogToTransaction] CT
                ON CT.PK_ControllersLogToTransactionGuid = [map].[tblControllersLogToTransaction].[ControllersLogToTransactionGuid]
        WHERE CT.PK_ControllersLogToTransactionGuid = @ControllersLogToTransactionGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
