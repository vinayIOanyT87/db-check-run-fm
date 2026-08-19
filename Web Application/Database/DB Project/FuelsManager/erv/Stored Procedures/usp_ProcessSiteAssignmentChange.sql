

/*
	DECLARE @tblBefore [erv].[utt_SiteList]
	INSERT INTO @tblBefore
	(SiteGuid)
	SELECT ChildSiteGuid FROM map.tblSiteToSite
	WHERE ParentsiteGuid = '00000000-0000-0000-0000-000000000001'
	AND ChildsiteGuid <> '7B61D0EA-A461-461D-9C08-832C6F672E4E'

	DECLARE @tblAfter [erv].[utt_SiteList]
	INSERT INTO @tblAfter
	SELECT ChildSiteGuid FROM map.tblSiteToSite
	WHERE ParentsiteGuid = '00000000-0000-0000-0000-000000000001'

	EXEC [erv].[usp_ProcessSiteAssignmentChange] '00000000-0000-0000-0000-000000000001', @tblBefore, @tblAfter
*/
/*
	DECLARE @tblBefore [erv].[utt_SiteList]
	INSERT INTO @tblBefore
	(SiteGuid)
	SELECT ChildSiteGuid FROM map.tblSiteToSite
	WHERE ParentsiteGuid = '00000000-0000-0000-0000-000000000001'

	INSERT INTO @tblBefore
	(SiteGuid)
	VALUES ('7B61D0EA-A461-461D-9C08-832C6F672E4E')

	DECLARE @tblAfter [erv].[utt_SiteList]
	INSERT INTO @tblAfter
	SELECT ChildSiteGuid FROM map.tblSiteToSite
	WHERE ParentsiteGuid = '00000000-0000-0000-0000-000000000001'
	AND ChildsiteGuid <> '7B61D0EA-A461-461D-9C08-832C6F672E4E'

	EXEC [erv].[usp_ProcessSiteAssignmentChange] '00000000-0000-0000-0000-000000000001', @tblBefore, @tblAfter
*/
CREATE PROCEDURE [erv].[usp_ProcessSiteAssignmentChange]
(
	@TargetSiteGroupGuid uniqueidentifier, @SiteAssignmentsBefore erv.utt_SiteList READONLY, @SiteAssignmentsAfter erv.utt_SiteList READONLY
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_ProcessSiteAssignmentChange] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Adjusts the entity-to-site mappings, the FLC configurations and the child record versions following a change in the list of sites/sitegroups assigned to a given sitegroup.
	-- Notes:
	-- 1. @TargetSiteGroupGuid: Guid of the target sitegroup whose site assignment list has changed
	-- 2. @SiteAssignmentsBefore: List of sites/sitegroups assigned to the target sitegroup before the assignment change.
	-- 3. @SiteAssignmentsAfter: List of sites/sitegroups assigned to the target sitegroup after the assignment change.
	-- 4. This stored procedure assumes that the site-to-site mapping changes have already been applied at the time it is called.
	-- 5. The only adjustments made to the entity-to-site mappings consists in cascade deleting those that are no longer applicable due to a deleted site-to-site assignment.
	-- 6. For both deleted and newly added site-to-site mappings, the FLC configurations of the sitegroups of the mappings are reprocessed down the site hierarchy, to ensure the FLC configurations are up-to-date with the site hiearchy.
	
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

		DECLARE @runningSiteGuid uniqueidentifier
		DECLARE @runningMappingLevel int

		IF (NOT EXISTS (SELECT * FROM tblSites WHERE SiteGuid = @TargetSiteGroupGuid AND SiteGroupFlag = 1))
		BEGIN
			RETURN
		END

		DECLARE @tblNewSiteAssignments TABLE
		(
			SiteGuid uniqueidentifier,
			SiteGroupFlag bit,
			HierarchyLevel int,
			Processed bit
		);
		DECLARE @tblDeletedSiteAssignments TABLE
		(
			SiteGuid uniqueidentifier,
			SiteGroupFlag bit,
			HierarchyLevel int,
			Processed bit
		);

		DECLARE @tblNewAndDeletedSiteAssignments TABLE
		(
			SiteGuid uniqueidentifier,
			SiteGroupFlag bit,
			HierarchyLevel int,
			Processed bit
		);

		
		INSERT INTO @tblNewSiteAssignments
		(SiteGuid, SiteGroupFlag, HierarchyLevel, Processed)
		(
			SELECT a.SiteGuid, b.SiteGroupFlag, b.HierarchyLevel, 0 FROM @SiteAssignmentsAfter a
			INNER JOIN
			(
				SELECT SiteGuid, SiteId, SiteGroupFlag, HierarchyLevel 
				FROM [erv].[udf_GetSiteHierarchy](NULL, 1)
			) b
			ON b.SiteGuid = a.SiteGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM @SiteAssignmentsBefore c
				WHERE c.SiteGuid = a.SiteGuid
			)
		)
		

		INSERT INTO @tblDeletedSiteAssignments
		(SiteGuid, SiteGroupFlag, HierarchyLevel, Processed)
		(
			SELECT a.SiteGuid, b.SiteGroupFlag, ISNULL(c.HierarchyLevel, 0), 0 FROM @SiteAssignmentsBefore a
			INNER JOIN tblSites b
			on b.SiteGuid = a.SiteGuid
			LEFT OUTER JOIN
			(
				SELECT SiteGuid, SiteId, SiteGroupFlag, HierarchyLevel 
				FROM [erv].[udf_GetSiteHierarchy](NULL, 1)
			) c
			ON c.SiteGuid = a.SiteGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM @SiteAssignmentsAfter d
				WHERE d.SiteGuid = a.SiteGuid
			)
		)

		INSERT INTO @tblNewAndDeletedSiteAssignments
		(SiteGuid, SiteGroupFlag, HierarchyLevel, Processed)
		(
			SELECT SiteGuid, SiteGroupFlag, HierarchyLevel, 0 FROM @tblNewSiteAssignments
			UNION 
			SELECT SiteGuid, SiteGroupFlag, HierarchyLevel, 0 FROM @tblDeletedSiteAssignments
		)


		DECLARE @BeginTran BIT = 0 
		
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION
            SET @BeginTran = 1   
		END  
		
		--Process deleted Site Assignments.
		WHILE ((SELECT COUNT(*) FROM @tblDeletedSiteAssignments WHERE Processed = 0) > 0)
		BEGIN
			SELECT TOP 1 @runningSiteGuid = SiteGuid FROM @tblDeletedSiteAssignments WHERE Processed = 0 ORDER BY HierarchyLevel, SiteGuid				

			EXEC [map].[usp_EquipmentToSiteDeleteAll] @TargetSiteGroupGuid, @runningSiteGuid
			EXEC [map].[usp_ProductToSiteDeleteAll] @TargetSiteGroupGuid, @runningSiteGuid
			EXEC [map].[usp_CompanyToSiteDeleteAll] @TargetSiteGroupGuid, @runningSiteGuid
			EXEC [map].[usp_TransactionAliasToSiteDeleteAll] @TargetSiteGroupGuid, @runningSiteGuid
			EXEC [map].[usp_PersonnelToSiteDeleteAll] @TargetSiteGroupGuid, @runningSiteGuid

			UPDATE @tblDeletedSiteAssignments SET Processed = 1 WHERE SiteGuid = @runningSiteGuid
		END		
		

		--Re-compute the FLC config for all the directly affected sitegroups (both from the new and the deleted assignments list) and propagate those new FLC configs down the site hierarchy.
		--In order to save on the processing time, the FLC configuration is not re-propagated from the parent sitegroup (@TargetSiteGroupGuid), but is re-processed and re-propagated down the 
		--site hierarchy individually for only the child sitegroups of the site-to-site mappings that have changed (added or deleted).
		DECLARE @tblProjectedFLCConfig [erv].[utt_FieldLevelConfig]
		DECLARE @ParentSiteGroupCount int
		WHILE ((SELECT COUNT(*) FROM @tblNewAndDeletedSiteAssignments WHERE SiteGroupFlag = 1 AND Processed = 0) > 0)
		BEGIN
			SELECT TOP 1 @runningSiteGuid = SiteGuid FROM @tblNewAndDeletedSiteAssignments WHERE SiteGroupFlag = 1 AND Processed = 0 ORDER BY HierarchyLevel, SiteGuid				
			DELETE @tblProjectedFLCConfig
						
			SELECT @ParentSiteGroupCount = COUNT(*) FROM map.tblSiteToSite
			WHERE ChildSiteGuid = @runningSiteGuid AND ParentSiteGuid <> @runningSiteGuid

			IF (@ParentSiteGroupCount = 0)
			BEGIN
				--@runningSiteGuid has no parents anymore, i.e. it is now a root sitegroup, and its FLC configurations are to be reset accordingly
				INSERT INTO @tblProjectedFLCConfig
				(EntitySegmentTemplateGuid, EntityTypeId, SiteGroupGuid, FilterFieldName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, InheritedControlMode, ForwardControlMode, HierarchyLevel)			
				SELECT a.EntitySegmentTemplateGuid, b.EntityTypeId, @runningSiteGuid, b.FilterFieldName, a.FilterValueGuid, a.FilterValueName, a.TargetField, a.IsExternalAttribute, a.InternalFieldName,  
				NULL, 'ParentSpecific', 0
				FROM [erv].[tblEntityRecordVersioningFieldConfig] a 
				INNER JOIN erv.tblEntitySegmentTemplate b
				ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
				WHERE a.SiteGroupGuid = @runningSiteGuid
			END
			ELSE
			BEGIN
				INSERT INTO @tblProjectedFLCConfig
				(EntitySegmentTemplateGuid, EntityTypeId, SiteGroupGuid, FilterFieldName, FilterValueGuid, FilterValueName, TargetField, IsExternalAttribute, InternalFieldName, InheritedControlMode, ForwardControlMode)			
				SELECT a.EntitySegmentTemplateGuid, b.EntityTypeId, @runningSiteGuid, b.FilterFieldName, a.FilterValueGuid, a.FilterValueName, a.TargetField, a.IsExternalAttribute, a.InternalFieldName,  
				CASE (COUNT(a.SiteGroupGuid)) WHEN @ParentSiteGroupCount THEN MIN(ISNULL(a.ForwardControlMode, 'ParentSpecific')) ELSE 'ParentSpecific' END, 
				CASE (COUNT(a.SiteGroupGuid)) WHEN @ParentSiteGroupCount THEN MIN(ISNULL(a.ForwardControlMode, 'ParentSpecific')) ELSE 'ParentSpecific' END
				FROM [erv].[tblEntityRecordVersioningFieldConfig] a 
				INNER JOIN erv.tblEntitySegmentTemplate b
				ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
				INNER JOIN map.tblSiteToSite c
				ON c.ParentSiteGuid = a.SiteGroupGuid
				WHERE c.ChildSiteGuid = @runningSiteGuid
				AND c.ParentSiteGuid <> @runningSiteGuid
				GROUP BY a.EntitySegmentTemplateGuid, b.EntityTypeId, b.FilterFieldName, a.FilterValueGuid, a.FilterValueName, a.TargetField, a.IsExternalAttribute, a.InternalFieldName
			END

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
				EXEC [erv].[usp_UpdateFLCForwardControlMode] @tblProjectedFLCConfig, @runningSiteGuid, NULL
			END			

			UPDATE @tblNewAndDeletedSiteAssignments SET Processed = 1 WHERE SiteGuid = @runningSiteGuid
		END		
		
		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		COMMIT TRANSACTION		
	END TRY
	BEGIN CATCH  
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
				ROLLBACK TRANSACTION
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
						+ 'Procedure Name: [erv].usp_ProcessSiteAssignmentChange' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
