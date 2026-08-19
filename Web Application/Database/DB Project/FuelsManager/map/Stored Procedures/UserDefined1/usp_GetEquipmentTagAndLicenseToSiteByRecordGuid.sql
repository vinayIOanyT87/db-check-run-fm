

/*
	EXEC [map].[usp_GetEquipmentTagAndLicenseToSiteByRecordGuid] 'C0DA2E5F-711C-4245-B4DA-7085831674F1', NULL
	EXEC [map].[usp_GetEquipmentTagAndLicenseToSiteByRecordGuid] '05C7D1E0-745D-488D-AF05-00DF1A70D05D', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', 1
*/

CREATE PROCEDURE [map].[usp_GetEquipmentTagAndLicenseToSiteByRecordGuid]
(
	@EntityRecordGuid uniqueidentifier,
	@AssignedToSiteGuid uniqueidentifier,
	@IncludeChildrenSites bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetEquipmentTagAndLicenseToSiteByRecordGuid]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves the EquipmentTagAndLicenseToSite assignments for a given EquipmentTagAndLicense RecordGuid to a given site/sitegroup, and optionally to children site/sitegroup of the given sitegroup as well. 	
	-- Notes:
	-- 1. @EntityRecordGuid: Guid of the EquipmentTagAndLicense record for which the mapping hierarchy is to be retrieved.
	-- 2. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup from which the mapping tree search is to start. If not provided, the owner sitegroup of the master record is used for the @AssignedToSiteGuid.
	-- 3. @IncludeChildrenSites: 0: Limit search to the @AssignedToSiteGuid only; 1: Extend search to the @assignedToSiteGuid and all its children sites/sitegroups.
	-- 4. The hierarchy includes every assignment that has been made on the @EntityRecordGuid against all the sitegroups.sites located at and below the AssignedToSiteGuid, irrespective of from where the assignment was made.
	-- 5. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	-- 6. If @AssignedToSiteGuid corresponds to the Owner sitegroup of the record, then the base mapping of the entity record (the assignment mapping that maps the entity record from the owner site/sitegroup to the owner site/sitegroup itself) will also be included.
	-- 7. This stored procedure replaces the EntityTositeMapClass.SelectSQL() inline SQL for the case where entityType is QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE and bInTransaction is false.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		IF (@EntityRecordGuid IS NULL)
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END

		IF (@AssignedToSiteGuid IS NULL)
		BEGIN
			SELECT @AssignedToSiteGuid = SiteGuid FROM tblQualifications
			WHERE QualificationGuid = @EntityRecordGuid
		END;

		/* Retrieve the SiteGroup hierarchy for the specified sitegroup */
		DECLARE @tblSiteGroupTree TABLE
		(
			SiteGroupGuid uniqueidentifier
			, SiteGroupId nvarchar(30)
			, HierarchyLevel int
		);

		IF (@IncludeChildrenSites = 1)
		BEGIN
			INSERT INTO @tblSiteGroupTree
			(SiteGroupGuid, SiteGroupId, HierarchyLevel)
			SELECT SiteGuid, SiteId, HierarchyLevel 
			FROM [erv].[udf_GetSiteHierarchy](@AssignedToSiteGuid, 1)
			ORDER BY HierarchyLevel, SiteId
		END
		ELSE
		BEGIN
			INSERT INTO @tblSiteGroupTree
			(SiteGroupGuid, SiteGroupId, HierarchyLevel)
			SELECT SiteGuid, ID, 0 FROM tblSites WHERE SiteGuid = @AssignedToSiteGuid
		END
	
		SELECT a.EquipmentTagAndLicenseToSiteGuid MappingGuid, a.QualificationGuid EntityRecordGuid, d.Id EntityId, a.SiteGuid AssignedToSiteGuid, b.SiteGroupId AssignedToSiteId, b.HierarchyLevel, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a._RowVersion, a.AssignedFromSiteGuid , c.ID AssignedFromSiteId 
		FROM map.tblEntityEquipmentTagAndLicenseToSite a
		INNER JOIN @tblSiteGroupTree b
		ON b.SiteGroupGuid = a.SiteGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.AssignedFromSiteGuid
		INNER JOIN tblQualifications d
		ON d.QualificationGuid = a.QualificationGuid
		WHERE a.QualificationGuid = @EntityRecordGuid
		ORDER BY b.HierarchyLevel

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
						+ 'Procedure Name: map.usp_GetEquipmentTagAndLicenseToSiteByRecordGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
