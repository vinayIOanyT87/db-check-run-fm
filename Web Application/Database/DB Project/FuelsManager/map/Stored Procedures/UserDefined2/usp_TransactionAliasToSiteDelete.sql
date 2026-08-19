

/*
	EXEC [map].[usp_TransactionAliasToSiteDelete] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'
	EXEC [map].[usp_TransactionAliasToSiteDelete] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', NULL
	EXEC [map].[usp_TransactionAliasToSiteDelete] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', NULL, 1

*/
CREATE PROCEDURE [map].[usp_TransactionAliasToSiteDelete]
(
	@EntityRecordGuid uniqueidentifier,
	@AssignedToSiteGuid uniqueidentifier,
	@DeleteBaseMapping bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_TransactionAliasToSiteDelete]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Deletes a TransactionAliasToSite mapping entry.
	-- Notes:
	-- 1. @EntityRecordGuid: Guid of the TransactionAlias record for which the mapping is to be deleted. This can be either the Master Record Guid or the actual record guid.
	-- 2. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup for which the mapping is to be deleted. 
	--    If the @AssignedToSiteGuid parameter is null, then all the TransactionAlias to Site mappings for the entity record are deleted.
	-- 3. @DeleteBaseMapping: 0: Do not delete the base mapping for the entity record. 1: Delete the base mapping for the entity record.
	-- 3. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	-- 4. This operation also deletes all the other TransactionAliasToSite assignments that have been made possible by the given assignment (Cascading entity assignment deletion).
	-- 5. For each EntityToSite assignment deleted by this operation, the associated record version, if it exists, is also deleted.
	-- 6. The base mapping is the assignment mapping that maps the entity record from the owner site/sitegroup to the owner site/sitegroup itself.
	--	  It is only deleted if the @DeleteBaseMapping parameter is set to 1 and the AssignedToSiteGuid is NULL
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @EntityMasterRecordGuid uniqueidentifier
		SELECT @EntityMasterRecordGuid = _MasterRecordGuid FROM tblTransactionAliases
		WHERE TransactionAliasGuid = @EntityRecordGuid

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
		SELECT MappingGuid, AssignedFromSiteGuid, SiteGuid, HierarchyLevel FROM [erv].[udf_GetTransactionAliasToSiteHierarchyByAssignment] (@EntityMasterRecordGuid, NULL, @AssignedToSiteGuid)

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --DeleteTransactionAliasAssignmentMappings
            SET @BeginTran = 1   
		END  
		BEGIN TRY
			--Delete all the child record versions from the assignment hierarchy
	
			--Delete the Associations mappings of the child record versions
			DELETE a FROM [map].[tblAssociatedTransactionAliases] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.ParentTransactionAliasGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND a.ParentTransactionAliasGuid <> b._MasterRecordGuid

			DELETE a FROM [map].[tblAssociatedTransactionAliases] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.ChildTransactionAliasGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND a.ChildTransactionAliasGuid <> b._MasterRecordGuid

			--Delete the Fields (and FieldOrder) mappings of the child record versions
			DELETE a FROM [dbo].[tblTransactionAliasFields] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND a.TransactionAliasGuid <> b._MasterRecordGuid

			--Delete the Statuses mappings of the child record versions
			DELETE a FROM [map].[tblTransactionAliasToStatus] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND a.TransactionAliasGuid <> b._MasterRecordGuid

			--Delete the UserData mappings of the child record versions
			DELETE a FROM [dbo].[tblUserDataListValueTransactionAlias] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
			ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
			INNER JOIN dbo.tblTransactionAliases c
			ON c.TransactionAliasGuid = b.TransactionAliasGuid
			INNER JOIN @tblEntityToSiteHierarchy d
			ON d.AssignedToSiteGuid = b.SiteGuid
			WHERE c._MasterRecordGuid = @EntityMasterRecordGuid
			AND c.TransactionAliasGuid <> c._MasterRecordGuid

			DELETE a FROM [dbo].[tblUserDataFieldTransactionAlias] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND a.TransactionAliasGuid <> b._MasterRecordGuid

			DELETE a FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
			ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
			INNER JOIN dbo.tblTransactionAliases c
			ON c.TransactionAliasGuid = b.TransactionAliasGuid
			INNER JOIN @tblEntityToSiteHierarchy d
			ON d.AssignedToSiteGuid = b.SiteGuid
			WHERE c._MasterRecordGuid = @EntityMasterRecordGuid
			AND c.TransactionAliasGuid <> c._MasterRecordGuid

			DELETE a FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND a.TransactionAliasGuid <> b._MasterRecordGuid

			--Delete the UserGroups mappings of the child record versions
			DELETE a FROM [map].[tblGroupToTransactionAlias] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON c.AssignedToSiteGuid = b.SiteGuid
			WHERE b._MasterRecordGuid = @EntityMasterRecordGuid
			AND a.TransactionAliasGuid <> b._MasterRecordGuid

			--Delete the child record versions
			DELETE a FROM dbo.tblTransactionAliases a
			INNER JOIN @tblEntityToSiteHierarchy b
			ON b.AssignedToSiteGuid = a.SiteGuid
			WHERE a._MasterRecordGuid = @EntityMasterRecordGuid
			AND a.TransactionAliasGuid <> a._MasterRecordGuid

			--Delete the assignment hierarchy
			DELETE a FROM map.tblEntityTransactionAliasToSite a
			INNER JOIN @tblEntityToSiteHierarchy b 
			ON b.MappingGuid = a.TransactionAliasToSiteGuid

			--Delete the base mapping
			DELETE a FROM map.tblEntityTransactionAliasToSite a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			AND b.SiteGuid = a.SiteGuid
			WHERE a.TransactionAliasGuid = @EntityMasterRecordGuid
			AND b.TransactionAliasGuid = b._MasterRecordGuid
			AND @AssignedToSiteGuid IS NULL
			AND @DeleteBaseMapping = 1

			IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
				COMMIT TRANSACTION --DeleteTransactionAliasAssignmentMappings
		END TRY
		BEGIN CATCH
			IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
				ROLLBACK TRANSACTION --DeleteTransactionAliasAssignmentMappings
			DECLARE @ErrorMessage NVARCHAR(4000);
			DECLARE @ErrorSeverity INT;
			DECLARE @ErrorState INT;
			SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
			RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		END CATCH
		
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
						+ 'Procedure Name: map.usp_TransactionAliasToSiteDelete' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
