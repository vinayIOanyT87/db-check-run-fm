-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmTestTemplate
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAlarmTestTemplate]
@AlarmTestTemplateGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAlarmTestTemplate].[AlarmTestTemplateGuid],[dbo].[tblAlarmTestTemplate].[AlarmTemplateGuid],[dbo].[tblAlarmTestTemplate].[ID],[dbo].[tblAlarmTestTemplate].[LimitTemplateTagGuid],[dbo].[tblAlarmTestTemplate].[TagField],[dbo].[tblAlarmTestTemplate].[AlarmPriorityGuid],[dbo].[tblAlarmTestTemplate].[NormalUnacknowledgedAlarmPriorityGuid],[dbo].[tblAlarmTestTemplate].[TestType],[dbo].[tblAlarmTestTemplate].[BitMask],[dbo].[tblAlarmTestTemplate].[Enabled],[dbo].[tblAlarmTestTemplate].[Order],[dbo].[tblAlarmTestTemplate].[AlarmState],[dbo].[tblAlarmTestTemplate].[Holdoff],[dbo].[tblAlarmTestTemplate].[AlarmText],[dbo].[tblAlarmTestTemplate].[HelpFile],[dbo].[tblAlarmTestTemplate].[DrawingGuid],[dbo].[tblAlarmTestTemplate].[CreatedDate],[dbo].[tblAlarmTestTemplate].[CreatedBy],[dbo].[tblAlarmTestTemplate].[UpdatedDate],[dbo].[tblAlarmTestTemplate].[UpdatedBy],[dbo].[tblAlarmTestTemplate].[BitwiseOperator],[dbo].[tblAlarmTestTemplate].[TimedHoldOffInSeconds], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAlarmTestTemplate]
            INNER JOIN [track].[tblAlarmTestTemplate] CT
                ON CT.PK_AlarmTestTemplateGuid = [dbo].[tblAlarmTestTemplate].[AlarmTestTemplateGuid]
        WHERE CT.PK_AlarmTestTemplateGuid = @AlarmTestTemplateGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
