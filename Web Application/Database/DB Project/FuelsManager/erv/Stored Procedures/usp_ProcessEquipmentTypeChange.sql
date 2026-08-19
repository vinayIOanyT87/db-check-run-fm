

/*

	EXEC [erv].[usp_ProcessEquipmentTypeChange] '1bb8c558-5277-47a5-90ae-2461bbd1eff7'

*/


CREATE PROCEDURE [erv].[usp_ProcessEquipmentTypeChange]
(
	@EntityRecordGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_ProcessEquipmentTypeChange] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Adjusts the child record versions of an equipment master record following an Equipment Type change on the equipment master record.
	-- Notes:
	-- 1. @EntityRecordGuid: Record Guid of the equipment record whose EquipmentType has been changed.
	-- 2. The EquipmentType field, which is used as a filter on the Equipment Segment Template, can only be modified for a master record.
	
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @entityMasterRecGuid uniqueidentifier
		DECLARE @newEquipmentTypeGuid uniqueidentifier
		DECLARE @masterSiteGroupGuid uniqueidentifier
		DECLARE @runningSiteGuid uniqueidentifier
		DECLARE @parentEntityGuid uniqueidentifier
		DECLARE @runningAssignedFromSiteGuid uniqueidentifier
		DECLARE @IsRecVerOn bit
		DECLARE @changeDate datetimeoffset(7)
		DECLARE @userId nvarchar(100)	

		SET @changeDate = SYSDATETIMEOFFSET()

		SELECT @entityMasterRecGuid = _MasterRecordGuid, @newEquipmentTypeGuid = EquipmentTypeGuid, @masterSiteGroupGuid = SiteGuid, @userId = UpdatedBy 
		FROM tblEquipment
		WHERE EquipmentGuid = @EntityRecordGuid

		DECLARE @BeginTran BIT = 0 
		
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION
            SET @BeginTran = 1   
		END  

		-- Retrieve the Entity To Site hierarchy below the owner sitegroup of the entity record whose EquipmentType was changed
		-- This corresponds to all the child record versions who derives, directly or indirectly, from the given record version.
		DECLARE @tblEntityToSiteHierarchy TABLE
		(
			SiteGuid uniqueidentifier
			, SiteId nvarchar(30)
			, HierarchyLevel int
			, SiteGroupFlag bit
			, AssignedFromSiteGuid uniqueidentifier
			, Processed bit
		);

		INSERT INTO @tblEntityToSiteHierarchy
		(SiteGuid, SiteId, HierarchyLevel, SiteGroupFlag, AssignedFromSiteGuid, Processed)
		SELECT SiteGuid, SiteId, HierarchyLevel, SiteGroupFlag, AssignedFromSiteGuid, 0
		FROM [erv].[udf_GetEquipmentToSiteHierarchyByRecordVersionGuid](@entityMasterRecGuid)
		WHERE HierarchyLevel > 0
		ORDER BY HierarchyLevel, SiteGuid

		-- Delete all child record versions that are owned by a site/sitegroup to which the EquipmentType that was newly assigned to the master equipment record has not been assigned.
		DELETE a FROM tblEquipment a
		INNER JOIN map.tblEntityEquipmentToSite b
		ON b.EquipmentGuid = a._MasterRecordGuid
		AND b.SiteGuid = a.SiteGuid
		WHERE a._MasterRecordGuid = @entityMasterRecGuid
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityEquipmentTypeToSite c
			WHERE  c.SiteGuid = a.SiteGuid
			AND c.EquipmentTypeGuid = @newEquipmentTypeGuid
		)
		AND a.EquipmentGuid <> a._MasterRecordGuid

		
		WHILE ((SELECT COUNT(*) FROM @tblEntityToSiteHierarchy WHERE Processed = 0) > 0)
		BEGIN
			SELECT TOP 1 @runningSiteGuid = SiteGuid, @runningAssignedFromSiteGuid = AssignedFromSiteGuid 
			FROM @tblEntityToSiteHierarchy 
			WHERE Processed = 0 
			ORDER BY HierarchyLevel, SiteGuid
						
			EXEC [erv].[usp_IsRecordVersioningOnForEntity] 'Equipment', @entityMasterRecGuid, @runningAssignedFromSiteGuid, @IsRecVerOn OUTPUT

			IF ((@IsRecVerOn IS NOT NULL) AND (@IsRecVerOn = 1))
			BEGIN				
				SELECT @parentEntityGuid = EquipmentGuid FROM tblEquipment WHERE _MasterRecordGuid = @entityMasterRecGuid AND SiteGuid = @runningAssignedFromSiteGuid
				IF (@parentEntityGuid IS NULL)
				BEGIN
					RAISERROR('Cannot locate the parent record version for the assignment.',16,1); 
					RETURN;
				END
				IF EXISTS (SELECT * FROM tblEquipment WHERE _MasterRecordGuid = @entityMasterRecGuid AND SiteGuid = @runningSiteGuid)
				BEGIN
					UPDATE tblEquipment 
					SET EquipmentTypeGuid = @newEquipmentTypeGuid, UpdatedBy = @userId, UpdatedDate = @changeDate
					WHERE _MasterRecordGuid = @entityMasterRecGuid AND SiteGuid = @runningSiteGuid
				END
				ELSE
				BEGIN
					EXEC [erv].[usp_CreateEquipmentChildRecordVersion] @parentEntityGuid, @runningSiteGuid, @changeDate, @userId
				END
			END
			ELSE
			BEGIN
				DELETE tblEquipment  WHERE _MasterRecordGuid = @entityMasterRecGuid AND SiteGuid = @runningSiteGuid
			END
			UPDATE @tblEntityToSiteHierarchy SET Processed = 1 WHERE SiteGuid = @runningSiteGuid
		END			
		
		
		--Re-propagate all the ParentSpecific fields from each Parent record version of the entity record whose EquipmentType was changed.
		--If a field was VersionSpecific on the old EquipmentType (as its value was actually edited in a child record version), but ParentSpecific on the new EquipmentType,
		--then if the record data propagation is not re-run, that field will be out of synchronous with the parent record after the EquipmentType change.
		
		-- Capture the parent sitegroups below the sitegroup where the EquipmentType configuration change was applied
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
		FROM [erv].[udf_GetSiteHierarchy](@masterSiteGroupGuid, 0)
		ORDER BY HierarchyLevel, SiteGuid		
		
		WHILE ((SELECT COUNT(*) FROM @tblSiteHierarchy WHERE Processed = 0) > 0)
		BEGIN
			SELECT TOP 1 @runningSiteGuid = SiteGuid FROM @tblSiteHierarchy WHERE Processed = 0 ORDER BY HierarchyLevel, SiteGuid
						
			SELECT @parentEntityGuid = EquipmentGuid FROM tblEquipment WHERE _MasterRecordGuid = @entityMasterRecGuid AND SiteGuid = @runningSiteGuid
			IF (@parentEntityGuid IS NULL)
			BEGIN
				RAISERROR('Cannot locate the parent record version for the assignment.',16,1); 
				RETURN;
			END
			EXEC [erv].[usp_PropagateEquipmentRevisionByEntityRecordChange] @parentEntityGuid

			UPDATE @tblSiteHierarchy SET Processed = 1 WHERE SiteGuid = @runningSiteGuid
		END							
		
		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		COMMIT TRANSACTION		
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
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [erv].usp_ProcessEquipmentTypeChange' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END