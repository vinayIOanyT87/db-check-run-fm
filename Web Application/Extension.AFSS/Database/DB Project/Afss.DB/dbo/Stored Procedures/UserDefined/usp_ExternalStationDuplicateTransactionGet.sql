CREATE PROCEDURE [dbo].[usp_ExternalStationDuplicateTransactionGet]
	@ExternalStationGuid UNIQUEIDENTIFIER,
	@DuplicateExternalStationTransactionIDs [dbo].[FindDuplicateExternalStationTransactionType] READONLY
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		IF (@ExternalStationGuid IS NOT NULL)
		BEGIN
			SELECT 
				dbo.tblExternalStationTransaction.ExternalStationTransactionGuid,
				dbo.tblExternalStationTransaction.ExternalStationGuid,
				dbo.tblExternalStationTransaction.SiteGuid,
				dbo.tblExternalStation.ID AS ExternalStationID,
				dbo.tblExternalStationTransaction.StationTransactionID,
				dbo.tblExternalStationTransaction.CreatedDate,
				dbo.tblExternalStationTransaction.LookupExternalStationTransactionStatusIndex,
				dbo.tblExternalStationTransaction.LookupExternalStationTransactionFailedStatusIndex
			FROM [dbo].[tblExternalStationTransaction]
				INNER JOIN [dbo].[tblExternalStation]
					ON dbo.tblExternalStation.ExternalStationGuid = dbo.tblExternalStationTransaction.[ExternalStationGuid]
				INNER JOIN @DuplicateExternalStationTransactionIDs IDsToCheck
					ON dbo.tblExternalStationTransaction.StationTransactionID = IDsToCheck.StationTransactionID
			WHERE dbo.tblExternalStationTransaction.[ExternalStationGuid] = @ExternalStationGuid 
				ORDER BY dbo.tblExternalStationTransaction.CreatedDate DESC
		END

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
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13) + CHAR(10)                 
						+ 'Procedure Name: usp_ExternalStationDuplicateTransactionGet' + CHAR(13) + CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13) + CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
