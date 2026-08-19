-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPointAccessGroupToAlarmTest
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointAccessGroupToAlarmTest]
@PointAccessGroupToAlarmTestGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPointAccessGroupToAlarmTest].[PointAccessGroupToAlarmTestGuid],[map].[tblPointAccessGroupToAlarmTest].[PointAccessGroupGuid],[map].[tblPointAccessGroupToAlarmTest].[AlarmTestGuid],[map].[tblPointAccessGroupToAlarmTest].[View],[map].[tblPointAccessGroupToAlarmTest].[Acknowledge],[map].[tblPointAccessGroupToAlarmTest].[CreatedDate],[map].[tblPointAccessGroupToAlarmTest].[CreatedBy],[map].[tblPointAccessGroupToAlarmTest].[UpdatedDate],[map].[tblPointAccessGroupToAlarmTest].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPointAccessGroupToAlarmTest]
            INNER JOIN [track].[tblPointAccessGroupToAlarmTest] CT
                ON CT.PK_PointAccessGroupToAlarmTestGuid = [map].[tblPointAccessGroupToAlarmTest].[PointAccessGroupToAlarmTestGuid]
        WHERE CT.PK_PointAccessGroupToAlarmTestGuid = @PointAccessGroupToAlarmTestGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
