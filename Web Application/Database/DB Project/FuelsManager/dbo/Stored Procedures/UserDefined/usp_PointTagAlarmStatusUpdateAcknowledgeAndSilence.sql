CREATE PROCEDURE [dbo].[usp_PointTagAlarmStatusUpdateAcknowledgeAndSilence]
(
	@AlarmStatusTempTable PointTagAlarmStatusDataType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

MERGE dbo.tblPointTagAlarmStatus ptas
USING @AlarmStatusTempTable    temp        ON temp.[PointTagAlarmStatusGuid] = ptas.[PointTagAlarmStatusGuid]
				                			                           
WHEN MATCHED AND temp.Acknowledged = 1
				AND temp.Acknowledged <> ptas.Acknowledged
				AND temp.AlarmTestFailed = ptas.AlarmTestFailed
				AND temp.AlarmTestFailedTimeStamp = ptas.AlarmTestFailedTimeStamp THEN
    UPDATE SET	Acknowledged = temp.Acknowledged  
				  ,AcknowledgedTimestamp = temp.AcknowledgedTimestamp
				  ,AcknowledgedBy = temp.AcknowledgedBy 
				  ,AcknowledgedComment = temp.AcknowledgedComment 
				  ,Silenced = temp.Silenced			
				  ,SilencedTimestamp = temp.SilencedTimestamp
				  ,SilencedBy = temp.SilencedBy
 				  ,UpdatedBy = temp.UpdatedBy 
				  ,UpdatedDate = Sysdatetimeoffset() ;


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
						+ 'Procedure Name: usp_PointTagAlarmStatusUpdateAcknowledge' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 

