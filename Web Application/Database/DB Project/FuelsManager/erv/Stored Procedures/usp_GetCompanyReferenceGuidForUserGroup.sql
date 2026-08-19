
------------------------------------------------------------------------------------------------------
-- Stored Procedure: [erv].[usp_GetCompanyReferenceGuidForUserGroup] 
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Returns the specific Guid (MasterRecordGuid or Child Record Version Guid) of a given company to be
--			used by a UserGroup in a Company-UserGroup mapping based on the Company Record Versioning state
--          and the FLC configuration on the UserGroup external field at the target site.
-- Notes: 
-- 1. @CompanyGuid: Record Guid of the Company record  (MasterRecordGuid or child record guid)
-- 2. @SiteGuid: Site/SiteGroup at which the mapping is to be created.	
--
-- Testing:
--	EXEC [erv].[usp_GetCompanyReferenceGuidForUserGroup] 'FED4BC88-D675-44CA-857C-79EB9E9D27CB', 'BEF999D0-75AB-400D-B890-691D46DC866F'
------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [erv].[usp_GetCompanyReferenceGuidForUserGroup]
(
	@CompanyGuid uniqueidentifier, @SiteGuid uniqueidentifier
)
	AS
	BEGIN
		BEGIN TRY	
			DECLARE @result uniqueidentifier
			SET @result = NULL

			DECLARE @EntityMasterRecGuid uniqueidentifier
			DECLARE @AssignedFromSiteGuid uniqueidentifier
			SELECT @EntityMasterRecGuid = _MasterRecordGuid FROM dbo.tblCompanies WHERE CompanyGuid = @CompanyGuid

			SELECT @AssignedFromSiteGuid = AssignedFromSiteGuid 
			FROM map.tblEntityCompanyToSite
			WHERE CompanyGuid = @EntityMasterRecGuid
			AND SiteGuid = @SiteGuid

			IF (@AssignedFromSiteGuid IS NOT NULL)
			BEGIN
				SET @result = @EntityMasterRecGuid

				DECLARE @callingRef1Guid uniqueidentifier
				SELECT @callingRef1Guid = NEWID()

				EXEC erv.usp_GetRecordVersioningFields 'Company', @EntityMasterRecGuid, @AssignedFromSiteGuid, 'VersionSpecific', @callingRef1Guid		

				IF EXISTS 
				(
					SELECT * FROM erv.tblTempRecordVersioningField
					WHERE _CallingReferenceGuid = @callingRef1Guid
					AND TargetField = 'UserGroups'
				)
				BEGIN
					SELECT @result = CompanyGuid FROM dbo.tblCompanies 
					WHERE _MasterRecordGuid = @EntityMasterRecGuid
					AND SiteGuid = @SiteGuid
				END
			END
			SELECT @result AS CompanyGuid

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
						+ 'Procedure Name: [erv].usp_GetCompanyReferenceGuidForUserGroup' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 
