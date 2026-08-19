CREATE PROCEDURE [dbo].[usp_ExternalStationTransactionDelete]
	@ExternalStationTransactionGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		-- See if there are any Failed Transactions that are still waiting to be resolved.
		IF EXISTS (SELECT 1 FROM [dbo].[tblExternalStationTransaction] 
									INNER JOIN [lookup].[tblExternalStationTransactionFailedStatus]
										ON [dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionFailedStatusIndex] = [lookup].[tblExternalStationTransactionFailedStatus].[ExternalStationTransactionFailedStatusIndex]
							WHERE [ExternalStationTransactionGuid] = @ExternalStationTransactionGuid
									AND [lookup].[tblExternalStationTransactionFailedStatus].[FinalState] = 0)
		BEGIN 
			RAISERROR('Attempted to delete an External Station Transaction that still contains open processing errors.',18,1); 
			RETURN; 
		END 

		DELETE FROM [dbo].[tblExternalStationTransaction] 
			WHERE [ExternalStationTransactionGuid] = @ExternalStationTransactionGuid 

		DELETE FROM tblExternalStationTransaction
		WHERE ExternalStationTransactionGuid = @ExternalStationTransactionGuid

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
						+ 'Procedure Name: usp_ExternalStationTransactionDelete' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH


END
