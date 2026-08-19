-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblBulkPaymentLinks
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblBulkPaymentLinks]
@BulkPaymentLinkGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblBulkPaymentLinks].[BulkPaymentID],[dbo].[tblBulkPaymentLinks].[RebateNumber],[dbo].[tblBulkPaymentLinks].[CreatedBy],[dbo].[tblBulkPaymentLinks].[CreatedDate],[dbo].[tblBulkPaymentLinks].[UpdatedBy],[dbo].[tblBulkPaymentLinks].[UpdatedDate],[dbo].[tblBulkPaymentLinks].[BulkPaymentLinkGuid],[dbo].[tblBulkPaymentLinks].[InvoiceTransactionGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblBulkPaymentLinks]
            INNER JOIN [track].[tblBulkPaymentLinks] CT
                ON CT.PK_BulkPaymentLinkGuid = [dbo].[tblBulkPaymentLinks].[BulkPaymentLinkGuid]
        WHERE CT.PK_BulkPaymentLinkGuid = @BulkPaymentLinkGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
