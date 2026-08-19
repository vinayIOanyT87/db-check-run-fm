
CREATE PROCEDURE [dbo].[usp_TransactionWeightReadingsInsert]
(
	@TransactionWeightReadings dbo.TransactionWeightReadingsType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		-- When we insert into the weight reading table we must increment the version number
		-- retained by the application. The version number can be determined by retrieving the 
		-- largest version number of the transaction's historical weight readings and adding 1. 
		INSERT INTO tblTransactionWeightReadings
		(
			TransactionGuid,
			CompartmentID,
			BeginQuantityValue,
			RequestedQuantityValue,
			FinalQuantityValue,
			FuelsManagerVersionNumber,
			SourceVersionNumber,
			HistoricalFlag,
			TransVersion,	
			VolumetricTopOffFlag,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy		
		)
		SELECT 
			NewWeightReadings.TransactionGuid,
			NewWeightReadings.CompartmentID,
			NewWeightReadings.BeginQuantityValue,
			NewWeightReadings.RequestedQuantityValue,
			NewWeightReadings.FinalQuantityValue,
			ISNULL(HistoricalWeightReadings.CurrentFuelsManagerVersionNumber, 0) + 1,
			NewWeightReadings.SourceVersionNumber,
			NewWeightReadings.HistoricalFlag,
			NewWeightReadings.TransVersion,
			NewWeightReadings.VolumetricTopOffFlag,
			SYSDATETIMEOFFSET(), --Created Date
			NewWeightReadings.CreatedUpdatedBy,
			SYSDATETIMEOFFSET(), --Updated Date
			NewWeightReadings.CreatedUpdatedBy		
		FROM @TransactionWeightReadings NewWeightReadings
		OUTER APPLY (SELECT MAX(FuelsManagerVersionNumber) AS CurrentFuelsManagerVersionNumber
			FROM tblTransactionWeightReadings
			WHERE tblTransactionWeightReadings.TransactionGuid = NewWeightReadings.TransactionGuid
			AND HistoricalFlag = 1) HistoricalWeightReadings

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
						+ 'Procedure Name: usp_TransactionWeightReadingsInsert' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 

