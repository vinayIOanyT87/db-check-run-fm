-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblFuelCardLimitPeriod
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblFuelCardLimitPeriod]
@FuelCardLimitPeriodIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblFuelCardLimitPeriod].[FuelCardLimitPeriodIndex],[lookup].[tblFuelCardLimitPeriod].[FuelCardLimitPeriodCode],[lookup].[tblFuelCardLimitPeriod].[FuelCardLimitPeriodName],[lookup].[tblFuelCardLimitPeriod].[FuelCardLimitPeriodGuid],[lookup].[tblFuelCardLimitPeriod].[CreatedBy],[lookup].[tblFuelCardLimitPeriod].[CreatedDate],[lookup].[tblFuelCardLimitPeriod].[UpdatedBy],[lookup].[tblFuelCardLimitPeriod].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblFuelCardLimitPeriod]
            INNER JOIN [track].[tblFuelCardLimitPeriod] CT
                ON CT.PK_FuelCardLimitPeriodIndex = [lookup].[tblFuelCardLimitPeriod].[FuelCardLimitPeriodIndex]
        WHERE CT.PK_FuelCardLimitPeriodIndex = @FuelCardLimitPeriodIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
