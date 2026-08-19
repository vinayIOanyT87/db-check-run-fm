

/*
	EXEC [dbo].[usp_GetProductsByCode] '00000000-0000-0000-0000-000000000001', 'ProdCode_SGX'
	EXEC [dbo].[usp_GetProductsByCode] 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421', 'PC_Site2_001'	

*/


CREATE PROCEDURE [dbo].[usp_GetProductsByCode]
(
	@TargetSiteGuid uniqueidentifier, @ProductCode nvarchar(15)
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetProductsByCode] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Product records that have a given Product Code and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	-- 2. @ProductCode: Limit the results to Products that have a ProductCode value that correspond to the @ProductCode value
	-- 3. This stored procedure replaces both the ProductClass.SelectByCodeSQL inline SQL.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT b.*, c.ProductID AS TrackingProductID
		FROM [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		LEFT OUTER JOIN tblProducts c
		ON c.ProductGuid = b.TrackingProductGuid
		WHERE (b.ProductCode = @ProductCode)
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
						+ 'Procedure Name: [dbo].usp_GetProductsByCode' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END