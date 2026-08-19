

/*
	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	--EXEC [map].[usp_CreateEquipmentToSiteMapping] '886AA683-C97D-461C-AFB6-AD9A4579E51D', '00000000-0000-0000-0000-000000000001', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', @dt, 'HB'
	--EXEC [map].[usp_CreateEquipmentToSiteMapping] 'b44649ad-877a-4a41-93b1-9b0e048be377', 'f4761a16-ab2f-41ee-b6fa-d17658df2602', '46426312-e408-4af8-85fd-338b622b32bf', @dt, 'HB'

	--EXEC [map].[usp_CreateEquipmentToSiteMapping] '5dd680be-f07c-42f6-858e-6c908f1bb87b', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', @dt, 'HB'

	EXEC [map].[usp_CreateEquipmentToSiteMapping] '1bb8c558-5277-47a5-90ae-2461bbd1eff7', '46426312-e408-4af8-85fd-338b622b32bf', 'aeba18e3-e97b-479e-8b2d-0bcd69c1c421', @dt, 'HB'

*/


CREATE PROCEDURE [map].[usp_CreateEquipmentToSiteMapping]
(
	@EntityRecordGuid uniqueidentifier, 
	@AssignedFromSiteGuid uniqueidentifier, 
	@AssignedToSiteGuid uniqueidentifier, 
	@ExtendToCompartments bit = 1,
	@CreatedDate datetimeoffset(7), 
	@CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_CreateEquipmentToSiteMapping] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new Equipment To Site mapping record, and creates a child record version as necessary.
	-- Notes:
	-- 1. @EntityRecordGuid: Record Guid of the entity record to be mapped. This can be either the Master Record Guid or the actual record guid of the record to be mapped.
	-- 2. @AssignedFromSiteGuid: SiteGroup from which the entity record should be mapped from.
	-- 3. @AssignedToSiteGuid: Site/SiteGroup to which the entity record should be mapped to.
	-- 4. @ExtendToCompartments: 0:Apply the mapping only on the target Equipment; 1: Extend the site assignment to the Compartments of the target Equipment as well. 
	-- 5. If the AssignedToSite is an indirect child of the AssignedFromSite, the entity-to-site mapping request is cascaded as necessary.
	-- 6. A child record version is only created following the assignment if Record Versioning is verified to be On for the newly created entity assignment/s.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @EntityMasterRecGuid uniqueidentifier
		DECLARE @ParentEquipmentGuid uniqueidentifier
		SELECT @EntityMasterRecGuid = _MasterRecordGuid, @ParentEquipmentGuid = ParentEquipmentGuid FROM tblEquipment
		WHERE EquipmentGuid = @EntityRecordGuid

		DECLARE @BeginTran BIT = 0 
		
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --MapEquipment
            SET @BeginTran = 1   
		END  

		IF (@AssignedToSiteGuid = @AssignedFromSiteGuid)
		BEGIN
			-- Create the self-site EntityToSite assignment
			INSERT INTO [map].[tblEntityEquipmentToSite]
		   (EquipmentGuid, SiteGuid, AssignedFromSiteGuid, CreatedDate, CreatedBy)
			SELECT @EntityMasterRecGuid, @AssignedToSiteGuid, @AssignedFromSiteGuid, @CreatedDate, @CreatedBy
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblEntityEquipmentToSite]
				WHERE EquipmentGuid = @EntityMasterRecGuid
				AND SiteGuid = @AssignedToSiteGuid
				AND AssignedFromSiteGuid = SiteGuid
			)

		  --Extend the Entity-to-Site assignment to all the Compartments of the target Equipment
		  INSERT INTO [map].[tblEntityEquipmentToSite]
		  (EquipmentGuid, SiteGuid, AssignedFromSiteGuid, CreatedDate, CreatedBy)
		  SELECT a.EquipmentGuid, @AssignedToSiteGuid, @AssignedFromSiteGuid, @CreatedDate, @CreatedBy
		  FROM dbo.tblEquipment a
		  WHERE a.ParentEquipmentGuid = @EntityMasterRecGuid
		  AND @ExtendToCompartments = 1
		  AND NOT EXISTS
		  (
				SELECT * FROM [map].[tblEntityEquipmentToSite] b
				WHERE b.EquipmentGuid = a.EquipmentGuid
				AND SiteGuid = @AssignedToSiteGuid
				AND AssignedFromSiteGuid = @AssignedFromSiteGuid
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
				INSERT INTO [map].[tblEntityEquipmentToSite]
				(EquipmentGuid, SiteGuid, AssignedFromSiteGuid, CreatedDate, CreatedBy)
				SELECT @EntityMasterRecGuid, @childSiteGuid, @parentSiteGuid, @CreatedDate, @CreatedBy
				WHERE NOT EXISTS
				(
					 SELECT * FROM [map].[tblEntityEquipmentToSite]
					 WHERE EquipmentGuid = @EntityMasterRecGuid
					 AND SiteGuid = @ChildSiteGuid
					 AND AssignedFromSiteGuid = @parentSiteGuid				
				)

				--Extend the Entity-to-Site assignment to all the Compartments of the target Equipment
				INSERT INTO [map].[tblEntityEquipmentToSite]
				(EquipmentGuid, SiteGuid, AssignedFromSiteGuid, CreatedDate, CreatedBy)
				SELECT a.EquipmentGuid, @childSiteGuid, @parentSiteGuid, @CreatedDate, @CreatedBy
				FROM dbo.tblEquipment a
				WHERE a.ParentEquipmentGuid = @EntityMasterRecGuid
				AND @ExtendToCompartments = 1
				AND NOT EXISTS
				(
					 SELECT * FROM [map].[tblEntityEquipmentToSite] b
					 WHERE b.EquipmentGuid = a.EquipmentGuid
					 AND SiteGuid = @childSiteGuid
					 AND AssignedFromSiteGuid = @parentSiteGuid
				)

				-- Record Versioning does not apply to Compartments (Equipments with a ParentEquipmentGuid value).
				IF (@ParentEquipmentGuid IS NOT NULL)
				BEGIN
					 CONTINUE;  
				END 

				--Create a new child record version if Record Versioning is verified to be ON for the newly created entity assignment
				DECLARE @IsRecVerOn bit
				EXEC [erv].[usp_IsRecordVersioningOnForEntity] 'Equipment', @EntityMasterRecGuid, @AssignedFromSiteGuid, @IsRecVerOn OUTPUT
				IF ((@IsRecVerOn IS NOT NULL) AND (@IsRecVerOn = 1))
				BEGIN
					DECLARE @parentEntityGuid uniqueidentifier
					SELECT @parentEntityGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', @EntityMasterRecGuid, @parentSiteGuid)
					IF (@parentEntityGuid IS NULL)
					BEGIN
						RAISERROR('Cannot locate the parent record version for the assignment.',16,1); 
						RETURN;
					END
					EXEC [erv].[usp_CreateEquipmentChildRecordVersion] @parentEntityGuid, @childSiteGuid, @CreatedDate, @CreatedBy
				END
						
				FETCH NEXT FROM TableCursor INTO @parentSiteGuid, @childSiteGuid, @hierarchyLevel 
			END 
		CLOSE TableCursor 
		DEALLOCATE TableCursor 

		
		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		COMMIT TRANSACTION --MapEquipment		
	END TRY
	BEGIN CATCH  
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
				ROLLBACK TRANSACTION --MapEquipment
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
							+ 'Procedure Name: [map].usp_CreateEquipmentToSiteMapping' + CHAR(13)+CHAR(10)                  
							+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
			RAISERROR(@_ErrMessage,18,1);
		END      
	END CATCH    
	
END