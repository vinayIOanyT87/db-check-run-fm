CREATE PROCEDURE [dbo].[usp_AlarmTestAddModify]
(
	@AlarmTestTempTable AlarmTestDataType READONLY,
	@EnableAdd BIT,
	@EnableModify BIT
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

MERGE dbo.tblAlarmTest targetDB
USING @AlarmTestTempTable    sourceDB        ON sourceDB.[AlarmTestGuid] = targetDB.[AlarmTestGuid]
              			   

								                        
WHEN MATCHED AND	@EnableModify = 1 AND 
										(
											sourceDB.AlarmGuid <> targetDB.AlarmGuid 
											OR sourceDB.ID <> targetDB.ID 
											OR sourceDB.LimitTagGuid <> targetDB.LimitTagGuid 
											OR sourceDB.TagField <> targetDB.TagField 
											OR sourceDB.AlarmPriorityGuid <> targetDB.AlarmPriorityGuid 
											OR sourceDB.NormalUnacknowledgedAlarmPriorityGuid <> targetDB.NormalUnacknowledgedAlarmPriorityGuid 
											OR sourceDB.TestType <> targetDB.TestType 
											OR sourceDB.BitMask <> targetDB.BitMask 
											OR sourceDB.[Enabled] <> targetDB.[Enabled] 
											OR sourceDB.[Order] <> targetDB.[Order] 
											OR sourceDB.AlarmState <> targetDB.AlarmState 
											OR sourceDB.Holdoff <> targetDB.Holdoff 
											OR sourceDB.AlarmText <> targetDB.AlarmText 
											OR sourceDB.HelpFile <> targetDB.HelpFile 
											OR sourceDB.DrawingGuid <> targetDB.DrawingGuid 
											OR (sourceDB.DrawingGuid IS NOT NULL AND targetDB.DrawingGuid IS NULL)
											OR sourceDB.BitwiseOperator <> targetDB.BitwiseOperator
											OR sourceDB.TimedHoldOffInSeconds <> targetDB.TimedHoldOffInSeconds
											OR sourceDB.AlarmTestTemplateGuid <> targetDB.AlarmTestTemplateGuid
										)
					THEN
    UPDATE SET	AlarmGuid = sourceDB.AlarmGuid           
				,ID = sourceDB.ID  
				,LimitTagGuid = sourceDB.LimitTagGuid  
				,TagField = sourceDB.TagField  
				,AlarmPriorityGuid = sourceDB.AlarmPriorityGuid
				,NormalUnacknowledgedAlarmPriorityGuid = sourceDB.NormalUnacknowledgedAlarmPriorityGuid
				,TestType = sourceDB.TestType  
				,BitMask = sourceDB.BitMask  
				,[Enabled] = sourceDB.[Enabled]  
				,[Order] = sourceDB.[Order]  
				,AlarmState = sourceDB.AlarmState  
				,Holdoff = sourceDB.Holdoff  
				,AlarmText = sourceDB.AlarmText  
				,HelpFile = sourceDB.HelpFile  
				,DrawingGuid = sourceDB.DrawingGuid  
				,UpdatedBy = sourceDB.UpdatedBy 
				,UpdatedDate = Sysdatetimeoffset() 
				,BitwiseOperator = sourceDB.BitwiseOperator
				,TimedHoldOffInSeconds = sourceDB.TimedHoldOffInSeconds
				,AlarmTestTemplateGuid = sourceDB.AlarmTestTemplateGuid

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
		sourceDB.AlarmTestGuid
		,sourceDB.AlarmGuid
		,sourceDB.ID
		,sourceDB.LimitTagGuid
		,sourceDB.TagField
		,sourceDB.AlarmPriorityGuid
		,sourceDB.NormalUnacknowledgedAlarmPriorityGuid
		,sourceDB.TestType
		,sourceDB.BitMask
		,sourceDB.[Enabled]
		,sourceDB.[Order]
		,sourceDB.AlarmState
		,sourceDB.Holdoff
		,sourceDB.AlarmText
		,sourceDB.HelpFile
		,sourceDB.DrawingGuid
		,sysdatetimeoffset()
		,sourceDB.UpdatedBy
		,sysdatetimeoffset()
		,sourceDB.UpdatedBy
		,sourceDB.BitwiseOperator
		,sourceDB.TimedHoldOffInSeconds
		,sourceDB.AlarmTestTemplateGuid
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
						+ 'Procedure Name: usp_AlarmTestAddModify' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END