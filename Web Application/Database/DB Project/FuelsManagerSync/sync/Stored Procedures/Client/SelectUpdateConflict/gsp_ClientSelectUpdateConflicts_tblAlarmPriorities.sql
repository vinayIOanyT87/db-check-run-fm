-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmPriorities
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAlarmPriorities]
@AlarmPriorityGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAlarmPriorities].[ID],[dbo].[tblAlarmPriorities].[BackgroundSteady],[dbo].[tblAlarmPriorities].[BackgroundAlternate],[dbo].[tblAlarmPriorities].[TextSteady],[dbo].[tblAlarmPriorities].[TextAlternate],[dbo].[tblAlarmPriorities].[SoundFile],[dbo].[tblAlarmPriorities].[CreatedDate],[dbo].[tblAlarmPriorities].[CreatedBy],[dbo].[tblAlarmPriorities].[UpdatedDate],[dbo].[tblAlarmPriorities].[UpdatedBy],[dbo].[tblAlarmPriorities].[AlarmPriorityGuid],[dbo].[tblAlarmPriorities].[SiteGuid],[dbo].[tblAlarmPriorities].[Priority], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAlarmPriorities]
            INNER JOIN [track].[tblAlarmPriorities] CT
                ON CT.PK_AlarmPriorityGuid = [dbo].[tblAlarmPriorities].[AlarmPriorityGuid]
        WHERE CT.PK_AlarmPriorityGuid = @AlarmPriorityGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
