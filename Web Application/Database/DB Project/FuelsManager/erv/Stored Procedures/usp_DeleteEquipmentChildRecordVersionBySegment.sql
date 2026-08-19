

/*

	EXEC [erv].[usp_DeleteEquipmentChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '0F7228B9-D8E4-41C8-A862-B71FB3F38763'
	EXEC [erv].[usp_DeleteEquipmentChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '3D95FDFA-3D72-4E4B-9264-B8E068ECD364'
*/

CREATE PROCEDURE [erv].[usp_DeleteEquipmentChildRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @FilterValueGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @IncludeChildRecordVersionsAssignedToSourceSiteGroup bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_DeleteEquipmentChildRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Deletes all the Equipment child record versions for all the entity assignments of a given Equipment segment from a given SiteGroup 
	-- (if @IncludeChildRecordVersionsAssignedToSourceSiteGroup = 0) or both from and to the given sitegroup (if @IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Segment Template that needs to be processed.
	-- 2. @FilterValueGuid: Filter value guid value that helps define the specific equipment segment that needs to be processed.
	-- 3. @SourceSiteGroupGuid: SiteGroup parent from which the child record versions to be deleted were created. This would correspond to the AssignedFrom Sitegroup.
	-- 4. @IncludeChildRecordVersionsAssignedToSourceSiteGroup: 
	--			0 (Default Mode). Only delete the child record versions assigned from the sitegroup.
	--			1: Delete both the child record versions assigned from and to the sitegroup.
	-- 5. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @callingRefGuid uniqueidentifier
		SET @callingRefGuid = NEWID()

		DECLARE @entityTypeId nvarchar(100)
		DECLARE @filterFieldName nvarchar(100)
		SELECT @entityTypeId = EntityTypeId, @filterFieldName = FilterFieldName FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		IF (@entityTypeId = 'Equipment' AND @filterFieldName = 'EquipmentTypeGuid')
		BEGIN
			--Capture the Site/SiteGroup, MasterRecordGuid, and the Entity Guid of the child record versions to be deleted.
			INSERT INTO [erv].[tblTempEntityRecordVersion]
			(SiteGuid, MasterRecordGuid, EntityGuid, _CallingReferenceGuid)
			SELECT b.SiteGuid, b.EquipmentGuid, a.EquipmentGuid, @callingRefGuid
			FROM tblEquipment a
			INNER JOIN map.tblEntityEquipmentToSite b
			ON b.EquipmentGuid = a._MasterRecordGuid
			AND b.SiteGuid = a.SiteGuid  
			WHERE a.EquipmentTypeGuid = @FilterValueGuid
			AND b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND b.SiteGuid <> b.AssignedFromSiteGuid
			AND a.EquipmentGuid <> a._MasterRecordGuid

			IF (@IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
			BEGIN
				INSERT INTO [erv].[tblTempEntityRecordVersion]
				(SiteGuid, MasterRecordGuid, EntityGuid, _CallingReferenceGuid)
				SELECT b.SiteGuid, b.EquipmentGuid, a.EquipmentGuid, @callingRefGuid
				FROM tblEquipment a
				INNER JOIN map.tblEntityEquipmentToSite b
				ON b.EquipmentGuid = a._MasterRecordGuid
				AND b.SiteGuid = a.SiteGuid  
				WHERE a.EquipmentTypeGuid = @FilterValueGuid
				AND b.SiteGuid = @SourceSiteGroupGuid
				AND b.SiteGuid <> b.AssignedFromSiteGuid
				AND a.EquipmentGuid <> a._MasterRecordGuid
			END
		END


		--Delete the external attributes of the parent record version
		--Tags and Licenses
		DELETE a FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.EquipmentGuid
		INNER JOIN tblEquipment c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.EquipmentGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Tests and Inspections
		DELETE a FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.EquipmentGuid
		INNER JOIN tblEquipment c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.EquipmentGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Delete the child record versions
		DELETE a FROM tblEquipment a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.MasterRecordGuid = a._MasterRecordGuid
		AND b.EntityGuid = a.EquipmentGuid
		WHERE b._CallingReferenceGuid = @callingRefGuid
		AND a.EquipmentGuid <> a._MasterRecordGuid

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
						+ 'Procedure Name: [erv].usp_DeleteEquipmentChildRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END