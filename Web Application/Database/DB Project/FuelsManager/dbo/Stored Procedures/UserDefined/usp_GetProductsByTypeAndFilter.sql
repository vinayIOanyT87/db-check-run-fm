

/*
	EXEC [dbo].[usp_GetProductsByTypeAndFilter] '00000000-0000-0000-0000-000000000001', NULL, NULL
	EXEC [dbo].[usp_GetProductsByTypeAndFilter] '00000000-0000-0000-0000-000000000001', 0, NULL
	EXEC [dbo].[usp_GetProductsByTypeAndFilter] '00000000-0000-0000-0000-000000000001', 0, '%HB%'

*/



CREATE PROCEDURE [dbo].[usp_GetProductsByTypeAndFilter]
(
	@TargetSiteGuid uniqueidentifier, @ProductType int, @SearchFilter nvarchar(100), @HideHiddenProducts BIT = 0,
	@limit int = 0
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetProductsByTypeAndFilter] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Product records that have a given Product Id and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	-- 2. @ProductType: Limit the results to Products that have a ProductType value that correspond to the @ProrductType value
	-- 3. @SearchFilter: Limit the results to Products that have an ID or Code or Description field value that contains the @SearchFilter value
	-- 4. @HideHiddenProducts: If true (1), only products with a NULL HiddenDate will be returned
	-- 5. This stored procedure replaces both the ProductClass.EnumerateByTypeAndFilterSQL inline SQL and the ProductClass.EnumerateByTypeSQL inline SQL.
	-- 6. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		SET @limit =  CASE WHEN @limit IS NOT NULL AND @limit !=0 THEN @limit ELSE 0x7fffff END;

		SELECT TOP (@limit) b.*, c.ProductID AS TrackingProductID
		FROM [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		LEFT OUTER JOIN tblProducts c
		ON c.ProductGuid = b.TrackingProductGuid
		WHERE ((b.LookupProductTypeIndex = @ProductType) OR (@ProductType IS NULL))
		AND
		(
			(@SearchFilter IS NULL) 
			OR (b.ProductID LIKE (UPPER(@SearchFilter)))
			OR (b.ProductCode LIKE (UPPER(@SearchFilter)))
			OR (b.Description LIKE (UPPER(@SearchFilter)))
		)
		AND (@HideHiddenProducts = 0 OR b.HiddenDate IS NULL)
	    ORDER BY b.ProductID

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
						+ 'Procedure Name: [dbo].usp_GetProductsByTypeAndFilter' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END