-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmTest
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAlarmTest]
@AlarmTestGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAlarmTest].[AlarmTestGuid],[dbo].[tblAlarmTest].[AlarmGuid],[dbo].[tblAlarmTest].[ID],[dbo].[tblAlarmTest].[LimitTagGuid],[dbo].[tblAlarmTest].[TagField],[dbo].[tblAlarmTest].[AlarmPriorityGuid],[dbo].[tblAlarmTest].[NormalUnacknowledgedAlarmPriorityGuid],[dbo].[tblAlarmTest].[TestType],[dbo].[tblAlarmTest].[BitMask],[dbo].[tblAlarmTest].[Enabled],[dbo].[tblAlarmTest].[Order],[dbo].[tblAlarmTest].[AlarmState],[dbo].[tblAlarmTest].[Holdoff],[dbo].[tblAlarmTest].[AlarmText],[dbo].[tblAlarmTest].[HelpFile],[dbo].[tblAlarmTest].[DrawingGuid],[dbo].[tblAlarmTest].[CreatedDate],[dbo].[tblAlarmTest].[CreatedBy],[dbo].[tblAlarmTest].[UpdatedDate],[dbo].[tblAlarmTest].[UpdatedBy],[dbo].[tblAlarmTest].[BitwiseOperator],[dbo].[tblAlarmTest].[TimedHoldOffInSeconds],[dbo].[tblAlarmTest].[AlarmTestTemplateGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAlarmTest]
            INNER JOIN [track].[tblAlarmTest] CT
                ON CT.PK_AlarmTestGuid = [dbo].[tblAlarmTest].[AlarmTestGuid]
        WHERE CT.PK_AlarmTestGuid = @AlarmTestGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
