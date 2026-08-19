CREATE PROCEDURE [dbo].[usp_TransactionTransportLineItemsInsertUpdate]
(
	@TransactionTransportLineItems dbo.TransactionTransportLineItemsType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY

		MERGE tblTransactionTransportLineItems AS target
		USING (
			SELECT
				TransactionTransportLineItemGuid, 
				TransactionGuid,
				TransportOrderNumber,
				TransVersion,
				LocationName,
				Address1,
				Address2,
				City,
				[State],
				Zip,
				POCName,
				POCPhone,
				CreatedUpdatedBy
			FROM @TransactionTransportLineItems
			) AS source
		ON source.TransactionTransportLineItemGuid = target.TransactionTransportLineItemGuid
		WHEN MATCHED THEN UPDATE SET
			TransVersion = source.TransVersion,
			LocationName = source.LocationName,
			Address1 = source.Address1,
			Address2 = source.Address2,
			City = source.City,
			[State] = source.[State],
			Zip = source.Zip,
			POCName = source.POCName,
			POCPhone = source.POCPhone,
			UpdatedDate = SYSDATETIMEOFFSET(),
			UpdatedBy = source.CreatedUpdatedBy
		WHEN NOT MATCHED THEN INSERT 
		(
			TransactionGuid,
			TransportOrderNumber,
			TransVersion,
			LocationName,
			Address1,
			Address2,
			City,
			[State],
			Zip,
			POCName,
			POCPhone,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		)
		VALUES
		(
			source.TransactionGuid,
			source.TransportOrderNumber,
			source.TransVersion,
			source.LocationName,
			source.Address1,
			source.Address2,
			source.City,
			source.[State],
			source.Zip,
			source.POCName,
			source.POCPhone,	
			SYSDATETIMEOFFSET(), --CreatedDate
			source.CreatedUpdatedBy,
			SYSDATETIMEOFFSET(), --UpdatedDate
			source.CreatedUpdatedBy
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
						+ 'Procedure Name: dbo.usp_TransactionTransportLineItemsInsertUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 
