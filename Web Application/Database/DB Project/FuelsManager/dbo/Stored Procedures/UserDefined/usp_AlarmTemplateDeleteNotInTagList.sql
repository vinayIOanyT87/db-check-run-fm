CREATE PROCEDURE [dbo].[usp_AlarmTemplateDeleteNotInTagList]( @PointTemplateGuid uniqueIdentifier, @TemplateTagGuids GuidListType READONLY )
AS
BEGIN
    SET NOCOUNT ON

	SELECT [AlarmGuid]
	INTO #AlarmsToDelete
	FROM [dbo].[tblAlarm] at
	INNER JOIN dbo.tblPointTag pt ON pt.PointTagGuid = at.InputTagGuid
	INNER JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid
	INNER JOIN dbo.tblPointTemplate ptt ON ptt.PointTemplateGuid = p.PointTemplateGuid
	WHERE pt.PointTemplateTagGuid IS NOT NULL
	AND ptt.PointTemplateGuid = @PointTemplateGuid
	AND NOT EXISTS (SELECT 1 FROM @TemplateTagGuids atl WHERE atl.Guid = pt.PointTemplateTagGuid)

	IF ( SELECT COUNT(*) FROM #AlarmsToDelete ) > 0
	BEGIN

		-- delete the children entries in tblPointTagAlarmStatus
		DELETE  ptas
		FROM [dbo].[tblPointTagAlarmStatus] ptas
		INNER JOIN [dbo].[tblAlarmTest] at ON ptas.AlarmTestGuid = at.AlarmTestGuid
		INNER JOIN #AlarmsToDelete atd ON atd.[AlarmGuid] = at.AlarmGuid

		SELECT [LimitTagGuid] as PointTagGuid
		INTO #LimitTagsToDelete
		FROM [dbo].[tblAlarmTest] at
		INNER JOIN #AlarmsToDelete atd ON atd.AlarmGuid = at.AlarmGuid

		-- delete the point access group to point tag maps for limit tags
		DELETE pagtpt
		FROM [map].[tblPointAccessGroupToPointTag] pagtpt
		INNER JOIN #LimitTagsToDelete lttd ON lttd.PointTagGuid = pagtpt.TagGuid

		-- delete the point access group to point alarm test maps
		DELETE pagtpat
		FROM [map].[tblPointAccessGroupToPointAlarmTest] pagtpat
		INNER JOIN [dbo].[tblAlarmTest] at ON at.AlarmTestGuid = pagtpat.AlarmTestGuid
		INNER JOIN #AlarmsToDelete atd ON atd.[AlarmGuid] = at.AlarmGuid
		
		-- delete alarm tests
		DELETE  at
		FROM [dbo].[tblAlarmTest] at
		INNER JOIN #AlarmsToDelete atd ON atd.[AlarmGuid] = at.AlarmGuid

		-- delete limit tags
		DELETE pt
		FROM tblPointTag pt
		INNER JOIN #LimitTagsToDelete lttd ON lttd.PointTagGuid = pt.PointTagGuid  

		SELECT [AlarmStateTagGuid] as PointTagGuid
		INTO #StatusTagsToDelete
		FROM [dbo].[tblAlarm] a
		INNER JOIN #AlarmsToDelete atd ON atd.AlarmGuid = a.AlarmGuid

		-- delete the point access group to point tag maps for status tags
		DELETE pagtpt
		FROM [map].[tblPointAccessGroupToPointTag] pagtpt
		INNER JOIN #StatusTagsToDelete sttd ON sttd.PointTagGuid = pagtpt.TagGuid

		-- delete alarms to delete
		DELETE  a
		FROM [dbo].[tblAlarm] a
		INNER JOIN #AlarmsToDelete atd ON atd.[AlarmGuid] = a.AlarmGuid

		-- delete staatus tags to delete
		DELETE pt
		FROM tblPointTag pt
		INNER JOIN #StatusTagsToDelete sttd ON sttd.PointTagGuid = pt.PointTagGuid  


	END

	SELECT [AlarmTemplateGuid]
	INTO #AlarmTemplatesToDelete
	FROM [dbo].[tblAlarmTemplate] att
	INNER JOIN dbo.tblPointTemplateTag ptt ON ptt.PointTemplateTagGuid = att.InputTemplateTagGuid AND ptt.PointTemplateGuid = @PointTemplateGuid
	WHERE NOT EXISTS (SELECT 1 FROM @TemplateTagGuids atl WHERE atl.Guid = att.InputTemplateTagGuid )

	IF ( SELECT COUNT(*) FROM #AlarmTemplatesToDelete ) > 0
	BEGIN


		-- delete the point access group to alarm test maps
		DELETE pagatt
		FROM [map].[tblPointAccessGroupToAlarmTest] pagatt
		INNER JOIN [dbo].[tblAlarmTestTemplate] att ON pagatt.AlarmTestGuid = att.AlarmTestTemplateGuid
 		INNER JOIN #AlarmTemplatesToDelete at	ON at.[AlarmTemplateGuid] = att.AlarmTemplateGuid

		-- delete the children entries in tblPointTemplateTagAlarmStatus
		DELETE  pttas
		FROM [dbo].[tblPointTemplateTagAlarmStatus] pttas
		INNER JOIN [dbo].[tblAlarmTestTemplate] att ON pttas.AlarmTestTemplateGuid = att.AlarmTestTemplateGuid
		INNER JOIN #AlarmTemplatesToDelete at	ON at.[AlarmTemplateGuid] = att.AlarmTemplateGuid

		-- delete the children entries in tblAlarmTestTemplate
		DELETE  att
		FROM [dbo].[tblAlarmTestTemplate] att
		INNER JOIN #AlarmTemplatesToDelete at	ON at.[AlarmTemplateGuid] = att.AlarmTemplateGuid

		-- delete the actual rows in the alarm template table
		DELETE  att
		FROM [dbo].[tblAlarmTemplate] att
		INNER JOIN #AlarmTemplatesToDelete at	ON at.[AlarmTemplateGuid] = att.AlarmTemplateGuid

	END
END
GO


