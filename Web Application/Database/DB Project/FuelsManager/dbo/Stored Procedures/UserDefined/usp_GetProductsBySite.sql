

/*
	EXEC [dbo].[usp_GetProductsBySite] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'
	EXEC [dbo].[usp_GetProductsBySite] 'df5060d4-25e4-4f56-ae46-50c25331863e'
	
*/


CREATE PROCEDURE [dbo].[usp_GetProductsBySite]
(
	@TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetProductsBySite] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve all the Product records for a given target site/sitegroup, together with their entity assignment details.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	-- 2. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

		SELECT b.ProductId, b.SiteGuid, b.ProductGuid, b._MasterRecordGuid, c.SiteGuid AssignedToSiteGuid, c.AssignedFromSiteGuid, d.Id AssignedFromSiteId   
		FROM [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblProducts b ON b.ProductGuid = a.ProductGuid
		INNER JOIN map.tblEntityProductToSite c WITH (NOLOCK) ON c.ProductGuid = b._MasterRecordGuid
        INNER JOIN tblSites d WITH (NOLOCK) ON d.SiteGuid = c.AssignedFromSiteGuid 
        WHERE c.SiteGuid = @TargetSiteGuid
        ORDER BY b.ProductId

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
						+ 'Procedure Name: [dbo].usp_GetProductsBySite' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END