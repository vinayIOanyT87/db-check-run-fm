
CREATE PROCEDURE [map].[usp_GetCompanyScheduleAccessBySite]
(
	@TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetCompanyScheduleAccessBySite] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the Product-To-Company mappings for a given CompanyGuid and Site/Sitegroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		SELECT sca.*  FROM tblScheduleCompanyAccess sca
		INNER JOIN (SELECT * FROM [erv].[udf_GetCompanyRecordVersions] (@TargetSiteGuid)) c	
		ON c.CompanyGuid = sca.CompanyGuid
		INNER JOIN map.tblCompanyToRole r
		ON r.CompanyGuid = c.MasterRecordGuid AND r.SiteGuid = @TargetSiteGuid
		WHERE r.LookupCompanyRoleIndex = 5 --Carrier
		ORDER BY CompanyGuid, LookupDayOfWeekIndex
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
						+ 'Procedure Name: [map].usp_GetCompanyScheduleAccessBySite' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     



