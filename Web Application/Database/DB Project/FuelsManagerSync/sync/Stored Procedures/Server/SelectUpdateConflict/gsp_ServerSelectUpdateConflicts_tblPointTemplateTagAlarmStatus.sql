-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTemplateTagAlarmStatus
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointTemplateTagAlarmStatus]
@PointTemplateTagAlarmStatusGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointTemplateTagAlarmStatus].[PointTemplateTagAlarmStatusGuid],[dbo].[tblPointTemplateTagAlarmStatus].[AlarmTestTemplateGuid],[dbo].[tblPointTemplateTagAlarmStatus].[Acknowledged],[dbo].[tblPointTemplateTagAlarmStatus].[AcknowledgedTimestamp],[dbo].[tblPointTemplateTagAlarmStatus].[AcknowledgedBy],[dbo].[tblPointTemplateTagAlarmStatus].[AcknowledgedComment],[dbo].[tblPointTemplateTagAlarmStatus].[Silenced],[dbo].[tblPointTemplateTagAlarmStatus].[SilencedTimestamp],[dbo].[tblPointTemplateTagAlarmStatus].[SilencedBy],[dbo].[tblPointTemplateTagAlarmStatus].[AlarmTestFailed],[dbo].[tblPointTemplateTagAlarmStatus].[AlarmTestFailedTimestamp],[dbo].[tblPointTemplateTagAlarmStatus].[CreatedDate],[dbo].[tblPointTemplateTagAlarmStatus].[CreatedBy],[dbo].[tblPointTemplateTagAlarmStatus].[UpdatedDate],[dbo].[tblPointTemplateTagAlarmStatus].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointTemplateTagAlarmStatus]
            INNER JOIN [track].[tblPointTemplateTagAlarmStatus] CT
                ON CT.PK_PointTemplateTagAlarmStatusGuid = [dbo].[tblPointTemplateTagAlarmStatus].[PointTemplateTagAlarmStatusGuid]
        WHERE CT.PK_PointTemplateTagAlarmStatusGuid = @PointTemplateTagAlarmStatusGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
