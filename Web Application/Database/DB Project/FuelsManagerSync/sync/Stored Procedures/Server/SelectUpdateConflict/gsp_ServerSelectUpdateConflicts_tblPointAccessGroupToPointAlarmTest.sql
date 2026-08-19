-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPointAccessGroupToPointAlarmTest
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointAccessGroupToPointAlarmTest]
@PointAccessGroupToPointAlarmTestGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPointAccessGroupToPointAlarmTest].[PointAccessGroupToPointAlarmTestGuid],[map].[tblPointAccessGroupToPointAlarmTest].[PointAccessGroupGuid],[map].[tblPointAccessGroupToPointAlarmTest].[AlarmTestGuid],[map].[tblPointAccessGroupToPointAlarmTest].[View],[map].[tblPointAccessGroupToPointAlarmTest].[Acknowledge],[map].[tblPointAccessGroupToPointAlarmTest].[CreatedDate],[map].[tblPointAccessGroupToPointAlarmTest].[CreatedBy],[map].[tblPointAccessGroupToPointAlarmTest].[UpdatedDate],[map].[tblPointAccessGroupToPointAlarmTest].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPointAccessGroupToPointAlarmTest]
            INNER JOIN [track].[tblPointAccessGroupToPointAlarmTest] CT
                ON CT.PK_PointAccessGroupToPointAlarmTestGuid = [map].[tblPointAccessGroupToPointAlarmTest].[PointAccessGroupToPointAlarmTestGuid]
        WHERE CT.PK_PointAccessGroupToPointAlarmTestGuid = @PointAccessGroupToPointAlarmTestGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
