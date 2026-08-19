-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEmailTemplateToAlarmAndEvent
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEmailTemplateToAlarmAndEvent]
@EmailTemplateToAlarmAndEventGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEmailTemplateToAlarmAndEvent].[EmailTemplateToAlarmAndEventGuid],[map].[tblEmailTemplateToAlarmAndEvent].[EmailTemplateGuid],[map].[tblEmailTemplateToAlarmAndEvent].[AlarmAndEventGuid],[map].[tblEmailTemplateToAlarmAndEvent].[CreatedDate],[map].[tblEmailTemplateToAlarmAndEvent].[CreatedBy],[map].[tblEmailTemplateToAlarmAndEvent].[UpdatedDate],[map].[tblEmailTemplateToAlarmAndEvent].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEmailTemplateToAlarmAndEvent]
            INNER JOIN [track].[tblEmailTemplateToAlarmAndEvent] CT
                ON CT.PK_EmailTemplateToAlarmAndEventGuid = [map].[tblEmailTemplateToAlarmAndEvent].[EmailTemplateToAlarmAndEventGuid]
        WHERE CT.PK_EmailTemplateToAlarmAndEventGuid = @EmailTemplateToAlarmAndEventGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
