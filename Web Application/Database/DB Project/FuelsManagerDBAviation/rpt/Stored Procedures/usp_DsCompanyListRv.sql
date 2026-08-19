CREATE PROCEDURE [rpt].[usp_DsCompanyListRv]
@SiteGuid UNIQUEIDENTIFIER,
@UserGuid UNIQUEIDENTIFIER,
@Role INTEGER,
@ActiveOnly BIT
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_DsCompanyListRv] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-05-08 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the Companies based Role and accessiblity for reports.
	-- Notes:
	-- 1. @SiteGuid: Limit results to companies that have been assigned to this site/sitegroup only
	-- 2. @UserGuid: Limit results to companies that are authorized for this user only
	-- 3. @Role: Limit results to companies that have been granted this specific role.
	-- 2. @ActiveOnly: Limit results to companies based upon their locked out status.
	--
	-- 2013-Nov-27 Removed INNER JOIN dbo.udf_AuthorizedCompaniesGuid(@SiteGuid,@UserGuid)
	--				because vendors, managers, etc need to be available as parameters even though
	--				the users does not have access to all the records associated with that entity.
	--				When the report is created, authorization will be validated for each displayed row.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		IF @ActiveOnly = 1
			BEGIN
				SELECT DISTINCT ba.ID AS CompanyName, ba._MasterRecordGuid
				FROM
				erv.udf_GetCompanyRecordVersions (@SiteGuid) aa 
				INNER JOIN tblCompanies ba 
				ON aa.CompanyGuid = ba.CompanyGuid
				INNER JOIN [map].[tblCompanyToRole] b 
				ON b.CompanyGuid = ba._MasterRecordGuid
				WHERE b.LookupCompanyRoleIndex = @Role
				AND b.SiteGuid = @SiteGuid
				AND ba.LockedOut = 0
				ORDER BY ba.ID
			END
		ELSE
			BEGIN
				SELECT ba.ID AS CompanyName, ba._MasterRecordGuid
				FROM
				erv.udf_GetCompanyRecordVersions (@SiteGuid) aa 
				INNER JOIN tblCompanies ba 
				ON aa.CompanyGuid = ba.CompanyGuid
				INNER JOIN [map].[tblCompanyToRole] b 
				ON b.CompanyGuid = ba._MasterRecordGuid
				WHERE b.LookupCompanyRoleIndex = @Role
				AND b.SiteGuid = @SiteGuid
				ORDER BY ba.ID
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
						+ 'Procedure Name: [rpt].[usp_DsCompanyListRv]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END