CREATE PROCEDURE [dbo].[usp_ExternalStationDelete]
	@IdentityGuid UNIQUEIDENTIFIER
AS
BEGIN	
	SET NOCOUNT ON

	BEGIN TRY

		-- Deletion of entity to site mapping records is handled separately in the service class
		DELETE FROM [map].[tblExternalStationToProduct]
		WHERE [map].[tblExternalStationToProduct].[ExternalStationGuid] = @IdentityGuid

		DELETE [dbo].[tblGasboyStationEvent] FROM [dbo].[tblGasboyStationEvent]
		INNER JOIN  [dbo].[tblExternalStationLog] 
		ON [dbo].[tblGasboyStationEvent].[ExternalStationLogGuid] = [dbo].[tblExternalStationLog].[ExternalStationLogGuid]
		WHERE  [dbo].[tblExternalStationLog].[ExternalStationGuid] = @IdentityGuid

		DELETE FROM [dbo].[tblExternalStationLog]
		WHERE [dbo].[tblExternalStationLog].[ExternalStationGuid] = @IdentityGuid

		DELETE [dbo].tblExternalStationTransaction FROM dbo.tblExternalStationTransaction
		INNER JOIN [dbo].[tblExternalStation]
		ON [dbo].[tblExternalStationTransaction].[ExternalStationGuid] = [dbo].[tblExternalStation].[ExternalStationGuid]
		WHERE [dbo].[tblExternalStation].[ExternalStationGuid] = @IdentityGuid

		DELETE FROM [dbo].[tblExternalStation]
		WHERE [dbo].[tblExternalStation].[ExternalStationGuid] = @IdentityGuid

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
						+ 'Procedure Name: usp_ExternalStationDelete' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END