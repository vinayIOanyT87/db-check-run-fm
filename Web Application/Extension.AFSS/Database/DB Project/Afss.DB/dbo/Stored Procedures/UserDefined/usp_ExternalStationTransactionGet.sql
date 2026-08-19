CREATE PROCEDURE [dbo].[usp_ExternalStationTransactionGet]
	@ExternalStationTransactionGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		SELECT 
			[dbo].[tblExternalStationTransaction].[ExternalStationTransactionGuid],
			[dbo].[tblExternalStationTransaction].[ExternalStationGuid],
			[dbo].[tblExternalStationTransaction].[SiteGuid],
			[dbo].[tblExternalStation].[ID] AS ExternalStationID,
			[dbo].[tblExternalStationTransaction].[StationTransactionID],
			[dbo].[tblExternalStationTransaction].[RawTransactionData],
			[dbo].[tblExternalStationTransaction].[CreatedBy],
			[dbo].[tblExternalStationTransaction].[CreatedDate],
			[dbo].[tblExternalStationTransaction].[UpdatedBy],
			[dbo].[tblExternalStationTransaction].[UpdatedDate],
			[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionStatusIndex],
			[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionFailedStatusIndex]
		FROM [dbo].[tblExternalStationTransaction]
			INNER JOIN [dbo].[tblExternalStation] ON [dbo].[tblExternalStationTransaction].[ExternalStationGuid] = [dbo].[tblExternalStation].[ExternalStationGuid]
		WHERE [dbo].[tblExternalStationTransaction].[ExternalStationTransactionGuid] = @ExternalStationTransactionGuid

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
						+ 'Procedure Name: usp_ExternalStationTransactionGet' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	