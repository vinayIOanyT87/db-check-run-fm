/*
	DROP PROCEDURE [erv].[usp_GetFieldLevelConfigMatrix]

	EXEC [erv].[usp_GetFieldLevelConfigMatrix] --Full hierarchy
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] NULL, 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'  --hierarchy for tree branch at and below a specific SiteGroup (SGA)
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] NULL, 'B7BD440B-674F-46F6-977A-CEFC540B1A90'  --hierarchy for tree branch at and below a specific SiteGroup (SGE)
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] 'Transaction Aliases', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'  --hierarchy for a specific EntityType (Transaction Aliases) for tree branch at and below a specific SiteGroup (SGA)	
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] 'Equipment', '00000000-0000-0000-0000-000000000001', 'EquipmentTypeGuid', 'B85D8705-6B48-41FE-B7A5-69C4BE66992F'  --hierarchy for a specific EntityType (Equipment), for a specific filter value (Hydrant Cart), for tree branch at and below a specific SiteGroup (SiteAdmin)
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] 'Transaction Aliases', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', Null, Null, 'AdditiveVolumeUnitIndex'  --hierarchy for a specific EntityType (Transaction Aliases), for a specific field (AdditiveVolumeUnitIndex) for tree branch at and below a specific SiteGroup (SGA)	
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] Null, NUll, Null, Null, 'LockedOut'  --hierarchy for a specific EntityType (Transaction Aliases), for a specific field (AdditiveVolumeUnitIndex) for tree branch at and below a specific SiteGroup (SGA)	
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] Null, Null, Null, Null, Null, 'VersionSpecific' --Full hierarchy for VersionSpecific items only
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] 'Equipment', '00000000-0000-0000-0000-000000000001', 'EquipmentTypeGuid', 'B85D8705-6B48-41FE-B7A5-69C4BE66992F', NULL, 'Configurable', NULL, 0	
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] 'Product', '00000000-0000-0000-0000-000000000001'
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] 'Transaction_Alias', '00000000-0000-0000-0000-000000000001'
	EXEC [erv].[usp_GetFieldLevelConfigMatrix] 'Personnel', '00000000-0000-0000-0000-000000000001'
*/


CREATE PROCEDURE [erv].[usp_GetFieldLevelConfigMatrix]
(
		@EntityTypeId  [nvarchar](100) = NULL
	  ,	@SiteGroupGuid uniqueidentifier = NULL
	  , @FilterFieldName [nvarchar](100) = NULL
	  ,	@FilterValueGuid [uniqueidentifier] = NULL
	  ,	@TargetField [nvarchar](100) = NULL
	  ,	@ControlMode [nvarchar](20) = NULL
	  ,	@CallingReferenceGuid uniqueidentifier = NULL
	  , @IncludeChildrenSiteGroups bit = 1
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_GetFieldConfigMatrix] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 2012-08-14
	-- Description: Retrieve a set of Field Level Control configurations based on a given set of parameters. 
	-- This Stored Procedure adds mocked up records in the result set for those combinations of SiteGroup, Entity, Filter, Filter Value and Target Field not found in 
	-- tblEntityRecordVersioningFieldConfig. Mocked up records have a Null FieldConfigGuid value and a ParentSpecific ForwardControlMode.
	-- Notes:
	-- 1. @EntityTypeId: Entity Type Id as captured in the Entity Segment Template (erv.tblEntitySegmentTemplate)
	-- 2. @SiteGroupGuid: SiteGroup for which the FLC configuration is to be retrieved.
	-- 3. @FilterFieldName: Name of the filter for which the FLC configuration is to be retrieved. This is only applicable when the Entity Segment Template defined for the EntityTypeId has a filter specified.
	-- 4. @FilterValueGuid: Only used when a @FieldFieldName is specified. Can take three types of values: 
	--		(i)	 A specific, non-empty guid (e.g. the guid for the Aircraft Equipment Type). The query will filter out the values to match exactly the non-null guid filter parameter.
	--		(ii) Null. This is the case that corresponds to the '<ALL>' option. The query will simply ignore the FilterValueGuid filter totally.	
	--		(ii) Guid.Empty (00000000-0000-0000-0000-000000000000). This covers the case where the filter value is undefined/null on the entity (e.g. An Equipment without an Equipment Type value). The query will filter out the values to match exactly the null guid filter parameter value.
	-- 5. @TargetField: Specific Target Field for which to retrieve the FLC configuration. Leave null to return all Target Fields.
	-- 6. @ControlMode: Control Mode for which to return retrieve the Field Level Control configurations. @ControlMode can take the following values:
	--		- Null: The @ControlMode filtering parameter is simply ignored.
	--		- VersionSpecific: Only those configuration records for which the FCM is VersionSpecific are returned.
	--      - GlobalSpecific: Only those configuration records for which the FCM is VersionSpecific are returned.
	--		- ParentSpecific: Only those configuration records for which the FCM is ParentSpecific are returned.
	--		- Configurable: Only those configuration records for which the ICM is either VersionSpecific or GlobalSpecific are returned.
	-- 7. @CallingReferenceGuid:  Used to allow another Stored Procedure than is calling the usp_GetFieldConfigMatrix Stored Procedure to specify the exact CallingReferenceGuid value that needs to be 
	--    used when storing the results of the Stored Procedure into the tblTempFieldLevelConfigMatrix table. This in turn allows the calling Stored Procedure to retrieve the results safely.
	-- 8. @IncludeChildrenSiteGroups: 0: limit results to the FLC configuration of the @SiteGroupGuid only. 1: Extend results to the children sitegroups of the @SiteGroupGuid as well.
	-- 9. A null value for any of the Store Procedure parameter means that the filtering that is performed by the query on that parameter will completely be ignored. This effectively corresponds to the '<ALL>' option for that parameter.
	
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	
	DECLARE @emptyGuid uniqueidentifier
	SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
	DECLARE @siteAdminGuid uniqueidentifier
	DECLARE @isARootSite bit
	SET @siteAdminGuid = '00000000-0000-0000-0000-000000000001'
	
	IF ((@SiteGroupGuid IS NOT NULL) AND (NOT EXISTS (SELECT * FROM tblSites WHERE SiteGuid = @SiteGroupGuid AND SiteGroupFlag = 1 )))
	BEGIN
		RAISERROR('SiteGroup context parameter does not correspond to a valid sitegroup',16,1); 
		RETURN;
	END

	IF (@SiteGroupGuid IS NULL)
	BEGIN
		IF (EXISTS (SELECT * FROM dbo.tblSites WHERE SiteGuid = @siteAdminGuid))
			SET @SiteGroupGuid = @siteAdminGuid
		ELSE
		BEGIN
			RAISERROR('SiteGroup context parameter is missing',16,1); 
			RETURN;
		END
	END;

	SET @isARootSite = 0
	IF 
	(
		(
			SELECT COUNT(*)
			FROM map.tblSiteToSite a
			WHERE a.ChildSiteGuid = @SiteGroupGuid
			AND ((a.ParentSiteGuid IS NULL) OR (a.ChildSiteGuid = a.ParentSiteGuid))
			AND NOT EXISTS
			(
				SELECT * FROM map.tblSiteToSite b
				WHERE b.ChildSiteGuid = a.ChildSiteGuid
				AND b.ChildSiteGuid <> ISNULL(b.ParentSiteGuid, b.ChildSiteGuid)
			)		
		) > 0
	)
	BEGIN
		SET @isARootSite = 1
	END


	/* Retrieve the SiteGroup hierarchy for the selected sitegroup */
	DECLARE @tblSiteGroupTree TABLE
	(
		SiteGroupGuid uniqueidentifier
		, SiteGroupId nvarchar(30)
		, HierarchyLevel int
	);

	IF (@IncludeChildrenSiteGroups = 0)
	BEGIN
		INSERT INTO @tblSiteGroupTree
		(SiteGroupGuid, SiteGroupId, HierarchyLevel)
		SELECT SiteGuid, id, 0
		FROM tblSites
		WHERE SiteGuid = @SiteGroupGuid
	END
	ELSE
	BEGIN
		INSERT INTO @tblSiteGroupTree
		(SiteGroupGuid, SiteGroupId, HierarchyLevel)
		SELECT SiteGuid, SiteId, HierarchyLevel 
		FROM [erv].[udf_GetSiteHierarchy](@SiteGroupGuid, 0)
		ORDER BY HierarchyLevel, SiteId
	END

	/*Create one record for each combination of entity segment and sitegroup */
	DECLARE @tblMockedUpFieldLevelConfigMatrix TABLE
	(
		runningIndex int identity
		, EntitySegmentTemplateGuid uniqueidentifier
		, AppTableName nvarchar(100)
		, EntityTypeId nvarchar(100)
		, EntityTypeDisplayName nvarchar(100)
		, FilterFieldName nvarchar(100)
		, FilterDisplayName nvarchar(100)
		, FilterValuesStoredProc nvarchar(100)
		, SiteGroupGuid uniqueidentifier
		, SiteGroupId nvarchar(30)
		, HierarchyLevel int
		, FilterValueGuid uniqueidentifier
		, FilterValueName nvarchar(100)
		, IsValidNullFilterValue bit
		, TargetField nvarchar(100)		
		, IsExternalAttribute bit
		, InternalFieldName nvarchar(100)
	);

	INSERT INTO @tblMockedUpFieldLevelConfigMatrix 
	(EntitySegmentTemplateGuid, AppTableName, EntityTypeId, EntityTypeDisplayName, FilterFieldName, FilterDisplayName, FilterValuesStoredProc, SiteGroupGuid, SiteGroupId, HierarchyLevel)
	SELECT a.EntitySegmentTemplateGuid, a.AppTableName, a.EntityTypeId, a.EntityTypeDisplayName, a.FilterFieldName, a.FilterDisplayName, 
	a.FilterValuesStoredProc, b.SiteGroupGuid, b.SiteGroupId, b.HierarchyLevel
	FROM erv.tblEntitySegmentTemplate a
	CROSS JOIN @tblSiteGroupTree b
	WHERE ((a.EntityTypeId = @EntityTypeId) OR (@EntityTypeId IS NULL))
	--Order By a.EntityTypeId, b.HierarchyLevel, b.SiteGroupId, a.FilterFieldName

	
	/* Retrieve each entity segment and sitegroup combination that supports filtering, retrieve all the possible filter values */	
	DECLARE @tblFilterValues TABLE
	(
		EntitySegmentTemplateGuid uniqueidentifier
		, FilterFieldName nvarchar(100)
		, FilterValueGuid uniqueidentifier
		, FilterValueName nvarchar(100)
		, SiteGuid uniqueidentifier
	);

	DECLARE @generate nvarchar(150)
	DECLARE @ParmDefinition nvarchar(500);
	DECLARE @entitySegmentTemplateGuid uniqueidentifier
	DECLARE @filterFieldNameTemp nvarchar(100)
	DECLARE @filterValuesStoredProc nvarchar(100)
	DECLARE @siteGroupIndex uniqueidentifier

	DECLARE EntitySegmentFilterCursor CURSOR FOR 
		SELECT EntitySegmentTemplateGuid, FilterFieldName, FilterValuesStoredProc, SiteGroupGuid
		FROM @tblMockedUpFieldLevelConfigMatrix
		WHERE FilterValuesStoredProc IS NOT NULL
	OPEN EntitySegmentFilterCursor

	FETCH NEXT FROM EntitySegmentFilterCursor 
	INTO @entitySegmentTemplateGuid, @filterFieldNameTemp, @filterValuesStoredProc, @siteGroupIndex

	WHILE @@FETCH_STATUS = 0
	BEGIN			
		SET @generate = N'EXEC ' + @filterValuesStoredProc + N' @sgGuid';
		SET @ParmDefinition = N'@sgGuid uniqueidentifier';
		INSERT @tblFilterValues 
		(FilterValueGuid, FilterValueName, SiteGuid)
		EXECUTE sp_executesql @generate, @ParmDefinition, @sgGuid = @siteGroupIndex;
		UPDATE @tblFilterValues SET EntitySegmentTemplateGuid = @entitySegmentTemplateGuid, FilterFieldName = @filterFieldNameTemp  WHERE EntitySegmentTemplateGuid IS NULL
		FETCH NEXT FROM EntitySegmentFilterCursor into @entitySegmentTemplateGuid, @filterFieldNameTemp, @filterValuesStoredProc, @siteGroupIndex 
	END 
	CLOSE EntitySegmentFilterCursor;
	DEALLOCATE EntitySegmentFilterCursor;


	/* Merge the filter values records retrieved above into the running matrix record collection*/
	INSERT INTO @tblMockedUpFieldLevelConfigMatrix
	(EntitySegmentTemplateGuid, AppTableName, EntityTypeId, EntityTypeDisplayName, FilterFieldName, FilterDisplayName, FilterValuesStoredProc, SiteGroupGuid, SiteGroupId, HierarchyLevel, FilterValueGuid, FilterValueName, IsValidNullFilterValue)
	SELECT a.EntitySegmentTemplateGuid, a.AppTableName, a.EntityTypeId, a.EntityTypeDisplayName, a.FilterFieldName, a.FilterDisplayName, a.FilterValuesStoredProc, a.SiteGroupGuid, a.SiteGroupId, a.HierarchyLevel, b.FilterValueGuid, b.FilterValueName, (CASE WHEN (b.FilterValueName IS NULL) THEN 1 ELSE NULL END)		-- If the stored procedure (defined in tblEntitySegmenTemplate) for the filter returned a null filter value, then null is a valid value for the filter
	FROM @tblMockedUpFieldLevelConfigMatrix a
	INNER JOIN @tblFilterValues b
	ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
	AND b.SiteGuid = a.SiteGroupGuid
	WHERE ((b.FilterFieldName =  @FilterFieldName) OR (@FilterFieldName IS NULL))
	AND ((b.FilterValueGuid = @FilterValueGuid) OR ((b.FilterValueGuid IS NULL) AND (@FilterValueGuid = @emptyGuid)) OR (@FilterValueGuid IS NULL))


	DELETE @tblMockedUpFieldLevelConfigMatrix
	WHERE FilterValuesStoredProc IS NOT NULL
	AND FilterValueName IS NULL
	AND IsValidNullFilterValue IS NULL
	

	/* Expand the running matrix record collection to include the internal fields for each entity type*/
	INSERT INTO @tblMockedUpFieldLevelConfigMatrix
	(EntitySegmentTemplateGuid, AppTableName, EntityTypeId, EntityTypeDisplayName, FilterFieldName, FilterDisplayName, FilterValuesStoredProc, SiteGroupGuid, SiteGroupId, HierarchyLevel, FilterValueGuid, FilterValueName, TargetField)
	SELECT a.EntitySegmentTemplateGuid, a.AppTableName, a.EntityTypeId, a.EntityTypeDisplayName, a.FilterFieldName, a.FilterDisplayName, a.FilterValuesStoredProc, a.SiteGroupGuid, a.SiteGroupId, a.HierarchyLevel, a.FilterValueGuid, a.FilterValueName, b.Name
	FROM @tblMockedUpFieldLevelConfigMatrix a
	CROSS JOIN 
	(
		SELECT Object_Id, Name FROM sys.columns
		WHERE Name not in ('ID', 'CreatedDate', 'CreatedBy', 'UpdatedDate', 'UpdatedBy', '_RowVersion', 'IsEthanol')
		AND Name NOT LIKE '%Guid'  --Fields of type guid are set as non-FLC configurable by default. They can be included individually in tblEntityExternalAttribute as necessary.
		AND Name NOT LIKE '[_]%'
		AND ((Name = @TargetField) OR (@TargetField IS NULL))
	) b
	WHERE b.Object_Id = OBJECT_ID(a.AppTableName)	

	
	/* Expand the running matrix record collection to include the external relationship fields for each entity type*/	
	DECLARE @tblExternalAttributes TABLE
	(
		runningIndex int identity
		, EntitySegmentTemplateGuid uniqueidentifier
		, RelationshipName nvarchar(100)
		, InternalFieldName nvarchar(100)
	);
	INSERT INTO @tblExternalAttributes
	(EntitySegmentTemplateGuid, RelationshipName, InternalFieldName)
	SELECT EntitySegmentTemplateGuid, RelationshipName, InternalFieldName FROM erv.tblEntityExternalAttribute
	WHERE EntitySegmentTemplateGuid IN (SELECT DISTINCT EntitySegmentTemplateGuid FROM  @tblMockedUpFieldLevelConfigMatrix)

	--If an external field is also listed as a regular field (that was not filtered out in the regular target field extraction in the previous steps), 
	--then need to delete the regular field, so as not to have duplicate target field entries (with diffferent names) for the same internal field.
	--This can happen when an erv.tblExternalAttributes entry is used not to reference an external attribute, but merely to provide a different display 
	--name to an internal field (e.g. renaming "LookupMajorCorrectionMethodIndex" to "Major Correction Method").
	DELETE a FROM @tblMockedUpFieldLevelConfigMatrix a
	INNER JOIN @tblExternalAttributes b
	ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
	AND b.InternalFieldName = a.TargetField
	
	INSERT INTO @tblMockedUpFieldLevelConfigMatrix
	(EntitySegmentTemplateGuid, AppTableName, EntityTypeId, EntityTypeDisplayName, FilterFieldName, FilterDisplayName, FilterValuesStoredProc, SiteGroupGuid, SiteGroupId, HierarchyLevel, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName)	
	SELECT a.EntitySegmentTemplateGuid, a.AppTableName, a.EntityTypeId, a.EntityTypeDisplayName, a.FilterFieldName, a.FilterDisplayName, a.FilterValuesStoredProc, a.SiteGroupGuid, a.SiteGroupId, a.HierarchyLevel, a.FilterValueGuid, a.FilterValueName, b.RelationshipName, 1 ExternalAttribute, b.InternalFieldName
	FROM @tblMockedUpFieldLevelConfigMatrix a
	CROSS JOIN
	(	
		SELECT EntitySegmentTemplateGuid, RelationshipName, InternalFieldName FROM @tblExternalAttributes 
		WHERE ((RelationshipName = @TargetField) OR (@TargetField IS NULL))
	) b
	WHERE a.TargetField IS NULL
	AND b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
	
	DELETE  @tblMockedUpFieldLevelConfigMatrix
	WHERE TargetField IS NULL

	--Remove TargetFields that are specific to each Entity Type and which are on the exclusion list for that Entity Type
	DELETE @tblMockedUpFieldLevelConfigMatrix
	WHERE EntityTypeId = 'Product'
	AND TargetField IN ('ProductId', 'LookupProductTypeIndex')

	DELETE @tblMockedUpFieldLevelConfigMatrix
	WHERE EntityTypeId = 'Transaction_Alias'
	AND TargetField IN ('AliasName', 'LookupTransTypeIndex')

	DELETE @tblMockedUpFieldLevelConfigMatrix
	WHERE EntityTypeId = 'Personnel'
	AND TargetField IN ('FirstName', 'MiddleName', 'LastName', 'BirthDate', 'SSAN', 'PersonId', 'PINNumber')

	CREATE TABLE #tblTempFieldLevelConfigMatrix
	(
		FieldConfigGuid uniqueidentifier, 
		EntitySegmentTemplateGuid uniqueidentifier, 
		AppTableName nvarchar(100), 
		EntityTypeId nvarchar(100), 
		EntityTypeDisplayName nvarchar(100), 
		SiteGroupGuid uniqueidentifier, 
		SiteGroupId nvarchar(30), 
		HierarchyLevel int, 
		FilterFieldName nvarchar(100), 
		FilterDisplayName nvarchar(100), 
		FilterValueGuid uniqueidentifier, 
		FilterValueName nvarchar(100), 
		TargetField nvarchar(100), 
		IsExternalAttribute bit, 
		InternalFieldName nvarchar(100), 
		InheritedControlMode nvarchar(20), 
		ForwardControlMode nvarchar(20), 
		CreatedDate datetimeoffset(7), 
		CreatedBy nvarchar(100), 
		UpdatedDate datetimeoffset(7), 
		UpdatedBy nvarchar(100), 
		_RowVersion int
	);	


	/* Merge the actual Field Level Configurations (as stored in tblEntityRecordVersioningFieldConfig) with the mocked up entries from @tblMockedUpFieldLevelConfigMatrix  */
	INSERT INTO #tblTempFieldLevelConfigMatrix
	(FieldConfigGuid, EntitySegmentTemplateGuid, AppTableName, EntityTypeId, EntityTypeDisplayName, SiteGroupGuid, SiteGroupId, HierarchyLevel, FilterFieldName, 
	FilterDisplayName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, InheritedControlMode, ForwardControlMode, CreatedDate, 
	CreatedBy, UpdatedDate, UpdatedBy, _RowVersion)
	SELECT x.FieldConfigGuid, x.EntitySegmentTemplateGuid, x.AppTableName, x.EntityTypeId, x.EntityTypeDisplayName, x.SiteGroupGuid, x.SiteGroupId, x.HierarchyLevel, x.FilterFieldName, 
	x.FilterDisplayName, x.FilterValueGuid, x.FilterValueName, x.TargetField, x.IsExternalAttribute, x.InternalFieldName, x.InheritedControlMode, x.ForwardControlMode, x.CreatedDate, 
	x.CreatedBy, x.UpdatedDate, x.UpdatedBy, x._RowVersion	
	FROM
	(
		SELECT a.FieldConfigGuid, a.EntitySegmentTemplateGuid, b.AppTableName, b.EntityTypeId, b.EntityTypeDisplayName, a.SiteGroupGuid, c.ID SiteGroupId, d.HierarchyLevel, b.FilterFieldName, b.FilterDisplayName, a.FilterValueGuid, a.FilterValueName, a.TargetField, a.IsExternalAttribute, a.InternalFieldName, a.InheritedControlMode, a.ForwardControlMode,	a.CreatedDate, a.CreatedBy,	a.UpdatedDate, a.UpdatedBy,	CONVERT(int, a._RowVersion) _RowVersion
		FROM [erv].[tblEntityRecordVersioningFieldConfig] a
		INNER JOIN erv.tblEntitySegmentTemplate b
		ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.SiteGroupGuid
		INNER JOIN @tblSiteGroupTree d
		ON d.SiteGroupGuid = a.SiteGroupGuid
		WHERE ((b.EntityTypeId = @EntityTypeId) OR (@EntityTypeId IS NULL))
		AND ((b.FilterFieldName =  @FilterFieldName) OR (@FilterFieldName IS NULL))
		AND ((a.FilterValueGuid = @FilterValueGuid) OR ((@FilterValueGuid = @emptyGuid) AND (a.FilterValueGuid IS NULL)) OR (@FilterValueGuid IS NULL))
		AND ((a.TargetField = @TargetField) OR (@TargetField IS NULL))
		AND 
		(
			(@ControlMode IS NULL)
			OR ((@ControlMode <> 'Configurable') AND (a.ForwardControlMode = @ControlMode))
			OR ((@ControlMode = 'Configurable') AND ((a.InheritedControlMode = 'VersionSpecific') OR (a.InheritedControlMode = 'GlobalSpecific') OR (a.InheritedControlMode IS NULL)))
		)
		UNION
		SELECT NULL FieldConfigGuid, EntitySegmentTemplateGuid, AppTableName, EntityTypeId, EntityTypeDisplayName, SiteGroupGuid, SiteGroupId, HierarchyLevel, FilterFieldName, FilterDisplayName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, (CASE @isARootSite WHEN 1 THEN NULL ELSE 'ParentSpecific' END) InheritedControlMode, 'ParentSpecific' ForwardControlMode, NULL CreatedDate, NULL CreatedBy, NULL UpdatedDate, NULL UpdatedBy, NULL _RowVersion
		FROM  @tblMockedUpFieldLevelConfigMatrix a
		WHERE NOT EXISTS
		(
			SELECT * FROM @tblMockedUpFieldLevelConfigMatrix b
			INNER JOIN [erv].[tblEntityRecordVersioningFieldConfig] c
			ON c.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
			AND c.SiteGroupGuid = b.SiteGroupGuid
			AND ((c.FilterValueGuid = b.FilterValueGuid) OR ((c.FilterValueGuid IS NULL) AND (b.FilterValueGuid IS NULL)))
			AND c.TargetField = b.TargetField
			WHERE a.runningIndex = b.runningIndex
		)
		AND 
		(
			(@ControlMode IS NULL) 
			OR (@ControlMode = 'ParentSpecific')
			OR ((@ControlMode = 'Configurable') AND (@SiteGroupGuid = @siteAdminGuid) AND (a.SiteGroupGuid = @siteAdminGuid))
		)
	) x
	
	IF (@CallingReferenceGuid IS NULL)
	BEGIN
		SELECT * FROM #tblTempFieldLevelConfigMatrix
		ORDER BY HierarchyLevel, SiteGroupId, EntityTypeDisplayName, FilterDisplayName, FilterFieldName, FilterValueName, TargetField
	END
	ELSE
	BEGIN
		ALTER INDEX IX_tblTempFieldLevelConfigMatrix_CallingReferenceGuid ON erv.tblTempFieldLevelConfigMatrix DISABLE

		INSERT INTO erv.tblTempFieldLevelConfigMatrix
		(_CallingReferenceGuid, FieldConfigGuid, EntitySegmentTemplateGuid, AppTableName, EntityTypeId, EntityTypeDisplayName, SiteGroupGuid, SiteGroupId, HierarchyLevel, FilterFieldName, FilterDisplayName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, InheritedControlMode, ForwardControlMode, FLCCreatedDate, FLCCreatedBy, FLCUpdatedDate, FLCUpdatedBy, FLCRowVersion)
		SELECT @CallingReferenceGuid, FieldConfigGuid, EntitySegmentTemplateGuid, AppTableName, EntityTypeId, EntityTypeDisplayName, SiteGroupGuid, SiteGroupId, HierarchyLevel, FilterFieldName, FilterDisplayName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, InheritedControlMode, ForwardControlMode, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, _RowVersion
		FROM #tblTempFieldLevelConfigMatrix

		ALTER INDEX IX_tblTempFieldLevelConfigMatrix_CallingReferenceGuid ON erv.tblTempFieldLevelConfigMatrix REBUILD
	END	
END