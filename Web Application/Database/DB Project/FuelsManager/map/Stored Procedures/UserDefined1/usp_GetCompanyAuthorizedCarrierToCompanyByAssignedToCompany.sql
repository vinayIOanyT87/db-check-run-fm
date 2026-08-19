
/*
	DROP PROCEDURE [map].[usp_GetCompanyAuthorizedCarrierToCompanyByAssignedToCompany]

	EXEC [map].[usp_GetCompanyAuthorizedCarrierToCompanyByAssignedToCompany] '25373e4c-9daa-4146-a28b-fafbd3558286', '6f38ff9e-d815-4e5b-b6b6-e6eac0b1b76b'
	EXEC [map].[usp_GetCompanyAuthorizedCarrierToCompanyByAssignedToCompany] '830EFAA0-23A6-42C3-888E-5E1033B0E1AE', 'DF5060D4-25E4-4F56-AE46-50C25331863E'


*/

CREATE PROCEDURE [map].[usp_GetCompanyAuthorizedCarrierToCompanyByAssignedToCompany]
(
	@AssignedToCompanyGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetCompanyAuthorizedCarrierToCompanyByAssignedToCompany]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2013-04-04 14:21:10.4470770 -10:00
	-- Purpose: Retrieves the Company AuthorisedCarrier-To-Company mappings for a given AssignedToCompanyGuid and Site/Sitegroup.
	-- Notes:
	-- 1. @CompanyGuid: AssignedToCompanyGuid for which to fetch the mapping records.
	-- 2. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
	-- 3. This stored procedure replaces the CompanyMapClass.EnumerateByAssignedToGuidAndTypeSQL() inline SQL, for the case where Type = COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP and where bInTransaction is false.
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
		LEFT OUTER JOIN tblCompanies c
		ON c.CompanyGuid = a.CompanyGuid
		INNER JOIN erv.udf_GetCompanyRecordVersions(@TargetSiteGuid) d
		ON d.CompanyGuid = a.AssignedToCompanyGuid
		WHERE a.AssignedToCompanyGuid = @AssignedToCompanyGuid
		ORDER BY AssignedID ASC

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
						+ 'Procedure Name: [map].[usp_GetCompanyAuthorizedCarrierToCompanyByAssignedToCompany]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
