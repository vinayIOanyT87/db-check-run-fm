CREATE PROCEDURE [dbo].[usp_AlarmDeleteNotInTagList]( @PointGuid uniqueIdentifier, @TagGuids GuidListType READONLY )
AS
BEGIN
    SET NOCOUNT ON

	SELECT [AlarmGuid]
	INTO #AlarmsToDelete
	FROM [dbo].[tblAlarm] att
	INNER JOIN dbo.tblPointTag ptt ON ptt.PointTagGuid = att.InputTagGuid AND ptt.PointGuid = @pointGuid
	WHERE NOT EXISTS (SELECT 1 
					FROM @TagGuids atl
					WHERE atl.Guid = att.InputTagGuid )

	IF ( SELECT COUNT(*) FROM #AlarmsToDelete ) > 0
	BEGIN

		-- delete the children entries in tblPointTagAlarmStatus
		DELETE  ptta
		FROM [dbo].[tblPointTagAlarmStatus] ptta
		INNER JOIN [dbo].[tblAlarmTest] att	ON ptta.AlarmTestGuid = att.AlarmTestGuid
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


