


/*
	EXEC [map].[usp_GetProductToSiteByRecordGuid] '80B08634-D356-4569-B9A2-CD36DF955BD0', NULL, 1
	EXEC [map].[usp_GetProductToSiteByRecordGuid] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', NULL, 1
	EXEC [map].[usp_GetProductToSiteByRecordGuid] '80B08634-D356-4569-B9A2-CD36DF955BD0', 'B7BD440B-674F-46F6-977A-CEFC540B1A90', 1
*/

CREATE PROCEDURE [map].[usp_GetProductToSiteByRecordGuid]
(
	@EntityRecordGuid uniqueidentifier,
	@AssignedToSiteGuid uniqueidentifier,
	@IncludeChildrenSites bit
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetProductToSiteByRecordGuid]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves the ProductToSite assignments for a given Product RecordGuid to a given site/sitegroup, and optionally to children site/sitegroup of the given sitegroup as well. 	
	-- Notes:
	-- 1. @EntityRecordGuid: Guid of the Product record for which the mapping hierarchy is to be retrieved. It can be either the index of the specific child record version being examined, or the index of the master record version.
	-- 2. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup for which the mapping tree search is to start. If not provided, the owner sitegroup of the master record is used for the @AssignedToSiteGuid.
	-- 3. @IncludeChildrenSites: 0: Limit search to the @AssignedToSiteGuid only; 1: Extend search to the @assignedToSiteGuid and all its children sites/sitegroups.
	-- 4. The hierarchy includes every assignment that has been made on the @EntityMasterRecordGuid against all the sitegroups.sites located at and below the AssignedToSiteGuid, irrespective of from where the assignment was made.
	-- 5. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	-- 6. If @AssignedToSiteGuid corresponds to the Owner sitegroup of the record, then the base mapping of the entity record (the assignment mapping that maps the entity record from the owner site/sitegroup to the owner site/sitegroup itself) will also be included.
	-- 7. This stored procedure replaces the EntityTositeMapClass.SelectSQL() inline SQL for the case where entityType is PRODUCT and bInTransaction is false.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		IF (@EntityRecordGuid IS NULL)
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END

		DECLARE @EntityMasterRecordGuid uniqueidentifier
		SELECT @EntityMasterRecordGuid = _MasterRecordGuid FROM tblProducts WHERE ProductGuid = @EntityRecordGuid

		IF (@AssignedToSiteGuid IS NULL)
		BEGIN
			SELECT @AssignedToSiteGuid = SiteGuid FROM tblProducts
			WHERE ProductGuid = @EntityMasterRecordGuid
			AND ProductGuid = _MasterRecordGuid
		END;

		/* Retrieve the SiteGroup hierarchy for the specified sitegroup */
		DECLARE @tblSiteGroupTree TABLE
		(
			SiteGroupGuid uniqueidentifier
			, SiteGroupId nvarchar(30)
			, HierarchyLevel int
		);

		if (@IncludeChildrenSites = 1)
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
		
		SELECT a.ProductToSiteGuid MappingGuid, a.ProductGuid EntityRecordGuid, d.ProductId EntityId, a.SiteGuid AssignedToSiteGuid, b.SiteGroupId AssignedToSiteId, b.HierarchyLevel, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a._RowVersion, a.AssignedFromSiteGuid , c.ID AssignedFromSiteId 
		FROM map.tblEntityProductToSite a
		INNER JOIN @tblSiteGroupTree b
		ON b.SiteGroupGuid = a.SiteGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.AssignedFromSiteGuid
		INNER JOIN tblProducts d
		ON d._MasterRecordGuid = a.ProductGuid
		AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', a.ProductGuid, a.SiteGuid)
		WHERE a.ProductGuid = @EntityMasterRecordGuid
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
						+ 'Procedure Name: map.usp_GetProductToSiteByRecordGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
