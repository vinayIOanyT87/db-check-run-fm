CREATE PROCEDURE [dbo].[usp_AlarmTestTemplateAddModify]
(
	@AlarmTestTempTable AlarmTestTemplateDataType READONLY,
	@EnableAdd BIT,
	@EnableModify BIT
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

MERGE dbo.tblAlarmTestTemplate at
USING @AlarmTestTempTable    temp        ON temp.[AlarmTestTemplateGuid] = at.[AlarmTestTemplateGuid]
              			   

								                        
WHEN MATCHED AND	@EnableModify = 1 AND (
						temp.AlarmTemplateGuid <> at.AlarmTemplateGuid 
						OR temp.ID <> at.ID 
						OR temp.LimitTemplateTagGuid <> at.LimitTemplateTagGuid 
						OR temp.TagField <> at.TagField 
						OR temp.AlarmPriorityGuid <> at.AlarmPriorityGuid
						OR temp.NormalUnacknowledgedAlarmPriorityGuid <> at.NormalUnacknowledgedAlarmPriorityGuid
						OR temp.TestType <> at.TestType 
						OR temp.BitMask <> at.BitMask 
						OR temp.[Enabled] <> at.[Enabled] 
						OR temp.[Order] <> at.[Order] 
						OR temp.AlarmState <> at.AlarmState 
						OR temp.Holdoff <> at.Holdoff 
						OR temp.AlarmText <> at.AlarmText 
						OR temp.HelpFile <> at.HelpFile 
						OR temp.DrawingGuid <> at.DrawingGuid 
						OR temp.BitwiseOperator <> at.BitwiseOperator
						OR temp.TimedHoldOffInSeconds <> at.TimedHoldOffInSeconds
					)THEN
    UPDATE SET	ID = temp.ID  
				,TagField = temp.TagField  
				,AlarmPriorityGuid = temp.AlarmPriorityGuid
				,NormalUnacknowledgedAlarmPriorityGuid = temp.NormalUnacknowledgedAlarmPriorityGuid  
				,TestType = temp.TestType  
				,BitMask = temp.BitMask  
				,[Enabled] = temp.[Enabled]  
				,[Order] = temp.[Order]  
				,AlarmState = temp.AlarmState  
				,Holdoff = temp.Holdoff  
				,AlarmText = temp.AlarmText  
				,HelpFile = temp.HelpFile  
				,DrawingGuid = temp.DrawingGuid  
				,UpdatedBy = temp.UpdatedBy 
				,UpdatedDate = Sysdatetimeoffset() 
				,BitwiseOperator = temp.BitwiseOperator
				,TimedHoldOffInSeconds = temp.TimedHoldOffInSeconds

WHEN NOT MATCHED AND @EnableAdd = 1 THEN
    INSERT 
	 (
			[AlarmTestTemplateGuid],
			[AlarmTemplateGuid],
			[ID],
			[LimitTemplateTagGuid],
			[TagField],
			[AlarmPriorityGuid],
			[NormalUnacknowledgedAlarmPriorityGuid],
			[TestType],
			[BitMask],
			[Enabled],
			[Order],
			[AlarmState],
			[Holdoff],
			[AlarmText],
			[HelpFile],
			[DrawingGuid],
			[CreatedDate],
			[CreatedBy],
			[UpdatedDate],
			[UpdatedBy],
			[BitwiseOperator],
			[TimedHoldOffInSeconds]
	)
	VALUES
	(
		temp.AlarmTestTemplateGuid
		,temp.AlarmTemplateGuid
		,temp.ID
		,temp.LimitTemplateTagGuid
		,temp.TagField
		,temp.AlarmPriorityGuid
		,temp.NormalUnacknowledgedAlarmPriorityGuid
		,temp.TestType
		,temp.BitMask
		,temp.[Enabled]
		,temp.[Order]
		,temp.AlarmState
		,temp.Holdoff
		,temp.AlarmText
		,temp.HelpFile
		,temp.DrawingGuid
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,temp.BitwiseOperator
		,temp.TimedHoldOffInSeconds
	);

MERGE dbo.tblAlarmTest at
USING @AlarmTestTempTable temp
JOIN tblAlarm a ON a.AlarmTemplateGuid = temp.[AlarmTemplateGuid]
JOIN tblPointTag pt1 ON pt1.PointTagGuid = a.[InputTagGuid]
JOIN tblPointTag pt2 ON pt2.PointTemplateTagGuid = temp.[LimitTemplateTagGuid]
INNER JOIN tblPoint p ON p.PointGuid = pt1.PointGuid AND p.PointGuid = pt2.PointGuid
ON temp.[AlarmTestTemplateGuid] = at.[AlarmTestTemplateGuid] AND at.[AlarmGuid] = a.[AlarmGuid]
              			   

								                        
WHEN MATCHED AND	@EnableModify = 1 AND (
						temp.ID <> at.ID 
						OR temp.TagField <> at.TagField 
						OR temp.AlarmPriorityGuid <> at.AlarmPriorityGuid
						OR temp.NormalUnacknowledgedAlarmPriorityGuid <> at.NormalUnacknowledgedAlarmPriorityGuid
						OR temp.TestType <> at.TestType 
						OR temp.BitMask <> at.BitMask 
						OR temp.[Enabled] <> at.[Enabled] 
						OR temp.[Order] <> at.[Order] 
						OR temp.HelpFile <> at.HelpFile 
						OR temp.DrawingGuid <> at.DrawingGuid 
						OR temp.BitwiseOperator <> at.BitwiseOperator
					)THEN
    UPDATE SET	ID = temp.ID  
				,TagField = temp.TagField  
				,AlarmPriorityGuid = temp.AlarmPriorityGuid
				,NormalUnacknowledgedAlarmPriorityGuid = temp.NormalUnacknowledgedAlarmPriorityGuid  
				,TestType = temp.TestType  
				,BitMask = temp.BitMask  
				,[Order] = temp.[Order]  
				,HelpFile = temp.HelpFile  
				,DrawingGuid = temp.DrawingGuid  
				,UpdatedBy = temp.UpdatedBy 
				,UpdatedDate = Sysdatetimeoffset() 
				,BitwiseOperator = temp.BitwiseOperator

WHEN NOT MATCHED AND @EnableAdd = 1 THEN
    INSERT 
	 (
			[AlarmTestGuid],
			[AlarmGuid],
			[ID],
			[LimitTagGuid],
			[TagField],
			[AlarmPriorityGuid],
			[NormalUnacknowledgedAlarmPriorityGuid],
			[TestType],
			[BitMask],
			[Enabled],
			[Order],
			[AlarmState],
			[Holdoff],
			[AlarmText],
			[HelpFile],
			[DrawingGuid],
			[CreatedDate],
			[CreatedBy],
			[UpdatedDate],
			[UpdatedBy],
			[BitwiseOperator],
			[TimedHoldOffInSeconds],
			[AlarmTestTemplateGuid]
	)
	VALUES
	(
		NEWID()
		,a.AlarmGuid
		,temp.ID
		,pt2.PointTagGuid
		,temp.TagField
		,temp.AlarmPriorityGuid
		,temp.NormalUnacknowledgedAlarmPriorityGuid
		,temp.TestType
		,temp.BitMask
		,temp.[Enabled]
		,temp.[Order]
		,temp.AlarmState
		,temp.Holdoff
		,temp.AlarmText
		,temp.HelpFile
		,temp.DrawingGuid
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,temp.BitwiseOperator
		,temp.TimedHoldOffInSeconds
		,temp.AlarmTestTemplateGuid
	);



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
						+ 'Procedure Name: usp_AlarmTestTemplateAddModify' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
GO


