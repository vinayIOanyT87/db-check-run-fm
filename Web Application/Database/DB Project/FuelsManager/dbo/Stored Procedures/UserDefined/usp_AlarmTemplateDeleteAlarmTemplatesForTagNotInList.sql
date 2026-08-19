CREATE PROCEDURE [dbo].[usp_AlarmTemplateDeleteAlarmTemplatesForTagNotInList](@inputTagTemplateGuid UNIQUEIDENTIFIER, @AlarmList GuidListType READONLY )
AS
BEGIN
    SET NOCOUNT ON

	SELECT [AlarmTemplateGuid]
	INTO #AlarmTemplatesToDelete
	FROM [dbo].[tblAlarmTemplate] att
	WHERE att.InputTemplateTagGuid = @inputTagTemplateGuid
	AND NOT EXISTS (SELECT 1 FROM @AlarmList atl WHERE atl.Guid = att.AlarmTemplateGuid )

	IF ( SELECT COUNT(*) FROM #AlarmTemplatesToDelete ) > 0
	BEGIN
		SELECT [AlarmGuid]
		INTO #AlarmsToDelete
		FROM [dbo].[tblAlarm] at
		INNER JOIN [dbo].[tblPointTag] pt ON pt.PointTagGuid = at.InputTagGuid
		WHERE pt.PointTemplateTagGuid IS NOT NULL
		AND pt.PointTemplateTagGuid = @inputTagTemplateGuid
		AND at.AlarmTemplateGuid IS NOT NULL
		AND NOT EXISTS (SELECT 1 FROM @AlarmList atl WHERE atl.Guid = at.AlarmTemplateGuid)


		IF ( SELECT COUNT(*) FROM #AlarmsToDelete ) > 0
		BEGIN

			-- delete the children entries in tblPointTagAlarmStatus
			DELETE  ptas
			FROM [dbo].[tblPointTagAlarmStatus] ptas
			INNER JOIN [dbo].[tblAlarmTest] at ON ptas.AlarmTestGuid = at.AlarmTestGuid
			INNER JOIN #AlarmsToDelete atd ON atd.[AlarmGuid] = at.AlarmGuid

			-- delete the children entries in tblAlarmTest
			SELECT [LimitTagGuid] as PointTagGuid
			INTO #LimitTagsToDelete
			FROM [dbo].[tblAlarmTest] at
			INNER JOIN #AlarmsToDelete atd ON atd.AlarmGuid = at.AlarmGuid

			-- delete the point access group to point alarm test maps
			DELETE pagtpat
			FROM [map].[tblPointAccessGroupToPointAlarmTest] pagtpat
			INNER JOIN [dbo].[tblAlarmTest] at ON at.AlarmTestGuid = pagtpat.AlarmTestGuid
			INNER JOIN #AlarmsToDelete atd ON atd.[AlarmGuid] = at.AlarmGuid

			DELETE  at
			FROM [dbo].[tblAlarmTest] at
			INNER JOIN #AlarmsToDelete atd ON atd.[AlarmGuid] = at.AlarmGuid

			DELETE pt
			FROM tblPointTag pt
			INNER JOIN #LimitTagsToDelete lttd ON lttd.PointTagGuid = pt.PointTagGuid  

			-- delete the actual rows in the alarm table
			SELECT [AlarmStateTagGuid] as PointTagGuid
			INTO #StatusTagsToDelete
			FROM [dbo].[tblAlarm] at
			INNER JOIN #AlarmsToDelete atd ON atd.AlarmGuid = at.AlarmGuid

			DELETE  at
			FROM [dbo].[tblAlarm] at
			INNER JOIN #AlarmsToDelete atd ON atd.[AlarmGuid] = at.AlarmGuid

			DELETE pt
			FROM tblPointTag pt
			INNER JOIN #StatusTagsToDelete sttd ON sttd.PointTagGuid = pt.PointTagGuid  

		END


		-- delete the point access group to alarm test maps
		DELETE pagatt
		FROM [map].[tblPointAccessGroupToAlarmTest] pagatt
		INNER JOIN [dbo].[tblAlarmTestTemplate] att ON pagatt.AlarmTestGuid = att.AlarmTestTemplateGuid
 		JOIN #AlarmTemplatesToDelete attd ON attd.[AlarmTemplateGuid] = att.AlarmTemplateGuid


		-- delete the children entries in tblPointTemplateTagAlarmStatus
		DELETE  pttas
		FROM [dbo].[tblPointTemplateTagAlarmStatus] pttas
		INNER JOIN [dbo].[tblAlarmTestTemplate] att ON pttas.AlarmTestTemplateGuid = att.AlarmTestTemplateGuid
		INNER JOIN #AlarmTemplatesToDelete attd ON attd.[AlarmTemplateGuid] = att.AlarmTemplateGuid

		-- delete the children entries in tblAlarmTestTemplate
		DELETE  att
		FROM [dbo].[tblAlarmTestTemplate] att
		INNER JOIN #AlarmTemplatesToDelete attd ON attd.[AlarmTemplateGuid] = att.AlarmTemplateGuid

		-- delete the actual rows in the alarm template table
		DELETE  att
		FROM [dbo].[tblAlarmTemplate] att
		INNER JOIN #AlarmTemplatesToDelete attd ON attd.[AlarmTemplateGuid] = att.AlarmTemplateGuid
	
	END
END

GO


