CREATE PROCEDURE [dbo].[usp_PointTagAlarmStatusUpdateTestFailed]
(
	@AlarmStatusTempTable PointTagAlarmStatusDataType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

MERGE dbo.tblPointTagAlarmStatus ptas
USING @AlarmStatusTempTable temp  ON temp.[PointTagAlarmStatusGuid] = ptas.[PointTagAlarmStatusGuid]
              			                           
WHEN MATCHED AND CAST(1 AS BIT) = (SELECT [Enabled] FROM dbo.tblAlarmTest at WHERE at.AlarmTestGuid = ptas.AlarmTestGuid)
				 AND CAST(1 AS BIT) = (SELECT [Enabled] FROM dbo.tblAlarm a WHERE a.AlarmGuid = (SELECT AlarmGuid FROM dbo.tblAlarmTest at WHERE at.AlarmTestGuid = ptas.AlarmTestGuid)) 
				 AND CAST(1 AS BIT) = (SELECT AlarmsEnabled FROM dbo.tblPointTag pt WHERE pt.PointTagGuid = (SELECT InputTagGuid FROM dbo.tblAlarm a WHERE a.AlarmGuid = (SELECT AlarmGuid FROM dbo.tblAlarmTest at WHERE at.AlarmTestGuid = ptas.AlarmTestGuid))) 
THEN
    UPDATE SET	AlarmTestFailed = temp.AlarmTestFailed  
	 			  ,UpdatedBy = temp.UpdatedBy 
				  ,UpdatedDate = Sysdatetimeoffset() 
				  ,AlarmTestFailedTimestamp = CASE
															WHEN temp.AlarmTestFailed = 1 THEN temp.AlarmTestFailedTimestamp 
															ELSE ptas.AlarmTestFailedTimestamp 
														END
              ,Acknowledged = CASE
											WHEN ptas.Acknowledged = 1 AND temp.AlarmTestFailed = 1 THEN 0 
											ELSE ptas.Acknowledged 
										END 
				  ,AcknowledgedTimestamp = CASE
														WHEN ptas.Acknowledged = 1 AND temp.AlarmTestFailed = 1 THEN null 
														ELSE ptas.AcknowledgedTimestamp 
													END  
				  ,AcknowledgedBy =	CASE
												WHEN ptas.Acknowledged = 1 AND temp.AlarmTestFailed = 1 THEN null 
												ELSE ptas.AcknowledgedBy 
											END   
				  ,AcknowledgedComment = CASE
													WHEN ptas.Acknowledged = 1 AND temp.AlarmTestFailed = 1 THEN null 
													ELSE ptas.AcknowledgedComment 
												 END
              ,Silenced = CASE
											WHEN ptas.Silenced = 1 AND temp.AlarmTestFailed = 1 THEN 0 
											ELSE ptas.Silenced 
										END 
				  ,SilencedTimestamp = CASE
														WHEN ptas.Silenced = 1 AND temp.AlarmTestFailed = 1 THEN null 
														ELSE ptas.SilencedTimestamp 
													END  
				  ,SilencedBy =	CASE
												WHEN ptas.Silenced = 1 AND temp.AlarmTestFailed = 1 THEN null 
												ELSE ptas.SilencedBy 
											END   


 ;

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
						+ 'Procedure Name: usp_PointTagAlarmStatusUpdateTestFailed' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 