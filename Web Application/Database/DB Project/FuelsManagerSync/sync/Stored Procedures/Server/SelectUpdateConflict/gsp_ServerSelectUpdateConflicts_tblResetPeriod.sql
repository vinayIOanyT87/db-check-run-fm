-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblResetPeriod
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblResetPeriod]
@ResetPeriodIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblResetPeriod].[ResetPeriodIndex],[lookup].[tblResetPeriod].[ResetPeriodCode],[lookup].[tblResetPeriod].[ResetPeriodName],[lookup].[tblResetPeriod].[ResetPeriodGuid],[lookup].[tblResetPeriod].[CreatedDate],[lookup].[tblResetPeriod].[CreatedBy],[lookup].[tblResetPeriod].[UpdatedDate],[lookup].[tblResetPeriod].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblResetPeriod]
            INNER JOIN [track].[tblResetPeriod] CT
                ON CT.PK_ResetPeriodIndex = [lookup].[tblResetPeriod].[ResetPeriodIndex]
        WHERE CT.PK_ResetPeriodIndex = @ResetPeriodIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
