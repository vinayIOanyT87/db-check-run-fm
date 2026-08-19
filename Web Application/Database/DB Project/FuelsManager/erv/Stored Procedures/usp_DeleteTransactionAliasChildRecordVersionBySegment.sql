

/*

	EXEC [erv].[usp_DeleteTransactionAliasChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', '00000000-0000-0000-0000-000000000001'
	EXEC [erv].[usp_DeleteTransactionAliasChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'
	EXEC [erv].[usp_DeleteTransactionAliasChildRecordVersionBySegment] '7C313838-6CA6-4484-9DF2-2E21B6159B10', '00000000-0000-0000-0000-000000000001'

*/



CREATE PROCEDURE [erv].[usp_DeleteTransactionAliasChildRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @IncludeChildRecordVersionsAssignedToSourceSiteGroup bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_DeleteTransactionAliasChildRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Deletes all the TransactionAlias child record versions for all the entity assignments of a given TransactionAlias segment (note: there is only one Segment Template for TransactionAliases) 
	-- from a given SiteGroup (if @IncludeChildRecordVersionsAssignedToSourceSiteGroup = 0) or both from and to the given sitegroup (if @IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Segment Template that needs to be processed.
	-- 2. @SourceSiteGroupGuid: SiteGroup parent from which the child record versions to be deleted were created. This would correspond to the AssignedFrom Sitegroup.
	-- 3. @IncludeChildRecordVersionsAssignedToSourceSiteGroup: 
	--			0 (Default Mode). Only delete the child record versions assigned from the sitegroup.
	--			1: Delete both the child record versions assigned from and to the sitegroup.
	-- 4. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	
	------------------------------------------------------------------------------------------------------
	BEGIN TRY
	
		DECLARE @callingRefGuid uniqueidentifier
		SET @callingRefGuid = NEWID()					

		DECLARE @entityTypeId nvarchar(100)
		SELECT @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		IF (@entityTypeId = 'Transaction_Alias')
		BEGIN
			--Capture the Site/SiteGroup, MasterRecordGuid, and the Entity Guid of the child record versions to be deleted.
			INSERT INTO [erv].[tblTempEntityRecordVersion]
			(SiteGuid, MasterRecordGuid, EntityGuid, AssignedFromSiteGuid, _CallingReferenceGuid)
			SELECT b.SiteGuid, b.TransactionAliasGuid, a.TransactionAliasGuid, b.AssignedFromSiteGuid, @callingRefGuid
			FROM tblTransactionAliases a
			INNER JOIN map.tblEntityTransactionAliasToSite b
			ON b.TransactionAliasGuid = a._MasterRecordGuid
			AND b.SiteGuid = a.SiteGuid  
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND b.SiteGuid <> b.AssignedFromSiteGuid
			AND a.TransactionAliasGuid <> a._MasterRecordGuid

			IF (@IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
			BEGIN
				INSERT INTO [erv].[tblTempEntityRecordVersion]
				(SiteGuid, MasterRecordGuid, EntityGuid, AssignedFromSiteGuid, _CallingReferenceGuid)
				SELECT b.SiteGuid, b.TransactionAliasGuid, a.TransactionAliasGuid, b.AssignedFromSiteGuid, @callingRefGuid
				FROM tblTransactionAliases a
				INNER JOIN map.tblEntityTransactionAliasToSite b
				ON b.TransactionAliasGuid = a._MasterRecordGuid
				AND b.SiteGuid = a.SiteGuid  
				WHERE b.SiteGuid = @SourceSiteGroupGuid
				AND b.SiteGuid <> b.AssignedFromSiteGuid
				AND a.TransactionAliasGuid <> a._MasterRecordGuid
			END
		END

		IF (@IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
		BEGIN
			UPDATE a 
			SET a.ParentRecordGuid = erv.udf_GetFirstParentRecordVersionGuid('Transaction_Alias', a.MasterRecordGuid, b.AssignedFromSiteGuid)
			FROM [erv].[tblTempEntityRecordVersion] a	
			INNER JOIN map.tblEntityTransactionAliasToSite b
			ON b.TransactionAliasGuid = a.MasterRecordGuid		
			AND b.SiteGuid = a.AssignedFromSiteGuid
			WHERE a._CallingReferenceGuid = @callingRefGuid
		END
		ELSE
		BEGIN
			UPDATE a 
			SET a.ParentRecordGuid = erv.udf_GetFirstParentRecordVersionGuid('Transaction_Alias', a.MasterRecordGuid, a.AssignedFromSiteGuid)
			FROM [erv].[tblTempEntityRecordVersion] a	
			WHERE a._CallingReferenceGuid = @callingRefGuid		
		END

		--Delete the external attributes of the parent record version
		
		--Process the [map].[tblAssociatedTransactionAliases] mappings
		--Delete the Associations mappings owned by the target  child record versions, i.e. where  where the target child record versions are involved as ParentTransactionAlias
		DELETE a FROM [map].[tblAssociatedTransactionAliases] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.ParentTransactionAliasGuid
		INNER JOIN dbo.tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid	
		AND b._CallingReferenceGuid = @callingRefGuid	

		--Reset the ChildTransactionAlias reference of those AssociatedTransactionAliases mappings that are tied to the ChildTransactionAliases that are to be deleted, so that the mappings point to the Parent TransactionAliases
		UPDATE a
		SET a.ChildTransactionAliasGuid = b.ParentRecordGuid
		FROM [map].[tblAssociatedTransactionAliases] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.ChildTransactionAliasGuid
		WHERE b._CallingReferenceGuid = @callingRefGuid

		--Delete the Associations mappings where the target child record versions are involved as ChildTransactionAlias
		DELETE a FROM [map].[tblAssociatedTransactionAliases] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.ChildTransactionAliasGuid
		INNER JOIN dbo.tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid		
		AND b._CallingReferenceGuid = @callingRefGuid


		--Delete the Fields and FieldOrder mappings of the child record versions
		DELETE a FROM [dbo].[tblTransactionAliasFields] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.TransactionAliasGuid
		INNER JOIN dbo.tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Reset the Shared Mappings between Product and TransactionAlias. This will delete, update, and clone the applicable Product-to-TransactionAlias mappings as necessary.
		--The following Target fields and mapping tables are covered by this process: 
		--(Products)->[map].[tblProductToTransactionAliasExclusion]
		DECLARE @tblTargetTransactionAliases erv.utt_EntityRecordVersions
		INSERT INTO @tblTargetTransactionAliases
		(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
		SELECT 'Transaction_Alias', EntityGuid, MasterRecordGuid, SiteGuid FROM [erv].[tblTempEntityRecordVersion] WHERE _CallingReferenceGuid = @callingRefGuid

		EXEC [erv].[usp_SetProductToTransactionAliasMappingsForDeletedTransactionAliases] @tblTargetTransactionAliases


		--Delete the Statuses mappings of the child record versions
		DELETE a FROM [map].[tblTransactionAliasToStatus] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.TransactionAliasGuid
		INNER JOIN dbo.tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Delete the UserData mappings of the child record versions
		--[dbo].[tblUserDataFieldTransactionAlias] and [dbo].[tblUserDataListValueTransactionAlias]
		DELETE a FROM [dbo].[tblUserDataListValueTransactionAlias] a
		INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
		ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
		INNER JOIN [erv].[tblTempEntityRecordVersion] c
		ON c.EntityGuid = b.TransactionAliasGuid
		INNER JOIN dbo.tblTransactionAliases d
		ON d._MasterRecordGuid = c.MasterRecordGuid
		AND d.SiteGuid = c.SiteGuid
		WHERE d._MasterRecordGuid <> d.TransactionAliasGuid
		AND c._CallingReferenceGuid = @callingRefGuid

		DELETE a FROM [dbo].[tblUserDataFieldTransactionAlias] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.TransactionAliasGuid
		INNER JOIN dbo.tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--[dbo].[tblUserDataFieldTransactionAliasLineItem] and [dbo].[tblUserDataListValueTransactionAliasLineItem]
		DELETE a FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] a
		INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
		ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
		INNER JOIN [erv].[tblTempEntityRecordVersion] c
		ON c.EntityGuid = b.TransactionAliasGuid
		INNER JOIN dbo.tblTransactionAliases d
		ON d._MasterRecordGuid = c.MasterRecordGuid
		AND d.SiteGuid = c.SiteGuid
		WHERE d._MasterRecordGuid <> d.TransactionAliasGuid
		AND c._CallingReferenceGuid = @callingRefGuid

		DELETE a FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.TransactionAliasGuid
		INNER JOIN dbo.tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND b._CallingReferenceGuid = @callingRefGuid



		--Delete the UserGroups mappings of the child record versions
		DELETE a FROM [map].[tblGroupToTransactionAlias] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.TransactionAliasGuid
		INNER JOIN dbo.tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND b._CallingReferenceGuid = @callingRefGuid


		--Delete the child record versions
		DELETE a FROM tblTransactionAliases a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.MasterRecordGuid = a._MasterRecordGuid
		AND b.EntityGuid = a.TransactionAliasGuid
		WHERE b._CallingReferenceGuid = @callingRefGuid
		AND a.TransactionAliasGuid <> a._MasterRecordGuid

		DELETE [erv].[tblTempEntityRecordVersion] WHERE _CallingReferenceGuid = @callingRefGuid
		

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
						+ 'Procedure Name: [erv].usp_DeleteTransactionAliasChildRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     