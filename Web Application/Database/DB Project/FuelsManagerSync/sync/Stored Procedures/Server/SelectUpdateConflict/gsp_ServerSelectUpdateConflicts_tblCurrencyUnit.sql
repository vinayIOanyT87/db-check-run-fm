-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblCurrencyUnit
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCurrencyUnit]
@CurrencyUnitIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblCurrencyUnit].[CurrencyUnitIndex],[lookup].[tblCurrencyUnit].[CurrencyUnitCode],[lookup].[tblCurrencyUnit].[CurrencyUnitName],[lookup].[tblCurrencyUnit].[CurrencyUnitGuid],[lookup].[tblCurrencyUnit].[CreatedDate],[lookup].[tblCurrencyUnit].[CreatedBy],[lookup].[tblCurrencyUnit].[UpdatedDate],[lookup].[tblCurrencyUnit].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblCurrencyUnit]
            INNER JOIN [track].[tblCurrencyUnit] CT
                ON CT.PK_CurrencyUnitIndex = [lookup].[tblCurrencyUnit].[CurrencyUnitIndex]
        WHERE CT.PK_CurrencyUnitIndex = @CurrencyUnitIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
