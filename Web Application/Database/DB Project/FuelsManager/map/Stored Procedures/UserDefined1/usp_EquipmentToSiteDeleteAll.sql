


/*

	EXEC [map].[usp_EquipmentToSiteDeleteAll] '1bb8c558-5277-47a5-90ae-2461bbd1eff7'

*/

CREATE PROCEDURE [map].[usp_EquipmentToSiteDeleteAll]
(
	@AssignedFromSiteGroupGuid uniqueidentifier, @AssignedToSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_EquipmentToSiteDeleteAll] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Cascade deletes all the equipment-to-site mappings and the associated record versions for all the equipment-to-site mappings between two sites.
	-- Notes:
	-- 1. @AssignedFromSiteGroupGuid: Guid of the AssignedFrom sitegroup for which the equipment-to-site assignments are to be deleted.
	-- 1. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup for which the equipment-to-site assignments are to be deleted.
	
	
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @runningMappingLevel int

		IF (@AssignedFromSiteGroupGuid = @AssignedToSiteGuid)  -- this is not to be used to delete base mappings
		BEGIN
			RETURN
		END


		DECLARE @tblEntityToSiteMappings TABLE
		(
			MasterRecGuid uniqueidentifier
			, AssignedFromSiteGuid uniqueidentifier
			, AssignedToSiteGuid uniqueidentifier
			, MappingLevel int
			, Processed bit
		);
		

		DECLARE @BeginTran BIT = 0 
		
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION
            SET @BeginTran = 1   
		END  
		

		--Retrieve all the direct entity-to-site assignment mappings from the Target sitegroup to any of the site/groups to which the target sitegroup is no longer a parent
		INSERT INTO @tblEntityToSiteMappings
		(MasterRecGuid, AssignedFromSiteGuid, AssignedToSiteGuid, MappingLevel, Processed)
		SELECT a.EquipmentGuid, a.AssignedFromSiteGuid, a.SiteGuid, 0, 0 FROM map.tblEntityEquipmentToSite a
		WHERE a.AssignedFromSiteGuid = @AssignedFromSiteGroupGuid
		AND a.SiteGuid = @AssignedToSiteGuid

		--Also extract all the subsequent entity-to-site mappings that derive from the direct mappings above
		SET @runningMappingLevel = 0
		WHILE ((SELECT COUNT(*) FROM @tblEntityToSiteMappings WHERE MappingLevel = @runningMappingLevel) > 0)
		BEGIN
			SET @runningMappingLevel = @runningMappingLevel + 1
			INSERT INTO @tblEntityToSiteMappings
			(MasterRecGuid, AssignedFromSiteGuid, AssignedToSiteGuid, MappingLevel, Processed)
			SELECT a.EquipmentGuid, a.AssignedFromSiteGuid, a.SiteGuid, @runningMappingLevel, 0 FROM map.tblEntityEquipmentToSite a
			INNER JOIN @tblEntityToSiteMappings b
			ON b.MasterRecGuid = a.EquipmentGuid
			WHERE b.MappingLevel = 0
			AND a.AssignedFromSiteGuid IN 
			(
				SELECT AssignedToSiteGuid FROM @tblEntityToSiteMappings WHERE MappingLevel = @runningMappingLevel-1
			)									
		END

		--For each affected entity-to-site mapping, delete the corresponding child record version
		--Delete the Tag and License External attributes of the child record versions
		DELETE a FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
		INNER JOIN dbo.tblEquipment b
		ON b.EquipmentGuid = a.EquipmentGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.EquipmentGuid <> b._MasterRecordGuid
		--Delete the Test and Inspection External attributes of the child record versions
		DELETE a FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
		INNER JOIN dbo.tblEquipment b
		ON b.EquipmentGuid = a.EquipmentGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.EquipmentGuid <> b._MasterRecordGuid			
		--Delete the child record versions
		DELETE a
		FROM tblEquipment a
		INNER JOIN @tblEntityToSiteMappings b
		ON b.MasterRecGuid = a._MasterRecordGuid
		AND a.SiteGuid = b.AssignedToSiteGuid
		WHERE a.EquipmentGuid <> a._MasterRecordGuid

		--Delete the entity-to-site mappings affected by the site-to-site mapping deletion
		DELETE a 
		FROM map.tblEntityEquipmentToSite a
		INNER JOIN @tblEntityToSiteMappings b
		ON b.MasterRecGuid = a.EquipmentGuid
		AND a.SiteGuid = b.AssignedToSiteGuid
		WHERE a.AssignedFromSiteGuid <> a.SiteGuid


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
						+ 'Procedure Name: [map].[usp_EquipmentToSiteDeleteAll]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
