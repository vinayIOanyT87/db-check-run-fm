CREATE PROCEDURE [dbo].[usp_AlarmTestTemplateDeleteByNotInListAlarmTestTemplateGuid]( @AlarmTemplateGuid UNIQUEIDENTIFIER, @AlarmTestList GuidListType READONLY )
AS
BEGIN

   SET NOCOUNT ON

	SELECT [AlarmTestTemplateGuid]
	INTO #AlarmTestTemplateToDelete
	FROM [dbo].[tblAlarmTestTemplate] att
	WHERE att.AlarmTemplateGuid = @AlarmTemplateGuid
	AND NOT EXISTS (SELECT 1 FROM @AlarmTestList atl WHERE atl.Guid = att.[AlarmTestTemplateGuid] )

	IF ( SELECT COUNT(*) FROM #AlarmTestTemplateToDelete ) > 0
	BEGIN

		SELECT [AlarmTestGuid]
		INTO #AlarmTestToDelete
		FROM [dbo].[tblAlarmTest] at
		INNER JOIN [dbo].[tblAlarm] a ON a.AlarmGuid = at.AlarmGuid
		WHERE a.AlarmTemplateGuid = @AlarmTemplateGuid
		AND NOT EXISTS (SELECT 1 FROM @AlarmTestList atl WHERE atl.Guid = at.[AlarmTestTemplateGuid] OR at.[AlarmTestTemplateGuid] IS NULL )

		IF ( SELECT COUNT(*) FROM #AlarmTestToDelete ) > 0
		BEGIN

			-- delete the children entries in tblPointTemplateTagAlarmStatus
			DELETE  ptas
			FROM [dbo].[tblPointTagAlarmStatus] ptas
			INNER JOIN #AlarmTestToDelete atd ON atd.AlarmTestGuid = ptas.AlarmTestGuid

			-- delete the actual rows
			DELETE  at
			FROM [dbo].[tblAlarmTest] at
			INNER JOIN [dbo].[tblAlarm] a ON a.AlarmGuid = at.AlarmGuid
			INNER JOIN #AlarmTestToDelete atd ON atd.AlarmTestGuid = at.AlarmTestGuid
			WHERE a.AlarmTemplateGuid = @AlarmTemplateGuid

		END

		-- delete the children entries in map.tblPointAccessGroupToAlarmTest
		DELETE  pagtat
		FROM [map].[tblPointAccessGroupToAlarmTest] pagtat
		INNER JOIN #AlarmTestTemplateToDelete attd ON attd.AlarmTestTemplateGuid = pagtat.AlarmTestGuid

		-- delete the children entries in tblPointTemplateTagAlarmStatus
		DELETE  pttas
		FROM [dbo].[tblPointTemplateTagAlarmStatus] pttas
		INNER JOIN #AlarmTestTemplateToDelete attd ON attd.AlarmTestTemplateGuid = pttas.AlarmTestTemplateGuid

		-- delete the actual rows
		DELETE  att
		FROM [dbo].[tblAlarmTestTemplate] att
		INNER JOIN #AlarmTestTemplateToDelete attd ON attd.AlarmTestTemplateGuid = att.AlarmTestTemplateGuid
		WHERE att.AlarmTemplateGuid = @AlarmTemplateGuid

	END
END
GO


