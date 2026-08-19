CREATE PROCEDURE [dbo].[usp_FuelCardLimitLineItemUpdate]
	@FuelCardLimitLineItemGuid UNIQUEIDENTIFIER,
	@Limit FLOAT,
	@Period INT,
	@ProductGuid UNIQUEIDENTIFIER = NULL,
	@ProductGroupApplicationStringGuid UNIQUEIDENTIFIER = NULL,
	@UpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		UPDATE tblFuelCardLimitLineItem
		SET
			Limit = @Limit,
			Period = @Period,
			ProductGuid = @ProductGuid,
			ProductGroupApplicationStringGuid = @ProductGroupApplicationStringGuid,
			UpdatedBy = @UpdatedBy,
			UpdatedDate = SYSDATETIMEOFFSET()
		WHERE FuelCardLimitLineItemGuid = @FuelCardLimitLineItemGuid

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
						+ 'Procedure Name: usp_FuelCardLimitLineItemUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
