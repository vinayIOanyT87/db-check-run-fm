/*
	DROP PROCEDURE [dbo].[usp_GetPersonnelById]

	EXEC [dbo].[usp_GetPersonnelById] 'AD74B677-F294-4BF8-8861-30D6B424ADC6', 'HBPeronnel01'
	EXEC [dbo].[usp_GetPersonnelById] 'AD74B677-F294-4BF8-8861-30D6B424ADC6', 'HBPeronnel01'

*/



CREATE PROCEDURE [dbo].[usp_GetPersonnelById]
(
	@TargetSiteGuid uniqueidentifier, @Id nvarchar(50)
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetPersonnelById] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the Personnel records that have a given Personnel Id and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Personnel that have been assigned to this site/sitegroup only
	-- 2. @ID: Limit the results to Personnel that have an ID value that correspond to the @Id value
	-- 3. @InTransaction: Specifies whether to use a lock.
	-- 3. This stored procedure replaces the PersonnelClass.SelectByIDSQL inline SQL.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	-- 5. The SiteGuid of the master record is included in the resultset to support the decryption of the Personnel.PINNumber in child record versions.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY
	BEGIN	
		SELECT b.*, 
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
		[erv].[udf_GetPersonnelRecordVersionsById] (@TargetSiteGuid, @Id) a
		INNER JOIN tblPersonnel b
		ON a.PersonnelGuid = b.PersonnelGuid
		LEFT JOIN tblCompanies C   
        	ON C._MasterRecordGuid = b.CompanyGuid
        	AND C.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', b.CompanyGuid, @TargetSiteGuid)
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
		WHERE b.PersonID = @Id
	    	ORDER BY b.PersonID
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
						+ 'Procedure Name: [dbo].usp_GetPersonnelById' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END