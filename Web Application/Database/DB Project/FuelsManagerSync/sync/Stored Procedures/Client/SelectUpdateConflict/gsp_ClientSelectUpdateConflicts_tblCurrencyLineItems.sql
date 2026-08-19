-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblCurrencyLineItems
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblCurrencyLineItems]
@CurrencyLineItemGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblCurrencyLineItems].[Date],[dbo].[tblCurrencyLineItems].[Rate],[dbo].[tblCurrencyLineItems].[CreatedBy],[dbo].[tblCurrencyLineItems].[CreatedDate],[dbo].[tblCurrencyLineItems].[UpdatedBy],[dbo].[tblCurrencyLineItems].[UpdatedDate],[dbo].[tblCurrencyLineItems].[CurrencyLineItemGuid],[dbo].[tblCurrencyLineItems].[CurrencyGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblCurrencyLineItems]
            INNER JOIN [track].[tblCurrencyLineItems] CT
                ON CT.PK_CurrencyLineItemGuid = [dbo].[tblCurrencyLineItems].[CurrencyLineItemGuid]
        WHERE CT.PK_CurrencyLineItemGuid = @CurrencyLineItemGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
