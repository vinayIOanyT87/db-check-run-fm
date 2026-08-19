
/*
	DROP PROCEDURE [map].[usp_GetCompanyAuthorizedCarrierToCompanyByCompany]

	EXEC [map].[usp_GetCompanyAuthorizedCarrierToCompanyByCompany] 'BF388E65-1360-41BA-9137-877D59C2409E', '478DDA64-F01D-4B87-AF63-25A989B7A06F'
	EXEC [map].[usp_GetCompanyAuthorizedCarrierToCompanyByCompany] '2CC31985-09A5-45F8-86B7-63B6402460AB', 'DF5060D4-25E4-4F56-AE46-50C25331863E'


*/

CREATE PROCEDURE [map].[usp_GetCompanyAuthorizedCarrierToCompanyByCompany]
(
	@CompanyGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetCompanyAuthorizedCarrierToCompanyByCompany]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the Company AuthorisedCarrier-To-Company mappings for a given CompanyGuid and Site/Sitegroup.
	-- Notes:
	-- 1. @CompanyGuid: CompanyGuid for which to fetch the mapping records.
	-- 2. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	-- 3. This stored procedure replaces the CompanyMapClass.EnumerateByAssignedGuidAndTypeSQL() inline SQL, for the case where Type = COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP.
	-- 4. This query is Company Record Versioning-aware. It examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), 
	--    and record versions that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF)
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		
		SELECT a.*, 
		b.ID AS AssignedToID, b.Name AS AssignedToName, b.Address1 AS AssignedToAddress, b.City AS AssignedToCity, b.State AS AssignedToState,
		c.ID AS AssignedID, c.LockedOut AS LockedOut, c.Name AS AssignedName, c.Address1 AS AssignedAddress, c.City AS AssignedCity, c.State AS AssignedState
		FROM map.tblCompanyAuthorizedCarrierToCompany a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		INNER JOIN tblCompanies c
		ON c.CompanyGuid = a.CompanyGuid
		INNER JOIN erv.udf_GetCompanyRecordVersions(@TargetSiteGuid) d
		ON d.CompanyGuid = a.CompanyGuid
		WHERE a.CompanyGuid = @CompanyGuid
		AND a.SiteGuid IN (SELECT SiteGuid FROM [dbo].[udf_GetSiteToSiteHierarchyListForSiteGuid]( @TargetSiteGuid,0,0,0,1,0,0))
		UNION ALL
		SELECT a.*, 
		b.ID AS AssignedToID, b.Name AS AssignedToName, b.Address1 AS AssignedToAddress, b.City AS AssignedToCity, b.State AS AssignedToState,
		NULL AS AssignedID, NULL AS LockedOut, NULL AS AssignedName, NULL AS AssignedAddress, NULL AS AssignedCity, NULL AS AssignedState
		FROM map.tblCompanyAuthorizedCarrierToCompany a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		WHERE a.CompanyGuid IS NULL
		AND a.SiteGuid IN (SELECT SiteGuid FROM [dbo].[udf_GetSiteToSiteHierarchyListForSiteGuid]( @TargetSiteGuid,0,0,0,1,0,0))
		ORDER BY AssignedToID ASC

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
						+ 'Procedure Name: [map].[usp_GetCompanyAuthorizedCarrierToCompanyByCompany]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
