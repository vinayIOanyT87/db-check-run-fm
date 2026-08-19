/*
	DROP PROCEDURE [erv].[usp_DeletePersonnelChildRecordVersionBySegment]
	
	EXEC [erv].[usp_DeletePersonnelChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', '00000000-0000-0000-0000-000000000001'
	EXEC [erv].[usp_DeletePersonnelChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'

*/

CREATE PROCEDURE [erv].[usp_DeletePersonnelChildRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @IncludeChildRecordVersionsAssignedToSourceSiteGroup bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_DeletePersonnelChildRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Deletes all the Personnel child record versions for all the entity assignments of a given Personnel segment (note: there is only one Segment Template for Personnel) 
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

		IF (@entityTypeId = 'Personnel')
		BEGIN
			--Capture the Site/SiteGroup, MasterRecordGuid, and the Entity Guid of the child record versions to be deleted.
			INSERT INTO [erv].[tblTempEntityRecordVersion]
			(SiteGuid, MasterRecordGuid, EntityGuid, _CallingReferenceGuid)
			SELECT b.SiteGuid, b.PersonnelGuid, a.PersonnelGuid, @callingRefGuid
			FROM tblPersonnel a
			INNER JOIN map.tblEntityPersonnelToSite b
			ON b.PersonnelGuid = a._MasterRecordGuid
			AND b.SiteGuid = a.SiteGuid  
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND b.SiteGuid <> b.AssignedFromSiteGuid
			AND a.PersonnelGuid <> a._MasterRecordGuid

			IF (@IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
			BEGIN
				INSERT INTO [erv].[tblTempEntityRecordVersion]
				(SiteGuid, MasterRecordGuid, EntityGuid, _CallingReferenceGuid)
				SELECT b.SiteGuid, b.PersonnelGuid, a.PersonnelGuid, @callingRefGuid
				FROM tblPersonnel a
				INNER JOIN map.tblEntityPersonnelToSite b
				ON b.PersonnelGuid = a._MasterRecordGuid
				AND b.SiteGuid = a.SiteGuid  
				WHERE b.SiteGuid = @SourceSiteGroupGuid
				AND b.SiteGuid <> b.AssignedFromSiteGuid
				AND a.PersonnelGuid <> a._MasterRecordGuid
			END
		END

		--Delete the external attributes of the parent record version
		
		--Delete assigned roles
		DELETE a FROM [map].[tblPersonnelToRole] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Delete assinged qualifications
		DELETE a FROM [map].[tblQualificationPersonQualificationToPerson] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Delete assinged licenses
		DELETE a FROM [map].[tblQualificationPersonLicenseToPerson] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Delete assinged training
		DELETE a FROM [map].[tblQualificationPersonTrainingToPerson] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Delete access schedule
		DELETE a FROM [dbo].[tblSchedulePersonnelAccess] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.PersonnelGuid
		INNER JOIN tblPersonnel c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.PersonnelGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Reset the Shared Mappings between Personnel and Company. This will delete, update, and clone the applicable Personnel-to-Company mappings as necessary.
		--The following Target fields and mapping tables are covered by this process: 
		--(Carrier)->[map].[tblCompanyPersonnelAssignedToCompany]
		DECLARE @tblTargetPersonnel erv.utt_EntityRecordVersions
		INSERT INTO @tblTargetPersonnel
		(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
		SELECT 'Personnel', EntityGuid, MasterRecordGuid, SiteGuid FROM [erv].[tblTempEntityRecordVersion] WHERE _CallingReferenceGuid = @callingRefGuid

		--Reset the Shared Mappings between Personnel and Company. This will delete, update, and clone the applicable Personnel-to-Company mappings as necessary.
		EXEC [erv].[usp_SetPersonnelToCompanyMappingsForDeletedPersonnel] @tblTargetPersonnel

		--Delete the child record versions
		DELETE a FROM tblPersonnel a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.MasterRecordGuid = a._MasterRecordGuid
		AND b.EntityGuid = a.PersonnelGuid
		WHERE b._CallingReferenceGuid = @callingRefGuid
		AND a.PersonnelGuid <> a._MasterRecordGuid

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
						+ 'Procedure Name: [erv].usp_DeletePersonnelChildRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
