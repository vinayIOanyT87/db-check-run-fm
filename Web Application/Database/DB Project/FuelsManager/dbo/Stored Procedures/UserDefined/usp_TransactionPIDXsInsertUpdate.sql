CREATE PROCEDURE [dbo].[usp_TransactionPIDXsInsertUpdate]
(
	@TransactionPIDXs dbo.TransactionPIDXsType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY
	
		MERGE tblTransactionPIDX AS target
		USING (
			SELECT 
				TransactionPIDXGuid,
				TransactionGuid,
				AuthorizationNumber, 
				SentFlag, 
				DateSent, 
				PIDXProfileGuid, 
				CompanyPersonnelToShipToBillToGuid, 
				BrokenBlend,
				CreatedUpdatedBy,
				BOLVersion
			FROM @TransactionPIDXs
			) AS source
		ON source.TransactionGuid = target.TransactionGuid
			AND source.PIDXProfileGuid = target.PIDXProfileGuid
			AND (source.AuthorizationNumber = target.AuthorizationNumber OR (source.AuthorizationNumber is null and target.AuthorizationNumber is null))
		WHEN MATCHED THEN UPDATE SET
			--The only field you can update on a PIDX record is the BrokenBlend field.
			BrokenBlend = source.BrokenBlend,
			UpdatedDate = SYSDATETIMEOFFSET(),
			UpdatedBy = source.CreatedUpdatedBy
		WHEN NOT MATCHED THEN INSERT 
		(
			TransactionGuid,
			AuthorizationNumber, 
			SentFlag, 
			DateSent, 
			PIDXProfileGuid, 
			CompanyPersonnelToShipToBillToGuid, 
			BrokenBlend,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy,
			BOLVersion
		)
		VALUES
		(
			source.TransactionGuid,
			source.AuthorizationNumber, 
			source.SentFlag, 
			source.DateSent, 
			source.PIDXProfileGuid, 
			source.CompanyPersonnelToShipToBillToGuid, 
			source.BrokenBlend,
			SYSDATETIMEOFFSET(), --CreatedDate
			source.CreatedUpdatedBy,
			SYSDATETIMEOFFSET(), --UpdatedDate
			source.CreatedUpdatedBy,
			source.BOLVersion
		);

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
						+ 'Procedure Name: dbo.usp_TransactionPIDXInsertUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 
