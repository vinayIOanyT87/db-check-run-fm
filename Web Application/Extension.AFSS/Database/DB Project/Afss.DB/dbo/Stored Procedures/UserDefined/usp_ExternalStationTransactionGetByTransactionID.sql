CREATE PROCEDURE [dbo].[usp_ExternalStationTransactionGetByTransactionID]
	@ExternalStationTransactionID nvarchar(20)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		SELECT 
			tblExternalStationTransaction.ExternalStationTransactionGuid, 
			tblExternalStationTransaction.ExternalStationGuid,
			tblExternalStationTransaction.SiteGuid, 
			tblExternalStationTransaction.StationTransactionID,
			tblExternalStationTransaction.RawTransactionData,
			tblExternalStationTransaction.CreatedBy,
			tblExternalStationTransaction.CreatedDate,
			tblExternalStationTransaction.UpdatedBy,
			tblExternalStationTransaction.UpdatedDate,
			tblExternalStationTransaction.LookupExternalStationTransactionStatusIndex,
			tblExternalStationTransaction.LookupExternalStationTransactionFailedStatusIndex,
			tblExternalStation.ID AS ExternalStationID
		FROM tblExternalStationTransaction
			INNER JOIN tblExternalStation ON tblExternalStationTransaction.[ExternalStationTransactionGuid] = tblExternalStation.ExternalStationGuid
		WHERE StationTransactionID = @ExternalStationTransactionID

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
						+ 'Procedure Name: usp_ExternalStationTransactionGetByTransactionID' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	