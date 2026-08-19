-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarm
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAlarm]
@AlarmGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAlarm].[AlarmGuid],[dbo].[tblAlarm].[InputTagGuid],[dbo].[tblAlarm].[ID],[dbo].[tblAlarm].[Enabled],[dbo].[tblAlarm].[AlarmCategoryApplicationStringGuid],[dbo].[tblAlarm].[Order],[dbo].[tblAlarm].[NotAlarmState],[dbo].[tblAlarm].[Comment],[dbo].[tblAlarm].[ShelvedStartTimeStamp],[dbo].[tblAlarm].[ShelvedEndTimeStamp],[dbo].[tblAlarm].[ShelvedOneShot],[dbo].[tblAlarm].[ShelvedBy],[dbo].[tblAlarm].[Suppressed],[dbo].[tblAlarm].[CreatedDate],[dbo].[tblAlarm].[CreatedBy],[dbo].[tblAlarm].[UpdatedDate],[dbo].[tblAlarm].[UpdatedBy],[dbo].[tblAlarm].[AlarmStateTagGuid],[dbo].[tblAlarm].[ExclusiveAlarm],[dbo].[tblAlarm].[AlarmTemplateGuid],[dbo].[tblAlarm].[Notify], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAlarm]
            INNER JOIN [track].[tblAlarm] CT
                ON CT.PK_AlarmGuid = [dbo].[tblAlarm].[AlarmGuid]
        WHERE CT.PK_AlarmGuid = @AlarmGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
