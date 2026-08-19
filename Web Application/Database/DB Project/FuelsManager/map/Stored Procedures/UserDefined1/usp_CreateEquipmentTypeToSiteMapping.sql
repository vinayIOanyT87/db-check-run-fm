




/*
	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	--EXEC [map].[usp_CreateEquipmentTypeToSiteMapping] 'b233964f-3d4c-4500-b43f-e170bae94f41', '00000000-0000-0000-0000-000000000001', '7b61d0ea-a461-461d-9c08-832c6f672e4e', @dt, 'HB'
	EXEC [map].[usp_CreateEquipmentTypeToSiteMapping] 'b85d8705-6b48-41fe-b7a5-69c4be66992f', '00000000-0000-0000-0000-000000000001', 'ace3b10b-e42f-4397-abca-fb59018ac960', @dt, 'HB'

*/


CREATE PROCEDURE [map].[usp_CreateEquipmentTypeToSiteMapping]
(
	@EntityRecordGuid uniqueidentifier, @AssignedFromSiteGuid uniqueidentifier, @AssignedToSiteGuid uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100))
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_CreateEquipmentTypeToSiteMapping] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new EquipmentType To Site mapping record. If the assignment is to a non-root sitegroup, this procedure also takes care of updating and enforcing the Equipment FLC 
	-- configurations of the AssignedTo sitegroup as necessary.
	-- Notes:
	-- 1. @EntityRecordGuid: Record Guid of the entity record to be mapped. 
	-- 2. @AssignedFromSiteGuid: SiteGroup from which the entity record should be mapped from.
	-- 3. @AssignedToSiteGuid: Site/SiteGroup to which the entity record should be mapped to.
	-- 4. EquipmentType is not under Record Versioning, but since it acts as a filter on Equipment Segments that are under Record Versioning, as new EquipmentType entity-to-site assignments are 
	--    created the Equipment FLC Configurations of the AssignedFrom sitegroup for the specific EquipmentType segment filter needs to be applied to the AssignedTo sitegroup.
	-- 5. Root-node sitegroups are excluded from this additional processing, because they do not depend any an upper sitegroup for their FLC configurations, and their FLC configurations are always 
	--    editable for all fields. Therefore, if a new Equipment Type has been assigned to a root-node sitegroup, we can always go directly to that sitegroup and edit the FLC configurations for that 
	--    newly assigned Equipment Type, which would automatically be propagated down.
	-- 4. If the AssignedToSite is an indirect child of the AssignedFromSite, the entity-to-site mapping request is cascaded as necessary.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @BeginTran BIT = 0 

		
		IF (@@TRANCOUNT = 0)   
      BEGIN  
		  BEGIN TRANSACTION --MapEquipmentType
        SET @BeginTran = 1   
		END 

		IF (@AssignedToSiteGuid = @AssignedFromSiteGuid)
		BEGIN		
			-- Create the self-site EntityToSite assignment
			INSERT INTO [map].[tblEntityEquipmentTypeToSite]
			(
				[EquipmentTypeGuid]
				,	[SiteGuid]
				,	[CreatedDate]
				,	[CreatedBy]
				,	[UpdatedDate]
				,	[UpdatedBy]
				,	[AssignedFromSiteGuid]
			)
			VALUES
			(
				@EntityRecordGuid
				,	@AssignedToSiteGuid
				,	@CreatedDate
				,	@CreatedBy
				,	@CreatedDate
				,	@CreatedBy
				,	@AssignedFromSiteGuid
			)

			-- Cascading Assignments do not apply to the base entity assignment (assignment of the entity record with its owner site guid).
			RETURN;  
		END

		
		DECLARE @isAssignedToNonRootSiteGroup bit		
		DECLARE @tblSiteHierarchy TABLE
		(
			ParentSiteGuid uniqueidentifier
			, ChildSiteGuid uniqueidentifier
			, ParentSiteId nvarchar(30)
			, ChildSiteId nvarchar(30)
			, HierarchyLevel int
		)
		INSERT INTO @tblSiteHierarchy
		SELECT ParentSiteGuid, ChildSiteGuid, ParentSiteId, ChildSiteId, HierarchyLevel 
		FROM [erv].[udf_GetReverseSiteHierarchy] (@AssignedToSiteGuid, @AssignedFromSiteGuid) ORDER BY HierarchyLevel

		--Cascade the entity-to-site mappings from the original parent sitegroup down to the target site.
		DECLARE @parentSiteGuid uniqueidentifier
		DECLARE @childSiteGuid uniqueidentifier
		DECLARE @hierarchyLevel int		

		DECLARE TableCursor CURSOR FOR 
		  SELECT ParentSiteGuid, ChildSiteGuid, HierarchyLevel FROM @tblSiteHierarchy 
		  WHERE ParentSiteGuid <> ChildSiteGuid 
		  ORDER BY HierarchyLevel
		OPEN TableCursor 

			FETCH NEXT FROM TableCursor INTO @parentSiteGuid, @childSiteGuid, @hierarchyLevel 
 
			WHILE @@FETCH_STATUS = 0  
			BEGIN 
				INSERT INTO [map].[tblEntityEquipmentTypeToSite]
				(
					 [EquipmentTypeGuid]
					 ,	[SiteGuid]
					 ,	[CreatedDate]
					 ,	[CreatedBy]
					 ,	[UpdatedDate]
					 ,	[UpdatedBy]
					 ,	[AssignedFromSiteGuid]
				)
				SELECT @EntityRecordGuid, @childSiteGuid, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy, @parentSiteGuid
				WHERE NOT EXISTS
				(
					 SELECT * FROM [map].[tblEntityEquipmentTypeToSite]
					 WHERE EquipmentTypeGuid = @EntityRecordGuid
					 AND SiteGuid = @ChildSiteGuid
					 AND AssignedFromSiteGuid = @parentSiteGuid				
				)
						
				FETCH NEXT FROM TableCursor INTO @parentSiteGuid, @childSiteGuid, @hierarchyLevel 
				END 				
		CLOSE TableCursor 
		DEALLOCATE TableCursor 


		DECLARE @topChildSiteGroupGuid uniqueidentifier = NULL
		SET @topChildSiteGroupGuid = (SELECT TOP(1) a.ChildSiteGuid FROM @tblSiteHierarchy a
			WHERE a.HierarchyLevel =
			(
				SELECT MIN(b.HierarchyLevel) FROM @tblSiteHierarchy b				
				INNER JOIN tblSites c ON c.SiteGuid = b.ChildSiteGuid
				WHERE c.SiteGroupFlag = 1
			)
		)
		-- If the assignment is to a non-root sitegroup, then update and enforce the Equipment FLC Configurations of the AssignedTo sitegroup, for that specific Equipment Type, accordingly.
	   SET @isAssignedToNonRootSiteGroup = 0
		
	   SELECT @isAssignedToNonRootSiteGroup = a.SiteGroupFlag 
	   FROM  tblSites a
	   INNER JOIN map.tblSiteToSite b ON b.ChildSiteGuid = a.SiteGuid
	   WHERE a.SiteGuid = @topChildSiteGroupGuid
	   AND NOT ((b.ParentSiteGuid IS NULL) OR (b.ParentSiteGuid = b.ChildSiteGuid))
	   AND a.SiteGroupFlag = 1

	   IF (@isAssignedToNonRootSiteGroup = 1)
	   BEGIN
		  DECLARE @ParentSiteGroupCount int
		  DECLARE @tblProjectedFLCConfig [erv].[utt_FieldLevelConfig]

		  SELECT @ParentSiteGroupCount = COUNT(*) FROM map.tblSiteToSite
		  WHERE ChildSiteGuid = @topChildSiteGroupGuid AND ParentSiteGuid <> @topChildSiteGroupGuid

		  INSERT INTO @tblProjectedFLCConfig
		  (EntitySegmentTemplateGuid, EntityTypeId, SiteGroupGuid, FilterFieldName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, InheritedControlMode, ForwardControlMode)			
		  SELECT a.EntitySegmentTemplateGuid, b.EntityTypeId, @topChildSiteGroupGuid, b.FilterFieldName, a.FilterValueGuid, a.FilterValueName, a.TargetField, a.IsExternalAttribute, a.InternalFieldName,  
		  CASE (COUNT(a.SiteGroupGuid)) WHEN @ParentSiteGroupCount THEN MIN(ISNULL(a.ForwardControlMode, 'ParentSpecific')) ELSE 'ParentSpecific' END, 
		  CASE (COUNT(a.SiteGroupGuid)) WHEN @ParentSiteGroupCount THEN MIN(ISNULL(a.ForwardControlMode, 'ParentSpecific')) ELSE 'ParentSpecific' END
		  FROM [erv].[tblEntityRecordVersioningFieldConfig] a 
		  INNER JOIN erv.tblEntitySegmentTemplate b
		  ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
		  INNER JOIN map.tblSiteToSite c
		  ON c.ParentSiteGuid = a.SiteGroupGuid
		  WHERE c.ChildSiteGuid = @topChildSiteGroupGuid
		  AND c.ParentSiteGuid <> @topChildSiteGroupGuid
		  AND b.EntityTypeId = 'Equipment'
		  AND b.FilterFieldName = 'EquipmentTypeGuid'
		  AND a.FilterValueGuid = @EntityRecordGuid
		  GROUP BY a.EntitySegmentTemplateGuid, b.EntityTypeId, b.FilterFieldName, a.FilterValueGuid, a.FilterValueName, a.TargetField, a.IsExternalAttribute, a.InternalFieldName						
			
		  UPDATE a 
		  SET a.FieldConfigGuid = b.FieldConfigGuid
		  FROM @tblProjectedFLCConfig a
		  INNER JOIN [erv].[tblEntityRecordVersioningFieldConfig] b
		  ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
		  AND b.SiteGroupGuid = a.SiteGroupGuid
		  AND ISNULL(b.FilterValueGuid, @emptyGuid) = ISNULL(a.FilterValueGuid, @emptyGuid)
		  AND b.TargetField = a.TargetField				

		  IF ((SELECT COUNT(*) FROM @tblProjectedFLCConfig) > 0)
		  BEGIN
				EXEC [erv].[usp_UpdateFLCForwardControlMode] @tblProjectedFLCConfig, @topChildSiteGroupGuid, NULL
		  END			
	   END 


		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		COMMIT TRANSACTION --MapEquipmentType	
	END TRY
	BEGIN CATCH  
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
				ROLLBACK TRANSACTION --MapEquipmentType
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		IF(@_ErrNumber = 547 AND CHARINDEX('Uniqueness',@_ErrMessage,0) <> 0)
			RAISERROR('Operation would result in duplicate identifiers.',16,1);
		ELSE
		BEGIN
			SET @_ErrProcName= ERROR_PROCEDURE();        
			SET @_ErrLineNumber = ERROR_LINE();            
			SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
							+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
							+ 'Procedure Name: [map].usp_CreateEquipmentTypeToSiteMapping' + CHAR(13)+CHAR(10)                  
							+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
			RAISERROR(@_ErrMessage,18,1);      
		END
	END CATCH    
	
END     

