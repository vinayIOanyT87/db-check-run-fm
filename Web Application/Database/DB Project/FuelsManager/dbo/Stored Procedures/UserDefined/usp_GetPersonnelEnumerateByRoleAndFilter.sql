/*
	DROP PROCEDURE [dbo].[usp_GetPersonnelEnumerateByRoleAndFilter]

	EXEC [dbo].[usp_GetPersonnelEnumerateByRoleAndFilter] 'AD74B677-F294-4BF8-8861-30D6B424ADC6', 2, 'a%', NULL, 10000, 0

*/



CREATE PROCEDURE [dbo].[usp_GetPersonnelEnumerateByRoleAndFilter]
(
	@TargetSiteGuid uniqueidentifier, @Role int, @SearchFilter nvarchar(max), @OrderBy nvarchar(200), @Limit int, @HideHiddenPersonnel BIT = 0
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetPersonnelEnumerateByRoleAndFilter] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the Personnel records that have a given Personnel Id and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Personnel that have been assigned to this site/sitegroup only
	-- 2. @Role: Limit results to Personnel that have the specific role specified as the role index.
	-- 3. @SearchFilter: Limit results by filtering the string on FirstName, MiddleName, LastName or CardNumber.
	-- 4. @OrderBy: String to order the results.  I don't like this because it is making me use dynamic SQL, but it is threaded throughout the code.
	-- 5. @HideHiddenPersonnel: If true (1), only personnel with a NULL HiddenDate will be returned
	-- 6. This stored procedure replaces the PersonnelClass.EnumerateByRoleSQL inline SQL.  
	-- 7. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	-- 8. The SiteGuid of the master record is included in the resultset to support the decryption of the Personnel.PINNumber in child record versions.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY
		DECLARE @execClause nvarchar(max)
		DECLARE @ParmDefinition nvarchar(200)	
		Declare @OrderByClause nvarchar(200)
		Declare @SelectClause nvarchar(50)
		Declare @WhereClause nvarchar(500)
		Declare @IsMaxRole int

		SET @OrderByClause = 'b.PersonID ASC'
		IF (@OrderBy IS NOT NULL AND LEN(@OrderBy ) > 0)
		BEGIN
			SET @OrderByClause = @OrderBy
		END
		Set @SelectClause = 'SELECT DISTINCT '
		IF(@Limit > 0)
		BEGIN
			Set @SelectClause = 'SELECT TOP(@LimitClause) '	
		END

		Set @IsMaxRole = (SELECT Count(*) FROM lookup.tblPersonnelRole R WHERE R.PersonnelRoleCode = 'MAX_PERSON_ROLE' AND R.PersonnelRoleIndex = @Role)
		IF(@IsMaxRole > 0)
		BEGIN
			Set @WhereClause = 'WHERE ' + 
			'(b.PersonID LIKE (@SearchFilterClause) ' + 
			'OR b.FirstName LIKE (@SearchFilterClause) ' + 
			'OR b.MiddleName LIKE (@SearchFilterClause) ' + 
			'OR b.LastName LIKE (@SearchFilterClause) ' + 
			'OR b.CardNumber LIKE (@SearchFilterClause) ) ' 
		END
		ELSE
		BEGIN
			Set @WhereClause = 'INNER JOIN map.tblPersonnelToRole H  ' + 
			'ON b.PersonnelGuid = H.PersonnelGuid ' + 
			'WHERE ' + 
			'H.LookupPersonnelRoleIndex = @RoleClause AND ' + 
			'(b.PersonID LIKE (@SearchFilterClause) ' + 
			'OR b.FirstName LIKE (@SearchFilterClause) ' + 
			'OR b.MiddleName LIKE (@SearchFilterClause) ' + 
			'OR b.LastName LIKE (@SearchFilterClause) ' + 
			'OR b.CardNumber LIKE (@SearchFilterClause) ) '
		END

		SET @WhereClause = @WhereClause + ' AND (@HideHiddenPersonnel = 0 OR b.HiddenDate IS NULL) '

		SET @ParmDefinition = N'@SiteGuid uniqueidentifier, @RoleClause int, @SearchFilterClause nvarchar(max), @LimitClause int, @HideHiddenPersonnel BIT'

		Set @execClause = @SelectClause + N'b.PersonnelGuid, ' +
		'b._MasterRecordGuid, ' +
		'b.SiteGuid, ' +
		'b.PersonID, ' +
		'b.CardNumber, ' +
		'b.UserGuid, ' +
		'b.FirstName, ' +
		'b.MiddleName, ' +
		'b.LastName, ' +
		'b.Title, ' +
		'b.Department, ' +
		'b.SupervisorPersonnelGuid, ' +
		'b.Address1, ' +
		'b.Address2, ' +
		'b.City, ' +
		'b.State, ' +
		'b.Zip, ' +
		'b.Country, ' +
		'b.Phone1, ' +
		'b.Phone2, ' +
		'b.AssignmentDate, ' +
		'b.SupervisionDate, ' +
		'b.SSAN, ' +
		'b.BirthDate, ' +
		'b.PayRate, ' +
		'b.LaborRate1, ' +
		'b.LaborRate2, ' +
		'b.LaborRate3, ' +
		'b.LaborRate4, ' +
		'b.Status, ' +
		'b.Email, ' +
		'b.ResponsibleOfficer, ' +
		'b.Shift, ' +
		'b.CompanyGuid, ' +
		'b.PINNumber, ' +
		'b.PINRequired, ' +
		'b.LockedOut, ' +
		'b.LockedOutReason, ' +
		'b.LockedOutDate, ' +
		'b.LastActivityDate, ' +
		'b.CardedIn, ' +
		'b.ShortCardNumber, ' +
		'b.HiddenDate, ' + 
		'b.AssignedEquipmentGuid, ' +
		'b.CreatedDate, ' +
		'b.CreatedBy, ' +
		'b.UpdatedDate, ' +
		'b.UpdatedBy, ' +
		'b.UserData1, ' +
		'b.UserData2, ' +
		'b.UserData3, ' +
		'b.UserData4, ' +
		'b.UserData5, ' +
		'b.UserData6, ' +
		'b.UserData7, ' +
		'b.UserData8, ' +
		'b.UserData9, ' +
		'b.UserData10, ' +
		'b.UserData11, ' +
		'b.UserData12, ' +
		'b.UserData13, ' +
		'b.UserData14, ' +
		'b.UserData15, ' +
		'b.UserData16, ' +
		'b.UserData17, ' +
		'b.UserData18, ' +
		'b.UserData19, ' +
		'b.UserData20, ' +
		'b.UserData21, ' +
		'b.UserData22, ' +
		'b.UserData23, ' +
		'b.UserData24, ' +
		'b.InhibitInactivityLockout, ' +
		'b._RowVersion, ' + 
		'C.ID AS CompanyID,C.Name AS CompanyName,C.Address1 AS CompanyAddress,C.City AS CompanyCity,C.State AS CompanyState,' + 
		'D.UserID AS UserID,' + 
		'E.PersonID AS SupervisorID,' + 
		'F.ID AS AssignedEquipmentID,' + 
		'a.AssignedFromSiteGuid AS ASSIGNEDFROMSITEGUID,' + 
		'a.AssignedToSiteGuid AS ASSIGNEDTOSITEGUID,' + 
		'G.ID AS ASSIGNEDFROMSITEID,' + 
		'(SELECT COUNT(*) FROM map.tblCompanyPersonnelAssignedToCompany CPA WHERE b.PersonnelGuid = CPA.PersonnelGuid) AS AssignedCompaniesCount, ' +
		'a.MasterSiteGuid ' +
		'FROM erv.udf_GetPersonnelRecordVersions(@SiteGuid) a ' + 
		'INNER JOIN tblPersonnel b ' + 
		'ON a.PersonnelGuid = b.PersonnelGuid ' +  
		'LEFT JOIN tblCompanies C    ' +
        	'ON C._MasterRecordGuid = b.CompanyGuid ' +
        	'AND C.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] (''Company'', b.CompanyGuid, @SiteGuid) ' +
        	'LEFT JOIN tblUsers D   ' +
        	'ON D.UserGuid = b.UserGuid ' +
        	'LEFT JOIN tblPersonnel E  ' +
        	'ON E._MasterRecordGuid = b.SupervisorPersonnelGuid ' +
        	'AND E.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] (''Personnel'', b.SupervisorPersonnelGuid, @SiteGuid) ' +
        	'LEFT JOIN tblEquipment F  ' +
		'ON F._MasterRecordGuid = b.AssignedEquipmentGuid ' +
        	'AND F.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] (''Equipment'', b.AssignedEquipmentGuid, @SiteGuid) ' +
		'LEFT JOIN tblSites G ' + 
		'ON a.AssignedFromSiteGuid = G.SiteGuid ' + 
		@WhereClause + 'ORDER BY ' + @OrderByClause 

		EXEC sp_executesql @execClause, @ParmDefinition, @SiteGuid = @TargetSiteGuid, @RoleClause = @Role, @SearchFilterClause = @SearchFilter, @LimitClause = @Limit, @HideHiddenPersonnel = @HideHiddenPersonnel

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
						+ 'Procedure Name: [dbo].usp_GetPersonnelEnumerateByRoleAndFilter' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END