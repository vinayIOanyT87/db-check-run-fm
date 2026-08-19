

/*
	EXEC [dbo].[usp_GetProductByManagerAndTanks] '00000000-0000-0000-0000-000000000001', NULL
	EXEC [dbo].[usp_GetProductByManagerAndTanks] 'aeba18e3-e97b-479e-8b2d-0bcd69c1c421', 'HBCompany01'

*/



CREATE PROCEDURE [dbo].[usp_GetProductByManagerAndTanks]
(
	@TargetSiteGuid uniqueidentifier, @ManagerId nvarchar(100), @HideHiddenProducts BIT = 0
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetProductByManagerAndTanks] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Product records that have a given Product Id and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	-- 2. @ID: Limit the results to Products that have an ID value that correspond to the @Id value
	-- 3. @HideHiddenProducts: If true (1), only products with a NULL HiddenDate will be returned
	-- 4. This stored procedure replaces the ProductClass.EnumerateByManagerAndTanksSQL inline SQL.
	-- 5. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT b.*, c.ProductID AS TrackingProductID
		FROM [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		LEFT OUTER JOIN tblProducts c
		ON c.ProductGuid = b.TrackingProductGuid
		WHERE 
		(
			((@ManagerId IS NULL)
			OR
			(			
				a.MasterRecordGuid IN 
				(
					SELECT d.ProductGuid FROM tblTanks d
					WHERE d.ManagerCompanyGuid IN 
					(
						SELECT e.MasterRecordGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid) e
						INNER JOIN tblCompanies f
						ON f.CompanyGuid = e.CompanyGuid 
						WHERE f.[ID] = @ManagerID
					)
				) 
			))
			AND (@HideHiddenProducts = 0 OR b.HiddenDate IS NULL) 
		)
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
						+ 'Procedure Name: [dbo].usp_GetProductByManagerAndTanks' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
