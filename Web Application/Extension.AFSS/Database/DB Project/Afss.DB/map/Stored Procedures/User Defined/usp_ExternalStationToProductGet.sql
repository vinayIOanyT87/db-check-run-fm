CREATE PROCEDURE [map].[usp_ExternalStationToProductGet]
	@ExternalStationToProductGuid UNIQUEIDENTIFIER = NULL,
	@ExternalStationGuid UNIQUEIDENTIFIER = NULL,
	@ExternalStationProduct NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		IF (@ExternalStationToProductGuid IS NOT NULL)
		BEGIN
			SELECT 
				map.tblExternalStationToProduct.ExternalStationToProductGuid, 
				map.tblExternalStationToProduct.ExternalStationGuid, 
				map.tblExternalStationToProduct.ExternalStationProduct, 
				map.tblExternalStationToProduct.ProductGuid,
				map.tblExternalStationToProduct.CreatedBy,
				map.tblExternalStationToProduct.CreatedDate,
				map.tblExternalStationToProduct.UpdatedBy,
				map.tblExternalStationToProduct.UpdatedDate,
				tblProducts.ProductID
			FROM map.tblExternalStationToProduct
			INNER JOIN tblProducts ON tblProducts.ProductGuid = map.tblExternalStationToProduct.ProductGuid
			-- When retrieving products, keep record versioning in mind. We store the product MasterRecordGuid in the product mapping table
			-- Since we're only getting the ID of the product here, it's OK to join on tblProducts.ProductGuid. ID can't change between master and child record versions. 
			-- But if you want to retrieve any other fields, you'd need to use erv.udf_GetProductRecordVersions
			WHERE ExternalStationToProductGuid = @ExternalStationToProductGuid
		END
		ELSE 
		BEGIN
			SELECT 
				map.tblExternalStationToProduct.ExternalStationToProductGuid, 
				map.tblExternalStationToProduct.ExternalStationGuid, 
				map.tblExternalStationToProduct.ExternalStationProduct, 
				map.tblExternalStationToProduct.ProductGuid,
				map.tblExternalStationToProduct.CreatedBy,
				map.tblExternalStationToProduct.CreatedDate,
				map.tblExternalStationToProduct.UpdatedBy,
				map.tblExternalStationToProduct.UpdatedDate,
				tblProducts.ProductID
			FROM map.tblExternalStationToProduct
			INNER JOIN tblProducts ON tblProducts.ProductGuid = map.tblExternalStationToProduct.ProductGuid
			-- When retrieving products, keep record versioning in mind. We store the product MasterRecordGuid in the product mapping table
			-- Since we're only getting the ID of the product here, it's OK to join on tblProducts.ProductGuid. ID can't change between master and child record versions. 
			-- But if you want to retrieve any other fields, you'd need to use erv.udf_GetProductRecordVersions
			WHERE ExternalStationGuid = @ExternalStationGuid AND ExternalStationProduct = @ExternalStationProduct

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
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: map.usp_ExternalStationToProductGet' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END