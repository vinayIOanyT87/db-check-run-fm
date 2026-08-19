/*
	DROP PROCEDURE [erv].[usp_CreateEquipmentChildRecordVersionBySegment]

	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	--EXEC [erv].[usp_CreateEquipmentChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '0F7228B9-D8E4-41C8-A862-B71FB3F38763', @dt, 'HB'
	EXEC [erv].[usp_CreateEquipmentChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '3D95FDFA-3D72-4E4B-9264-B8E068ECD364', @dt, 'HB'

	SELECT EquipmentGuid, Id, _MasterRecordGuid, SiteGuid, * FROM tblEquipment WHERE _MasterRecordGuid = 'F5EA57B8-2CFB-4605-9B55-8850199671C7'	
*/

CREATE PROCEDURE [erv].[usp_CreateEquipmentChildRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @FilterValueGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_CreateEquipmentChildRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new Equipment record version for each of the existing entity assignments of a given Equipment segment from a given SiteGroup.
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Segment Template that needs to be processed.
	-- 1. @FilterValueGuid: Filter value guid value that helps define the specific equipment segment that needs to be processed.
	-- 3. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	-- 4. @SourceSiteGroupGuid: SiteGroup parent from which the record version are to be created. This would correspond to the AssignedFrom Sitegroup.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		--Verify if RecordVersioning is turned ON for the source sitegroup before creating new child record versions from it.	
		DECLARE @tblVersionSpecificFields TABLE
		(
			TargetField nvarchar(100) NOT NULL,
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL
		);
		INSERT @tblVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		EXEC erv.usp_GetVersionSpecificFieldsBySegment @EntitySegmentTemplateGuid, @FilterValueGuid, @SourceSiteGroupGuid

		IF ((SELECT COUNT(*) FROM @tblVersionSpecificFields) = 0)
		BEGIN
			RETURN
		END


		--Capture the Site/SiteGroup, MasterRecordGuid, and the parent record versions for the entity assignments from which new record versions need to be created/cloned.
		DECLARE @tblTargetEntitySite TABLE
		(
			SiteGuid uniqueidentifier,
			MasterRecordGuid uniqueidentifier,
			ParentEntityGuid uniqueidentifier
		)

		DECLARE @entityTypeId nvarchar(100)
		DECLARE @filterFieldName nvarchar(100)
		SELECT @entityTypeId = EntityTypeId, @filterFieldName = FilterFieldName FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		IF (@entityTypeId = 'Equipment' AND @filterFieldName = 'EquipmentTypeGuid')
		BEGIN
			INSERT INTO @tblTargetEntitySite
			(SiteGuid, MasterRecordGuid, ParentEntityGuid)
			SELECT b.SiteGuid, b.EquipmentGuid, a.EquipmentGuid
			FROM tblEquipment a
			INNER JOIN map.tblEntityEquipmentToSite b
			ON b.EquipmentGuid = a._MasterRecordGuid
			AND b.AssignedFromSiteGuid = a.SiteGuid  --Note: we do not use erv.udf_GetFirstParentRecordVersionGuid in that instance, but instead we get the parent record version directly from the AssignedFrom sitegroup. This simplification is made possible by the fact that this process is only concerned about creating child record versions. If the parent record version is not available from the direct AssignedFrom sitegroup, then it means that Record Versioning has been turned off at the parent sitegroup, and if that is the case it would be turned off at the current sitegroup/site as well, and therefore their would be no need to create a child record version in any case.
			WHERE a.EquipmentTypeGuid = @FilterValueGuid
			AND b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND NOT EXISTS
			(SELECT * FROM tblEquipment c
			WHERE c._MasterRecordGuid = a._MasterRecordGuid
			AND c.SiteGuid = b.SiteGuid)
			AND b.SiteGuid <> b.AssignedFromSiteGuid
		END

		--Create the child record versions by cloning the internal fields of the parent record version
		INSERT INTO tblEquipment
		(ID,SiteGuid,Description,Make,Model,Year,IssPtNum,Fixed,StorageType,InUse,FixedVolume,IntoPlane,Mobile,AttachedTo,MediaType,Meters,DefuelMeterForwards,PulseRatio,Round,Xref,LowStockWarning,StockTrack,Totalisor1,Totalisor2,FuelingState,Volume,MeterReading,Consecutive_OOS_Variance,Notes,Capacity,SafeFill,VolumeUnitIndex,TemperatureUnitIndex,DensityUnitIndex,MassUnitIndex,VolumeDecimalPlaces,TemperatureDecimalPlaces,DensityDecimalPlaces,MassDecimalPlaces,EquipmentSequence,LockedOut,LockedOutReason,LockedOutDate,SerialNumber,CompanyEquipmentID,TruckCardNumber,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,RatedGPM,ActualGPM,FuelAdditiveFlag,ManufactureDate,InstallationDate,InspectionDate,CalibrationDate,QCDate,SecondaryStorageFlag,ManagedEquipmentFlag,FuelingType,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,UserData9,UserData10,UserData11,UserData12,UserData13,UserData14,UserData15,UserData16,UserData17,UserData18,UserData19,UserData20,UserData21,UserData22,UserData23,UserData24,CompanyGuid,ParentEquipmentGuid,EquipmentTypeGuid,FuelCardGuid,ProductGuid,AssignedToMeterGuid, _MasterRecordGuid,HiddenDate,AssetTrackingDeviceGuid,ScullyRequired)		
		SELECT a.ID, b.SiteGuid, a.Description, a.Make, a.Model, a.Year, a.IssPtNum, a.Fixed, a.StorageType, a.InUse, a.FixedVolume, a.IntoPlane, a.Mobile, a.AttachedTo, a.MediaType, a.Meters, a.DefuelMeterForwards, a.PulseRatio, a.Round, a.Xref, a.LowStockWarning, a.StockTrack, a.Totalisor1, a.Totalisor2, a.FuelingState, a.Volume, a.MeterReading, a.Consecutive_OOS_Variance, a.Notes, a.Capacity, a.SafeFill, a.VolumeUnitIndex, a.TemperatureUnitIndex, a.DensityUnitIndex, a.MassUnitIndex, a.VolumeDecimalPlaces, a.TemperatureDecimalPlaces, a.DensityDecimalPlaces, a.MassDecimalPlaces, a.EquipmentSequence, a.LockedOut, a.LockedOutReason, a.LockedOutDate, a.SerialNumber, a.CompanyEquipmentID, a.TruckCardNumber, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy, a.RatedGPM, a.ActualGPM, a.FuelAdditiveFlag, a.ManufactureDate, a.InstallationDate, a.InspectionDate, a.CalibrationDate, a.QCDate, a.SecondaryStorageFlag, a.ManagedEquipmentFlag, a.FuelingType, a.UserData1, a.UserData2, a.UserData3, a.UserData4, a.UserData5, a.UserData6, a.UserData7, a.UserData8, a.UserData9, a.UserData10, a.UserData11, a.UserData12, a.UserData13, a.UserData14, a.UserData15, a.UserData16, a.UserData17, a.UserData18, a.UserData19, a.UserData20, a.UserData21, a.UserData22, a.UserData23, a.UserData24, a.CompanyGuid, a.ParentEquipmentGuid, a.EquipmentTypeGuid, a.FuelCardGuid, a.ProductGuid, a.AssignedToMeterGuid, a._MasterRecordGuid, a.HiddenDate, a.AssetTrackingDeviceGuid, a.ScullyRequired
		FROM tblEquipment a
		INNER JOIN @tblTargetEntitySite b
		ON b.MasterRecordGuid = a._MasterRecordGuid
		AND b.ParentEntityGuid = a.EquipmentGuid


		--Clone the external attributes of the parent record version

		--Retrieve the first available Equipment record version applicable for all Equipment mappings to @SourceSiteGroupGuid
		--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it just updates the AssignedFromSiteGuid and the EntityGuid of the initial mapping record to reflect the actual parent record.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempEntityMappingHierarchy
		(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
		SELECT a.EquipmentGuid, b.EquipmentGuid, a.SiteGuid, 0, @callingRef1Guid
		FROM map.tblEntityEquipmentToSite a
		LEFT OUTER JOIN tblEquipment b
		ON b._MasterRecordGuid = a.EquipmentGuid
		AND b.SiteGuid = a.SiteGuid
		WHERE a.SiteGuid = @SourceSiteGroupGuid
		AND b.EquipmentTypeGuid = @FilterValueGuid

		DECLARE @level int
		SET @level = 0

		WHILE ((SELECT COUNT(*) FROM erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef1Guid AND EntityGuid IS NULL) > 0)
		BEGIN
			SET @level = @level - 1
			IF (@level < -20)
			BEGIN
				RAISERROR('Maximum iteration of mapping hierarchy reached.',16,1);   --safeguard against infinite looping
				RETURN;
			END
			UPDATE a 
			SET a.AssignedFromSiteGuid = b.SiteGuid, a.EntityGuid = c.EquipmentGuid
			FROM erv.tblTempEntityMappingHierarchy a
			INNER JOIN map.tblEntityEquipmentToSite b
			ON b.EquipmentGuid = a.EntityMasterGuid
			AND b.SiteGuid = a.AssignedFromSiteGuid
			LEFT OUTER JOIN tblEquipment c
			ON c._MasterRecordGuid = b.EquipmentGuid
			AND c.SiteGuid = b.SiteGuid
			WHERE a._CallingReferenceGuid = @callingRef1Guid
			AND a.EntityGuid IS NULL
		END				

		--Tags and Licenses
		UPDATE a 
		SET a.EquipmentGuid = e.CompanyGuid
		FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.EquipmentGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblQualifications d
		ON d.QualificationGuid = a.QualificationGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblEquipment e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblEquipment f
		ON f.EquipmentGuid = a.EquipmentGuid
		WHERE e._MasterRecordGuid <> e.CompanyGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid	

		INSERT INTO [map].[tblQualificationEquipmentTagAndLicenseToEquipment]
		(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT a.QualificationGuid, c.EquipmentGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.ID, a.Rating, a.HistoricalRecord, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.EquipmentGuid
		INNER JOIN tblEquipment c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.EquipmentGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] e 
			WHERE e.EquipmentGuid = c.EquipmentGuid
			AND e.QualificationGuid = a.QualificationGuid
		)

		--Tests and Equipments
		UPDATE a 
		SET a.EquipmentGuid = e.CompanyGuid
		FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.EquipmentGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblQualifications d
		ON d.QualificationGuid = a.QualificationGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblEquipment e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblEquipment f
		ON f.EquipmentGuid = a.EquipmentGuid
		WHERE e._MasterRecordGuid <> e.CompanyGuid
		AND f.SiteGuid <> c.SiteGuid	
		AND b._CallingReferenceGuid = @callingRef1Guid

		INSERT INTO [map].[tblQualificationEquipmentTestAndInspectionToEquipment]
		(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT a.QualificationGuid, c.EquipmentGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.ID, a.Rating, a.HistoricalRecord, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.EquipmentGuid
		INNER JOIN tblEquipment c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.EquipmentGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] e 
			WHERE e.EquipmentGuid = c.EquipmentGuid
			AND e.QualificationGuid = a.QualificationGuid
		)

		DELETE erv.tblTempEntityMappingHierarchy
		WHERE _CallingReferenceGuid = @callingRef1Guid
		
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
						+ 'Procedure Name: [erv].usp_CreateEquipmentChildRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
