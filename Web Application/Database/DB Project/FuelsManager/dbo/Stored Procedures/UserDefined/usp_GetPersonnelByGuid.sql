
/*
	DROP PROCEDURE [dbo].[usp_GetPersonnelByGuid]

	EXEC [dbo].[usp_GetPersonnelByGuid] NULL, 'E1BDCCC7-75AC-4CD9-A180-1BCCEE891768'
	EXEC [dbo].[usp_GetPersonnelByGuid] '6F38FF9E-D815-4E5B-B6B6-E6EAC0B1B76B', '80B08634-D356-4569-B9A2-CD36DF955BD0'
	EXEC [dbo].[usp_GetPersonnelByGuid] '6F38FF9E-D815-4E5B-B6B6-E6EAC0B1B76B', '605E190D-3AF5-4287-9C11-8953AA3D009F'
*/

CREATE PROCEDURE [dbo].[usp_GetPersonnelByGuid]
(
	@TargetSiteGuid uniqueidentifier, @PersonnelGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetPersonnelByGuid] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-03-29 10:42:10.4470770 -10:00
	-- Purpose: Retrieve the Personnel records that have a given Personnel Id and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @PersonnelGuid: If @TargetSiteGuid is null, then @PersonnelGuid is the Guid of the Personnel to retrieve. Otherwise, it is the Guid that is used to retrieve the MasterRecordGuid of the Personnel record to retrieve.
	-- 2. @TargetSiteGuid: If TargetSiteGuid is not null, then it is used as the target owner site of the record version that needs to be retrieved.
	-- 3. This query can be used in two modes: 
	--		(a) When the exact GUID of the target Personnel record is known, in which case the @TargetSiteGuid can be left null.
	--		(b) When trying to verify if a Personnel record has a record version (child or parent) against a specific site/sitegroup, in which case the @TargetSiteGuid must be provided.
	-- 4. This stored procedure replaces the PersonnelClass.SelectSQL inline SQL for the case where the bInTransaction parameter is false.
	-- 5. The SiteGuid of the master record is included in the resultset to support the decryption of the Personnel.PINNumber in child record versions.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @masterRecordGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid FROM tblPersonnel
		WHERE PersonnelGuid = @PersonnelGuid
		
		DECLARE @targetRecordGuid uniqueidentifier
		SET @targetRecordGuid = NULL
		IF (@TargetSiteGuid IS NOT NULL)
		BEGIN
			SELECT @targetRecordGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', @masterRecordGuid, @TargetSiteGuid)
		END
		ELSE
		BEGIN
			SET @targetRecordGuid = @PersonnelGuid
		END

		SELECT a.*, 
		C.ID AS CompanyID,C.Name AS CompanyName,C.Address1 AS CompanyAddress,C.City AS CompanyCity,C.State AS CompanyState,
		D.UserID AS UserID,
		E.PersonID AS SupervisorID,
		F.ID AS AssignedEquipmentID,
		(SELECT COUNT(*) FROM map.tblCompanyPersonnelAssignedToCompany CPA WHERE a.PersonnelGuid = CPA.PersonnelGuid) AS AssignedCompaniesCount,
		b.AssignedFromSiteGuid AS MasterSiteGuid
		FROM tblPersonnel a
		INNER JOIN map.tblEntityPersonnelToSite b
		ON b.PersonnelGuid = a._MasterRecordGuid
		LEFT JOIN tblCompanies C   
		ON a.CompanyGuid = C.CompanyGuid
		LEFT JOIN tblUsers D  
		ON D.UserGuid = a.UserGuid
		LEFT JOIN tblPersonnel E 
		ON a.SupervisorPersonnelGuid = E.PersonnelGuid
		LEFT JOIN tblEquipment F 
		ON a.AssignedEquipmentGuid = F.EquipmentGuid
		WHERE a.PersonnelGuid = @targetRecordGuid
		AND b.AssignedFromSiteGuid = b.SiteGuid

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
						+ 'Procedure Name: [dbo].usp_GetPersonnelByGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END