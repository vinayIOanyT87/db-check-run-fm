/*
	DROP PROCEDURE [erv].[usp_UpdateFLCForwardControlMode]
*/
CREATE PROCEDURE [erv].[usp_UpdateFLCForwardControlMode]
(
	@FieldLevelConfigParamTable erv.utt_FieldLevelConfig READONLY, @SiteGroupGuid uniqueidentifier, @UserId nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_UpdateFLCForwardControlMode] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Update the Forward Control Mode of a set of Field Level Configuration records and propagates the changes down the site hierarchy.
	-- Notes:
	-- 1. @FieldLevelConfigParamTable: Table containing the FLC entries to be applied.
	-- 2. @SiteGroupGuid: The first (top-level) SiteGroup to which to start applying the changes.
	-- 3. @UserId: Id of the user that needs to be tied to the changes
	-- 4. Can handle more than one Field Configuration Record update request in a single procedure call, with the help of the @FieldLevelConfigParamTable parameter with is of a table type.
	-- 5. The only fields of the utt_FieldLLevelConfig table of the @FieldLevelConfigParamTable that need to be set are: FieldConfigGuid, EntitySegmentTemplateGuid, EntityTypeId, SiteGroupGuid, FilterFieldName, FilterValueGuid, FilterValueName, TargetField, ForwardControlMode
	-- 6. For each record of @FieldLevelConfigParamTable the procedure supports both the case where the FieldConfigGuid is not null, i.e. the update of an actual record of table [erv].[tblEntityRecordVersioningFieldConfig],
	--	  and the case where @FieldConfigGuid is null, i.e. a mocked-up Field Level Configuration record. In the last case a new record is added to table
	--    [erv].[tblEntityRecordVersioningFieldConfig]. In both cases, the change is propagated down the site hierarchy.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

		DECLARE @SiteGroupCount int
		SELECT @SiteGroupCount = COUNT(DISTINCT SiteGroupGuid) FROM @FieldLevelConfigParamTable

		IF (@SiteGroupCount > 1)
		BEGIN
			RAISERROR('Cannot modify Field Level Control for more than one sitegroup at a time.',16,1); 
			RETURN;
		END
		
		DECLARE @SiteGroupTest uniqueidentifier
		SET @SiteGroupTest = (SELECT TOP(1) SiteGroupGuid FROM @FieldLevelConfigParamTable)
		IF (@SiteGroupTest <> @SiteGroupGuid)
		BEGIN
			RAISERROR('SiteGroupGuid mismatch between the input dataset and the SiteGroupGuid parameter.',16,1); 
			RETURN;
		END

		--Capture the initial FLC configuration state of the entity segment/s that are impacted at the sitegroup where the FLC configuration changes have been initiated
		DECLARE @tblBaseTargetSegments TABLE
		(
			[BaseTargetSegmentIndex] int identity,
			[EntitySegmentTemplateGuid] [uniqueidentifier],
			[EntityTypeId] [nvarchar](100) NULL,
			[SiteGroupGuid] [uniqueidentifier] NULL,
			[FilterValueGuid] [uniqueidentifier] NULL,
			RecordVersioningInitialState bit,
			Processed bit
		)		
		
		INSERT INTO @tblBaseTargetSegments
		(EntitySegmentTemplateGuid, EntityTypeId, SiteGroupGuid, FilterValueGuid, RecordVersioningInitialState, Processed)
		SELECT DISTINCT EntitySegmentTemplateGuid, EntityTypeId, SiteGroupGuid, FilterValueGuid, 0, 0 FROM @FieldLevelConfigParamTable

		UPDATE a
		SET a.RecordVersioningInitialState = 1
		FROM @tblBaseTargetSegments a
		WHERE EXISTS
		(
			SELECT * FROM erv.tblEntityRecordVersioningFieldConfig b
			WHERE b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
			AND ISNULL(b.FilterValueGuid, @emptyGuid) = ISNULL(a.FilterValueGuid, @emptyGuid)
			AND b.SiteGroupGuid = a.SiteGroupGuid
			AND b.ForwardControlMode = 'VersionSpecific'
		)		

		DECLARE @tblInitialParentSpecificOrGlobalSpecificFields TABLE
		(
			[BaseTargetSegmentIndex] int,
			[TargetField] nvarchar(100)

		)
		DECLARE @tblFinalParentSpecificOrGlobalSpecificFields TABLE
		(
			[BaseTargetSegmentIndex] int,
			[TargetField] nvarchar(100)
		)

		INSERT INTO @tblInitialParentSpecificOrGlobalSpecificFields
		(BaseTargetSegmentIndex, TargetField)
		SELECT a.BaseTargetSegmentIndex, b.TargetField FROM @tblBaseTargetSegments a
		INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
		ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
		AND ISNULL(b.FilterValueGuid, @emptyGuid) = ISNULL(a.FilterValueGuid, @emptyGuid)
		AND b.SiteGroupGuid = a.SiteGroupGuid
		WHERE b.ForwardControlMode in ('ParentSpecific', 'GlobalSpecific')


		DECLARE @tblTopLevelRecordsToInsertUpdate erv.utt_FieldLevelConfig	
		DECLARE @tblRecordsToInsertUpdate erv.utt_FieldLevelConfig
		--Capture top-level node records to be updated
		INSERT INTO @tblTopLevelRecordsToInsertUpdate
		(FieldConfigGuid, EntitySegmentTemplateGuid, EntityTypeId, FilterFieldName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, ForwardControlMode)
		SELECT (CASE WHEN a.FieldConfigGuid = @emptyGuid THEN NULL ELSE a.FieldConfigGuid END), a.EntitySegmentTemplateGuid, a.EntityTypeId, a.FilterFieldName, 
		(CASE WHEN a.FilterValueGuid = @emptyGuid THEN NULL ELSE a.FilterValueGuid END), a.FilterValueName, 
		a.TargetField, a.IsExternalAttribute, a.InternalFieldName, a.ForwardControlMode
		FROM @FieldLevelConfigParamTable a
		INNER JOIN [erv].[tblEntityRecordVersioningFieldConfig] b
		ON b.FieldConfigGuid = a.FieldConfigGuid
		WHERE b.ForwardControlMode <> a.ForwardControlMode
		AND b.SiteGroupGuid = @SiteGroupGuid


		--Capture top-level node records to be inserted
		INSERT INTO @tblTopLevelRecordsToInsertUpdate
		(FieldConfigGuid, EntitySegmentTemplateGuid, EntityTypeId, FilterFieldName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, ForwardControlMode)
		SELECT (CASE WHEN FieldConfigGuid = @emptyGuid THEN NULL ELSE FieldConfigGuid END), EntitySegmentTemplateGuid, EntityTypeId, FilterFieldName, 
		(CASE WHEN FilterValueGuid = @emptyGuid THEN NULL ELSE FilterValueGuid END), FilterValueName, 
		TargetField, IsExternalAttribute, InternalFieldName, ForwardControlMode
		FROM @FieldLevelConfigParamTable
		WHERE ((FieldConfigGuid IS NULL) OR (FieldConfigGuid = @emptyGuid))
		AND ForwardControlMode in ('VersionSpecific', 'GlobalSpecific')


		--Retrieve the applicable, existing FieldLevelControl mappings			
		DECLARE @entityTypeIdSelection nvarchar(100)
		DECLARE @filterNameSelection nvarchar(100)
		DECLARE @filterValueGuidSelection uniqueidentifier
		DECLARE @callingRefGuid uniqueidentifier
		SET @callingRefGuid = NEWID()
		SET @entityTypeIdSelection = NULL		
		SET @filterNameSelection = NULL		
		SET @filterValueGuidSelection = NULL
		IF ((SELECT COUNT(DISTINCT EntitySegmentTemplateGuid) FROM @FieldLevelConfigParamTable) = 1)
		BEGIN
			SET @entityTypeIdSelection = (SELECT TOP(1) EntityTypeId FROM @FieldLevelConfigParamTable)
			IF ((SELECT COUNT(DISTINCT FilterValueGuid) FROM @FieldLevelConfigParamTable) = 1)
			BEGIN
				SET @filterNameSelection = (SELECT TOP(1) FilterFieldName FROM @FieldLevelConfigParamTable) 
				SET @filterValueGuidSelection = (SELECT TOP(1) FilterValueGuid FROM @FieldLevelConfigParamTable) 
			END
		END

		EXEC [erv].[usp_GetFieldLevelConfigMatrix] @entityTypeIdSelection, @SiteGroupGuid, @filterNameSelection, @filterValueGuidSelection, NULL, NULL, @callingRefGuid


		-- Capture all the records that need to be inserted or updated throughout the site hierarchy
		INSERT INTO @tblRecordsToInsertUpdate
		(FieldConfigGuid, EntitySegmentTemplateGuid, SiteGroupGuid,  HierarchyLevel, EntityTypeId, FilterFieldName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, InheritedControlMode, ForwardControlMode)
		SELECT a.FieldConfigGuid, b.EntitySegmentTemplateGuid, a.SiteGroupGuid,  a.HierarchyLevel,  b.EntityTypeId, b.FilterFieldName, b.FilterValueGuid, b.FilterValueName, b.TargetField, b.IsExternalAttribute, b.InternalFieldName, a.InheritedControlMode, b.ForwardControlMode
		FROM erv.tblTempFieldLevelConfigMatrix a
		INNER JOIN @tblTopLevelRecordsToInsertUpdate b
		ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
		AND b.EntityTypeId = a.EntityTypeId
		AND b.TargetField = a.TargetField
		WHERE 
		a._CallingReferenceGuid = @callingRefGuid
		AND
		(
			(b.FilterFieldName = a.FilterFieldName)		-- Entities configured with a filter in tblEntitySegmentTemplate, e.g. Equipment configured with an EquipmentType filter
			OR (a.FilterFieldName IS NULL)				-- Entities not configured with a filter in tblEntitySegmentTemplate, e.g. Product
		)
		AND 
		(
			(b.FilterValueGuid = a.FilterValueGuid)																	-- Entities configured with a non-null filter value, e.g. Equipment configured with an EquipmentType of 'Aircraft'
			OR ((b.FilterFieldName IS NOT NULL) AND (b.FilterValueGuid IS NULL) AND (a.FilterValueGuid IS NULL))	-- Entities configured with a filter in tblEntitySegmentTemplate, but having a null value for the actual field field value, e.g. Equipment with a null Equipment Type
			OR ((b.FilterFieldName IS NULL) AND (a.FilterValueGuid IS NULL))										-- Entities not configured with a filter in tblEntitySegmentTemplate, e.g. Product
		)


		DELETE erv.tblTempFieldLevelConfigMatrix
		WHERE _CallingReferenceGuid = @callingRefGuid

		DECLARE @fieldConfigGuidTemp uniqueidentifier
		DECLARE @siteGroupGuidTemp uniqueidentifier
		DECLARE @hierarchyLevelTemp int
		DECLARE @entitySegmentTemplateGuidTemp uniqueidentifier
		DECLARE @entityTypeIdTemp nvarchar(100)	
		DECLARE @filterFieldNameTemp nvarchar(100)
		DECLARE @filterValueGuidTemp uniqueidentifier
		DECLARE @filterValueGuidTemp1 uniqueidentifier
		DECLARE @filterValueNameTemp nvarchar(100)
		DECLARE @targetFieldTemp nvarchar(100)
		DECLARE @isExternalAttributeTemp bit
		DECLARE @internalFieldNameTemp nvarchar(100)
		DECLARE @inheritedControlModeTemp nvarchar(20)
		DECLARE @forwardControlModeTemp nvarchar(20)
		DECLARE @hasParentSpecificParent bit		

		DECLARE RecordsToInsertUpdateCursor CURSOR FOR 
			SELECT SiteGroupGuid, HierarchyLevel, FieldConfigGuid, EntitySegmentTemplateGuid, EntityTypeId, FilterFieldName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, InheritedControlMode, ForwardControlMode
			FROM @tblRecordsToInsertUpdate
			ORDER BY HierarchyLevel, SiteGroupGuid
		OPEN RecordsToInsertUpdateCursor


		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --PropagateFLCChanges
            SET @BeginTran = 1   
		END  
		BEGIN TRY

			--Propagate the FLC changes
			FETCH NEXT FROM RecordsToInsertUpdateCursor 
			INTO @siteGroupGuidTemp,  @hierarchyLevelTemp, @fieldConfigGuidTemp, @entitySegmentTemplateGuidTemp, @entityTypeIdTemp, @filterFieldNameTemp, @filterValueGuidTemp, @filterValueNameTemp, @targetFieldTemp, @isExternalAttributeTemp, @internalFieldNameTemp, @inheritedControlModeTemp, @forwardControlModeTemp

			WHILE @@FETCH_STATUS = 0
			BEGIN		
				SET @filterValueGuidTemp1 = @filterValueGuidTemp
				IF ((@filterFieldNameTemp IS NOT NULL) AND (@filterValueGuidTemp IS NULL))
				BEGIN
					SET @filterValueGuidTemp1 = @emptyGuid
				END
				EXEC [erv].[usp_HasParentSpecificControlModeParent] @entityTypeIdTemp, @siteGroupGuidTemp, @filterFieldNameTemp, @filterValueGuidTemp1, @targetFieldTemp, @hasParentSpecificParent OUTPUT
					--SELECT @fieldConfigGuidTemp, @entityTypeIdTemp, @siteGroupGuidTemp, @filterFieldNameTemp, @filterValueGuidTemp, @targetFieldTemp, @isExternalAttributeTemp, @internalFieldNameTemp, @forwardControlModeTemp, @hasParentSpecificParent
				IF (@fieldConfigGuidTemp IS NOT NULL)
				BEGIN
					UPDATE [erv].[tblEntityRecordVersioningFieldConfig]
					SET ForwardControlMode = @forwardControlModeTemp				
					, InheritedControlMode = (CASE WHEN @hierarchyLevelTemp = 0 THEN NULL ELSE @forwardControlModeTemp END)
					, UpdatedDate = SYSDATETIMEOFFSET()
					, UpdatedBy = ISNULL(@UserId,SUSER_SNAME())
					WHERE FieldConfigGuid = @fieldConfigGuidTemp
					AND
					(
						((@forwardControlModeTemp IN ('VersionSpecific', 'GlobalSpecific')) AND (@hasParentSpecificParent = 0))
						OR (@forwardControlModeTemp = 'ParentSpecific')
					)
				END
				ELSE IF (
							(@forwardControlModeTemp IN ('VersionSpecific', 'GlobalSpecific')) AND (@hasParentSpecificParent = 0)
						)
				BEGIN
					INSERT INTO [erv].[tblEntityRecordVersioningFieldConfig] 
					([EntitySegmentTemplateGuid],	[SiteGroupGuid],	[TargetField], [IsExternalAttribute], [InternalFieldName], [FilterValueGuid], [FilterValueName], [InheritedControlMode],	[ForwardControlMode],	[CreatedDate],	[CreatedBy],	[UpdatedDate],	[UpdatedBy]	)
					VALUES
					(@EntitySegmentTemplateGuidTemp, @SiteGroupGuidTemp, @TargetFieldTemp, @isExternalAttributeTemp, @internalFieldNameTemp, @FilterValueGuidTemp, @filterValueNameTemp, 
					(CASE WHEN @hierarchyLevelTemp = 0 THEN NULL ELSE @forwardControlModeTemp END)
					, @ForwardControlModeTemp, SYSDATETIMEOFFSET(), ISNULL(@userId,SUSER_SNAME()), SYSDATETIMEOFFSET(), ISNULL(@userId,SUSER_SNAME()))
				END

				FETCH NEXT FROM RecordsToInsertUpdateCursor 
				INTO @siteGroupGuidTemp,  @hierarchyLevelTemp, @fieldConfigGuidTemp, @entitySegmentTemplateGuidTemp, @entityTypeIdTemp, @filterFieldNameTemp, @filterValueGuidTemp, @filterValueNameTemp, @targetFieldTemp, @isExternalAttributeTemp, @internalFieldNameTemp, @inheritedControlModeTemp, @forwardControlModeTemp
			END 

			--Enforce the FLC changes onto Record Versioning
			DECLARE @btsIndex int
			DECLARE @btsEntitySegmentTemplateGuid uniqueidentifier
			DECLARE @btsEntityTypeId nvarchar(100)
			DECLARE @btsSiteGroupGuid uniqueidentifier
			DECLARE @btsFilterValueGuid uniqueidentifier
			DECLARE @btsRecordVersioningInitialState bit
			DECLARE @btsRecordVersioningFinalState bit
			DECLARE @btsRecordVersioningStatusChange nvarchar(10)

			INSERT INTO @tblFinalParentSpecificOrGlobalSpecificFields
			(BaseTargetSegmentIndex, TargetField)
			SELECT DISTINCT a.BaseTargetSegmentIndex, b.TargetField FROM @tblBaseTargetSegments a
			INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
			ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
			AND ISNULL(b.FilterValueGuid, @emptyGuid) = ISNULL(a.FilterValueGuid, @emptyGuid)
			AND b.SiteGroupGuid = b.SiteGroupGuid
			WHERE b.ForwardControlMode in ('ParentSpecific', 'GlobalSpecific')

			WHILE ((SELECT COUNT(*) FROM @tblBaseTargetSegments WHERE Processed = 0) > 0)
			BEGIN
				SELECT TOP 1 @btsIndex = BaseTargetSegmentIndex, @btsEntitySegmentTemplateGuid = EntitySegmentTemplateGuid, @btsEntityTypeId = EntityTypeId, @btsSiteGroupGuid = SiteGroupGuid, @btsFilterValueGuid = FilterValueGuid, @btsRecordVersioningInitialState = RecordVersioningInitialState 
				FROM @tblBaseTargetSegments WHERE Processed = 0 ORDER BY BaseTargetSegmentIndex				

				SET @btsRecordVersioningFinalState = 0
				IF 
				(
					(SELECT COUNT(*) FROM erv.tblEntityRecordVersioningFieldConfig  
 					 WHERE EntitySegmentTemplateGuid = @btsEntitySegmentTemplateGuid
					 AND ISNULL(FilterValueGuid, @emptyGuid) = ISNULL(@btsFilterValueGuid, @emptyGuid)
					 AND SiteGroupGuid = @btsSiteGroupGuid
					 AND ForwardControlMode = 'VersionSpecific') > 0
				)
				BEGIN
					SET @btsRecordVersioningFinalState = 1
				END

				IF ((@btsRecordVersioningInitialState = 1) AND (@btsRecordVersioningFinalState = 0))
				BEGIN
					SET @btsRecordVersioningStatusChange = 'ON_TO_OFF'
				END
				ELSE IF ((@btsRecordVersioningInitialState = 0) AND (@btsRecordVersioningFinalState = 1))
				BEGIN
					SET @btsRecordVersioningStatusChange = 'OFF_TO_ON'
				END
				ELSE IF ((@btsRecordVersioningInitialState = 1) AND (@btsRecordVersioningFinalState = 1))
				BEGIN					
					IF 
					(	--Check for an FLC mode settings that would warrant a field value propagation from the owner record, even though there is no change in the RecordVersioning state, i.e. RecordVersioning stays on.
						(SELECT COUNT(*) FROM @tblFinalParentSpecificOrGlobalSpecificFields a
						WHERE NOT EXISTS
						(
							SELECT * FROM @tblInitialParentSpecificOrGlobalSpecificFields b
							WHERE b.BaseTargetSegmentIndex = a.BaseTargetSegmentIndex
							AND b.TargetField = a.TargetField
						)) > 0
					)
					BEGIN
						SET @btsRecordVersioningStatusChange = 'ON_TO_ON' 
						--Note: For the ON_TO_ON case, we only need to cover for scenarios where a field is turned from VersionSpecific to ParentSpecifica or to GlobalSpecific. If there were already one or more VersionSpecific fields (i.e. RecordVersioing was already ON), then no action is required if another ParentSpecific or GlobalSpecific field is changed to VersionSpecific.
					END
				END

				IF (@btsRecordVersioningStatusChange IS NOT NULL) 
				BEGIN
					IF (@btsEntityTypeId = 'Equipment')
						EXEC [erv].[usp_EnforceFLCChangesOnEquipmentRecordVersioning] @btsEntitySegmentTemplateGuid, @btsFilterValueGuid, @btsSiteGroupGuid, @UserId, @btsRecordVersioningStatusChange
					ELSE IF (@btsEntityTypeId = 'Product')
						EXEC [erv].[usp_EnforceFLCChangesOnProductRecordVersioning] @btsEntitySegmentTemplateGuid, @btsSiteGroupGuid, @UserId, @btsRecordVersioningStatusChange
					ELSE IF (@btsEntityTypeId = 'Company')
						EXEC [erv].[usp_EnforceFLCChangesOnCompanyRecordVersioning] @btsEntitySegmentTemplateGuid, @btsSiteGroupGuid, @UserId, @btsRecordVersioningStatusChange
					ELSE IF (@btsEntityTypeId = 'Transaction_Alias')
						EXEC [erv].[usp_EnforceFLCChangesOnTransactionAliasRecordVersioning] @btsEntitySegmentTemplateGuid, @btsSiteGroupGuid, @UserId, @btsRecordVersioningStatusChange
					ELSE IF (@btsEntityTypeId = 'Personnel')
						EXEC [erv].[usp_EnforceFLCChangesOnPersonnelRecordVersioning] @btsEntitySegmentTemplateGuid, @btsSiteGroupGuid, @UserId, @btsRecordVersioningStatusChange
				END

				UPDATE @tblBaseTargetSegments SET Processed = 1 WHERE BaseTargetSegmentIndex = @btsIndex
			END
			
			--If the @SiteGroupGuid sitegroup is a root node and if all its fields are now set as ParentSpecific, then since the default ForwardControlMode is ParentSpecific, 
			--its erv.tblEntityRecordVersioningFieldConfig entries can safely be deleted. The deletion further removes external references maintained by the @SiteGoupGuid,
			--and allows the @SiteGroupGuid sitegroup to be deleted later if necessary.
			DECLARE @ParentSiteGroupCount int
			SELECT @ParentSiteGroupCount = COUNT(*) FROM map.tblSiteToSite	WHERE ChildSiteGuid = @SiteGroupGuid AND ParentSiteGuid <> @SiteGroupGuid
			IF (@ParentSiteGroupCount = 0) AND ((SELECT COUNT(*) FROM erv.tblEntityRecordVersioningFieldConfig WHERE SiteGroupGuid = @SiteGroupGuid AND ForwardControlMode IN ('VersionSpecific', 'GlobalSpecific')) = 0)
			BEGIN
				DECLARE @tblSiteHierarchy table
				(
					SiteGuid uniqueidentifier,
					SiteId nvarchar(30),
					HierarchyLevel integer
				)
				
				INSERT INTO @tblSiteHierarchy
				EXEC [erv].[usp_GetFLCSiteHierarchy] @SiteGroupGuid, 1

				DELETE a FROM erv.tblEntityRecordVersioningFieldConfig a
				WHERE EXISTS
				(SELECT * FROM @tblSiteHierarchy b WHERE b.SiteGuid = a.SiteGroupGuid)
			END


			IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
				COMMIT TRANSACTION --PropagateFLCChanges

		END TRY
		BEGIN CATCH
			IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
				ROLLBACK TRANSACTION --PropagateFLCChanges
			IF CURSOR_STATUS('global','RecordsToInsertUpdateCursor') >= 0 
			BEGIN
				CLOSE RecordsToInsertUpdateCursor;
				DEALLOCATE RecordsToInsertUpdateCursor;
			END
			DECLARE @ErrorMessage NVARCHAR(4000);
			DECLARE @ErrorSeverity INT;
			DECLARE @ErrorState INT;
			SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
			RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		END CATCH
		CLOSE RecordsToInsertUpdateCursor;
		DEALLOCATE RecordsToInsertUpdateCursor;
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
						+ 'Procedure Name: [erv].usp_UpdateFLCForwardControlMode' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
