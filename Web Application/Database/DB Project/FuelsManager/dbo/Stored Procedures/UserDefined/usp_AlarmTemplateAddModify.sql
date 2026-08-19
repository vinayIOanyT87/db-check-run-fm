CREATE PROCEDURE [dbo].[usp_AlarmTemplateAddModify]
(
	@AlarmTempTable AlarmTemplateDataType READONLY,
	@EnableAdd BIT,
	@EnableModify BIT
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

IF (@EnableModify = 1)
BEGIN 

	UPDATE at
	SET	ID = temp.ID  
					,InputTemplateTagGuid = temp.InputTemplateTagGuid  
					,[Enabled] = temp.[Enabled]  
					,AlarmCategoryApplicationStringGuid = temp.AlarmCategoryApplicationStringGuid  
					,[Order] = temp.[Order]  
					,NotAlarmState = temp.NotAlarmState 
					,Comment = temp.Comment  
					,ShelvedStartTimeStamp = temp.ShelvedStartTimeStamp  
					,ShelvedEndTimeStamp = temp.ShelvedEndTimeStamp  
					,ShelvedOneShot = temp.ShelvedOneShot 
					,ShelvedBy = temp.ShelvedBy 
					,Suppressed = temp.Suppressed   
					,UpdatedBy = temp.UpdatedBy 
					,UpdatedDate = Sysdatetimeoffset() 
					,AlarmStateTemplateTagGuid = temp.AlarmStateTemplateTagGuid
					,ExclusiveAlarm = temp.ExclusiveAlarm
	 FROM dbo.tblAlarmTemplate at
	JOIN @AlarmTempTable    temp        
	ON temp.[AlarmTemplateGuid] = at.[AlarmTemplateGuid]
	WHERE temp.ID <> at.ID 
		OR temp.InputTemplateTagGuid <> at.InputTemplateTagGuid 
		OR temp.[Enabled] <> at.[Enabled] 
		OR temp.AlarmCategoryApplicationStringGuid <> at.AlarmCategoryApplicationStringGuid
		OR temp.[Order] <> at.[Order] 
		OR temp.NotAlarmState <> at.NotAlarmState 	
		OR temp.Comment <> at.Comment 
		OR temp.ShelvedStartTimeStamp <> at.ShelvedStartTimeStamp 
		OR temp.ShelvedEndTimeStamp <> at.ShelvedEndTimeStamp 
		OR temp.ShelvedOneShot <> at.ShelvedOneShot 
		OR temp.ShelvedBy <> at.ShelvedBy 
		OR temp.Suppressed <> at.Suppressed 
		OR temp.AlarmStateTemplateTagGuid <> at.AlarmStateTemplateTagGuid
		OR temp.ExclusiveAlarm <> at.ExclusiveAlarm


    UPDATE at 
	SET	ID = temp.ID  
	,AlarmCategoryApplicationStringGuid = temp.AlarmCategoryApplicationStringGuid  
	,[Order] = temp.[Order]  
	,NotAlarmState = temp.NotAlarmState 
	,ShelvedBy = temp.ShelvedBy 
	,Suppressed = temp.Suppressed   
	,UpdatedBy = temp.UpdatedBy 
	,UpdatedDate = Sysdatetimeoffset() 
	,ExclusiveAlarm = temp.ExclusiveAlarm
	FROM dbo.tblAlarm at
	JOIN @AlarmTempTable temp
	ON temp.[AlarmTemplateGuid] = at.[AlarmTemplateGuid] 
	JOIN tblPointTag pt1 ON pt1.PointTemplateTagGuid = temp.InputTemplateTagGuid AND pt1.PointTagGuid = at.InputTagGuid 
	JOIN tblPointTag pt2 ON pt2.PointTemplateTagGuid = temp.AlarmStateTemplateTagGuid
	INNER JOIN tblPoint p ON p.PointGuid = pt1.PointGuid AND p.PointGuid = pt2.PointGuid
	WHERE temp.ID <> at.ID 
	OR temp.[Enabled] <> at.[Enabled] 
	OR temp.AlarmCategoryApplicationStringGuid <> at.AlarmCategoryApplicationStringGuid
	OR temp.[Order] <> at.[Order] 
	OR temp.NotAlarmState <> at.NotAlarmState 	
	OR temp.ExclusiveAlarm <> at.ExclusiveAlarm


END

IF ( @EnableAdd = 1 )
BEGIN
    INSERT  dbo.tblAlarmTemplate 
	 (
			[AlarmTemplateGuid],
			[InputTemplateTagGuid],
			[ID],
			[Enabled],
			[AlarmCategoryApplicationStringGuid],
			[Order],
			[NotAlarmState],
			[Comment],
			[ShelvedStartTimeStamp],
			[ShelvedEndTimeStamp],
			[ShelvedOneShot],
			[ShelvedBy],
			[Suppressed],
			[CreatedDate],
			[CreatedBy],
			[UpdatedDate],
			[UpdatedBy],
			[AlarmStateTemplateTagGuid],
			[ExclusiveAlarm]
	)
	SELECT 
		temp.AlarmTemplateGuid
		,temp.InputTemplateTagGuid
		,temp.ID
		,temp.[Enabled]
		,temp.AlarmCategoryApplicationStringGuid
		,temp.[Order]
		,temp.NotAlarmState
		,temp.Comment
		,temp.ShelvedStartTimeStamp
		,temp.ShelvedEndTimeStamp
		,temp.ShelvedOneShot
		,temp.ShelvedBy
		,temp.Suppressed
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,temp.AlarmStateTemplateTagGuid
		,temp.ExclusiveAlarm
	FROM @AlarmTempTable    temp        
	WHERE NOT EXISTS (SELECT 1 FROM dbo.tblAlarmTemplate at WHERE temp.[AlarmTemplateGuid] = at.[AlarmTemplateGuid] )


    INSERT dbo.tblAlarm
	 (
			[AlarmGuid],
			[InputTagGuid],
			[ID],
			[Enabled],
			[AlarmCategoryApplicationStringGuid],
			[Order],
			[NotAlarmState],
			[Comment],
			[ShelvedStartTimeStamp],
			[ShelvedEndTimeStamp],
			[ShelvedOneShot],
			[ShelvedBy],
			[Suppressed],
			[CreatedDate],
			[CreatedBy],
			[UpdatedDate],
			[UpdatedBy],
			[AlarmStateTagGuid],
			[ExclusiveAlarm],
			[AlarmTemplateGuid]
	)
	SELECT
		NEWID()
		,pt1.PointTagGuid
		,temp.ID
		,temp.[Enabled]
		,temp.AlarmCategoryApplicationStringGuid
		,temp.[Order]
		,temp.NotAlarmState
		,temp.Comment
		,temp.ShelvedStartTimeStamp
		,temp.ShelvedEndTimeStamp
		,temp.ShelvedOneShot
		,temp.ShelvedBy
		,temp.Suppressed
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,pt2.PointTagGuid
		,temp.ExclusiveAlarm
		,temp.AlarmTemplateGuid
	FROM @AlarmTempTable    temp
	JOIN tblPointTag pt1 ON pt1.PointTemplateTagGuid = temp.InputTemplateTagGuid
	JOIN tblPointTag pt2 ON pt2.PointTemplateTagGuid = temp.AlarmStateTemplateTagGuid
	INNER JOIN tblPoint p ON p.PointGuid = pt1.PointGuid AND p.PointGuid = pt2.PointGuid
	WHERE NOT EXISTS (SELECT 1 FROM dbo.tblAlarm at WHERE temp.[AlarmTemplateGuid] = at.[AlarmTemplateGuid] AND pt1.PointTagGuid = at.InputTagGuid)
END


	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;      
				      
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: usp_AlarmTemplateAddModify' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
GO