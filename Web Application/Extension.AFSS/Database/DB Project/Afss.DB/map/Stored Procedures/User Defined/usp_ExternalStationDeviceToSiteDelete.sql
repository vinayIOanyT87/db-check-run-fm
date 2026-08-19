CREATE PROCEDURE [map].[usp_ExternalStationDeviceToSiteDelete]
(
	@EntityRecordGuid uniqueidentifier,
	@AssignedToSiteGuid uniqueidentifier,
	@DeleteBaseMapping bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_ExternalStationDeviceToSiteDelete]
	-- Author: Caleb Townsend
	-- created from:
	-- Stored procedure: [map].[usp_DataDictionaryToSiteDelete]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Deletes a DataDictionaryToSite mapping entry.
	-- Notes:
	-- 1. @EntityRecordGuid: Guid of the DataDictionary record for which the mapping is to be deleted.
	-- 2. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup for which the mapping is to be deleted. 
	--    If the @AssignedToSiteGuid parameter is null, then all the DataDictionary to Site mappings for the entity record are deleted.
	-- 3. @DeleteBaseMapping: 0: Do not delete the base mapping for the entity record. 1: Delete the base mapping for the entity record.
	-- 4. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	-- 5. This operation also deletes all the other DataDictionaryToSite assignments that have been made possible by the given assignment (Cascading entity assignment deletion).
	-- 6. The base mapping is the assignment mapping that maps the entity record from the owner site/sitegroup to the owner site/sitegroup itself.
	--	  It is only deleted if the @DeleteBaseMapping parameter is set to 1 and the AssignedToSiteGuid is NULL
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @tblEntityToSiteHierarchy TABLE
		(
			MappingGuid uniqueidentifier,
			AssignedFromSiteGuid uniqueidentifier,
			AssignedToSiteGuid uniqueidentifier,
			HierarchyLevel integer
		)

		--Get the assignment hierarchy that was built off the assignment that is to be deleted, i.e. the assignments that were subsequently created from the assignment that is to be deleted.
		INSERT INTO @tblEntityToSiteHierarchy
		(MappingGuid, AssignedFromSiteGuid, AssignedToSiteGuid, HierarchyLevel)
		SELECT MappingGuid, AssignedFromSiteGuid, SiteGuid, HierarchyLevel FROM [map].[udf_GetGasboyDeviceToSiteHierarchyByAssignment] (@EntityRecordGuid, NULL, @AssignedToSiteGuid)

		--Capture the base mapping
		IF ((@DeleteBaseMapping = 1) AND (@AssignedToSiteGuid IS NULL))
		BEGIN
			INSERT INTO @tblEntityToSiteHierarchy
			(MappingGuid, AssignedFromSiteGuid, AssignedToSiteGuid, HierarchyLevel)
			SELECT b.GasboyDeviceToSiteGuid, a.SiteGuid, a.SiteGuid, -1 FROM dbo.tblSites a
			INNER JOIN map.tblEntityGasboyDeviceToSite b
			ON b.OwnerSiteGuid = a.SiteGuid
			AND b.MapToSiteGuid = a.SiteGuid
			WHERE b.OwnerSiteGuid = @EntityRecordGuid
			AND b.AssignedFromSiteGuid = b.MapToSiteGuid  
		END

		--Delete the assignment hierarchy
		DELETE a FROM map.tblEntityGasboyDeviceToSite a
		INNER JOIN @tblEntityToSiteHierarchy b 
		ON b.MappingGuid = a.GasboyDeviceToSiteGuid
		
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
						+ 'Procedure Name: map.usp_ExternalStationDeviceToSiteDelete' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END