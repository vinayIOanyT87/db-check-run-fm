/*
	DROP PROCEDURE [dbo].[usp_GetPersonnelEnumerateByRole]

	EXEC [dbo].[usp_GetPersonnelEnumerateByRole] 'AD74B677-F294-4BF8-8861-30D6B424ADC6', '2', 0, 1, 10000, 0
	EXEC [dbo].[usp_GetPersonnelEnumerateByRole] 'AD74B677-F294-4BF8-8861-30D6B424ADC6', '2', 1, 1, 10000, 0


*/



CREATE PROCEDURE [dbo].[usp_GetPersonnelEnumerateByRole]
(
	@TargetSiteGuid uniqueidentifier, @Role int, @StandardSelect bit, @Limit int, @HideHiddenPersonnel BIT = 0
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetPersonnelEnumerateByRole] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the Personnel records that have a given Personnel Id and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Personnel that have been assigned to this site/sitegroup only
	-- 2. @Role: Limit results to Personnel that have the specific role specified as the role index.
	-- 3. @InTransaction: Indicates whether a lock is set.
	-- 4. @StandardSelect: Indicates a different query.  I am keeping this together to be consistent with the old code, but this looks like a hack
	-- 5. @Limit: Limit number of results returned.  If @Limit <= 0 then all Distinct elements are returned
	-- 6. @HideHiddenPersonnel: If true (1), only personnel with a NULL HiddenDate will be returned. If @StandardSelect = 0, the @HideHiddenPersonnel value is not honored.
	-- 7. This stored procedure replaces the PersonnelClass.EnumerateByRoleSQL inline SQL.  
	-- 8. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	-- 9. The SiteGuid of the master record is included in the resultset to support the decryption of the Personnel.PINNumber in child record versions.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		Declare @IsMaxRole int
		Set @IsMaxRole = (SELECT Count(*) FROM lookup.tblPersonnelRole R WHERE R.PersonnelRoleCode = 'MAX_PERSON_ROLE' AND R.PersonnelRoleIndex = @Role)
		BEGIN
		IF(@StandardSelect = 1)
			BEGIN
			IF(@IsMaxRole > 0)
				BEGIN
				--GetPersonnelEnumeratByMaxRole
				IF(@Limit > 0)
					BEGIN
						SELECT TOP(@Limit) 
						b.PersonnelGuid,
						b._MasterRecordGuid,
						b.SiteGuid,
						b.PersonID,
						b.CardNumber,
						b.UserGuid,
						b.FirstName,
						b.MiddleName,
						b.LastName,
						b.Title,
						b.Department,
						b.SupervisorPersonnelGuid,
						b.Address1,
						b.Address2,
						b.City,
						b.State,
						b.Zip,
						b.Country,
						b.Phone1,
						b.Phone2,
						b.AssignmentDate,
						b.SupervisionDate,
						b.SSAN,
						b.BirthDate,
						b.PayRate,
						b.LaborRate1,
						b.LaborRate2,
						b.LaborRate3,
						b.LaborRate4,
						b.Status,
						b.Email,
						b.ResponsibleOfficer,
						b.Shift,
						MPC.CompanyGuid,
						b.PINNumber,
						b.PINRequired,
						b.LockedOut,
						b.LockedOutReason,
						b.LockedOutDate,
						b.LastActivityDate,
						b.CardedIn,
						b.ShortCardNumber,
						b.HiddenDate,
						b.AssignedEquipmentGuid,
						b.CreatedDate,
						b.CreatedBy,
						b.UpdatedDate,
						b.UpdatedBy,
						b.UserData1,
						b.UserData2,
						b.UserData3,
						b.UserData4,
						b.UserData5,
						b.UserData6,
						b.UserData7,
						b.UserData8,
						b.UserData9,
						b.UserData10,
						b.UserData11,
						b.UserData12,
						b.UserData13,
						b.UserData14,
						b.UserData15,
						b.UserData16,
						b.UserData17,
						b.UserData18,
						b.UserData19,
						b.UserData20,
						b.UserData21,
						b.UserData22,
						b.UserData23,
						b.UserData24,
						b.InhibitInactivityLockout,
						b._RowVersion,
						C.ID AS CompanyID,C.Name AS CompanyName,C.Address1 AS CompanyAddress,C.City AS CompanyCity,C.State AS CompanyState,
						D.UserID AS UserID,
						E.PersonID AS SupervisorID,
						F.ID AS AssignedEquipmentID,
						a.AssignedFromSiteGuid AS ASSIGNEDFROMSITEGUID,
						a.AssignedToSiteGuid AS ASSIGNEDTOSITEGUID,
						G.ID AS ASSIGNEDFROMSITEID,
						(SELECT COUNT(*) FROM map.tblCompanyPersonnelAssignedToCompany CPA WHERE b.PersonnelGuid = CPA.PersonnelGuid) AS AssignedCompaniesCount,
						a.MasterSiteGuid
						FROM 
						[erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) a 
						INNER JOIN tblPersonnel b 
						ON a.PersonnelGuid = b.PersonnelGuid
						LEFT JOIN map.tblCompanyPersonnelAssignedToCompany MPC
						ON b._MasterRecordGuid=MPC.PersonnelGuid
						LEFT JOIN tblCompanies C   
        					ON C._MasterRecordGuid = MPC.CompanyGuid
        					AND C.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', MPC.CompanyGuid, @TargetSiteGuid)
        					LEFT JOIN tblUsers D  
        					ON D.UserGuid = b.UserGuid
        					LEFT JOIN tblPersonnel E 
        					ON E._MasterRecordGuid = b.SupervisorPersonnelGuid
        					AND E.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', b.SupervisorPersonnelGuid, @TargetSiteGuid)
        					LEFT JOIN tblEquipment F 
						ON F._MasterRecordGuid = b.AssignedEquipmentGuid
        					AND F.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', b.AssignedEquipmentGuid, @TargetSiteGuid)
						LEFT JOIN tblSites G
						ON a.AssignedFromSiteGuid = G.SiteGuid
						WHERE (@HideHiddenPersonnel = 0 OR b.HiddenDate IS NULL)
						ORDER BY b.PersonID ASC
					END
				ELSE
					BEGIN
						SELECT DISTINCT 
						b.PersonnelGuid,
						b._MasterRecordGuid,
						b.SiteGuid,
						b.PersonID,
						b.CardNumber,
						b.UserGuid,
						b.FirstName,
						b.MiddleName,
						b.LastName,
						b.Title,
						b.Department,
						b.SupervisorPersonnelGuid,
						b.Address1,
						b.Address2,
						b.City,
						b.State,
						b.Zip,
						b.Country,
						b.Phone1,
						b.Phone2,
						b.AssignmentDate,
						b.SupervisionDate,
						b.SSAN,
						b.BirthDate,
						b.PayRate,
						b.LaborRate1,
						b.LaborRate2,
						b.LaborRate3,
						b.LaborRate4,
						b.Status,
						b.Email,
						b.ResponsibleOfficer,
						b.Shift,
						b.CompanyGuid,
						b.PINNumber,
						b.PINRequired,
						b.LockedOut,
						b.LockedOutReason,
						b.LockedOutDate,
						b.LastActivityDate,
						b.CardedIn,
						b.ShortCardNumber,
						b.HiddenDate,
						b.AssignedEquipmentGuid,
						b.CreatedDate,
						b.CreatedBy,
						b.UpdatedDate,
						b.UpdatedBy,
						b.UserData1,
						b.UserData2,
						b.UserData3,
						b.UserData4,
						b.UserData5,
						b.UserData6,
						b.UserData7,
						b.UserData8,
						b.UserData9,
						b.UserData10,
						b.UserData11,
						b.UserData12,
						b.UserData13,
						b.UserData14,
						b.UserData15,
						b.UserData16,
						b.UserData17,
						b.UserData18,
						b.UserData19,
						b.UserData20,
						b.UserData21,
						b.UserData22,
						b.UserData23,
						b.UserData24,
						b.InhibitInactivityLockout,
						b._RowVersion,
						C.ID AS CompanyID,C.Name AS CompanyName,C.Address1 AS CompanyAddress,C.City AS CompanyCity,C.State AS CompanyState,
						D.UserID AS UserID,
						E.PersonID AS SupervisorID,
						F.ID AS AssignedEquipmentID,
						a.AssignedFromSiteGuid AS ASSIGNEDFROMSITEGUID,
						a.AssignedToSiteGuid AS ASSIGNEDTOSITEGUID,
						G.ID AS ASSIGNEDFROMSITEID,
						(SELECT COUNT(*) FROM map.tblCompanyPersonnelAssignedToCompany CPA WHERE b.PersonnelGuid = CPA.PersonnelGuid) AS AssignedCompaniesCount,
						a.MasterSiteGuid
						FROM 
						[erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) a 
						INNER JOIN tblPersonnel b 
						ON a.PersonnelGuid = b.PersonnelGuid
						LEFT JOIN map.tblCompanyPersonnelAssignedToCompany MPC
						ON b._MasterRecordGuid=MPC.PersonnelGuid
						LEFT JOIN tblCompanies C   
        					ON C._MasterRecordGuid = MPC.CompanyGuid
        					AND C.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', MPC.CompanyGuid, @TargetSiteGuid)
        					LEFT JOIN tblUsers D  
        					ON D.UserGuid = b.UserGuid
        					LEFT JOIN tblPersonnel E 
        					ON E._MasterRecordGuid = b.SupervisorPersonnelGuid
        					AND E.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', b.SupervisorPersonnelGuid, @TargetSiteGuid)
        					LEFT JOIN tblEquipment F 
						ON F._MasterRecordGuid = b.AssignedEquipmentGuid
        					AND F.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', b.AssignedEquipmentGuid, @TargetSiteGuid)
						LEFT JOIN tblSites G
						ON a.AssignedFromSiteGuid = G.SiteGuid
						WHERE (@HideHiddenPersonnel = 0 OR b.HiddenDate IS NULL)
						ORDER BY b.PersonID ASC
					END
				END
			ELSE
				BEGIN
				--GetPersonnelEnumeratByRole
				IF(@Limit > 0)
					BEGIN
						SELECT TOP(@Limit) 
						b.PersonnelGuid,
						b._MasterRecordGuid,
						b.SiteGuid,
						b.PersonID,
						b.CardNumber,
						b.UserGuid,
						b.FirstName,
						b.MiddleName,
						b.LastName,
						b.Title,
						b.Department,
						b.SupervisorPersonnelGuid,
						b.Address1,
						b.Address2,
						b.City,
						b.State,
						b.Zip,
						b.Country,
						b.Phone1,
						b.Phone2,
						b.AssignmentDate,
						b.SupervisionDate,
						b.SSAN,
						b.BirthDate,
						b.PayRate,
						b.LaborRate1,
						b.LaborRate2,
						b.LaborRate3,
						b.LaborRate4,
						b.Status,
						b.Email,
						b.ResponsibleOfficer,
						b.Shift,
						MPC.CompanyGuid,
						b.PINNumber,
						b.PINRequired,
						b.LockedOut,
						b.LockedOutReason,
						b.LockedOutDate,
						b.LastActivityDate,
						b.CardedIn,
						b.ShortCardNumber,
						b.HiddenDate,
						b.AssignedEquipmentGuid,
						b.CreatedDate,
						b.CreatedBy,
						b.UpdatedDate,
						b.UpdatedBy,
						b.UserData1,
						b.UserData2,
						b.UserData3,
						b.UserData4,
						b.UserData5,
						b.UserData6,
						b.UserData7,
						b.UserData8,
						b.UserData9,
						b.UserData10,
						b.UserData11,
						b.UserData12,
						b.UserData13,
						b.UserData14,
						b.UserData15,
						b.UserData16,
						b.UserData17,
						b.UserData18,
						b.UserData19,
						b.UserData20,
						b.UserData21,
						b.UserData22,
						b.UserData23,
						b.UserData24,
						b.InhibitInactivityLockout,
						b._RowVersion, 
						C.ID AS CompanyID,C.Name AS CompanyName,C.Address1 AS CompanyAddress,C.City AS CompanyCity,C.State AS CompanyState,
						D.UserID AS UserID,
						E.PersonID AS SupervisorID,
						F.ID AS AssignedEquipmentID,
						a.AssignedFromSiteGuid AS ASSIGNEDFROMSITEGUID,
						a.AssignedToSiteGuid AS ASSIGNEDTOSITEGUID,
						G.ID AS ASSIGNEDFROMSITEID,
						(SELECT COUNT(*) FROM map.tblCompanyPersonnelAssignedToCompany CPA WHERE b.PersonnelGuid = CPA.PersonnelGuid) AS AssignedCompaniesCount,
						a.MasterSiteGuid
						FROM 
						[erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) a 
						INNER JOIN tblPersonnel b 
						ON a.PersonnelGuid = b.PersonnelGuid
						LEFT JOIN map.tblCompanyPersonnelAssignedToCompany MPC
						ON b._MasterRecordGuid=MPC.PersonnelGuid
						LEFT JOIN tblCompanies C   
        					ON C._MasterRecordGuid = MPC.CompanyGuid
        					AND C.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', MPC.CompanyGuid, @TargetSiteGuid)
        					LEFT JOIN tblUsers D  
        					ON D.UserGuid = b.UserGuid
        					LEFT JOIN tblPersonnel E 
        					ON E._MasterRecordGuid = b.SupervisorPersonnelGuid
        					AND E.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', b.SupervisorPersonnelGuid, @TargetSiteGuid)
        					LEFT JOIN tblEquipment F 
						ON F._MasterRecordGuid = b.AssignedEquipmentGuid
        					AND F.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', b.AssignedEquipmentGuid, @TargetSiteGuid)
						LEFT JOIN tblSites G
						ON a.AssignedFromSiteGuid = G.SiteGuid
						INNER JOIN map.tblPersonnelToRole H 
						ON b.PersonnelGuid = H.PersonnelGuid
						WHERE H.LookupPersonnelRoleIndex = @Role
						AND (@HideHiddenPersonnel = 0 OR b.HiddenDate IS NULL)
						ORDER BY b.PersonID ASC
					END
				ELSE
					BEGIN
						SELECT DISTINCT
						b.PersonnelGuid,
						b._MasterRecordGuid,
						b.SiteGuid,
						b.PersonID,
						b.CardNumber,
						b.UserGuid,
						b.FirstName,
						b.MiddleName,
						b.LastName,
						b.Title,
						b.Department,
						b.SupervisorPersonnelGuid,
						b.Address1,
						b.Address2,
						b.City,
						b.State,
						b.Zip,
						b.Country,
						b.Phone1,
						b.Phone2,
						b.AssignmentDate,
						b.SupervisionDate,
						b.SSAN,
						b.BirthDate,
						b.PayRate,
						b.LaborRate1,
						b.LaborRate2,
						b.LaborRate3,
						b.LaborRate4,
						b.Status,
						b.Email,
						b.ResponsibleOfficer,
						b.Shift,
						b.CompanyGuid,
						b.PINNumber,
						b.PINRequired,
						b.LockedOut,
						b.LockedOutReason,
						b.LockedOutDate,
						b.LastActivityDate,
						b.CardedIn,
						b.ShortCardNumber,
						b.HiddenDate,
						b.AssignedEquipmentGuid,
						b.CreatedDate,
						b.CreatedBy,
						b.UpdatedDate,
						b.UpdatedBy,
						b.UserData1,
						b.UserData2,
						b.UserData3,
						b.UserData4,
						b.UserData5,
						b.UserData6,
						b.UserData7,
						b.UserData8,
						b.UserData9,
						b.UserData10,
						b.UserData11,
						b.UserData12,
						b.UserData13,
						b.UserData14,
						b.UserData15,
						b.UserData16,
						b.UserData17,
						b.UserData18,
						b.UserData19,
						b.UserData20,
						b.UserData21,
						b.UserData22,
						b.UserData23,
						b.UserData24,
						b.InhibitInactivityLockout,
						b._RowVersion,						 
						C.ID AS CompanyID,C.Name AS CompanyName,C.Address1 AS CompanyAddress,C.City AS CompanyCity,C.State AS CompanyState,
						D.UserID AS UserID,
						E.PersonID AS SupervisorID,
						F.ID AS AssignedEquipmentID,
						a.AssignedFromSiteGuid AS ASSIGNEDFROMSITEGUID,
						a.AssignedToSiteGuid AS ASSIGNEDTOSITEGUID,
						G.ID AS ASSIGNEDFROMSITEID,
						(SELECT COUNT(*) FROM map.tblCompanyPersonnelAssignedToCompany CPA WHERE b.PersonnelGuid = CPA.PersonnelGuid) AS AssignedCompaniesCount,
						a.MasterSiteGuid
						FROM 
						[erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) a 
						INNER JOIN tblPersonnel b 
						ON a.PersonnelGuid = b.PersonnelGuid
						LEFT JOIN map.tblCompanyPersonnelAssignedToCompany MPC
						ON b._MasterRecordGuid=MPC.PersonnelGuid
						LEFT JOIN tblCompanies C   
        					ON C._MasterRecordGuid = MPC.CompanyGuid
        					AND C.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', MPC.CompanyGuid, @TargetSiteGuid)
        					LEFT JOIN tblUsers D  
        					ON D.UserGuid = b.UserGuid
        					LEFT JOIN tblPersonnel E 
        					ON E._MasterRecordGuid = b.SupervisorPersonnelGuid
        					AND E.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', b.SupervisorPersonnelGuid, @TargetSiteGuid)
        					LEFT JOIN tblEquipment F 
						ON F._MasterRecordGuid = b.AssignedEquipmentGuid
        					AND F.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', b.AssignedEquipmentGuid, @TargetSiteGuid)
						LEFT JOIN tblSites G
						ON a.AssignedFromSiteGuid = G.SiteGuid
						INNER JOIN map.tblPersonnelToRole H 
						ON b.PersonnelGuid = H.PersonnelGuid
						WHERE H.LookupPersonnelRoleIndex = @Role
						AND (@HideHiddenPersonnel = 0 OR b.HiddenDate IS NULL)
						ORDER BY b.PersonID ASC
					END
				END
			END
		ELSE
			BEGIN
			IF(@IsMaxRole > 0)
				BEGIN
				--GetPersonnelEnumeratByMaxRole_NonStandardSelect
					SELECT TOP 1 CONVERT(NVARCHAR(100), CAST(b._RowVersion AS BINARY(8)), 1) AS RowVersionString	
					FROM 
					[erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) a 
					INNER JOIN tblPersonnel b 
					ON a.PersonnelGuid = b.PersonnelGuid
					ORDER BY b._RowVersion DESC
				END
			ELSE
				BEGIN
				--GetPersonnelEnumeratByRole_NonStandardSelect
					SELECT TOP 1 CONVERT(NVARCHAR(100), CAST(b._RowVersion AS BINARY(8)), 1) AS RowVersionString	
					FROM 
					[erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) a 
					INNER JOIN tblPersonnel b 
					ON a.PersonnelGuid = b.PersonnelGuid
					INNER JOIN map.tblPersonnelToRole H 
					ON b.PersonnelGuid = H.PersonnelGuid
					WHERE H.LookupPersonnelRoleIndex = @Role
					ORDER BY b._RowVersion DESC
			END
		END
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
						+ 'Procedure Name: [dbo].usp_GetPersonnelEnumerateByRole' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END