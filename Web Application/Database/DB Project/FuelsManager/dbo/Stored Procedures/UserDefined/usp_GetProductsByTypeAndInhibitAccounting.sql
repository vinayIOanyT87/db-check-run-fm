/*
	EXEC [dbo].[usp_GetProductsByTypeAndInhibitAccounting] '00000000-0000-0000-0000-000000000001', NULL, NULL
	EXEC [dbo].[usp_GetProductsByTypeAndInhibitAccounting] '00000000-0000-0000-0000-000000000001', 0, NULL
	EXEC [dbo].[usp_GetProductsByTypeAndInhibitAccounting] '00000000-0000-0000-0000-000000000001', 0, 1
	EXEC [dbo].[usp_GetProductsByTypeAndInhibitAccounting] '00000000-0000-0000-0000-000000000001', 0, 0
*/

CREATE PROCEDURE [dbo].[usp_GetProductsByTypeAndInhibitAccounting]
(
	@TargetSiteGuid uniqueidentifier, @ProductType int, @InhibitAccounting bit
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetProductsByTypeAndInhibitAccounting] 
	-- Author: Richard R. Panachida
	-- Version/Date: 1.0.003 / 2015-02-03
	-- Purpose: Retrieve the Product records for a given site, product type, and inhibit accounting flag.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	-- 2. @ProductType: Limit the results to Products that have a ProductType value that correspond to the @ProrductType value
	-- 2. @InhibitAccounting: Limit the results to Products that equals the inhibit accounting flag.
	-- 3. This stored procedure replaces both the ProductClass.EnumerateByTypeAndInhibitAccountingSQL inline SQL and 
	--    the ProductClass.EnumerateByTypeSQL inline SQL.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT b.*, c.ProductID AS TrackingProductID
		FROM [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) a
			 INNER JOIN tblProducts b ON b.ProductGuid = a.ProductGuid
			 LEFT OUTER JOIN tblProducts c ON c.ProductGuid = b.TrackingProductGuid
		WHERE (b.LookupProductTypeIndex = @ProductType OR @ProductType IS NULL)
			  AND (@InhibitAccounting IS NULL OR b.InhibitAccounting = @InhibitAccounting)
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
						+ 'Procedure Name: [dbo].usp_GetProductsByTypeAndInhibitAccounting' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
GO



