-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblLedgerAggregateColumns
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblLedgerAggregateColumns]
@LedgerAggregateColumnGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblLedgerAggregateColumns].[ID],[dbo].[tblLedgerAggregateColumns].[CreatedDate],[dbo].[tblLedgerAggregateColumns].[CreatedBy],[dbo].[tblLedgerAggregateColumns].[UpdatedDate],[dbo].[tblLedgerAggregateColumns].[UpdatedBy],[dbo].[tblLedgerAggregateColumns].[CustomFunctionName],[dbo].[tblLedgerAggregateColumns].[LedgerAggregateColumnGuid],[dbo].[tblLedgerAggregateColumns].[SiteGuid],[dbo].[tblLedgerAggregateColumns].[LookupAggregateFieldIndex], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblLedgerAggregateColumns]
            INNER JOIN [track].[tblLedgerAggregateColumns] CT
                ON CT.PK_LedgerAggregateColumnGuid = [dbo].[tblLedgerAggregateColumns].[LedgerAggregateColumnGuid]
        WHERE CT.PK_LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
