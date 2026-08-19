CREATE PROCEDURE [dbo].[usp_PointTemplateTagAlarmStatusAddModify]
(
	@AlarmStatusTempTable PointTemplateTagAlarmStatusDataType READONLY,
	@EnableAdd BIT,
	@EnableModify BIT
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

MERGE dbo.tblPointTemplateTagAlarmStatus pttas
USING @AlarmStatusTempTable temp ON temp.[PointTemplateTagAlarmStatusGuid] = pttas.[PointTemplateTagAlarmStatusGuid]
              			                           
WHEN MATCHED AND	@EnableModify = 1 AND (
						temp.Acknowledged <> pttas.Acknowledged 
						OR temp.AcknowledgedTimestamp <> pttas.AcknowledgedTimestamp 
						OR temp.AcknowledgedBy <> pttas.AcknowledgedBy 
						OR temp.AcknowledgedComment <> pttas.AcknowledgedComment 
						OR temp.Silenced <> pttas.Silenced
						OR temp.SilencedTimestamp <> pttas.SilencedTimestamp
						OR temp.SilencedBy <> pttas.SilencedBy 
						OR temp.AlarmTestFailed <> pttas.AlarmTestFailed 
						OR temp.AlarmTestFailedTimestamp <> pttas.AlarmTestFailedTimestamp 
					)THEN
    UPDATE SET	AlarmTestTemplateGuid = temp.AlarmTestTemplateGuid           
				,Acknowledged = temp.Acknowledged  
				,AcknowledgedTimestamp = temp.AcknowledgedTimestamp  
				,AcknowledgedBy = temp.AcknowledgedBy  
				,AcknowledgedComment = temp.AcknowledgedComment  
				,Silenced = temp.Silenced
				,SilencedTimestamp = temp.SilencedTimestamp
				,SilencedBy = temp.SilencedBy  
				,AlarmTestFailed = temp.AlarmTestFailed 
				,AlarmTestFailedTimestamp = temp.AlarmTestFailedTimestamp  
				,UpdatedBy = temp.UpdatedBy 
				,UpdatedDate = Sysdatetimeoffset() 

WHEN NOT MATCHED AND @EnableAdd = 1 THEN
    INSERT ([PointTemplateTagAlarmStatusGuid],
	[AlarmTestTemplateGuid],
	[Acknowledged],
	[AcknowledgedTimestamp],
	[AcknowledgedBy],
	[AcknowledgedComment],
	[Silenced],
	[SilencedTimestamp],
	[SilencedBy],
	[AlarmTestFailed],
	[AlarmTestFailedTimestamp],
	[CreatedDate],
	[CreatedBy],
	[UpdatedDate],
	[UpdatedBy]
	)
     VALUES
           (temp.PointTemplateTagAlarmStatusGuid
		   ,temp.AlarmTestTemplateGuid
		   ,temp.Acknowledged
		   ,temp.AcknowledgedTimestamp
		   ,temp.AcknowledgedBy
		   ,temp.AcknowledgedComment
			,temp.Silenced
			,temp.SilencedTimestamp
			,temp.SilencedBy
		   ,temp.AlarmTestFailed
			,temp.AlarmTestFailedTimestamp
		   ,sysdatetimeoffset()
		   ,temp.UpdatedBy
		   ,sysdatetimeoffset()
		   ,temp.UpdatedBy);

MERGE dbo.tblPointTagAlarmStatus ptas
USING @AlarmStatusTempTable temp
JOIN tblAlarmTest at ON at.AlarmTestTemplateGuid = temp.AlarmTestTemplateGuid
JOIN tblAlarm a ON a.AlarmGuid = at.AlarmGuid
ON at.[AlarmTestGuid] = ptas.[AlarmTestGuid] AND a.AlarmGuid = at.AlarmGuid
              			                           
WHEN NOT MATCHED AND @EnableAdd = 1 THEN
    INSERT ([PointTagAlarmStatusGuid],
	[AlarmTestGuid],
	[Acknowledged],
	[AcknowledgedTimestamp],
	[AcknowledgedBy],
	[AcknowledgedComment],
	[Silenced],
	[SilencedTimestamp],
	[SilencedBy],
	[AlarmTestFailed],
	[AlarmTestFailedTimestamp],
	[CreatedDate],
	[CreatedBy],
	[UpdatedDate],
	[UpdatedBy]
	)
     VALUES
           (NEWID()
		   ,at.AlarmTestGuid
		   ,temp.Acknowledged
		   ,temp.AcknowledgedTimestamp
		   ,temp.AcknowledgedBy
		   ,temp.AcknowledgedComment
			,temp.Silenced
			,temp.SilencedTimestamp
			,temp.SilencedBy
		   ,temp.AlarmTestFailed
			,temp.AlarmTestFailedTimestamp
		   ,sysdatetimeoffset()
		   ,temp.UpdatedBy
		   ,sysdatetimeoffset()
		   ,temp.UpdatedBy);


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
						+ 'Procedure Name: usp_PointTemplateTagAlarmStatusAddModify' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
GO


