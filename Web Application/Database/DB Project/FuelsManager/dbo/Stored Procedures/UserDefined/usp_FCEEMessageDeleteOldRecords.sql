
CREATE PROCEDURE [dbo].[usp_FCEEMessageDeleteOldRecords]
AS
BEGIN
	DECLARE @MaximumDays AS INT
	SELECT @MaximumDays = tblSites.MaximumDaysToRetainLogs From tblSites WHERE SiteGuid = '00000000-0000-0000-0000-000000000001'
	BEGIN TRY
		-- Delete records from the alarm and event log that are older than the maximum number of days to retain logs specified for the site
		DELETE FROM tblFCEEMessage
		FROM tblFCEEMessage FCEEMessages 
		WHERE FCEEMessages.CreatedDate < DATEADD(DAY, ISNULL(@MaximumDays, 60) * -1, SYSDATETIMEOFFSET())
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
						+ 'Procedure Name: dbo.usp_FCEEMessagesDeleteOldRecords' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END