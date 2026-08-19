-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmAndEventLog
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAlarmAndEventLog]
@AlarmAndEventLogGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAlarmAndEventLog].[Source],[dbo].[tblAlarmAndEventLog].[Alarm],[dbo].[tblAlarmAndEventLog].[ID],[dbo].[tblAlarmAndEventLog].[AssociatedData],[dbo].[tblAlarmAndEventLog].[CategoryID],[dbo].[tblAlarmAndEventLog].[PriorityID],[dbo].[tblAlarmAndEventLog].[Acknowledged],[dbo].[tblAlarmAndEventLog].[CreatedDate],[dbo].[tblAlarmAndEventLog].[CreatedBy],[dbo].[tblAlarmAndEventLog].[UpdatedDate],[dbo].[tblAlarmAndEventLog].[UpdatedBy],[dbo].[tblAlarmAndEventLog].[AlarmAndEventLogGuid],[dbo].[tblAlarmAndEventLog].[SiteGuid],[dbo].[tblAlarmAndEventLog].[SourceNode], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAlarmAndEventLog]
            INNER JOIN [track].[tblAlarmAndEventLog] CT
                ON CT.PK_AlarmAndEventLogGuid = [dbo].[tblAlarmAndEventLog].[AlarmAndEventLogGuid]
        WHERE CT.PK_AlarmAndEventLogGuid = @AlarmAndEventLogGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
