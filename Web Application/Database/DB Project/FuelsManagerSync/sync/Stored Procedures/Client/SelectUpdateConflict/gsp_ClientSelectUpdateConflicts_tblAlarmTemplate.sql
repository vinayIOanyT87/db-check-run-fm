-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmTemplate
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAlarmTemplate]
@AlarmTemplateGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAlarmTemplate].[AlarmTemplateGuid],[dbo].[tblAlarmTemplate].[InputTemplateTagGuid],[dbo].[tblAlarmTemplate].[ID],[dbo].[tblAlarmTemplate].[Enabled],[dbo].[tblAlarmTemplate].[AlarmCategoryApplicationStringGuid],[dbo].[tblAlarmTemplate].[Order],[dbo].[tblAlarmTemplate].[NotAlarmState],[dbo].[tblAlarmTemplate].[Comment],[dbo].[tblAlarmTemplate].[ShelvedStartTimeStamp],[dbo].[tblAlarmTemplate].[ShelvedEndTimeStamp],[dbo].[tblAlarmTemplate].[ShelvedOneShot],[dbo].[tblAlarmTemplate].[ShelvedBy],[dbo].[tblAlarmTemplate].[Suppressed],[dbo].[tblAlarmTemplate].[CreatedDate],[dbo].[tblAlarmTemplate].[CreatedBy],[dbo].[tblAlarmTemplate].[UpdatedDate],[dbo].[tblAlarmTemplate].[UpdatedBy],[dbo].[tblAlarmTemplate].[AlarmStateTemplateTagGuid],[dbo].[tblAlarmTemplate].[ExclusiveAlarm], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAlarmTemplate]
            INNER JOIN [track].[tblAlarmTemplate] CT
                ON CT.PK_AlarmTemplateGuid = [dbo].[tblAlarmTemplate].[AlarmTemplateGuid]
        WHERE CT.PK_AlarmTemplateGuid = @AlarmTemplateGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
