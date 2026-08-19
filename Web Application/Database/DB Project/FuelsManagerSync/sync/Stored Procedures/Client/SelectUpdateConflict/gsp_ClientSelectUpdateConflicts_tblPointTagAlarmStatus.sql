-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTagAlarmStatus
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPointTagAlarmStatus]
@PointTagAlarmStatusGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointTagAlarmStatus].[PointTagAlarmStatusGuid],[dbo].[tblPointTagAlarmStatus].[AlarmTestGuid],[dbo].[tblPointTagAlarmStatus].[Acknowledged],[dbo].[tblPointTagAlarmStatus].[AcknowledgedTimestamp],[dbo].[tblPointTagAlarmStatus].[AcknowledgedBy],[dbo].[tblPointTagAlarmStatus].[AcknowledgedComment],[dbo].[tblPointTagAlarmStatus].[Silenced],[dbo].[tblPointTagAlarmStatus].[SilencedTimestamp],[dbo].[tblPointTagAlarmStatus].[SilencedBy],[dbo].[tblPointTagAlarmStatus].[AlarmTestFailed],[dbo].[tblPointTagAlarmStatus].[AlarmTestFailedTimestamp],[dbo].[tblPointTagAlarmStatus].[CreatedDate],[dbo].[tblPointTagAlarmStatus].[CreatedBy],[dbo].[tblPointTagAlarmStatus].[UpdatedDate],[dbo].[tblPointTagAlarmStatus].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointTagAlarmStatus]
            INNER JOIN [track].[tblPointTagAlarmStatus] CT
                ON CT.PK_PointTagAlarmStatusGuid = [dbo].[tblPointTagAlarmStatus].[PointTagAlarmStatusGuid]
        WHERE CT.PK_PointTagAlarmStatusGuid = @PointTagAlarmStatusGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
