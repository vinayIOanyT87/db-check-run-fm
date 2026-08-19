-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionTransportLineItems
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTransactionTransportLineItems]
@TransactionTransportLineItemGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTransactionTransportLineItems].[TransportOrderNumber],[dbo].[tblTransactionTransportLineItems].[TransVersion],[dbo].[tblTransactionTransportLineItems].[LocationName],[dbo].[tblTransactionTransportLineItems].[Address1],[dbo].[tblTransactionTransportLineItems].[Address2],[dbo].[tblTransactionTransportLineItems].[City],[dbo].[tblTransactionTransportLineItems].[State],[dbo].[tblTransactionTransportLineItems].[Zip],[dbo].[tblTransactionTransportLineItems].[POCName],[dbo].[tblTransactionTransportLineItems].[POCPhone],[dbo].[tblTransactionTransportLineItems].[CreatedBy],[dbo].[tblTransactionTransportLineItems].[CreatedDate],[dbo].[tblTransactionTransportLineItems].[UpdatedBy],[dbo].[tblTransactionTransportLineItems].[UpdatedDate],[dbo].[tblTransactionTransportLineItems].[TransactionTransportLineItemGuid],[dbo].[tblTransactionTransportLineItems].[TransactionGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTransactionTransportLineItems]
            INNER JOIN [track].[tblTransactionTransportLineItems] CT
                ON CT.PK_TransactionTransportLineItemGuid = [dbo].[tblTransactionTransportLineItems].[TransactionTransportLineItemGuid]
        WHERE CT.PK_TransactionTransportLineItemGuid = @TransactionTransportLineItemGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
