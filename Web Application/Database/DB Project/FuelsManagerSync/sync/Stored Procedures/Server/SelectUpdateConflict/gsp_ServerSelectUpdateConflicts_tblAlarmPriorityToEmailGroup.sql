-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblAlarmPriorityToEmailGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAlarmPriorityToEmailGroup]
@AlarmPriorityEmailGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblAlarmPriorityToEmailGroup].[AlarmPriorityEmailGroupGuid],[map].[tblAlarmPriorityToEmailGroup].[AlarmPriorityGuid],[map].[tblAlarmPriorityToEmailGroup].[EmailGroupGuid],[map].[tblAlarmPriorityToEmailGroup].[CreatedDate],[map].[tblAlarmPriorityToEmailGroup].[CreatedBy],[map].[tblAlarmPriorityToEmailGroup].[UpdatedDate],[map].[tblAlarmPriorityToEmailGroup].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblAlarmPriorityToEmailGroup]
            INNER JOIN [track].[tblAlarmPriorityToEmailGroup] CT
                ON CT.PK_AlarmPriorityEmailGroupGuid = [map].[tblAlarmPriorityToEmailGroup].[AlarmPriorityEmailGroupGuid]
        WHERE CT.PK_AlarmPriorityEmailGroupGuid = @AlarmPriorityEmailGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
