CREATE PROCEDURE [dbo].[usp_AlarmDeleteAlarmsForTagNotInList](@inputTagGuid UNIQUEIDENTIFIER, @AlarmList GuidListType READONLY )
AS
BEGIN
    SET NOCOUNT ON

	SELECT [AlarmGuid]
	INTO #AlarmsToDelete
	FROM [dbo].[tblAlarm] att
	WHERE att.InputTagGuid = @inputTagGuid
	AND NOT EXISTS (SELECT 1 
					FROM @AlarmList atl
					WHERE atl.Guid = att.AlarmGuid )

	IF ( SELECT COUNT(*) FROM #AlarmsToDelete ) > 0
	BEGIN

		-- delete the children entries in map.tblPointAccessGroupToPointAlarmTest
		DELETE  pagtpat
		FROM [map].[tblPointAccessGroupToPointAlarmTest] pagtpat
		INNER JOIN [dbo].[tblAlarmTest] att ON pagtpat.AlarmTestGuid = att.AlarmTestGuid
		INNER JOIN #AlarmsToDelete at ON at.[AlarmGuid] = att.AlarmGuid
	
		-- delete the children entries in tblPointTagAlarmStatus
		DELETE  ptta
		FROM [dbo].[tblPointTagAlarmStatus] ptta
		INNER JOIN [dbo].[tblAlarmTest] att ON ptta.AlarmTestGuid = att.AlarmTestGuid
		INNER JOIN #AlarmsToDelete at ON at.[AlarmGuid] = att.AlarmGuid

		-- delete the children entries in tblAlarmTest
		DELETE  att
		FROM [dbo].[tblAlarmTest] att
		INNER JOIN #AlarmsToDelete at ON at.[AlarmGuid] = att.AlarmGuid

		-- delete the actual rows in the alarm  table
		DELETE  att
		FROM [dbo].[tblAlarm] att
		INNER JOIN #AlarmsToDelete at ON at.[AlarmGuid] = att.AlarmGuid

	END
END
GO


