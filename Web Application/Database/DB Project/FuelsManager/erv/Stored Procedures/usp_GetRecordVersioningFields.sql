/*
	DROP PROCEDURE [erv].[usp_GetRecordVersioningFields]

	EXEC [erv].[usp_GetRecordVersioningFields] 'Product', '679AD57D-93B2-4C45-82D4-E5C4557EB487', 'AD74B677-F294-4BF8-8861-30D6B424ADC6', 'VersionSpecific', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'
	EXEC [erv].[usp_GetRecordVersioningFields] 'Equipment', '05C83626-004B-4097-A028-E343F4C856F5', '00000000-0000-0000-0000-000000000001', 'VersionSpecific'
	EXEC erv.usp_GetRecordVersioningFields 'Equipment', 'B44649AD-877A-4A41-93B1-9B0E048BE377', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', 'VersionSpecific'
	EXEC erv.usp_GetRecordVersioningFields 'Equipment', 'B44649AD-877A-4A41-93B1-9B0E048BE377', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', 'VersionSpecific'
	EXEC erv.usp_GetRecordVersioningFields 'Equipment', '1BB8C558-5277-47A5-90AE-2461BBD1EFF7', '00000000-0000-0000-0000-000000000001', 'VersionSpecific'
*/



CREATE PROCEDURE [erv].[usp_GetRecordVersioningFields]
(
	@EntityTypeId nvarchar(100), @EntityMasterRecGuid uniqueidentifier, @AssignedFromSiteGuid uniqueidentifier, @FieldLevelControlMode nvarchar(40), @CallingReferenceGuid uniqueidentifier = NULL
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_GetRecordVersioningFields] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose :Returns the fields that belong to a given FCM type for a given entity assignment.
	-- Notes:
	-- 1. @EntityTypeId: Entity Type Id as captured in the Entity Segment Template (erv.tblEntitySegmentTemplate)
	-- 2. @EntityMasterRecGuid: master record Guid of the record
	-- 3. @AssignedFromSiteGuid: AssignedFrom site/sitegroup of the assignment for which the VersionSpecific fields are to be examined. An entity cannot be assigned to another site/sitegroup from a site, but it will have an entity assignment record to/from itws owner site if locally created from a site.
	-- 4. @FieldLevelControlMode: The specific FLC mode to which the returned field set should belong. The options are:
	--    - VersionSpecific: return only VersionSpecific fields
	--    - GlobalSpecific: return only GlobalSpecific fields
	--    - VersionSpecificAndGlobalSpecific: return only VersionSpecific fields and GlobalSpecific fields
	-- 5. @CallingReferenceGuid: 
	--		NULL: Output the results of the query directly
	--		NOT NULL: Save the results of the query in table erv.tblTempRecordVersioningField, using the @CallingReferenceGuid to tag the inserted records.
	-- 6. This SP can be used:
	--		(i) To determine the VersionSpecific and GlobalSpecific fields of a record version at the site/sitegroup owner of an existing record version
	--			E.g. a. When loading a record version (child or parent) for display. 
	--				@AssignedFromSiteGuid is the site/sitegroup from which the record version was created/assigned from.
	--				@EntityMasterRecGuid will be the MasterRecordGuid of the record being loaded.
	--      (ii) To determine the non-VersionSpecific fields of an entity record at the site/sitegroup owner of an existing record version
	--			E.g. b. When deciding on the data field changes to propagate down on a child record version when an entity record is modified.
	--				@AssignedFromSiteGuid is the sitegroup from which the child record version was created/assigned from.
	--				@EntityMasterRecGuid will be the MasterRecordGuid of the record being updated.
	--		(iii) To determine the VersionSpecific fields of an entity record at the sitegroup from which an assignment is taking place. 
	--			E.g. when used to determine if Record Versioning is on for a specific Master record that has been newly assigned to a given site/sitegroup to decide whether a 
	--			record version needs to be created for the new assignment. 
	--				@AssignedFromSiteGuid is the sitegroup from which the assignment took place.
	--				@EntityMasterRecGuid will be the MasterRecordGuid of the record being assigned.
	--      (iv) To determine the GlobalSpecific fields on an entity record at a sitegroup or site
	--          E.g. when figuring out which field of a child record version to replicate onto the local master record copy following changes to the child record version
	--				@AssignedFromSiteGuid is the sitegroup from which the child record version was created/assigned from.
	--				@EntityMasterRecGuid will be the MasterRecordGuid of the record being updated.	
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

		IF ((@EntityTypeId IS NULL) OR (@EntityMasterRecGuid IS NULL) OR (@AssignedFromSiteGuid IS NULL) OR (@FieldLevelControlMode IS NULL))
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END
		IF (@FieldLevelControlMode NOT IN ('VersionSpecific', 'GlobalSpecific', 'VersionSpecificAndGlobalSpecific'))
		BEGIN
			RAISERROR('Invalid FieldLevelControlMode parameter value.',16,1); 
			RETURN;
		END
		DECLARE @tblSegmentInfo TABLE
		(
			FilterValueGuid uniqueidentifier NULL,
			EntitySegmentTemplateGuid uniqueidentifier NOT NULL
		);		

		DECLARE @tblResult TABLE
		(
			TargetField nvarchar(100) NOT NULL,
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL
		);


		--Fetch all the entity segments that apply to the record. This query will usually return a single record.
		--The only situation where the query can return more than one record is that there is more than one entity segment (i.e. more than one filter field) are defined for 
		--the entity type of the entity record.
		INSERT INTO @tblSegmentInfo
		(FilterValueGuid, EntitySegmentTemplateGuid)
		SELECT FilterValueGuid, EntitySegmentTemplateGuid
		FROM [erv].[udf_GetEntitySegmentsByEntityGuid] (@EntityTypeId, @EntityMasterRecGuid)
		--Note: The entity segment info is retrieved using the MasterRecordGuid. This assumes that the Master record version shares the same segment as the specific record version for
		--		which the query is being run. This assumption is based on the fact that the segment templates are established on Guid fields, and fields with an ending of 'Guid' are 
		--		automatically excluded from the FLC (see [erv].[usp_GetFieldLevelConfigMatrix]), i.e. filter fields used to define segment templates are locked as ParentSpecific.
		
		IF NOT EXISTS (SELECT * FROM @tblSegmentInfo)
		BEGIN
			RAISERROR('Cannot locate the segment information for the selected entity record.',16,1); 
			RETURN;
		END

		DECLARE @entitySegmentCount int		
		SELECT @entitySegmentCount = COUNT(*) FROM @tblSegmentInfo

		--Fetch the basic information about the target entity record. 
		DECLARE @MasterRecordGuid uniqueidentifier
		DECLARE @OwnerSiteGuid uniqueidentifier
		EXEC [erv].[usp_GetEntityBasicInfo] @EntityTypeId, @EntityMasterRecGuid, @MasterRecordGuid OUTPUT,  @OwnerSiteGuid OUTPUT

		IF ((@MasterRecordGuid IS NULL) OR (@MasterRecordGuid <> @EntityMasterRecGuid) OR (@OwnerSiteGuid IS NULL))
		BEGIN
			RAISERROR('Cannot locate the information for the selected entity record.',16,1); 
			RETURN;
		END


		--For a sitegroup owner it is not necessary to check the parent sitegroups since the InheritedControlMode field already combines the net ForwardControlMode effect of the parent sitegroups.
		--A TargetField for an entity record version assigned from a sitegroup is determined as being VersionSpecific if all the tblEntityRecordVersioningFieldConfig records for the
		--TargetField of the record version have a ForwardControlMode value of VersionSpecific.
		--Usually a TargetField will have only one tblEntityRecordVersioningFieldConfig record for a given SiteGroup, except if more than one entity segment (i.e. more than one 
		--filter field) are defined for the entity type of the TargetField.				
		INSERT INTO @tblResult
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		SELECT b.TargetField, b.IsExternalAttribute, b.InternalFieldName, b.ForwardControlMode FROM @tblSegmentInfo a		
		INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
		ON (b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid) 				
		AND (ISNULL(b.FilterValueGuid, @emptyGuid) = ISNULL(a.FilterValueGuid, @emptyGuid))
		WHERE b.SiteGroupGuid = @AssignedFromSiteGuid
		AND 
		(
			((@FieldLevelControlMode = 'VersionSpecificAndGlobalSpecific') AND (b.ForwardControlMode IN ('VersionSpecific', 'GlobalSpecific')))
			OR (b.ForwardControlMode = @FieldLevelControlMode)
		)
		GROUP BY b.TargetField, b.IsExternalAttribute, b.InternalFieldName, b.ForwardControlMode
		HAVING COUNT(*) = @entitySegmentCount


		IF (@CallingReferenceGuid IS NULL)
		BEGIN
			SELECT * FROM @tblResult
			ORDER BY TargetField
		END
		ELSE
		BEGIN
			INSERT INTO erv.tblTempRecordVersioningField
			(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode, _CallingReferenceGuid)
			SELECT TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode, @CallingReferenceGuid
			FROM @tblResult
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
						+ 'Procedure Name: [erv].usp_GetRecordVersioningFields' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
