CREATE PROCEDURE [map].[usp_ExternalStationToProductUpdate]
	@ExternalStationToProductGuid UNIQUEIDENTIFIER, 
    @ExternalStationGuid UNIQUEIDENTIFIER, 
    @ExternalStationProduct NVARCHAR(50), 
    @ProductGuid UNIQUEIDENTIFIER, 
	@UpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		UPDATE map.tblExternalStationToProduct
		SET 
			ExternalStationGuid = @ExternalStationGuid, 
			ExternalStationProduct = @ExternalStationProduct, 
			ProductGuid = @ProductGuid,
			UpdatedBy = @UpdatedBy,
			UpdatedDate = SYSDATETIMEOFFSET()
		WHERE ExternalStationToProductGuid = @ExternalStationToProductGuid

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
						+ 'Procedure Name: map.usp_ExternalStationToProductUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH
END
