-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmAndEvents
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAlarmAndEvents]
@AlarmAndEventGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAlarmAndEvents].[Source],[dbo].[tblAlarmAndEvents].[Alarm],[dbo].[tblAlarmAndEvents].[ID],[dbo].[tblAlarmAndEvents].[CategoryIndex],[dbo].[tblAlarmAndEvents].[PriorityIndex],[dbo].[tblAlarmAndEvents].[CreatedDate],[dbo].[tblAlarmAndEvents].[CreatedBy],[dbo].[tblAlarmAndEvents].[UpdatedDate],[dbo].[tblAlarmAndEvents].[UpdatedBy],[dbo].[tblAlarmAndEvents].[Enabled],[dbo].[tblAlarmAndEvents].[AlarmAndEventGuid],[dbo].[tblAlarmAndEvents].[SiteGuid],[dbo].[tblAlarmAndEvents].[CategoryGuid],[dbo].[tblAlarmAndEvents].[PriorityGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAlarmAndEvents]
            INNER JOIN [track].[tblAlarmAndEvents] CT
                ON CT.PK_AlarmAndEventGuid = [dbo].[tblAlarmAndEvents].[AlarmAndEventGuid]
        WHERE CT.PK_AlarmAndEventGuid = @AlarmAndEventGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
