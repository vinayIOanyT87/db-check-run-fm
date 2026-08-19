-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblCurrencies
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCurrencies]
@CurrencyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblCurrencies].[Country],[dbo].[tblCurrencies].[UnitDisplayName],[dbo].[tblCurrencies].[DisplayFlag],[dbo].[tblCurrencies].[CreatedBy],[dbo].[tblCurrencies].[CreatedDate],[dbo].[tblCurrencies].[UpdatedBy],[dbo].[tblCurrencies].[UpdatedDate],[dbo].[tblCurrencies].[CurrencyGuid],[dbo].[tblCurrencies].[SiteGuid],[dbo].[tblCurrencies].[LookupCurrencyUnitIndex], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblCurrencies]
            INNER JOIN [track].[tblCurrencies] CT
                ON CT.PK_CurrencyGuid = [dbo].[tblCurrencies].[CurrencyGuid]
        WHERE CT.PK_CurrencyGuid = @CurrencyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
