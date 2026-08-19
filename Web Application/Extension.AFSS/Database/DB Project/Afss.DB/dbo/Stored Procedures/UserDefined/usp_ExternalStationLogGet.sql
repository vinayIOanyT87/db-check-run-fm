CREATE PROCEDURE [dbo].[usp_ExternalStationLogGet]
	@ExternalStationLogGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		-- Retrieve the External Station Log record by its primary key
		SELECT 
			esl.ExternalStationLogGuid,
			esl.SiteGuid,  
			esl.ExternalStationGuid, 
			esl.LogText, 
			esl.LookupExternalStationLogTypeIndex, 
			esl.LogDate,
			esl.CreatedBy,
			esl.CreatedDate,
			esl.UpdatedBy,
			esl.UpdatedDate,
			es.ID AS ExternalStationID
		FROM [dbo].[tblExternalStationLog] esl
		INNER JOIN [dbo].[tblExternalStation] es ON esl.ExternalStationGuid = es.ExternalStationGuid
		WHERE ExternalStationLogGuid = @ExternalStationLogGuid
	
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
						+ 'Procedure Name: usp_ExternalStationLogGet' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	