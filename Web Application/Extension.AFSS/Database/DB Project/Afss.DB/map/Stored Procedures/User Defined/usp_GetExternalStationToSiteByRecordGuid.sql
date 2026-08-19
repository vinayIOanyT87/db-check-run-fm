CREATE PROCEDURE [map].[usp_GetExternalStationToSiteByRecordGuid]
(
	@EntityRecordGuid UNIQUEIDENTIFIER,
	@AssignedToSiteGuid UNIQUEIDENTIFIER,
	@IncludeChildrenSites BIT = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetExternalStationToSiteByRecordGuid]
	-- Author: Ryan Hill
	-- Purpose: Retrieves the ExternalStationToSite assignments for a given External Station RecordGuid to a given site/sitegroup, and optionally to children site/sitegroup of the given sitegroup as well. 	
	-- Notes:
	-- 1. @EntityRecordGuid: Guid of the External Station record for which the mapping hierarchy is to be retrieved.
	-- 2. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup from which the mapping tree search is to start. If not provided, the owner sitegroup of the master record is used for the @AssignedToSiteGuid.
	-- 3. @IncludeChildrenSites: 0: Limit search to the @AssignedToSiteGuid only; 1: Extend search to the @assignedToSiteGuid and all its children sites/sitegroups.
	-- 4. The hierarchy includes every assignment that has been made on the @EntityRecordGuid against all the sitegroups.sites located at and below the AssignedToSiteGuid, irrespective of from where the assignment was made.
	-- 5. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	-- 6. If @AssignedToSiteGuid corresponds to the Owner sitegroup of the record, then the base mapping of the entity record (the assignment mapping that maps the entity record from the owner site/sitegroup to the owner site/sitegroup itself) will also be included.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON

	BEGIN TRY

		IF (@EntityRecordGuid IS NULL)
		BEGIN
			RAISERROR('Invalid parameter value.', 16, 1); 
			RETURN;
		END

		IF (@AssignedToSiteGuid IS NULL)
		BEGIN
			SELECT @AssignedToSiteGuid = SiteGuid 
			FROM tblExternalStation
			WHERE ExternalStationGuid = @EntityRecordGuid
		END;

		/* Retrieve the SiteGroup hierarchy for the specified sitegroup */
		DECLARE @tblSiteGroupTree TABLE
		(
			SiteGroupGuid UNIQUEIDENTIFIER, 
			SiteGroupId NVARCHAR(30),
			HierarchyLevel INT
		);

		IF (@IncludeChildrenSites = 1)
		BEGIN
			INSERT INTO @tblSiteGroupTree
			(
				SiteGroupGuid, 
				SiteGroupId, 
				HierarchyLevel
			)
			SELECT 
				SiteGuid, 
				SiteId, 
				HierarchyLevel 
			FROM [erv].[udf_GetSiteHierarchy](@AssignedToSiteGuid, 1)
			ORDER BY HierarchyLevel, SiteId
		END
		ELSE
		BEGIN
			INSERT INTO @tblSiteGroupTree
			(
				SiteGroupGuid,
				SiteGroupId, 
				HierarchyLevel
			)

			SELECT SiteGuid, 
				ID, 
				0 
			FROM tblSites 
			WHERE SiteGuid = @AssignedToSiteGuid
		END
	
		SELECT a.ExternalStationToSiteGuid MappingGuid, 
			a.ExternalStationGuid EntityRecordGuid, 
			d.ID EntityId, 
			a.SiteGuid AssignedToSiteGuid, 
			b.SiteGroupId AssignedToSiteId, 
			b.HierarchyLevel, 
			a.CreatedDate, 
			a.CreatedBy, 
			a.UpdatedDate, 
			a.UpdatedBy, 
			a._RowVersion, 
			a.AssignedFromSiteGuid, 
			c.ID AssignedFromSiteId 
		FROM map.tblEntityExternalStationToSite a
		INNER JOIN @tblSiteGroupTree b ON b.SiteGroupGuid = a.SiteGuid
		INNER JOIN tblSites c ON c.SiteGuid = a.AssignedFromSiteGuid
		INNER JOIN tblExternalStation d ON d.ExternalStationGuid = a.ExternalStationGuid
		WHERE a.ExternalStationGuid = @EntityRecordGuid
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
						+ 'Procedure Name: map.usp_GetExternalStationToSiteByRecordGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
