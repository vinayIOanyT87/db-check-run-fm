

/*
	EXEC [dbo].[usp_GetProductsById] '00000000-0000-0000-0000-000000000001', 'HBProduct02'

*/



CREATE PROCEDURE [dbo].[usp_GetProductsById]
(
	@TargetSiteGuid uniqueidentifier, @Id nvarchar(30)
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetProductsById] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Product records that have a given Product Id and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	-- 2. @ID: Limit the results to Products that have an ID value that correspond to the @Id value
	-- 3. This stored procedure replaces the ProductClass.SelectByIDSQL inline SQL for the case where the bInTransaction is false.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT b.*, c.ProductID AS TrackingProductID
		FROM [erv].[udf_GetProductRecordVersionsById](@TargetSiteGuid, @Id) a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		LEFT OUTER JOIN tblProducts c
		ON c.ProductGuid = b.TrackingProductGuid
		WHERE b.ProductID = @Id
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
						+ 'Procedure Name: [dbo].usp_GetProductsById' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END