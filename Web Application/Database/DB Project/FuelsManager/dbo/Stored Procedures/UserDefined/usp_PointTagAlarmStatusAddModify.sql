CREATE PROCEDURE [dbo].[usp_PointTagAlarmStatusAddModify]
(
	@AlarmStatusTempTable PointTagAlarmStatusDataType READONLY,
	@EnableAdd BIT,
	@EnableModify BIT
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

MERGE dbo.tblPointTagAlarmStatus ptas
USING @AlarmStatusTempTable    temp        ON temp.[PointTagAlarmStatusGuid] = ptas.[PointTagAlarmStatusGuid]
              			                           
WHEN MATCHED AND	@EnableModify = 1 AND (
						temp.AlarmTestGuid <> ptas.AlarmTestGuid 
						OR temp.Acknowledged <> ptas.Acknowledged 
						OR temp.AcknowledgedTimestamp <> ptas.AcknowledgedTimestamp 
						OR temp.AcknowledgedBy <> ptas.AcknowledgedBy 
						OR temp.AcknowledgedComment <> ptas.AcknowledgedComment
						OR temp.Silenced <> ptas.Silenced
						OR temp.SilencedTimestamp <> ptas.SilencedTimestamp
						OR temp.SilencedBy <> ptas.SilencedBy 
						OR temp.AlarmTestFailed <> ptas.AlarmTestFailed 
						OR temp.AlarmTestFailedTimestamp <> ptas.AlarmTestFailedTimestamp 
					)THEN
    UPDATE SET	AlarmTestGuid = temp.AlarmTestGuid           
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
           (temp.PointTagAlarmStatusGuid
		   ,temp.AlarmTestGuid
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
						+ 'Procedure Name: usp_PointTagAlarmStatusAddModify' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 