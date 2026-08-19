CREATE PROCEDURE [dbo].[usp_ExternalStationGeneralConfigurationUpdate]
	@ExternalStationGeneralConfigurationGuid UNIQUEIDENTIFIER, 
	@SiteGuid UNIQUEIDENTIFIER, 
	@RetailSaleTransactionAliasGuid UNIQUEIDENTIFIER = NULL,
	@DownloadTransactionsIntervalMinutes INT = NULL, 
	@DownloadEventsIntervalMinutes INT = NULL, 
	@UpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		UPDATE tblExternalStationGeneralConfiguration
		SET SiteGuid = @SiteGuid,
			RetailSaleTransactionAliasGuid = @RetailSaleTransactionAliasGuid,
			DownloadTransactionsIntervalMinutes = @DownloadTransactionsIntervalMinutes,
			DownloadEventsIntervalMinutes = @DownloadEventsIntervalMinutes,
			UpdatedBy = @UpdatedBy,
			UpdatedDate = SYSDATETIMEOFFSET()
		WHERE ExternalStationGeneralConfigurationGuid = @ExternalStationGeneralConfigurationGuid

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
						+ 'Procedure Name: usp_ExternalStationGeneralConfigurationUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH
END
