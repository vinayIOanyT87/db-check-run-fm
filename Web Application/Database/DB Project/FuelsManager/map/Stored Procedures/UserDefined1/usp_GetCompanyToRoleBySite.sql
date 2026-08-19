
CREATE PROCEDURE [map].[usp_GetCompanyToRoleBySite]
(
	@TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetCompanyToRoleBySite] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the Company-To-Company Role mappings for a given Site/Sitegroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		
		SELECT role.* FROM map.tblCompanyToRole role
		INNER JOIN erv.udf_GetCompanyRecordVersions(@TargetSiteGuid) d
		ON d.MasterRecordGuid = role.CompanyGuid
		WHERE role.SiteGuid = @TargetSiteGuid

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
						+ 'Procedure Name: [map].usp_GetCompanyToRoleBySite' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     



