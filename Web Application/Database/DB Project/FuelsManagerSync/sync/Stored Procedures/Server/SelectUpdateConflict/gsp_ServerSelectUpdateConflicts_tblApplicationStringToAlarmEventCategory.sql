-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToAlarmEventCategory
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblApplicationStringToAlarmEventCategory]
@ApplicationStringToAlarmEventCategoryGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToAlarmEventCategory].[ApplicationStringToAlarmEventCategoryGuid],[map].[tblApplicationStringToAlarmEventCategory].[ApplicationStringGuid],[map].[tblApplicationStringToAlarmEventCategory].[EmailGroupGuid],[map].[tblApplicationStringToAlarmEventCategory].[Sequence],[map].[tblApplicationStringToAlarmEventCategory].[CreatedDate],[map].[tblApplicationStringToAlarmEventCategory].[CreatedBy],[map].[tblApplicationStringToAlarmEventCategory].[UpdatedDate],[map].[tblApplicationStringToAlarmEventCategory].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToAlarmEventCategory]
            INNER JOIN [track].[tblApplicationStringToAlarmEventCategory] CT
                ON CT.PK_ApplicationStringToAlarmEventCategoryGuid = [map].[tblApplicationStringToAlarmEventCategory].[ApplicationStringToAlarmEventCategoryGuid]
        WHERE CT.PK_ApplicationStringToAlarmEventCategoryGuid = @ApplicationStringToAlarmEventCategoryGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
