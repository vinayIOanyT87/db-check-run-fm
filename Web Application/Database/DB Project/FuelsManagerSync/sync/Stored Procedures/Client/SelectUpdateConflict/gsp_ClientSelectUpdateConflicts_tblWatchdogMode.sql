-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblWatchdogMode
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblWatchdogMode]
@WatchdogModeIndex tinyint
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblWatchdogMode].[WatchdogModeIndex],[lookup].[tblWatchdogMode].[WatchdogModeCode],[lookup].[tblWatchdogMode].[WatchdogModeName],[lookup].[tblWatchdogMode].[WatchdogModeGuid],[lookup].[tblWatchdogMode].[CreatedDate],[lookup].[tblWatchdogMode].[CreatedBy],[lookup].[tblWatchdogMode].[UpdatedDate],[lookup].[tblWatchdogMode].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblWatchdogMode]
            INNER JOIN [track].[tblWatchdogMode] CT
                ON CT.PK_WatchdogModeIndex = [lookup].[tblWatchdogMode].[WatchdogModeIndex]
        WHERE CT.PK_WatchdogModeIndex = @WatchdogModeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
