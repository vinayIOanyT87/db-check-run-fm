--Adding of tblAlarm to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblAlarm' AS [TableName],
			'Point Tag - Alarm' AS [TypeID],
			'Point' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN p1.ID IS NOT NULL THEN p1.ID ELSE CASE WHEN p2.ID IS NOT NULL THEN p2.ID ELSE CASE WHEN pa1.ID IS NOT NULL THEN pa1.ID ELSE pa2.ID END END END + '' - '''
			+ ' + CASE WHEN pt.ID IS NULL THEN pta.ID ELSE pt.ID END + '' - '''
			+ ' + a.ID'
			+ ' FROM  [fmaudit].[tblAlarm] a'
			+ ' LEFT JOIN [dbo].[tblPointTag] pt ON pt.PointTagGuid = a.InputTagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTag] pta ON pta.PointTagGuid = a.InputTagGuid AND pta._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPoint] p1 ON p1.PointGuid = pt.PointGuid'
			+ ' LEFT JOIN [dbo].[tblPoint] p2 ON p2.PointGuid = pta.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa1 ON pa1.PointGuid = pt.PointGuid AND pa1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa2 ON pa2.PointGuid = pta.PointGuid AND pa2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN p1.SiteGuid IS NOT NULL THEN p1.SiteGuid ELSE CASE WHEN p2.SiteGuid IS NOT NULL THEN p2.SiteGuid ELSE CASE WHEN pa1.Siteguid IS NOT NULL THEN pa1.SiteGuid ELSE pa2.SiteGuid END END END'
			+ ' FROM  [fmaudit].[tblAlarm] a'
			+ ' LEFT JOIN [dbo].[tblPointTag] pt ON pt.PointTagGuid = a.InputTagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTag] pta ON pta.PointTagGuid = a.InputTagGuid AND pta._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPoint] p1 ON p1.PointGuid = pt.PointGuid'
			+ ' LEFT JOIN [dbo].[tblPoint] p2 ON p2.PointGuid = pta.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa1 ON pa1.PointGuid = pt.PointGuid AND pa1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa2 ON pa2.PointGuid = pta.PointGuid AND pa2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery])
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of dbo.tblAlarmPriorities to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblAlarmPriorities' AS [TableName],
			'Alarm Priorities' AS [TypeID],
			'' AS [ParentTypeID],
			'SELECT @ID = a.ID'
			+ ' FROM [fmAudit].[tblAlarmPriorities] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAlarmPriorities] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of tblAlarmTemplate to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblAlarmTemplate' AS [TableName],
			'Point Template Tag - Alarm' AS [TypeID],
			'Point Template' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pt1.ID IS NOT NULL THEN pt1.ID ELSE CASE WHEN pt2.ID IS NOT NULL THEN pt2.ID ELSE CASE WHEN pta1.ID IS NOT NULL THEN pta1.ID ELSE pta2.ID END END END + '' - '''
			+ ' + CASE WHEN ptt.ID IS NULL THEN ptta.ID ELSE ptt.ID END + '' - '''
			+ ' + a.ID'
			+ ' FROM  [fmaudit].[tblAlarmTemplate] a'
			+ ' LEFT JOIN [dbo].[tblPointTemplateTag] ptt ON ptt.PointTemplateTagGuid = a.InputTemplateTagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] ptta ON ptta.PointTemplateTagGuid = a.InputTemplateTagGuid AND ptta._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt1 ON pt1.PointTemplateGuid = ptt.PointTemplateGuid'
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt2 ON pt2.PointTemplateGuid = ptta.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta1 ON pta1.PointTemplateGuid = ptt.PointTemplateGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta2 ON pta2.PointTemplateGuid = ptta.PointTemplateGuid AND pta2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid =  CASE WHEN pt1.SiteGuid IS NOT NULL THEN pt1.SiteGuid ELSE CASE WHEN pt2.SiteGuid IS NOT NULL THEN pt2.SiteGuid ELSE CASE WHEN pta1.SiteGuid IS NOT NULL THEN pta1.SiteGuid ELSE pta2.SiteGuid END END END'
			+ ' FROM  [fmaudit].[tblAlarmTemplate] a'
			+ ' LEFT JOIN [dbo].[tblPointTemplateTag] ptt ON ptt.PointTemplateTagGuid = a.InputTemplateTagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] ptta ON ptta.PointTemplateTagGuid = a.InputTemplateTagGuid AND ptta._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt1 ON pt1.PointTemplateGuid = ptt.PointTemplateGuid'
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt2 ON pt2.PointTemplateGuid = ptta.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta1 ON pta1.PointTemplateGuid = ptt.PointTemplateGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta2 ON pta2.PointTemplateGuid = ptta.PointTemplateGuid AND pta2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery])
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of tblAlarmTest to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblAlarmTest' AS [TableName],
			'Alarm - Alarm Test' AS [TypeID],
			'Point' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN p1.ID IS NOT NULL THEN p1.ID ELSE CASE WHEN p2.ID IS NOT NULL THEN p2.ID ELSE CASE WHEN pa1.ID IS NOT NULL THEN pa1.ID ELSE pa2.ID END END END + '' - '''
			+ ' +  CASE WHEN pt1.ID IS NOT NULL THEN pt1.ID ELSE CASE WHEN pt2.ID IS NOT NULL THEN pt2.ID ELSE CASE WHEN pta1.ID IS NOT NULL THEN pta1.ID ELSE pta2.ID END END END + '' - '''
			+ ' + CASE WHEN al.ID IS NULL THEN ala.ID ELSE al.ID END + '' - '''
			+ ' + a.ID'
			+ ' FROM  [fmaudit].[tblAlarmTest] a'
			+ ' LEFT JOIN [dbo].[tblAlarm] al ON al.AlarmGuid = a.AlarmGuid'
			+ ' LEFT JOIN [fmaudit].[tblAlarm] ala ON ala.AlarmGuid = a.AlarmGuid AND ala._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTag] pt1 ON pt1.PointTagGuid = al.InputTagGuid'
			+ ' LEFT JOIN [dbo].[tblPointTag] pt2 ON pt2.PointTagGuid = ala.InputTagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTag] pta1 ON pta1.PointTagGuid = al.InputTagGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTag] pta2 ON pta2.PointTagGuid = ala.InputTagGuid AND pta2._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPoint] p1 ON p1.PointGuid = pt1.PointGuid'
			+ ' LEFT JOIN [dbo].[tblPoint] p2 ON p2.PointGuid = pt2.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa1 ON pa1.PointGuid = pta1.PointGuid AND pa1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa2 ON pa2.PointGuid = pta2.PointGuid AND pa2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			+ ' SELECT @SiteGuid = CASE WHEN p1.SiteGuid IS NOT NULL THEN p1.SiteGuid ELSE CASE WHEN p2.SiteGuid IS NOT NULL THEN p2.SiteGuid ELSE CASE WHEN pa1.SiteGuid IS NOT NULL THEN pa1.SiteGuid ELSE pa2.SiteGuid END END END'
			+ ' FROM  [fmaudit].[tblAlarmTest] a'
			+ ' LEFT JOIN [dbo].[tblAlarm] al ON al.AlarmGuid = a.AlarmGuid'
			+ ' LEFT JOIN [fmaudit].[tblAlarm] ala ON ala.AlarmGuid = a.AlarmGuid AND ala._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTag] pt1 ON pt1.PointTagGuid = al.InputTagGuid'
			+ ' LEFT JOIN [dbo].[tblPointTag] pt2 ON pt2.PointTagGuid = ala.InputTagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTag] pta1 ON pta1.PointTagGuid = al.InputTagGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTag] pta2 ON pta2.PointTagGuid = ala.InputTagGuid AND pta2._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPoint] p1 ON p1.PointGuid = pt1.PointGuid'
			+ ' LEFT JOIN [dbo].[tblPoint] p2 ON p2.PointGuid = pt2.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa1 ON pa1.PointGuid = pta1.PointGuid AND pa1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa2 ON pa2.PointGuid = pta2.PointGuid AND pa2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery])
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of tblAlarmTestTemplate to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblAlarmTestTemplate' AS [TableName],
			'Alarm - Alarm Test Template' AS [TypeID],
			'Point Template' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pt1.ID IS NOT NULL THEN pt1.ID ELSE CASE WHEN pt2.ID IS NOT NULL THEN pt2.ID ELSE CASE WHEN pta1.ID IS NOT NULL THEN pta1.ID ELSE pta2.ID END END END + '' - '''
			+ ' + CASE WHEN ptt1.ID IS NOT NULL THEN ptt1.ID ELSE CASE WHEN ptt2.ID IS NOT NULL THEN ptt2.ID ELSE CASE WHEN ptta1.ID IS NOT NULL THEN ptta1.ID ELSE ptta2.ID END END END + '' - '''
			+ ' + CASE WHEN at.ID IS NULL THEN ata.ID ELSE at.ID END + '' - '''
			+ ' + a.ID'
			+ ' FROM  [fmaudit].[tblAlarmTestTemplate] a'
			+ ' LEFT JOIN [dbo].[tblAlarmTemplate] at ON at.AlarmTemplateGuid = a.AlarmTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblAlarmTemplate] ata ON ata.AlarmTemplateGuid = a.AlarmTemplateGuid AND ata._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplateTag] ptt1 ON ptt1.PointTemplateTagGuid = at.InputTemplateTagGuid'
			+ ' LEFT JOIN [dbo].[tblPointTemplateTag] ptt2 ON ptt2.PointTemplateTagGuid = ata.InputTemplateTagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] ptta1 ON ptta1.PointTemplateTagGuid = at.InputTemplateTagGuid AND ptta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] ptta2 ON ptta2.PointTemplateTagGuid = ata.InputTemplateTagGuid AND ptta2._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt1 ON pt1.PointTemplateGuid = ptt1.PointTemplateGuid'
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt2 ON pt2.PointTemplateGuid = ptt2.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta1 ON pta1.PointTemplateGuid = ptta1.PointTemplateGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta2 ON pta2.PointTemplateGuid = ptta2.PointTemplateGuid AND pta2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			+ ' SELECT @SiteGuid = CASE WHEN pt1.SiteGuid IS NOT NULL THEN pt1.SiteGuid ELSE CASE WHEN pt2.SiteGuid IS NOT NULL THEN pt2.SiteGuid ELSE CASE WHEN pta1.SiteGuid IS NOT NULL THEN pta1.SiteGuid ELSE pta2.SiteGuid END END END'
			+ ' FROM  [fmaudit].[tblAlarmTestTemplate] a'
			+ ' LEFT JOIN [dbo].[tblAlarmTemplate] at ON at.AlarmTemplateGuid = a.AlarmTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblAlarmTemplate] ata ON ata.AlarmTemplateGuid = a.AlarmTemplateGuid AND ata._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplateTag] ptt1 ON ptt1.PointTemplateTagGuid = at.InputTemplateTagGuid'
			+ ' LEFT JOIN [dbo].[tblPointTemplateTag] ptt2 ON ptt2.PointTemplateTagGuid = ata.InputTemplateTagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] ptta1 ON ptta1.PointTemplateTagGuid = at.InputTemplateTagGuid AND ptta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] ptta2 ON ptta2.PointTemplateTagGuid = ata.InputTemplateTagGuid AND ptta2._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt1 ON pt1.PointTemplateGuid = ptt1.PointTemplateGuid'
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt2 ON pt2.PointTemplateGuid = ptt2.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta1 ON pta1.PointTemplateGuid = ptta1.PointTemplateGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta2 ON pta2.PointTemplateGuid = ptta2.PointTemplateGuid AND pta2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery])
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of tblAnimation to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblAnimation' AS [TableName],
			'Animation Manager' AS [TypeID],
			'' AS [ParentTypeID],
			'SELECT @ID = a.ID'
			+ ' FROM [fmAudit].[tblAnimation] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAnimation] a'
			+ ' WHERE a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map_tblAnimationToDrawing to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblAnimationToDrawing' AS [TableName],
			'Animation Mapping' AS [TypeID],
			'' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN a.ID IS NULL THEN aa.ID ELSE a.ID END + '' - '''
			+ ' + CASE WHEN d.ID IS NULL THEN da.ID ELSE d.ID END'
			+ ' FROM [fmAudit].[map_tblAnimationToDrawing] atd'
			+ ' LEFT JOIN [dbo].[tblAnimation] a ON a.AnimationGuid = atd.AnimationGuid'
			+ ' LEFT JOIN [fmaudit].[tblAnimation] aa ON aa.AnimationGuid = atd.AnimationGuid AND aa._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblDrawings] d ON d.DrawingGuid = atd.DrawingGuid'
			+ ' LEFT JOIN [fmaudit].[tblDrawings] da ON da.DrawingGuid = atd.DrawingGuid AND da._AuditEventType = ''D'''
			+ ' WHERE atd._AuditEventSequence = 1 AND atd._AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = _AuditSiteGuid FROM [fmaudit].[map_tblAnimationToDrawing] atd'
			+ ' WHERE atd._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of tblDrawings to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblDrawings' AS [TableName],
			'Drawings' AS [TypeID],
			'' AS [ParentTypeID],
			'SELECT @ID = a.ID'
			+ ' FROM [fmAudit].[tblDrawings] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblDrawings] a WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map.map_tblApplicationStringToPointCategory to tblAuditHandler
	
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblApplicationStringToPointCategory' AS [TableName],
			'Point - Category' AS [TypeID],
			'Points' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '''
			+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
			+ ' FROM  [fmaudit].[map_tblApplicationStringToPointCategory] a'
			+ ' LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = a.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
			+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToPointCategory] a'
							+ ' LEFT JOIN [dbo].[tblPoint] s ON s.PointGuid = a.PointGuid'
							+ ' LEFT JOIN [fmaudit].[tblPoint] sa ON sa.PointGuid = a.PointGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of map.map_tblEntityPointCategoryToSite to tblAuditHandler
	
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblEntityPointCategoryToSite' AS [TableName],
			'Site - Point Category' AS [TypeID],
			'Point Categories' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
			+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
			+ ' FROM  [fmaudit].[map_tblEntityPointCategoryToSite] a'
			+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
			+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
			+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityPointCategoryToSite] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

	--Adding of map_tblEntityPointTemplateToSite to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblEntityPointTemplateToSite' AS [TableName],
			'Site - Point Template' AS [TypeID],
			'Sites' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
			+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
			+ ' FROM  [fmaudit].[map_tblEntityPointTemplateToSite] a'
			+ ' LEFT JOIN [dbo].[tblPointTemplate] p ON p.PointTemplateGuid = a.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pa ON pa.PointTemplateGuid = a.PointTemplateGuid AND pa._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
			+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityPointTemplateToSite] a' 
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of map.map_tblEntityPointTemplateTypeToSite to tblAuditHandler
	
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblEntityPointTemplateTypeToSite' AS [TableName],
			'Site - Point Type' AS [TypeID],
			'Point Types' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
			+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
			+ ' FROM  [fmaudit].[map_tblEntityPointTemplateTypeToSite] a'
			+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
			+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
			+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityPointTemplateTypeToSite] a '
			+ 'WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding audit of map.tblQualificationEquipmentTagAndLicenseToStation
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblQualificationEquipmentTagAndLicenseToStation' as [TableName],
	'Stations - Tag and License' as [TypeID],
	'Stations' as [ParentTypeID],
	'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
	+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
	+ ' FROM  [fmaudit].[map_tblQualificationEquipmentTagAndLicenseToStation] a'
	+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
	+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
	+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
	+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
	+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' AS [IDQuery],
	NULL AS [SiteGuidQuery])
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);
	
--Adding of map.map_tblSiteToSite to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblSiteToSite' AS [TableName],
			'Site - Site' AS [TypeID],
			'Sites' AS [ParentTypeID],
			'SELECT @ID ='
			+ ' ISNULL(sp.ID,(SELECT sca.ID FROM [fmaudit].[map_tblSiteToSite] a'
			+ ' LEFT JOIN [fmaudit].[tblSites] sca ON sca.SiteGuid = a.ParentSiteGuid'
			+ ' WHERE a. _AuditGUID = @_AuditGUID AND sca._AuditEventType = ''D'') ) + '
			+ ''' - '''
			+ ' + CASE WHEN sc.ID IS NULL THEN sca.ID ELSE sc.ID END'
			+ ' FROM [fmaudit].[map_tblSiteToSite] a'
			+ ' LEFT JOIN [dbo].[tblSitesShadow] sc ON sc.SiteGuid = a.ChildSiteGuid'
			+ ' LEFT JOIN [fmaudit].[tblSites] sca  ON sca.SiteGuid = a.ChildSiteGuid'
			+ ' LEFT JOIN [dbo].[tblSitesShadow] sp ON sp.SiteGuid = a.ParentSiteGuid'
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [IDQuery],
			'Select @SiteGuid = ParentSiteGuid from [fmaudit].[map_tblSiteToSite] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of tblModule to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblModule' AS [TableName],
			'Module' AS [TypeID],
			'' AS [ParentTypeID],
			'SELECT @ID = a.ID'
			+ ' FROM [fmAudit].[tblModule] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblModule] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map_tblEntityModuleToSite to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblEntityModuleToSite' AS [TableName],
			'Site - Module' AS [TypeID],
			'Sites' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
			+ ' + CASE WHEN m.ID IS NULL THEN ma.ID ELSE m.ID END'
			+ ' FROM  [fmaudit].[map_tblEntityModuleToSite] a'
			+ ' LEFT JOIN [dbo].[tblModule] m ON m.ModuleGuid = a.ModuleGuid'
			+ ' LEFT JOIN [fmaudit].[tblModule] ma ON ma.ModuleGuid = a.ModuleGuid AND ma._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
			+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityModuleToSite] a' 
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of tblOpcUaServer to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblOpcUaServer' AS [TableName],
			'OPC UA Server' AS [TypeID],
			'' AS [ParentTypeID],
			'Select @ID = opc.ServerEndPoint  from [fmaudit].[tblOpcUAServer] opc'
			+ ' WHERE opc._AuditEventSequence = 1 AND opc._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'Select @SiteGuid = p.SiteGuid from  [fmaudit].[tblOpcUAServer] opc'
			+ ' INNER JOIN dbo.tblPointTag pt on pt.OpcUaServerGuid = opc.OpcUaServerGuid'
			+ ' INNER JOIN dbo.tblPoint p on p.PointGuid = pt.PointGuid'
			+ ' WHERE opc._AuditEventSequence = 1 AND opc._AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of tblPictures to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblPictures' AS [TableName],
			'Picture' AS [TypeID],
			'' AS [ParentTypeID],
			'SELECT @ID = a.ID'
			+ ' FROM [fmAudit].[tblPictures] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblPictures] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of tblPoint to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblPoint' AS [TableName],
			'Point' AS [TypeID],
			'' AS [ParentTypeID],
			'SELECT @ID = a.ID'
			+ ' FROM [fmAudit].[tblPoint] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblPoint] a WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of tblPointAccessGroup to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblPointAccessGroup' AS [TableName],
			'Site - Point Access Group' AS [TypeID],
			'Point Access Group' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
			+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
			+ ' FROM  [fmaudit].[tblPointAccessGroup] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] p ON p.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] pa ON pa.PointAccessGroupGuid = a.PointAccessGroupGuid AND pa._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
			+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''  
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
			+ ' FROM  [fmaudit].[tblPointAccessGroup] pag'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = pag.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' WHERE pag._AuditEventSequence = 1 AND pag. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map_tblPointAccessGroupToAlarmTest to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblPointAccessGroupToAlarmTest' AS [TableName],
			'Point Access Group - Alarm Test' AS [TypeID],
			'Point Access Group' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pag.ID IS NULL THEN paga.ID ELSE pag.ID END + '' - '''
			+ ' + CASE WHEN pt1.ID IS NOT NULL THEN pt1.ID ELSE CASE WHEN pt2.ID IS NOT NULL THEN pt2.ID ELSE CASE WHEN pta1.ID IS NOT NULL THEN pta1.ID ELSE pta2.ID END END END + '' - '''
			+ ' + CASE WHEN alt.ID IS NULL THEN alta.ID ELSE alt.ID END + '' - '''
			+ ' + CASE WHEN altt.ID IS NULL THEN altta.ID ELSE altt.ID END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToAlarmTest] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON pag.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblAlarmTestTemplate] altt ON altt.AlarmTestTemplateGuid = a.AlarmTestGuid'
			+ ' LEFT JOIN [fmaudit].[tblAlarmTestTemplate] altta ON altta.AlarmTestTemplateGuid = a.AlarmTestGuid AND altta._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblAlarmTemplate] alt ON alt.AlarmTemplateGuid = altt.AlarmTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblAlarmTemplate] alta ON alta.AlarmTemplateGuid = altta.AlarmTemplateGuid AND alta._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplateTag] ptt1 ON ptt1.PointTemplateTagGuid = alt.InputTemplateTagGuid'
			+ ' LEFT JOIN [dbo].[tblPointTemplateTag] ptt2 ON ptt2.PointTemplateTagGuid = alta.InputTemplateTagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] ptta1 ON ptta1.PointTemplateTagGuid = alt.InputTemplateTagGuid AND ptta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] ptta2 ON ptta2.PointTemplateTagGuid = alta.InputTemplateTagGuid AND ptta2._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt1 ON pt1.PointTemplateGuid = ptt1.PointTemplateGuid'
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt2 ON pt2.PointTemplateGuid = ptt2.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta1 ON pta1.PointTemplateGuid = ptta1.PointTemplateGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta2 ON pta2.PointTemplateGuid = ptta2.PointTemplateGuid AND pta2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToAlarmTest] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON pag.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map_tblPointAccessGroupToPointAlarmTest to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblPointAccessGroupToPointAlarmTest' AS [TableName],
			'Point Access Group - Point Alarm Test' AS [TypeID],
			'Point Access Group' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pag.ID IS NULL THEN paga.ID ELSE pag.ID END + '' - '''
			+ ' + CASE WHEN p1.ID IS NOT NULL THEN p1.ID ELSE CASE WHEN p2.ID IS NOT NULL THEN p2.ID ELSE CASE WHEN pa1.ID IS NOT NULL THEN pa1.ID ELSE pa2.ID END END END + '' - '''
			+ ' + CASE WHEN al.ID IS NULL THEN ala.ID ELSE al.ID END + '' - '''
			+ ' + CASE WHEN alt.ID IS NULL THEN alta.ID ELSE alt.ID END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToPointAlarmTest] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON pag.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblAlarmTest] alt ON alt.AlarmTestGuid = a.AlarmTestGuid'
			+ ' LEFT JOIN [fmaudit].[tblAlarmTest] alta ON alta.AlarmTestGuid = a.AlarmTestGuid AND alta._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblAlarm] al ON al.AlarmGuid = alt.AlarmGuid'
			+ ' LEFT JOIN [fmaudit].[tblAlarm] ala ON ala.AlarmGuid = alta.AlarmGuid AND alta._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTag] pt1 ON pt1.PointTagGuid = al.InputTagGuid'
			+ ' LEFT JOIN [dbo].[tblPointTag] pt2 ON pt2.PointTagGuid = ala.InputTagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTag] pta1 ON pta1.PointTagGuid = al.InputTagGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTag] pta2 ON pta2.PointGuid = ala.InputTagGuid AND pta2._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPoint] p1 ON p1.PointGuid = pt1.PointGuid'
			+ ' LEFT JOIN [dbo].[tblPoint] p2 ON p2.PointGuid = pt2.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa1 ON pa1.PointGuid = pta1.PointGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa2 ON pa2.PointGuid = pta2.PointGuid AND pta2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToPointAlarmTest] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON a.PointAccessGroupGuid = pag.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map_tblPointAccessGroupToPointTag to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblPointAccessGroupToPointTag' AS [TableName],
			'Point Access Group - Point Tag' AS [TypeID],
			'Point Access Group' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pag.ID IS NULL THEN paga.ID ELSE pag.ID END + '' - '''
			+ ' + CASE WHEN p1.ID IS NOT NULL THEN p1.ID ELSE CASE WHEN p2.ID IS NOT NULL THEN p2.ID ELSE CASE WHEN pa1.ID IS NOT NULL THEN pa1.ID ELSE pa2.ID END END END + '' - '''
			+ ' + CASE WHEN pt.ID IS NULL THEN pta.ID ELSE pt.ID END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToPointTag] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON pag.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTag] pt ON pt.PointTagGuid = a.TagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTag] pta ON pta.PointTagGuid = a.TagGuid AND pta._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPoint] p1 ON p1.PointGuid = pt.PointGuid'
			+ ' LEFT JOIN [dbo].[tblPoint] p2 ON p2.PointGuid = pta.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa1 ON pa1.PointGuid = pt.PointGuid AND pa1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa2 ON pa2.PointGuid = pta.PointGuid AND pa2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToPointTag] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON a.PointAccessGroupGuid = pag.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of map.tblPointAccessGroupToExposedPointSetting to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblPointAccessGroupToExposedPointSetting' AS [TableName],
			'Point Access Group - Point Exposed Point Setting' AS [TypeID],
			'Point Access Group' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pag.ID IS NULL THEN paga.ID ELSE pag.ID END + '' - '''
			+ ' + CASE WHEN pt.ID IS NULL THEN pta.ID + '' - '' + a.PropertyID ELSE pt.ID + '' - '' + a.PropertyID END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToExposedPointSetting] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON pag.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt ON pt.PointTemplateGuid = a.PointSettingGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta ON pta.PointTemplateGuid = a.PointSettingGuid AND pta._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToExposedPointSetting] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON a.PointAccessGroupGuid = pag.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map.tblPointAccessGroupToExposedPropertySetting to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblPointAccessGroupToExposedPropertySetting' AS [TableName],
			'Point Access Group - Point Exp. Property Setting' AS [TypeID],
			'Point Access Group' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pag.ID IS NULL THEN paga.ID ELSE pag.ID END + '' - '''
			+ ' + CASE WHEN pt1.ID IS NOT NULL THEN pt1.ID ELSE CASE WHEN pt2.ID IS NOT NULL THEN pt2.ID ELSE CASE WHEN pta1.ID IS NOT NULL THEN pta1.ID ELSE pta2.ID END END END + '' - '''
			+ ' + CASE WHEN ptp.ID IS NULL THEN ptpa.ID + '' - '' + a.PropertyID ELSE ptp.ID + '' - '' + a.PropertyID END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToExposedPropertySetting] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON pag.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplateProperty] ptp ON ptp.PointTemplatePropertyGuid = a.PointSettingGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateProperty] ptpa ON ptpa.PointTemplatePropertyGuid = a.PointSettingGuid AND ptpa._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt1 ON pt1.PointTemplateGuid = ptp.PointTemplateGuid'
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt2 ON pt2.PointTemplateGuid = ptpa.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] pta1 ON pta1.PointTemplateGuid = ptp.PointTemplateGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] pta2 ON pta2.PointTemplateGuid = ptpa.PointTemplateGuid AND pta2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToExposedPropertySetting] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON a.PointAccessGroupGuid = pag.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of map.tblPointAccessGroupToPoint to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblPointAccessGroupToPoint' AS [TableName],
			'Point Access Group - Point' AS [TypeID],
			'Point Access Group' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pag.ID IS NULL THEN paga.ID ELSE pag.ID END + '' - '''
			+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToPoint] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON pag.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = a.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToPoint] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON a.PointAccessGroupGuid = pag.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'   AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of map.tblPointAccessGroupToPointTemplate to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblPointAccessGroupToPointTemplate' AS [TableName],
			'Point Access Group - Point Template' AS [TypeID],
			'Point Access Group' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pag.ID IS NULL THEN paga.ID ELSE pag.ID END + '' - '''
			+ ' + CASE WHEN pt.ID IS NULL THEN pta.ID ELSE pt.ID END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToPointTemplate] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON pag.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt ON pt.PointTemplateGuid = a.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta ON pta.PointTemplateGuid = a.PointTemplateGuid AND pta._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToPointTemplate] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON a.PointAccessGroupGuid = pag.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map.tblPointAccessGroupToTag to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblPointAccessGroupToTag' AS [TableName],
			'Point Access Group - Tag' AS [TypeID],
			'Point Access Group' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pag.ID IS NULL THEN paga.ID ELSE pag.ID END + '' - '''
			+ ' + CASE WHEN pt1.ID IS NOT NULL THEN pt1.ID ELSE CASE WHEN pt2.ID IS NOT NULL THEN pt2.ID ELSE CASE WHEN pta1.ID IS NOT NULL THEN pta1.ID ELSE pta2.ID END END END + '' - '''
			+ ' + CASE WHEN ptt.ID IS NULL THEN ptta.ID ELSE ptt.ID END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToTag] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON pag.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplateTag] ptt ON ptt.PointTemplateTagGuid = a.TagGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] ptta ON ptta.PointTemplateTagGuid = a.TagGuid AND ptta._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt1 ON pt1.PointTemplateGuid = ptt.PointTemplateGuid'
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt2 ON pt2.PointTemplateGuid = ptta.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] pta1 ON pta1.PointTemplateGuid = ptt.PointTemplateGuid AND pta1._AuditEventType = ''D'''
			+ ' LEFT JOIN [fmaudit].[tblPointTemplateTag] pta2 ON pta2.PointTemplateGuid = ptta.PointTemplateGuid AND pta2._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToTag] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON a.PointAccessGroupGuid = pag.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map.tblPointAccessGroupToUserGroup to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblPointAccessGroupToUserGroup' AS [TableName],
			'Point Access Group - User Group' AS [TypeID],
			'Point Access Group' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pag.ID IS NULL THEN paga.ID ELSE pag.ID END + '' - '''
			+ ' + CASE WHEN ug.GroupID IS NULL THEN uga.GroupID ELSE ug.GroupID END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToUserGroup] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON pag.PointAccessGroupGuid = a.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblGroups] ug ON ug.GroupGuid = a.UserGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblGroups] uga ON uga.GroupGuid = a.UserGroupGuid AND uga._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
			+ ' FROM  [fmaudit].[map_tblPointAccessGroupToUserGroup] a'
			+ ' LEFT JOIN [dbo].[tblPointAccessGroup] pag ON a.PointAccessGroupGuid = pag.PointAccessGroupGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = a.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery])
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of dbo.tblPointProperty to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblPointProperty' AS [TableName],
			'Point - Property' AS [TypeID],
			'Point' AS [ParentTypeID],
			'Select @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.ID from [fmaudit].[tblPointProperty] a'
			+ ' LEFT JOIN dbo.tblPoint p on p.PointGuid = a.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'Select @SiteGuid = p.SiteGuid from [fmaudit].[tblPointProperty] a'
			+ ' INNER JOIN dbo.tblPoint p on p.PointGuid = a.PointGuid'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of tblPointTag to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblPointTag' AS [TableName],
			'Point - Tag' AS [TypeID],
			'Point' AS [ParentTypeID],
			'Select @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.ID from [fmaudit].[tblPointTag] a'
			+ ' LEFT JOIN dbo.tblPoint p on p.PointGuid = a.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'Select @SiteGuid = CASE WHEN p.SiteGuid IS NULL THEN pa.SiteGuid ELSE p.SiteGuid END from [fmaudit].[tblPointTag] a'
			+ ' LEFT JOIN dbo.tblPoint p on p.PointGuid = a.PointGuid'
			+ ' LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);



--Adding of tblPointTemplate to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblPointTemplate' AS [TableName],
			'PointTemplate' AS [TypeID],
			'' AS [ParentTypeID],
			'SELECT @ID = a.ID'
			+ ' FROM [fmAudit].[tblPointTemplate] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblPointTemplate] a WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of dbo.tblPointTemplateProperty to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblPointTemplateProperty' AS [TableName],
			'PointTemplate - Property' AS [TypeID],
			'PointTemplate' AS [ParentTypeID],
			'Select @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.ID from [fmaudit].[tblPointTemplateProperty] a'
			+ ' LEFT JOIN dbo.tblPointTemplate p on p.PointTemplateGuid = a.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pa ON pa.PointTemplateGuid = a.PointTemplateGuid AND pa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'Select @SiteGuid = p.SiteGuid from [fmaudit].[tblPointTemplateProperty] a'
			+ ' INNER JOIN dbo.tblPointTemplate p on p.PointTemplateGuid = a.PointTemplateGuid'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of tblPointTemplateTag to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblPointTemplateTag' AS [TableName],
			'PointTemplate - Tag' AS [TypeID],
			'PointTemplate' AS [ParentTypeID],
			'Select @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.ID from [fmaudit].[tblPointTemplateTag] a'
			+ ' LEFT JOIN dbo.tblPointTemplate p on p.PointTemplateGuid = a.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pa ON pa.PointTemplateGuid = a.PointTemplateGuid AND pa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'Select @SiteGuid = CASE WHEN p.SiteGuid IS NULL THEN pa.SiteGuid ELSE p.SiteGuid END from [fmaudit].[tblPointTemplateTag] a'
			+ ' LEFT JOIN dbo.tblPointTemplate p on p.PointTemplateGuid = a.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pa ON pa.PointTemplateGuid = a.PointTemplateGuid AND pa._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of tblTrend to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblTrend' AS [TableName],
			'Trend' AS [TypeID],
			'' AS [ParentTypeID],
			'SELECT @ID = a.ID'
			+ ' FROM [fmAudit].[tblTrend] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblTrend] a WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);





--Adding of tblTrendPenToDetailTrend

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblTrendPenToDetailTrend' AS [TableName],
			'Trend - Pen' AS [TypeID],
			'Trend' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN t.ID IS NULL THEN ta.ID ELSE t.ID END + '' - '' + '
			+ ' CASE'
			+ ' WHEN ptp1.ID IS NOT NULL THEN ptp1.ID'
			+ ' WHEN ptp2.ID IS NOT NULL THEN ptp2.ID'
			+ ' WHEN ptpa.ID IS NOT NULL THEN ptpa.ID'
			+ ' ELSE ''Unknown Point'''
			+ ' END + ''.'' + '
			+ ' CASE'
			+ ' WHEN ptt.ID IS NOT NULL THEN ptt.ID'
			+ ' WHEN ptta.ID IS NOT NULL THEN ptta.ID'
			+ ' ELSE ''Unknown Tag'''
			+ ' END'
			+ ' FROM fmaudit.map_tblTrendPenToDetailTrend tp'
			+ ' LEFT JOIN dbo.tblTrend t on t.TrendGuid = tp.TrendGuid'
			+ ' LEFT JOIN fmaudit.tblTrend ta ON ta.TrendGuid = tp.TrendGuid AND ta._AuditEventType = ''D'''
			+ ' LEFT JOIN dbo.tblPointTemplateTag ptt on ptt.PointTemplateTagGuid = tp.PointTemplateTagGuid'
			+ ' LEFT JOIN dbo.tblPointTemplate ptp1 on ptp1.PointTemplateGuid = ptt.PointTemplateGuid'
			+ ' LEFT JOIN fmaudit.tblPointTemplateTag ptta on ptta.PointTemplateTagGuid = tp.PointTemplateTagGuid AND ptta._AuditEventType = ''D'''
			+ ' LEFT JOIN dbo.tblPointTemplate ptp2 on ptp2.PointTemplateGuid = ptta.PointTemplateGuid'
			+ ' LEFT JOIN fmaudit.tblPointTemplate ptpa on ptpa.PointTemplateGuid = ptta.PointTemplateGuid AND ptpa._AuditEventType = ''D'''
			+ ' WHERE tp._AuditEventSequence = 1 AND tp._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = CASE'
			+ ' WHEN ptp1.SiteGuid IS NOT NULL THEN ptp1.SiteGuid'
			+ ' WHEN ptp2.SiteGuid IS NOT NULL THEN ptp2.SiteGuid'
			+ ' WHEN ptpa.SiteGuid IS NOT NULL THEN ptpa.SiteGuid'
			+ ' ELSE ''00000000-0000-0000-0000-000000000001'''
			+ ' END'
			+ ' FROM fmaudit.map_tblTrendPenToDetailTrend tp'
			+ ' LEFT JOIN dbo.tblPointTemplateTag ptt on ptt.PointTemplateTagGuid = tp.PointTemplateTagGuid'
			+ ' LEFT JOIN dbo.tblPointTemplate ptp1 on ptp1.PointTemplateGuid = ptt.PointTemplateGuid'
			+ ' LEFT JOIN fmaudit.tblPointTemplateTag ptta on ptta.PointTemplateTagGuid = tp.PointTemplateTagGuid AND ptta._AuditEventType = ''D'''
			+ ' LEFT JOIN dbo.tblPointTemplate ptp2 on ptp2.PointTemplateGuid = ptta.PointTemplateGuid'
			+ ' LEFT JOIN fmaudit.tblPointTemplate ptpa on ptpa.PointTemplateGuid = ptta.PointTemplateGuid AND ptpa._AuditEventType = ''D'''
			+ ' WHERE tp._AuditEventSequence = 1 AND tp._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


--Adding of tblTrendPenToPointTrend

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblTrendPenToPointTrend' AS [TableName],
			'Trend - Pen' AS [TypeID],
			'Trend' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN t.ID IS NULL THEN ta.ID ELSE t.ID END + '' - '' + '
			+ ' CASE'
			+ ' WHEN p1.ID IS NOT NULL THEN p1.ID'
			+ ' WHEN p2.ID IS NOT NULL THEN p2.ID'
			+ ' WHEN pa.ID IS NOT NULL THEN pa.ID'
			+ ' ELSE ''Unknown Point'''
			+ ' END + ''.'' + '
			+ ' CASE'
			+ ' WHEN pt.ID IS NOT NULL THEN pt.ID'
			+ ' WHEN pta.ID IS NOT NULL THEN pta.ID'
			+ ' ELSE ''Unknown Tag'''
			+ ' END'
			+ ' FROM fmaudit.map_tblTrendPenToPointTrend tp'
			+ ' LEFT JOIN dbo.tblTrend t on t.TrendGuid = tp.TrendGuid'
			+ ' LEFT JOIN fmaudit.tblTrend ta ON ta.TrendGuid = tp.TrendGuid AND ta._AuditEventType = ''D'''
			+ ' LEFT JOIN dbo.tblPointTag pt on pt.PointTagGuid = tp.PointTagGuid'
			+ ' LEFT JOIN dbo.tblPoint p1 on p1.PointGuid = pt.PointGuid'
			+ ' LEFT JOIN fmaudit.tblPointTag pta on pta.PointTagGuid = tp.PointTagGuid AND pta._AuditEventType = ''D'''
			+ ' LEFT JOIN dbo.tblPoint p2 on p2.PointGuid = pta.PointGuid'
			+ ' LEFT JOIN fmaudit.tblPoint pa on pa.PointGuid = pta.PointGuid AND pa._AuditEventType = ''D'''
			+ ' WHERE tp._AuditEventSequence = 1 AND tp._AuditGUID = @_AuditGUID'  AS [IDQuery],
			'SELECT @SiteGuid = CASE'
			+ ' WHEN p1.SiteGuid IS NOT NULL THEN p1.SiteGuid'
			+ ' WHEN p2.SiteGuid IS NOT NULL THEN p2.SiteGuid'
			+ ' WHEN pa.SiteGuid IS NOT NULL THEN pa.SiteGuid'
			+ ' ELSE ''00000000-0000-0000-0000-000000000001'''
			+ ' END'
			+ ' FROM fmaudit.map_tblTrendPenToPointTrend tp'
			+ ' LEFT JOIN dbo.tblPointTag pt on pt.PointTagGuid = tp.PointTagGuid'
			+ ' LEFT JOIN dbo.tblPoint p1 on p1.PointGuid = pt.PointGuid'
			+ ' LEFT JOIN fmaudit.tblPointTag pta on pta.PointTagGuid = tp.PointTagGuid AND pta._AuditEventType = ''D'''
			+ ' LEFT JOIN dbo.tblPoint p2 on p2.PointGuid = pta.PointGuid'
			+ ' LEFT JOIN fmaudit.tblPoint pa on pa.PointGuid = pta.PointGuid AND pa._AuditEventType = ''D'''
			+ ' WHERE tp._AuditEventSequence = 1 AND tp._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


-- Adding of tblAccessibilityConfigurationSettings

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblAccessibilityConfigurationSettings' AS [TableName],
			'Users - Accessibility Settings' AS [TypeID],
			'Users' AS [ParentTypeID],
			'SELECT @ID = COALESCE( u.UserID, ua.UserID) + '' - '' + COALESCE(la.DisplayName, ''Not Specified'') '
			+ ' FROM [fmaudit].tblAccessibilityConfigurationSettings a '
			+ ' LEFT JOIN [dbo].[tblUsers] u ON u.UserGuid = a.UserGuid '
			+ ' LEFT JOIN [fmaudit].[tblUsers] ua ON ua.UserGuid = a.UserGuid AND ua._AuditEventType = ''D'' '
			+ ' LEFT JOIN [lookup].[tblAccessibilities] la ON a.AccessibilityGuid = la.AccessibilityGuid '
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '  AS [IDQuery],
			'SELECT @SiteGuid = _AuditSiteGuid FROM [fmaudit].[tblAccessibilityConfigurationSettings] a WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID '  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

-- Adding of tblOperateScreenConfiguration

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblOperateScreenConfiguration' AS [TableName],
			'Operate Screen Configuration' AS [TypeID],
			'Users' AS [ParentTypeID],
			'SELECT @ID = COALESCE(u.UserID, ua.UserID, CONVERT(NVARCHAR(50), a.UserGuid)) + '' - '' + COALESCE(a.ClientIpAddress, '''')'
			+ ' FROM [fmaudit].[tblOperateScreenConfiguration] a'
			+ ' LEFT JOIN [dbo].[tblUsers] u ON u.UserGuid = a.UserGuid'
			+ ' LEFT JOIN [fmaudit].[tblUsers] ua ON ua.UserGuid = a.UserGuid AND ua._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = a.SiteGuid FROM [fmaudit].[tblOperateScreenConfiguration] a'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery])
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map.tblModuleToPointTemplate to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblModuleToPointTemplate' AS [TableName],
			'Module - Point Template' AS [TypeID],
			'Module' AS [ParentTypeID],
			'SELECT @ID = CASE WHEN pag.ID IS NULL THEN paga.ID ELSE pag.ID END + '' - '''
			+ ' + CASE WHEN pt.ID IS NULL THEN pta.ID ELSE pt.ID END'
			+ ' FROM  [fmaudit].[map_tblModuleToPointTemplate] a'
			+ ' LEFT JOIN [dbo].[tblmodule] pag ON pag.ModuleGuid = a.ModuleGuid'
			+ ' LEFT JOIN [fmaudit].[tblModule] paga ON paga.ModuleGuid = a.ModuleGuid AND paga._AuditEventType = ''D'''
			+ ' LEFT JOIN [dbo].[tblPointTemplate] pt ON pt.PointTemplateGuid = a.PointTemplateGuid'
			+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pta ON pta.PointTemplateGuid = a.PointTemplateGuid AND pta._AuditEventType = ''D'''
			+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
			+ 'Select @SiteGuid = p.SiteGuid from [fmaudit].[map_tblModuleToPointTemplate] a'
			+ ' INNER JOIN dbo.tblPointTemplate p on p.PointTemplateGuid = a.PointTemplateGuid'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of map.tblCompanyPersonnelAssignedToCompany to tblAuditHandler

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'map_tblCompanyPersonnelAssignedToCompany' AS [TableName],
			'Personnel - Company' AS [TypeID],
			'Personnel' AS [ParentTypeID],
			' SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '' + COALESCE( l.ID, '''') '
			+ ' FROM  [fmaudit].[map_tblCompanyPersonnelAssignedToCompany] a '
			+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid '
			+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'' '
			+ ' LEFT JOIN [dbo].[tblcompanies] l ON l.CompanyGuid = a.CompanyGuid WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' AS [IDQuery],
			+ 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblCompanyPersonnelAssignedToCompany] a'
			+ ' LEFT JOIN [dbo].[tblPersonnel] s ON s.PersonnelGuid = a.PersonnelGuid'
			+ ' LEFT JOIN [fmaudit].[tblPersonnel] sa ON sa.PersonnelGuid = a.PersonnelGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


/*************************************************************************************
--AcculoadOPC Database Tables Added to Fuelsmanagerdb.dbo.tblAuditHandler 
AcculoadOPC_tblPorts
AcculoadOPC_tblCardReaders
AcculoadOPC_tblArms
AcculoadOPC_tblAcculoads
*************************************************************************************/

--Adding of AcculoadOPC.dbo.tblArms to fuelsmanagerdb.dbo.tblAuditHandler --
BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'AcculoadOPC_tblArms' AS [TableName],
			'AculoadOPC - Arms' AS [TypeID],
			'Arms' AS [ParentTypeID],
			'Select @ID = a.AcculoadIndex + a.Number from [fmaudit].[AcculoadOPC_tblArms] a'
			+ ' LEFT JOIN AcculoadOPC.dbo.tblArms p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END


--Adding of AcculoadOPC.dbo.tblAcculoads  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'AcculoadOPC_tblAcculoads' AS [TableName],
			'AculoadOPC - Acculoads' AS [TypeID],
			'Acculoads' AS [ParentTypeID],
			'Select @ID = a.ID from [fmaudit].[AcculoadOPC_tblAcculoads] a'
			+ ' LEFT JOIN AcculoadOPC.dbo.tblAcculoads p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  -- VERIFY WITH CHRIS, IF SITE ADMIN
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END



--Adding of AcculoadOPC.dbo.tblCardReaders  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'AcculoadOPC_tblCardReaders' AS [TableName],
			'AculoadOPC - CardReaders' AS [TypeID],
			'CardReaders' AS [ParentTypeID],
			'Select @ID = a.ID from [fmaudit].[AcculoadOPC_tblCardReaders] a'
			+ ' LEFT JOIN AcculoadOPC.dbo.tblCardReaders p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  -- VERIFY WITH CHRIS, IF SITE ADMIN
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END




--Adding of AcculoadOPC.dbo.tblPorts  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'AcculoadOPC_tblPorts' AS [TableName],
			'AculoadOPC - Ports' AS [TypeID],
			'Ports' AS [ParentTypeID],
			'Select @ID =  a.ID from fuelsmanagerdb.fmaudit.AcculoadOPC_tblPorts a'
			+ ' LEFT JOIN AcculoadOPC.dbo.tblPorts p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  -- VERIFY WITH CHRIS, IF SITE ADMIN
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);
END




/***************************************************************************
OptomuxOPC Database Tables Added to Fuelsmanagerdb.dbo.tblAuditHandler 
OptomuxOPC_tblPorts
OptomuxOPCtblPorts

********************************************************************************/

--Adding of OptomuxOPC.dbo.tblPorts  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'OptomuxOPC_tblPorts' AS [TableName],
			'OptomuxOPC - tblPorts' AS [TypeID],
			'Ports' AS [ParentTypeID],
			'Select @ID = a.ID from [fuelsmanagerdb].[fmaudit].[OptomuxOPC_tblPorts] a'
			+ ' LEFT JOIN OptomuxOPC.dbo.tblPorts p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  -- VERIFY WITH CHRIS, IF SITE ADMIN
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END


--Adding of OptomuxOPC.dbo.tblOptomuxControllers  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'OptomuxOPC_tblOptomuxControllers' AS [TableName],
			'OptomuxOPC - tblOptomuxControllers' AS [TypeID],
			'Controllers' AS [ParentTypeID],
			'Select @ID = a.ID from [fuelsmanagerdb].[fmaudit].[OptomuxOPC_tblOptomuxControllers] a'
			+ ' LEFT JOIN OptomuxOPC.dbo.tblOptomuxControllers p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  -- VERIFY WITH CHRIS, IF SITE ADMIN
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END

/***************************************************************************
OsdpOPC Database Tables Added to Fuelsmanagerdb.dbo.tblAuditHandler 
OsdpOPC_tblPorts
OsdpOPCtblPorts

********************************************************************************/

--Adding of OsdpOPC.dbo.tblPorts  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'OsdpOPC_tblPorts' AS [TableName],
			'OsdpOPC - tblPorts' AS [TypeID],
			'Ports' AS [ParentTypeID],
			'Select @ID = a.ID from [fuelsmanagerdb].[fmaudit].[OsdpOPC_tblPorts] a'
			+ ' LEFT JOIN OsdpOPC.dbo.tblPorts p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  -- VERIFY WITH CHRIS, IF SITE ADMIN
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END


--Adding of OsdpOPC.dbo.tblOsdpControllers  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'OsdpOPC_tblOsdpControllers' AS [TableName],
			'OsdpOPC - tblOsdpControllers' AS [TypeID],
			'Controllers' AS [ParentTypeID],
			'Select @ID = a.ID from [fuelsmanagerdb].[fmaudit].[OsdpOPC_tblOsdpControllers] a'
			+ ' LEFT JOIN OsdpOPC.dbo.tblOsdpControllers p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  -- VERIFY WITH CHRIS, IF SITE ADMIN
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END



/***************************************************************************
ScullyOPC Database Tables Added to Fuelsmanagerdb.dbo.tblAuditHandler 
ScullyOPC_tblPorts
ScullyOPC_tblScullys

********************************************************************************/

--Adding of ScullyOPC.dbo.tblPorts  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'ScullyOPC_tblPorts' AS [TableName],
			'ScullyOPC - tblPorts' AS [TypeID],
			'Ports' AS [ParentTypeID],
			'Select @ID = a.ID from [fuelsmanagerdb].[fmaudit].[ScullyOPC_tblPorts] a'
			+ ' LEFT JOIN ScullyOPC.dbo.tblPorts p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  -- VERIFY WITH CHRIS, IF SITE ADMIN
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END


--Adding of ScullyOPC.dbo.tblScullys  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'ScullyOPC_tblScullys' AS [TableName],
			'ScullyOPC - tblScullys' AS [TypeID],
			'Scullys' AS [ParentTypeID],
			'Select @ID = a.ID from [fuelsmanagerdb].[fmaudit].[ScullyOPC_tblScullys] a'
			+ ' LEFT JOIN ScullyOPC.dbo.tblScullys p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  -- VERIFY WITH CHRIS, IF SITE ADMIN
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END



/***************************************************************************
WeightScaleOPC Database Tables Added to Fuelsmanagerdb.dbo.tblAuditHandler 
WeightScaleOPC_tblWeightScaleTypeLookup
WeightScaleOPC_tblWeightScaleStopBitsLookup
WeightScaleOPC_tblWeightScales
WeightScaleOPC_tblWeightScaleParityLookup
WeightScaleOPC_tblWeightScaleDataBitsLookup
WeightScaleOPC_tblWeightScaleBaudLookup
WeightScaleOPC_tblPorts
********************************************************************************/

--Adding of WeightScaleOPC.dbo.tblPorts  to fuelsmanagerdb.dbo.tblAuditHandler --


BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'WeightScaleOPC_tblPorts' AS [TableName],
			'WeightScaleOPC - tblPorts' AS [TypeID],
			'Ports' AS [ParentTypeID],
			'Select @ID = a.ID from [fuelsmanagerdb].[fmaudit].[WeightScaleOPC_tblPorts] a'
			+ ' LEFT JOIN WeightScaleOPC.dbo.tblPorts p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  -- VERIFY WITH CHRIS, IF SITE ADMIN
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END



--Adding of WeightScaleOPC.dbo.tblWeightScaleBaudLookup  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'WeightScaleOPC_tblWeightScaleBaudLookup' AS [TableName],
			'WeightScaleOPC - tblWeightScaleBaudLookup' AS [TypeID],
			'BaudLookup' AS [ParentTypeID],
			'Select @ID = a.[BaudDescription] from [fuelsmanagerdb].[fmaudit].[WeightScaleOPC_tblWeightScaleBaudLookup] a'  
			+ ' LEFT JOIN WeightScaleOPC.dbo.tblWeightScaleBaudLookup p on p.[BaudIndex] = a.[BaudIndex]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END



--Adding of WeightScaleOPC.dbo.tblWeightScaleDataBitsLookup  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'WeightScaleOPC_tblWeightScaleDataBitsLookup' AS [TableName],
			'WeightScaleOPC - tblWeightScaleDataBitsLookup' AS [TypeID],
			'DataBitsLookup' AS [ParentTypeID],
			'Select @ID = a.[DataBitsDescription] from [fuelsmanagerdb].[fmaudit].[WeightScaleOPC_tblWeightScaleDataBitsLookup] a'  
			+ ' LEFT JOIN WeightScaleOPC.dbo.tblWeightScaleDataBitsLookup p on p.[DataBitsIndex] = a.[DataBitsIndex]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END


--Adding of WeightScaleOPC.dbo.tblWeightScaleParityLookup  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'WeightScaleOPC_tblWeightScaleParityLookup' AS [TableName],
			'WeightScaleOPC - tblWeightScaleParityLookup' AS [TypeID],
			'ParityLookup' AS [ParentTypeID],
			'Select @ID = a.[ParityDescription] from [fuelsmanagerdb].[fmaudit].[WeightScaleOPC_tblWeightScaleParityLookup] a'  
			+ ' LEFT JOIN WeightScaleOPC.dbo.tblWeightScaleParityLookup p on p.[ParityIndex] = a.[ParityIndex]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END



--Adding of WeightScaleOPC.dbo.tblWeightScaleStopBitsLookup  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'WeightScaleOPC_tblWeightScaleStopBitsLookup' AS [TableName],
			'WeightScaleOPC - tblWeightScaleStopBitsLookup' AS [TypeID],
			'StopBitsLookup' AS [ParentTypeID],
			'Select @ID = a.[StopBitsDescription] from [fuelsmanagerdb].[fmaudit].[WeightScaleOPC_tblWeightScaleStopBitsLookup] a'  
			+ ' LEFT JOIN WeightScaleOPC.dbo.tblWeightScaleStopBitsLookup p on p.[StopBitsIndex] = a.[StopBitsIndex]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END



--Adding of WeightScaleOPC.dbo.tblWeightScaleTypeLookup  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'WeightScaleOPC_tblWeightScaleTypeLookup' AS [TableName],
			'WeightScaleOPC - tblWeightScaleTypeLookup' AS [TypeID],
			'TypeLookup' AS [ParentTypeID],
			'Select @ID = a.[TypeDescription] from [fuelsmanagerdb].[fmaudit].[WeightScaleOPC_tblWeightScaleTypeLookup] a'  
			+ ' LEFT JOIN WeightScaleOPC.dbo.tblWeightScaleTypeLookup p on p.[TypeIndex] = a.[TypeIndex]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END



--Adding of WeightScaleOPC.dbo.tblWeightScales  to fuelsmanagerdb.dbo.tblAuditHandler --

BEGIN
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'WeightScaleOPC_tblWeightScales' AS [TableName],
			'WeightScaleOPC - tblWeightScales' AS [TypeID],
			'Scales' AS [ParentTypeID],
			'Select @ID = a.[ID] from [fuelsmanagerdb].[fmaudit].[WeightScaleOPC_tblWeightScales] a'  
			+ ' LEFT JOIN WeightScaleOPC.dbo.tblWeightScales p on p.[Index] = a.[Index]'
			+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001'''  AS [SiteGuidQuery])  
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], 
	[SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery]
	, Source.[SiteGuidQuery]
	);
END

-- FM-3590 - Add audit handler for tblUserDataFieldUser and tblUserDataFieldIATA
--				Add tblUserDataListValueUser and tblUserDataListValueIATA as well
MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblUserDataFieldUser' AS [TableName],
			'User Data' AS [TypeID],
			'User Data' AS [ParentTypeID],
			'SELECT @ID =  ''Users '''
						+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
						+ ' FROM [fmAudit].[tblUserDataFieldUser] a'
						+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN udfu.SiteGuid IS NULL THEN udfua.SiteGuid ELSE udfu.SiteGuid END'
							+ ' FROM  [fmaudit].[tblUserDataFieldUser] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldUser] udfu ON a.UserDataFieldUserGuid = udfu.UserDataFieldUserGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldUser] udfua ON udfua.UserDataFieldUserGuid = a.UserDataFieldUserGuid AND udfua._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblUserDataFieldIATA' AS [TableName],
			'User Data' AS [TypeID],
			'User Data' AS [ParentTypeID],
			'SELECT @ID = ''IATA '''
						+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
						+ ' FROM [fmAudit].[tblUserDataFieldIATA] a'
						+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
			'SELECT @SiteGuid = CASE WHEN udfi.SiteGuid IS NULL THEN udfia.SiteGuid ELSE udfi.SiteGuid END'
						+ ' FROM  [fmaudit].[tblUserDataFieldIATA] a'
						+ ' LEFT JOIN [dbo].[tblUserDataFieldIATA] udfi ON a.UserDataFieldIATAGuid = udfi.UserDataFieldIATAGuid'
						+ ' LEFT JOIN [fmaudit].[tblUserDataFieldIATA] udfia ON udfia.UserDataFieldIATAGuid = a.UserDataFieldIATAGuid AND udfia._AuditEventType = ''D'''
						+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblUserDataListValueUser' AS [TableName],
			'User Data' AS [TypeID],
			'User Data' AS [ParentTypeID],
			'SELECT @ID = ''Users '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueUser] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldUser] ud ON ud.UserDataFieldUserGuid = a.UserDataFieldUserGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldUser] uda ON uda.UserDataFieldUserGuid = a.UserDataFieldUserGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' as [IDQuery],
			'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END'
							+ ' FROM  [fmaudit].[tblUserDataListValueUser] a LEFT JOIN [dbo].[tblUserDataFieldUser] s ON s.UserDataFieldUserGuid = a.UserDataFieldUserGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldUser] sa ON sa.UserDataFieldUserGuid = a.UserDataFieldUserGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

MERGE dbo.tblAuditHandler AS Target
USING
( SELECT 'tblUserDataListValueIATA' AS [TableName],
			'User Data' AS [TypeID],
			'User Data' AS [ParentTypeID],
			'SELECT @ID = ''IATA '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueIATA] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldIATA] ud ON ud.UserDataFieldIATAGuid = a.UserDataFieldIATAGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldIATA] uda ON uda.UserDataFieldIATAGuid = a.UserDataFieldIATAGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' as [IDQuery],
			'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END'
							+ ' FROM  [fmaudit].[tblUserDataListValueIATA] a LEFT JOIN [dbo].[tblUserDataFieldIATA] s ON s.UserDataFieldIATAGuid = a.UserDataFieldIATAGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldIATA] sa ON sa.UserDataFieldIATAGuid = a.UserDataFieldIATAGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' AS [SiteGuidQuery]) 
AS Source
ON (Target.[TableName] = Source.[TableName])
WHEN MATCHED THEN
	UPDATE SET target.[TypeID] = source.[TypeID],
					target.[ParentTypeID]		= source.[ParentTypeID],
					target.[IDQuery]	= source.[IDQuery],
					target.[SiteGuidQuery] = source.[SiteGuidQuery]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
	VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

--Adding of dbo.tblMovementHistory to fuelsmanagerdb.dbo.tblAuditHandler --
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblMovementHistory' AS [TableName],
				'Movement History' AS [TypeID],
				'' AS [ParentTypeID],
				'SELECT @ID = '
					+ ' CASE WHEN a.Node IS NOT NULL AND a.Node <> '''''
					+ ' THEN CONVERT(NVARCHAR(10), a.InitiationCount) + '' - '' + s.ID + '' > '' + a.Name + '' > '' + a.Node'
					+ ' ELSE CONVERT(NVARCHAR(10), a.InitiationCount) + '' - '' + s.ID + '' > '' + a.Name'
					+ ' END'
					+ ' FROM [fmAudit].tblMovementHistory a INNER JOIN [dbo].tblSites s ON a.SiteGuid = s.SiteGuid'
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' AS [IDQuery],
				'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblMovementHistory] a' 
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery])
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID]				= source.[TypeID],
						target.[ParentTypeID]	= source.[ParentTypeID],
						target.[IDQuery]		= source.[IDQuery],
						target.[SiteGuidQuery]	= source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);
END

--Adding of dbo.tblMovementSummary to fuelsmanagerdb.dbo.tblAuditHandler --
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblMovementSummary' AS [TableName],
				'Movement Summary' AS [TypeID],
				'' AS [ParentTypeID],
				'SELECT @ID = '
					+ ' CASE WHEN a.ID IS NULL'
					+ ' THEN ''Unknown ID'''
					+ ' ELSE a.ID'
					+ ' END'
					+ ' FROM [fmAudit].tblMovementSummary a INNER JOIN [dbo].tblSites s ON a.SiteGuid = s.SiteGuid'
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
				'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblMovementSummary] a' 
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery])
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID]				= source.[TypeID],
						target.[ParentTypeID]	= source.[ParentTypeID],
						target.[IDQuery]		= source.[IDQuery],
						target.[SiteGuidQuery]	= source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);
END


--Adding of dbo.tblTransactionAliasFieldPlacementInformation to fuelsmanagerdb.dbo.tblAuditHandler --
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblTransactionAliasFieldPlacementInformation' AS [TableName],
				'Transaction Alias - Fields Placement' AS [TypeID],
				'Transaction Alias' AS [ParentTypeID],
				'SELECT @ID = '
					+ ' CASE WHEN ta.AliasName IS NULL'
					+ ' THEN ''Unknown ID'''
					+ ' ELSE ta.AliasName'
					+ ' END'
					+ ' FROM [fmAudit].[tblTransactionAliasFieldPlacementInformation] a'
					+ ' INNER JOIN [dbo].tblTransactionAliases ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid'
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
				'SELECT @SiteGuid = ta.SiteGuid FROM [fmaudit].[tblTransactionAliasFieldPlacementInformation] a' 
					+ ' INNER JOIN [dbo].tblTransactionAliases ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid'
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery])
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID]				= source.[TypeID],
						target.[ParentTypeID]	= source.[ParentTypeID],
						target.[IDQuery]		= source.[IDQuery],
						target.[SiteGuidQuery]	= source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);
END

--Adding of dbo.tblMobileDispatchSiteIntegrationInfo to fuelsmanagerdb.dbo.tblAuditHandler --
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblMobileDispatchSiteIntegrationInfo' AS [TableName],
				'Mobile Dispatch - Site Integration' AS [TypeID],
				'' AS [ParentTypeID],
				'SELECT @ID = s.ID'
					+ ' FROM [fmAudit].[tblMobileDispatchSiteIntegrationInfo] a INNER JOIN [dbo].tblSites s ON a.SiteGuid = s.SiteGuid'
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
				'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblMobileDispatchSiteIntegrationInfo] a' 
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery])
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID]				= source.[TypeID],
						target.[ParentTypeID]	= source.[ParentTypeID],
						target.[IDQuery]		= source.[IDQuery],
						target.[SiteGuidQuery]	= source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);
END


--Adding of dbo.tblPoint to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPoint' AS [TableName],
				'Point' AS [TypeID],
				'' AS [ParentTypeID],
				'SELECT @ID = a.ID'
				+ ' FROM [fmAudit].[tblPoint] a'
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblPoint] a WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID] = source.[TypeID],
						target.[ParentTypeID]		= source.[ParentTypeID],
						target.[IDQuery]	= source.[IDQuery],
						target.[SiteGuidQuery] = source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

END
                
--Adding of dbo.tblPointAccessGroup to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPointAccessGroup' AS [TableName],
				'Site - Point Access Group' AS [TypeID],
				'Point Access Group' AS [ParentTypeID],
				'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
				+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
				+ ' FROM  [fmaudit].[tblPointAccessGroup] a'
				+ ' LEFT JOIN [dbo].[tblPointAccessGroup] p ON p.PointAccessGroupGuid = a.PointAccessGroupGuid'
				+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] pa ON pa.PointAccessGroupGuid = a.PointAccessGroupGuid AND pa._AuditEventType = ''D'''
				+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
				+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''  
				+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID' AS [IDQuery],
				'SELECT @SiteGuid = CASE WHEN pag.SiteGuid IS NULL THEN paga.SiteGuid ELSE pag.SiteGuid END'
				+ ' FROM  [fmaudit].[tblPointAccessGroup] pag'
				+ ' LEFT JOIN [fmaudit].[tblPointAccessGroup] paga ON paga.PointAccessGroupGuid = pag.PointAccessGroupGuid AND paga._AuditEventType = ''D'''
				+ ' WHERE pag._AuditEventSequence = 1 AND pag. _AuditGUID = @_AuditGUID' AS [SiteGuidQuery]) 
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID] = source.[TypeID],
						target.[ParentTypeID]		= source.[ParentTypeID],
						target.[IDQuery]	= source.[IDQuery],
						target.[SiteGuidQuery] = source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

END

--Adding of dbo.tblPointGroup to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPointGroup' AS [TableName],
				'Point Group' AS [TypeID],
				'' AS [ParentTypeID],
				'SELECT @ID = a.ID'
					+ ' FROM [fmAudit].[tblPointGroup] a INNER JOIN [dbo].tblSites s ON a.SiteGuid = s.SiteGuid'
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
				'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblPointGroup] a' 
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery])
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID]				= source.[TypeID],
						target.[ParentTypeID]	= source.[ParentTypeID],
						target.[IDQuery]		= source.[IDQuery],
						target.[SiteGuidQuery]	= source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);
END
                --"Point Group Columns",

--Adding of dbo.tblPointGroupColumns to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPointGroupColumns' AS [TableName],
				'Columns' AS [TypeID],
				'Point Group' AS [ParentTypeID],
				'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.ColumnsDefinition'
					+ ' FROM [fmAudit].[tblPointGroupColumns] a INNER JOIN [dbo].tblSites s ON a.SiteGuid = s.SiteGuid'
					+ ' LEFT JOIN dbo.tblPointGroup p on p.PointGroupGuid = a.PointGroupGuid'
					+ ' JOIN [fmAudit].[tblPointGroup] pa ON pa.PointGroupGuid = a.PointGroupGuid  AND pa._AuditEventType = ''D'''
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
				'SELECT @SiteGuid = p.SiteGuid FROM [fmaudit].[tblPointGroupColumns] a' 
					+ ' INNER JOIN dbo.tblPointGroup p on p.PointGroupGuid = a.PointGroupGuid'
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery])
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID]				= source.[TypeID],
						target.[ParentTypeID]	= source.[ParentTypeID],
						target.[IDQuery]		= source.[IDQuery],
						target.[SiteGuidQuery]	= source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);
END

--Adding of dbo.tblPointGroupRows to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPointGroupRows' AS [TableName],
				'Rows' AS [TypeID],
				'Point Group' AS [ParentTypeID],
				'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.RowsDefinition'
					+ ' FROM [fmAudit].[tblPointGroupRows] a INNER JOIN [dbo].tblSites s ON a.SiteGuid = s.SiteGuid'
					+ ' LEFT JOIN dbo.tblPointGroup p on p.PointGroupGuid = a.PointGroupGuid'
					+ ' JOIN [fmAudit].[tblPointGroup] pa ON pa.PointGroupGuid = a.PointGroupGuid  AND pa._AuditEventType = ''D'''
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
				'SELECT @SiteGuid = p.SiteGuid FROM [fmaudit].[tblPointGroupRows] a' 
					+ ' INNER JOIN dbo.tblPointGroup p on p.PointGroupGuid = a.PointGroupGuid'
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery])
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID]				= source.[TypeID],
						target.[ParentTypeID]	= source.[ParentTypeID],
						target.[IDQuery]		= source.[IDQuery],
						target.[SiteGuidQuery]	= source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);
END

--Adding of dbo.tblPointGroupSchedule to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPointGroupSchedule' AS [TableName],
				'Schedule' AS [TypeID],
				'Point Group' AS [ParentTypeID],
				'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.CronSchedule'
					+ ' FROM [fmAudit].[tblPointGroupSchedule] a INNER JOIN [dbo].tblSites s ON a.SiteGuid = s.SiteGuid'
					+ ' LEFT JOIN dbo.tblPointGroup p on p.PointGroupGuid = a.PointGroupGuid'
					+ ' JOIN [fmAudit].[tblPointGroup] pa ON pa.PointGroupGuid = a.PointGroupGuid  AND pa._AuditEventType = ''D'''
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [IDQuery],
				'SELECT @SiteGuid = p.SiteGuid FROM [fmaudit].[tblPointGroupSchedule] a' 
					+ ' INNER JOIN dbo.tblPointGroup p on p.PointGroupGuid = a.PointGroupGuid'
					+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' AS [SiteGuidQuery])
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID]				= source.[TypeID],
						target.[ParentTypeID]	= source.[ParentTypeID],
						target.[IDQuery]		= source.[IDQuery],
						target.[SiteGuidQuery]	= source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);
END

--Adding of dbo.tblPointProperty to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPointProperty' AS [TableName],
				'Point - Property' AS [TypeID],
				'Point' AS [ParentTypeID],
				'Select @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.ID from [fmaudit].[tblPointProperty] a'
				+ ' LEFT JOIN dbo.tblPoint p on p.PointGuid = a.PointGuid'
				+ ' LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'''
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = p.SiteGuid from [fmaudit].[tblPointProperty] a'
				+ ' INNER JOIN dbo.tblPoint p on p.PointGuid = a.PointGuid'
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID] = source.[TypeID],
						target.[ParentTypeID]		= source.[ParentTypeID],
						target.[IDQuery]	= source.[IDQuery],
						target.[SiteGuidQuery] = source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

END

--Adding of dbo.tblPointTag to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPointTag' AS [TableName],
				'Point - Tag' AS [TypeID],
				'Point' AS [ParentTypeID],
				'Select @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.ID from [fmaudit].[tblPointTag] a'
				+ ' LEFT JOIN dbo.tblPoint p on p.PointGuid = a.PointGuid'
				+ ' LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'''
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = CASE WHEN p.SiteGuid IS NULL THEN pa.SiteGuid ELSE p.SiteGuid END from [fmaudit].[tblPointTag] a'
				+ ' LEFT JOIN dbo.tblPoint p on p.PointGuid = a.PointGuid'
				+ ' LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'''
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID] = source.[TypeID],
						target.[ParentTypeID]		= source.[ParentTypeID],
						target.[IDQuery]	= source.[IDQuery],
						target.[SiteGuidQuery] = source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);


END

--Adding of dbo.tblPointTemplate to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPointTemplate' AS [TableName],
				'PointTemplate' AS [TypeID],
				'' AS [ParentTypeID],
				'SELECT @ID = a.ID'
				+ ' FROM [fmAudit].[tblPoint] a'
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblPoint] a WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID] = source.[TypeID],
						target.[ParentTypeID]		= source.[ParentTypeID],
						target.[IDQuery]	= source.[IDQuery],
						target.[SiteGuidQuery] = source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

END

--Adding of dbo.tblPointTemplateProperty to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPointTemplateProperty' AS [TableName],
				'PointTemplate - Property' AS [TypeID],
				'PointTemplate' AS [ParentTypeID],
				'Select @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.ID from [fmaudit].[tblPointTemplateProperty] a'
				+ ' LEFT JOIN dbo.tblPointTemplate p on p.PointTemplateGuid = a.PointTemplateGuid'
				+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pa ON pa.PointTemplateGuid = a.PointTemplateGuid AND pa._AuditEventType = ''D'''
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = p.SiteGuid from [fmaudit].[tblPointTemplateProperty] a'
				+ ' INNER JOIN dbo.tblPointTemplate p on p.PointTemplateGuid = a.PointTemplateGuid'
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID] = source.[TypeID],
						target.[ParentTypeID]		= source.[ParentTypeID],
						target.[IDQuery]	= source.[IDQuery],
						target.[SiteGuidQuery] = source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

END

--Adding of dbo.tblPointTemplateTag to fuelsmanagerdb.dbo.tblAuditHandler
BEGIN
	MERGE dbo.tblAuditHandler AS Target
	USING
	( SELECT 'tblPointTemplateTag' AS [TableName],
				'PointTemplate - Tag' AS [TypeID],
				'PointTemplate' AS [ParentTypeID],
				'Select @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '' + a.ID from [fmaudit].[tblPointTemplateTag] a'
				+ ' LEFT JOIN dbo.tblPointTemplate p on p.PointTemplateGuid = a.PointTemplateGuid'
				+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pa ON pa.PointTemplateGuid = a.PointTemplateGuid AND pa._AuditEventType = ''D'''
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [IDQuery],
				'Select @SiteGuid = CASE WHEN p.SiteGuid IS NULL THEN pa.SiteGuid ELSE p.SiteGuid END from [fmaudit].[tblPointTemplateTag] a'
				+ ' LEFT JOIN dbo.tblPointTemplate p on p.PointTemplateGuid = a.PointTemplateGuid'
				+ ' LEFT JOIN [fmaudit].[tblPointTemplate] pa ON pa.PointTemplateGuid = a.PointTemplateGuid AND pa._AuditEventType = ''D'''
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'  AS [SiteGuidQuery]) 
	AS Source
	ON (Target.[TableName] = Source.[TableName])
	WHEN MATCHED THEN
		UPDATE SET target.[TypeID] = source.[TypeID],
						target.[ParentTypeID]		= source.[ParentTypeID],
						target.[IDQuery]	= source.[IDQuery],
						target.[SiteGuidQuery] = source.[SiteGuidQuery]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([TableName], [TypeID], [ParentTypeID], [IDQuery], [SiteGuidQuery])
		VALUES (Source.[TableName], Source.[TypeID], Source.[ParentTypeID], Source.[IDQuery], Source.[SiteGuidQuery]);

END
