

/*
	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	--EXEC [map].[usp_CreateTransactionAliasToSiteMapping] '886AA683-C97D-461C-AFB6-AD9A4579E51D', '00000000-0000-0000-0000-000000000001', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', @dt, 'HB'

	EXEC [map].[usp_CreateTransactionAliasToSiteMapping] '0dc68aca-11ad-4f43-ad2b-87609738c453', '00000000-0000-0000-0000-000000000001', 'f4761a16-ab2f-41ee-b6fa-d17658df2602', @dt, 'HB'

*/


CREATE PROCEDURE [map].[usp_CreateTransactionAliasToSiteMapping]
(
	@EntityRecordGuid uniqueidentifier, @AssignedFromSiteGuid uniqueidentifier, @AssignedToSiteGuid uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100))
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_CreateTransactionAliasToSiteMapping] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new TransactionAlias To Site mapping record, and creates a child record version as necessary.
	-- Notes:
	-- 1. @EntityRecordGuid: Record Guid of the entity record to be mapped. This can be either the Master Record Guid or the actual record guid of the record to be mapped.
	-- 2. @AssignedFromSiteGuid: SiteGroup from which the entity record should be mapped from.
	-- 3. @AssignedToSiteGuid: Site/SiteGroup to which the entity record should be mapped to.
	-- 4. If the AssignedToSite is an indirect child of the AssignedFromSite, the entity-to-site mapping request is cascaded as necessary.
	-- 5. A child record version is only created following the assignment if Record Versioning is verified to be On for the newly created entity assignment.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @EntityMasterRecGuid uniqueidentifier
		SELECT @EntityMasterRecGuid = _MasterRecordGuid FROM tblTransactionAliases
		WHERE TransactionAliasGuid = @EntityRecordGuid

		DECLARE @BeginTran BIT = 0 
		
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --MapTransactionAlias
            SET @BeginTran = 1   
		END  

		IF (@AssignedToSiteGuid = @AssignedFromSiteGuid)
		BEGIN
		  -- Create the self-site EntityToSite assignment
		  INSERT INTO [map].[tblEntityTransactionAliasToSite]
		  (TransactionAliasGuid, SiteGuid, AssignedFromSiteGuid, CreatedDate, CreatedBy)
		  SELECT @EntityMasterRecGuid, @AssignedToSiteGuid, @AssignedFromSiteGuid, @CreatedDate, @CreatedBy
		  WHERE NOT EXISTS
		  (
				SELECT * FROM [map].[tblEntityTransactionAliasToSite]
				WHERE TransactionAliasGuid = @EntityMasterRecGuid
				AND SiteGuid = @AssignedToSiteGuid
				AND AssignedFromSiteGuid = SiteGuid
		  )

			-- Cascading Assignments and Record Versioning do not apply to the base entity assignment (assignment of the entity record with its owner site guid).
			RETURN;  
		END 

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
				INSERT INTO [map].[tblEntityTransactionAliasToSite]
				(TransactionAliasGuid, SiteGuid, AssignedFromSiteGuid, CreatedDate, CreatedBy)
				SELECT @EntityMasterRecGuid, @childSiteGuid, @parentSiteGuid, @CreatedDate, @CreatedBy
				WHERE NOT EXISTS
				(
					SELECT * FROM [map].[tblEntityTransactionAliasToSite]
					WHERE TransactionAliasGuid = @EntityMasterRecGuid
					AND SiteGuid = @ChildSiteGuid
					AND AssignedFromSiteGuid = @parentSiteGuid				
				)

				--Create a new child record version if Record Versioning is verified to be ON for the newly created entity assignment
				DECLARE @IsRecVerOn bit
				EXEC [erv].[usp_IsRecordVersioningOnForEntity] 'Transaction_Alias', @EntityMasterRecGuid, @AssignedFromSiteGuid, @IsRecVerOn OUTPUT
				IF ((@IsRecVerOn IS NOT NULL) AND (@IsRecVerOn = 1))
				BEGIN
					DECLARE @parentEntityGuid uniqueidentifier
					SELECT @parentEntityGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @EntityMasterRecGuid, @parentSiteGuid)
					IF (@parentEntityGuid IS NULL)
					BEGIN
						RAISERROR('Cannot locate the parent record version for the assignment.',16,1); 
						RETURN;
					END
					EXEC [erv].[usp_CreateTransactionAliasChildRecordVersion] @parentEntityGuid, @childSiteGuid, @CreatedDate, @CreatedBy
				END
						
				FETCH NEXT FROM TableCursor INTO @parentSiteGuid, @childSiteGuid, @hierarchyLevel 
			END 
		CLOSE TableCursor 
		DEALLOCATE TableCursor


		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		COMMIT TRANSACTION --MapTransactionAlias		
	END TRY
	BEGIN CATCH  
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
				ROLLBACK TRANSACTION --MapTransactionAlias
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
							+ 'Procedure Name: [map].usp_CreateTransactionAliasToSiteMapping' + CHAR(13)+CHAR(10)                  
							+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
			RAISERROR(@_ErrMessage,18,1);      
		END
	END CATCH    
	
END