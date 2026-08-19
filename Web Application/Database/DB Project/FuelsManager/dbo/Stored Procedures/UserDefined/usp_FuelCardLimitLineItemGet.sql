CREATE PROCEDURE [dbo].[usp_FuelCardLimitLineItemGet]
	@SiteGuid UNIQUEIDENTIFIER,
	@FuelCardLimitLineItemGuid UNIQUEIDENTIFIER = NULL,
	@FuelCardLimitGuid UNIQUEIDENTIFIER = NULL,
	@ProductGuid UNIQUEIDENTIFIER = NULL,
	@ProductGroupApplicationStringGuid UNIQUEIDENTIFIER = NULL,
	@Period INT = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		-- Retrieve Fuel Card Limit Line Items using one of the following methods:
		-- 1. Get the Fuel Card Limit Line Item matching the primary key value provided
		-- 2. Get all Line Items belonging to the Fuel Card Limit matching the provided @FuelCardLimitGuid
		-- 3. Get the line item belonging to the Fuel Card Limit matching the provided @FuelCardLimitGuid with the same period and mapped to the same entity (product, product group, or all products).
		--    this is used for duplicate checking

		IF (@FuelCardLimitLineItemGuid IS NOT NULL)
		BEGIN
			--Retrieve the line item by its primary key
			SELECT 
				FuelCardLimitLineItemGuid,
				FuelCardLimitGuid,
				Limit,
				Period,
				tblFuelCardLimitLineItem.ProductGuid,
				ProductGroupApplicationStringGuid,
				AssignedProductGroupOrProductID = CASE WHEN ProductGroupApplicationStringGuid IS NOT NULL THEN tblApplicationString.ID
													WHEN tblFuelCardLimitLineItem.ProductGuid IS NOT NULL THEN products.ProductID
													ELSE NULL END, -- All products
				tblFuelCardLimitLineItem.CreatedBy,
				tblFuelCardLimitLineItem.CreatedDate,
				tblFuelCardLimitLineItem.UpdatedBy,
				tblFuelCardLimitLineItem.UpdatedDate
			FROM tblFuelCardLimitLineItem
			LEFT JOIN (SELECT tblProducts.ProductGuid, tblProducts._MasterRecordGuid, tblProducts.ProductID FROM tblProducts 
				INNER JOIN erv.udf_GetProductRecordVersions(@SiteGuid) productRecordVersions ON tblProducts.ProductGuid = productRecordVersions.ProductGuid) products
			ON products._MasterRecordGuid = tblFuelCardLimitLineItem.ProductGuid
			LEFT JOIN tblApplicationString ON tblApplicationString.ApplicationStringGuid = tblFuelCardLimitLineItem.ProductGroupApplicationStringGuid
			WHERE FuelCardLimitLineItemGuid = @FuelCardLimitLineItemGuid
		END
		ELSE  
		BEGIN
			IF (@Period IS NULL)
			BEGIN
				--Retrieve all line items belonging to the fuel card limit matching the provided FuelCardLimitGuid
				SELECT 
					FuelCardLimitLineItemGuid,
					FuelCardLimitGuid,
					Limit,
					Period,
					tblFuelCardLimitLineItem.ProductGuid,
					ProductGroupApplicationStringGuid,
					AssignedProductGroupOrProductID = CASE WHEN ProductGroupApplicationStringGuid IS NOT NULL THEN tblApplicationString.ID
									WHEN tblFuelCardLimitLineItem.ProductGuid IS NOT NULL THEN products.ProductID
									ELSE NULL END, -- All products
					tblFuelCardLimitLineItem.CreatedBy,
					tblFuelCardLimitLineItem.CreatedDate,
					tblFuelCardLimitLineItem.UpdatedBy,
					tblFuelCardLimitLineItem.UpdatedDate
				FROM tblFuelCardLimitLineItem
				LEFT JOIN (SELECT tblProducts.ProductGuid, tblProducts._MasterRecordGuid, tblProducts.ProductID FROM tblProducts 
					INNER JOIN erv.udf_GetProductRecordVersions(@SiteGuid) productRecordVersions ON tblProducts.ProductGuid = productRecordVersions.ProductGuid) products
				ON products._MasterRecordGuid = tblFuelCardLimitLineItem.ProductGuid
				LEFT JOIN tblApplicationString ON tblApplicationString.ApplicationStringGuid = tblFuelCardLimitLineItem.ProductGroupApplicationStringGuid
				WHERE FuelCardLimitGuid = @FuelCardLimitGuid
			END
			ELSE 
			BEGIN
				--Retrieve fuel card limits matching the provided FuelCardLimitGuid, period, and product, product group, or assigned to all products.
				--This is used for duplicate checking.
				IF (@ProductGuid IS NOT NULL)
				BEGIN
					SELECT 
						FuelCardLimitLineItemGuid,
						FuelCardLimitGuid,
						Limit,
						Period,
						tblFuelCardLimitLineItem.ProductGuid,
						ProductGroupApplicationStringGuid,
						AssignedProductGroupOrProductID = products.ProductID,
						tblFuelCardLimitLineItem.CreatedBy,
						tblFuelCardLimitLineItem.CreatedDate,
						tblFuelCardLimitLineItem.UpdatedBy,
						tblFuelCardLimitLineItem.UpdatedDate
					FROM tblFuelCardLimitLineItem
					LEFT JOIN (SELECT tblProducts.ProductGuid, tblProducts._MasterRecordGuid, tblProducts.ProductID FROM tblProducts 
						INNER JOIN erv.udf_GetProductRecordVersions(@SiteGuid) productRecordVersions ON tblProducts.ProductGuid = productRecordVersions.ProductGuid) products
					ON products._MasterRecordGuid = tblFuelCardLimitLineItem.ProductGuid
					WHERE FuelCardLimitGuid = @FuelCardLimitGuid
						AND tblFuelCardLimitLineItem.ProductGuid = @ProductGuid
						AND Period = @Period
				END
				ELSE IF (@ProductGroupApplicationStringGuid IS NOT NULL)
				BEGIN
					SELECT 
						FuelCardLimitLineItemGuid,
						FuelCardLimitGuid,
						Limit,
						Period,
						tblFuelCardLimitLineItem.ProductGuid,
						ProductGroupApplicationStringGuid,
						AssignedProductGroupOrProductID = tblApplicationString.ID,
						tblFuelCardLimitLineItem.CreatedBy,
						tblFuelCardLimitLineItem.CreatedDate,
						tblFuelCardLimitLineItem.UpdatedBy,
						tblFuelCardLimitLineItem.UpdatedDate
					FROM tblFuelCardLimitLineItem
					LEFT JOIN tblApplicationString ON tblApplicationString.ApplicationStringGuid = tblFuelCardLimitLineItem.ProductGroupApplicationStringGuid
					WHERE FuelCardLimitGuid = @FuelCardLimitGuid
						AND ProductGroupApplicationStringGuid = @ProductGroupApplicationStringGuid
						AND Period = @Period
				END
				ELSE 
				BEGIN
					SELECT 
						FuelCardLimitLineItemGuid,
						FuelCardLimitGuid,
						Limit,
						Period,
						ProductGuid,
						ProductGroupApplicationStringGuid,
						AssignedProductGroupOrProductID = NULL, -- All products
						CreatedBy,
						CreatedDate,
						UpdatedBy,
						UpdatedDate
					FROM tblFuelCardLimitLineItem
					WHERE FuelCardLimitGuid = @FuelCardLimitGuid
						AND ProductGroupApplicationStringGuid IS NULL
						AND ProductGuid IS NULL
						AND Period = @Period
				END
			END	
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
						+ 'Procedure Name: usp_FuelCardLimitLineItemGet' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END