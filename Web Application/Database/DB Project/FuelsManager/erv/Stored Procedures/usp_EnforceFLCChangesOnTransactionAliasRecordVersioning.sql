

/*

	EXEC [erv].[usp_EnforceFLCChangesOnTransactionAliasRecordVersioning] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', '00000000-0000-0000-0000-000000000001', 'Administrator', 'ON_TO_OFF'

*/


CREATE PROCEDURE [erv].[usp_EnforceFLCChangesOnTransactionAliasRecordVersioning]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SiteGroupGuid uniqueidentifier, @UserId nvarchar(100), @RecordVersioningStatusChange nvarchar(10)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_EnforceFLCChangesOnTransactionAliasRecordVersioning] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Enforce the effect of the latest Field Level Control configuration changes onto Record Versioning. 
	-- This operation will delete existing child record versions, or create new child record versions, or update existing child record versions, as dictated by the latest FLC changes.
	-- This operation will operate on a given entity type, down the site hierarchy, below a given site group.
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Segment Template for which to enforce the FLC changes. This should correspond to the EntitySegmentTemplate for which the FLC changes was initiated.
	-- 2. @SiteGroupGuid: The site group from which the changes need to be propagated down the site hierarchy. This should correspond to the sitegroup where the FLC changes were initiated.
	-- 3. @UserId: Id of the user that initiated the FLC configuration changes
	-- 4. @RecordVersioningStatusChange: 
	--		ON_TO_OFF: The FLC Changes caused RecordVersioning to be turned OFF at the sitegroup where they were initiated.
	--		OFF_TO_ON: The FLC Changes caused RecordVersioning to be turned ON at the sitegroup where they were initiated.
	--		ON_TO_ON:  RecordVersioing stayed ON at the sitegroup where the FLC changes were initiated.
	-- 5. Because of the volume of record changes that this operation might cover, it might be best not to run this process live, right after FLC configuration changes, but instead as a nightly 
	--    scheduled process. In this case, there will be two sets of FLC Configurations: an active set that has been processed, and an unprocessed set that is waiting to be processed. 
	--    Only one (the latest) unprocessed FLC Configuration set should be maintained for each combination of EntitySegmentTemplateGuid, FilterValueGuid, and SiteGroup.
	--    For each unprocessed combination of EntitySegmentTemplateGuid and FilterValueGuid, the nightly process will extract the new FLC configurations for each sitegroup in the site hierarchy order, 
	--    starting with the root node/s. For each EntitySegmentTemplateGuid, FilterValueGuid and Sitegroup combination where a difference is noted between the active and unprocessed FLC configurations,
	--    the process will run [erv].[usp_UpdateFLCForwardControlMode]. The latest value of the LastUpdatedBy field for the unprocessed segment will be used for the UserId parameter 
	--    in [erv].[usp_UpdateFLCForwardControlMode].
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @EntityTypeId nvarchar(100)
		DECLARE @createdDate datetimeoffset(7)
		DECLARE @runningSiteGroupGuid uniqueidentifier		


		SET @createdDate = SYSDATETIMEOFFSET()
		SET @UserId = ISNULL(@userId,SUSER_SNAME())

		SELECT @EntityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid


		IF (@EntityTypeId <> 'Transaction_Alias')
		BEGIN
			RAISERROR('Invalid Entity Type.',16,1); 
			RETURN;
		END

		-- Capture the site/sitegroups below the sitegroup where the FLC configuration changes were applied
		DECLARE @tblSiteHierarchy TABLE
		(
			SiteGuid uniqueidentifier
			, SiteId nvarchar(30)
			, HierarchyLevel int
			, SiteGroupFlag bit
			, Processed bit
		);

		INSERT INTO @tblSiteHierarchy
		(SiteGuid, SiteId, HierarchyLevel, SiteGroupFlag, Processed)
		SELECT SiteGuid, SiteId, HierarchyLevel, SiteGroupFlag, 0
		FROM [erv].[udf_GetSiteHierarchy](@SiteGroupGuid, 0)
		ORDER BY HierarchyLevel, SiteGuid


		--If the FLC changes at the originating sitegroup caused FCM-RecordVersioning to be turned OFF for the originating node, then delete all the child record versions at all the children nodes for the given Entity Segment Template and Filter value, irrespective of the AssignedFrom sitegroup.
		IF (@RecordVersioningStatusChange = 'ON_TO_OFF')
		BEGIN		
			WHILE ((SELECT Count(*) From @tblSiteHierarchy Where Processed = 0) > 0)
			BEGIN
				SELECT TOP 1 @runningSiteGroupGuid = SiteGuid FROM @tblSiteHierarchy WHERE Processed = 0 ORDER BY HierarchyLevel, SiteGuid
				EXEC [erv].[usp_DeleteTransactionAliasChildRecordVersionBySegment] @EntitySegmentTemplateGuid, @runningSiteGroupGuid
				UPDATE @tblSiteHierarchy SET Processed = 1 WHERE SiteGuid = @runningSiteGroupGuid
			END					
		END
		--If the FLC changes at the originating sitegroup caused FCM-RecordVersioning to be turned ON for the originating node, then create new child record versions at all the children nodes for the given Entity Segment Template and Filter value, for all TransactionAlias entity assignement assigned to the children nodes and for which a child record version is missing, irrespective of the AssignedFrom sitegroup.
		ELSE IF (@RecordVersioningStatusChange = 'OFF_TO_ON')
		BEGIN					
			WHILE ((SELECT COUNT(*) FROM @tblSiteHierarchy WHERE Processed = 0) > 0)
			BEGIN
				SELECT TOP 1 @runningSiteGroupGuid = SiteGuid FROM @tblSiteHierarchy WHERE Processed = 0 ORDER BY HierarchyLevel, SiteGuid
				EXEC [erv].[usp_CreateTransactionAliasChildRecordVersionBySegment] @EntitySegmentTemplateGuid, @runningSiteGroupGuid, @createdDate, @UserId
				UPDATE @tblSiteHierarchy SET Processed = 1 WHERE SiteGuid = @runningSiteGroupGuid
			END
		END
		--If the FLC changes at the originating sitegroup caused FCM-RecordVersioning to remain ON for the originating node, then re-run record versioning propagation down all the the children nodes for the given Entity Segment Template and Filter value, for all TransactionAlias entity assignement assigned to the children nodes and for which a child record version exists, only for mappings with an AssignedFrom sitegroup that corresponds to the sitegroup where the FLC configuration changes were initiated or from one of its children sitegroups.
		ELSE IF (@RecordVersioningStatusChange = 'ON_TO_ON')
		BEGIN
			WHILE ((SELECT COUNT(*) FROM @tblSiteHierarchy WHERE Processed = 0) > 0)
			BEGIN
				SELECT TOP 1 @runningSiteGroupGuid = SiteGuid FROM @tblSiteHierarchy WHERE Processed = 0 ORDER BY HierarchyLevel, SiteGuid			
				IF ((SELECT COUNT(*) FROM erv.tblEntityRecordVersioningFieldConfig WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid AND SiteGroupGuid = @runningSiteGroupGuid AND ISNULL(InheritedControlMode, 'VersionSpecific') = 'VersionSpecific') = 0)	
				BEGIN
					--This is a case of Indirect Record Versioning Turn OFF, whereby the FLC changes at the originating sitegroup did not turn OFF Record Versioning at that sitegroup, but resulted in 
					--the ICM of a lower sitegroup (@runningSiteGroupGuid) to be all ParentSpecific. 
					EXEC [erv].[usp_DeleteTransactionAliasChildRecordVersionBySegment] @EntitySegmentTemplateGuid, @runningSiteGroupGuid, 1
				END
				ELSE
				BEGIN
					EXEC [erv].[usp_PropagateTransactionAliasRecordVersionBySegment] @EntitySegmentTemplateGuid, @runningSiteGroupGuid
				END
				UPDATE @tblSiteHierarchy SET Processed = 1 WHERE SiteGuid = @runningSiteGroupGuid
			END
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
						+ 'Procedure Name: [erv].usp_EnforceFLCChangesOnTransactionAliasRecordVersioning' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END   
