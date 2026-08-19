/*
	DROP PROCEDURE [erv].[usp_PropagateEquipmentRecordVersionBySegment]

	EXEC [erv].[usp_PropagateEquipmentRecordVersionBySegment] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_PropagateEquipmentRecordVersionBySegment] '1eacc1d7-292d-4932-bc59-9c02740c6c19'

*/

CREATE PROCEDURE [erv].[usp_PropagateEquipmentRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @FilterValueGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagateEquipmentRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate all the Parent Specific fields of all the record versions in a segment from a given sitegroup down to all the sites/sitegroups that have a direct assignment from the given sitegroup.
	-- This Stored Procedure is to be used to enforce the effect of fields being changed from VersionSpecific to ParentSpecific as a result of Field Level Control configuration changes.
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Entity Segment Template that needs to be processed.
	-- 2. @FilterValueGuid: Filter value guid value that helps define the specific equipment segment that needs to be processed.
	-- 3. @SourceSiteGroupGuid: Guid of the segment SiteGroup from which the ParentSpecific fields are to be propagated.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		--Retrieve the VersionSpecific fields for the entity record
		DECLARE @tblSourceVersionSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL,
			Processed bit
		)
		INSERT @tblSourceVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		EXEC erv.usp_GetVersionSpecificFieldsBySegment @EntitySegmentTemplateGuid, @FilterValueGuid, @SourceSiteGroupGuid

		IF (NOT EXISTS (SELECT * FROM @tblSourceVersionSpecificFields))
		BEGIN				
			/*
				All fields are ParentSpecific. This means that there will be no child record versions of the entity record for any site/sitegroup in the hierarchy below owner 
				sitegroup of the entity record, i.e. Record Versioning field data propagation does not apply.
			*/
			RETURN;
		END

		DECLARE @entityTypeId nvarchar(100)
		DECLARE @filterFieldName nvarchar(100)
		SELECT @entityTypeId = EntityTypeId, @filterFieldName = FilterFieldName FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		DECLARE @callingRefGuid uniqueidentifier
		SET @callingRefGuid = NEWID()

		--Capture the Site/SiteGroup, MasterRecordGuid, and EquipmentGuid of the child record versions that need to be updated.
		--This includes all the child record versions down the site hierarchy that have the same masterrecordguid as those owned by the SourceSiteGroup and which share the same filter value as the segment being processed, irrespective of where they were assigned from.
		IF (@entityTypeId = 'Equipment' AND @filterFieldName = 'EquipmentTypeGuid')
		BEGIN
			INSERT INTO erv.tblTempTargetEntitySite
			(SiteGuid, MasterRecordGuid, EntityGuid, ParentEntityGuid, _CallingReferenceGuid)
			SELECT a.SiteGuid, a._MasterRecordGuid, a.EquipmentGuid, d.EquipmentGuid, @callingRefGuid
			FROM [dbo].[tblEquipment] a
			INNER JOIN map.tblEntityEquipmentToSite b
			ON b.EquipmentGuid = a._MasterRecordGuid
			AND b.SiteGuid = a.SiteGuid
			INNER JOIN tblEquipment d
			ON d._MasterRecordGuid = b.EquipmentGuid
			AND d.SiteGuid = b.AssignedFromSiteGuid  --Note: we do not use erv.udf_GetFirstParentRecordVersionGuid in that instance, but instead we get the parent record version directly from the AssignedFrom sitegroup. This simplification is made possible by the fact that this process is only concerned about updating child record versions. If the parent record version is not available from the direct AssignedFrom sitegroup, then it means that Record Versioning has been turned off at the parent sitegroup, and if that is the case it would be turned off at the current sitegroup/site as well, and therefore their would be no child record version to update in any case.
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND ((a.EquipmentTypeGuid = @FilterValueGuid) OR ((@FilterValueGuid IS NULL) AND (a.EquipmentTypeGuid IS NULL)))
			AND a.EquipmentGuid <> a._MasterRecordGuid
		END											
		
		IF (NOT EXISTS (SELECT * FROM erv.tblTempTargetEntitySite WHERE _CallingReferenceGuid = @callingRefGuid))
		BEGIN							
			RETURN;
		END

		--Build a table that has one flag column for each column of the tblEquipment table, and set the flag according to whether the field is VersionSpecific or not.
		INSERT INTO erv.tblTempEquipmentRecordVersioningFlag
		(EquipmentGuid, _CallingReferenceGuid)
		SELECT DISTINCT MasterRecordGuid, @callingRefGuid FROM erv.tblTempTargetEntitySite WHERE _CallingReferenceGuid = @callingRefGuid

		EXEC [erv].[usp_PivotFLCConfigurationsForSegment] @EntitySegmentTemplateGuid, @FilterValueGuid, @SourceSiteGroupGuid, NULL, @callingRefGuid
		
		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --PropagateToChildRecordVersions
            SET @BeginTran = 1   
		END  		

		-- Update all the internal ParentSpecific fields for all applicable sites and sitegroups
		UPDATE a
		SET a.[ActualGPM] = (CASE e.[ActualGPM_RVFlag] WHEN 1 THEN a.[ActualGPM] ELSE b.[ActualGPM] END),
			a.[AssetTrackingDeviceGuid] = (CASE e.[AssetTrackingDeviceGuid_RVFlag] WHEN 1 THEN a.[AssetTrackingDeviceGuid] ELSE b.[AssetTrackingDeviceGuid] END),
			a.[AssignedToMeterGuid] = (CASE e.[AssignedToMeterGuid_RVFlag] WHEN 1 THEN a.[AssignedToMeterGuid] ELSE b.[AssignedToMeterGuid] END),
			a.[AttachedTo] = (CASE e.[AttachedTo_RVFlag] WHEN 1 THEN a.[AttachedTo] ELSE b.[AttachedTo] END),
			a.[CalibrationDate] = (CASE e.[CalibrationDate_RVFlag] WHEN 1 THEN a.[CalibrationDate] ELSE b.[CalibrationDate] END),
			a.[Capacity] = (CASE e.[Capacity_RVFlag] WHEN 1 THEN a.[Capacity] ELSE b.[Capacity] END),
			a.[CompanyEquipmentID] = (CASE e.[CompanyEquipmentID_RVFlag] WHEN 1 THEN a.[CompanyEquipmentID] ELSE b.[CompanyEquipmentID] END),
			a.[CompanyGuid] = (CASE e.[CompanyGuid_RVFlag] WHEN 1 THEN a.[CompanyGuid] ELSE b.[CompanyGuid] END),
			a.[Consecutive_OOS_Variance] = (CASE e.[Consecutive_OOS_Variance_RVFlag] WHEN 1 THEN a.[Consecutive_OOS_Variance] ELSE b.[Consecutive_OOS_Variance] END),
			a.[DefuelMeterForwards] = (CASE e.[DefuelMeterForwards_RVFlag] WHEN 1 THEN a.[DefuelMeterForwards] ELSE b.[DefuelMeterForwards] END),
			a.[DensityDecimalPlaces] = (CASE e.[DensityDecimalPlaces_RVFlag] WHEN 1 THEN a.[DensityDecimalPlaces] ELSE b.[DensityDecimalPlaces] END),
			a.[DensityUnitIndex] = (CASE e.[DensityUnitIndex_RVFlag] WHEN 1 THEN a.[DensityUnitIndex] ELSE b.[DensityUnitIndex] END),
			a.[Description] = (CASE e.[Description_RVFlag] WHEN 1 THEN a.[Description] ELSE b.[Description] END),
			a.[EquipmentSequence] = (CASE e.[EquipmentSequence_RVFlag] WHEN 1 THEN a.[EquipmentSequence] ELSE b.[EquipmentSequence] END),
			a.[EquipmentTypeGuid] = (CASE e.[EquipmentTypeGuid_RVFlag] WHEN 1 THEN a.[EquipmentTypeGuid] ELSE b.[EquipmentTypeGuid] END),
			a.[Fixed] = (CASE e.[Fixed_RVFlag] WHEN 1 THEN a.[Fixed] ELSE b.[Fixed] END),
			a.[FixedVolume] = (CASE e.[FixedVolume_RVFlag] WHEN 1 THEN a.[FixedVolume] ELSE b.[FixedVolume] END),
			a.[FuelAdditiveFlag] = (CASE e.[FuelAdditiveFlag_RVFlag] WHEN 1 THEN a.[FuelAdditiveFlag] ELSE b.[FuelAdditiveFlag] END),
			a.[FuelCardGuid] = (CASE e.[FuelCardGuid_RVFlag] WHEN 1 THEN a.[FuelCardGuid] ELSE b.[FuelCardGuid] END),
			a.[FuelingState] = (CASE e.[FuelingState_RVFlag] WHEN 1 THEN a.[FuelingState] ELSE b.[FuelingState] END),
			a.[FuelingType] = (CASE e.[FuelingType_RVFlag] WHEN 1 THEN a.[FuelingType] ELSE b.[FuelingType] END),
			a.[HiddenDate] = (CASE e.[HiddenDate_RVFlag] WHEN 1 THEN a.[HiddenDate] ELSE b.[HiddenDate] END),
			a.[ID] = (CASE e.[ID_RVFlag] WHEN 1 THEN a.[ID] ELSE b.[ID] END),
			a.[InspectionDate] = (CASE e.[InspectionDate_RVFlag] WHEN 1 THEN a.[InspectionDate] ELSE b.[InspectionDate] END),
			a.[InstallationDate] = (CASE e.[InstallationDate_RVFlag] WHEN 1 THEN a.[InstallationDate] ELSE b.[InstallationDate] END),
			a.[IntoPlane] = (CASE e.[IntoPlane_RVFlag] WHEN 1 THEN a.[IntoPlane] ELSE b.[IntoPlane] END),
			a.[InUse] = (CASE e.[InUse_RVFlag] WHEN 1 THEN a.[InUse] ELSE b.[InUse] END),
			a.[IssPtNum] = (CASE e.[IssPtNum_RVFlag] WHEN 1 THEN a.[IssPtNum] ELSE b.[IssPtNum] END),
			a.[LockedOut] = (CASE e.[LockedOut_RVFlag] WHEN 1 THEN a.[LockedOut] ELSE b.[LockedOut] END),
			a.[LockedOutDate] = (CASE e.[LockedOutDate_RVFlag] WHEN 1 THEN a.[LockedOutDate] ELSE b.[LockedOutDate] END),
			a.[LockedOutReason] = (CASE e.[LockedOutReason_RVFlag] WHEN 1 THEN a.[LockedOutReason] ELSE b.[LockedOutReason] END),
			a.[LowStockWarning] = (CASE e.[LowStockWarning_RVFlag] WHEN 1 THEN a.[LowStockWarning] ELSE b.[LowStockWarning] END),
			a.[Make] = (CASE e.[Make_RVFlag] WHEN 1 THEN a.[Make] ELSE b.[Make] END),
			a.[ManagedEquipmentFlag] = (CASE e.[ManagedEquipmentFlag_RVFlag] WHEN 1 THEN a.[ManagedEquipmentFlag] ELSE b.[ManagedEquipmentFlag] END),
			a.[ManufactureDate] = (CASE e.[ManufactureDate_RVFlag] WHEN 1 THEN a.[ManufactureDate] ELSE b.[ManufactureDate] END),
			a.[MassDecimalPlaces] = (CASE e.[MassDecimalPlaces_RVFlag] WHEN 1 THEN a.[MassDecimalPlaces] ELSE b.[MassDecimalPlaces] END),
			a.[MassUnitIndex] = (CASE e.[MassUnitIndex_RVFlag] WHEN 1 THEN a.[MassUnitIndex] ELSE b.[MassUnitIndex] END),
			a.[MediaType] = (CASE e.[MediaType_RVFlag] WHEN 1 THEN a.[MediaType] ELSE b.[MediaType] END),
			a.[MeterReading] = (CASE e.[MeterReading_RVFlag] WHEN 1 THEN a.[MeterReading] ELSE b.[MeterReading] END),
			a.[Meters] = (CASE e.[Meters_RVFlag] WHEN 1 THEN a.[Meters] ELSE b.[Meters] END),
			a.[Mobile] = (CASE e.[Mobile_RVFlag] WHEN 1 THEN a.[Mobile] ELSE b.[Mobile] END),
			a.[Model] = (CASE e.[Model_RVFlag] WHEN 1 THEN a.[Model] ELSE b.[Model] END),
			a.[Notes] = (CASE e.[Notes_RVFlag] WHEN 1 THEN a.[Notes] ELSE b.[Notes] END),
			a.[ProductGuid] = (CASE e.[ProductGuid_RVFlag] WHEN 1 THEN a.[ProductGuid] ELSE b.[ProductGuid] END),
			a.[PulseRatio] = (CASE e.[PulseRatio_RVFlag] WHEN 1 THEN a.[PulseRatio] ELSE b.[PulseRatio] END),
			a.[QCDate] = (CASE e.[QCDate_RVFlag] WHEN 1 THEN a.[QCDate] ELSE b.[QCDate] END),
			a.[RatedGPM] = (CASE e.[RatedGPM_RVFlag] WHEN 1 THEN a.[RatedGPM] ELSE b.[RatedGPM] END),
			a.[Round] = (CASE e.[Round_RVFlag] WHEN 1 THEN a.[Round] ELSE b.[Round] END),
			a.[SafeFill] = (CASE e.[SafeFill_RVFlag] WHEN 1 THEN a.[SafeFill] ELSE b.[SafeFill] END),
			a.[ScullyRequired] = (CASE e.[ScullyRequired_RVFlag] WHEN 1 THEN a.[ScullyRequired] ELSE b.[ScullyRequired] END),
			a.[SecondaryStorageFlag] = (CASE e.[SecondaryStorageFlag_RVFlag] WHEN 1 THEN a.[SecondaryStorageFlag] ELSE b.[SecondaryStorageFlag] END),
			a.[SerialNumber] = (CASE e.[SerialNumber_RVFlag] WHEN 1 THEN a.[SerialNumber] ELSE b.[SerialNumber] END),
			a.[StockTrack] = (CASE e.[StockTrack_RVFlag] WHEN 1 THEN a.[StockTrack] ELSE b.[StockTrack] END),
			a.[StorageType] = (CASE e.[StorageType_RVFlag] WHEN 1 THEN a.[StorageType] ELSE b.[StorageType] END),
			a.[TemperatureDecimalPlaces] = (CASE e.[TemperatureDecimalPlaces_RVFlag] WHEN 1 THEN a.[TemperatureDecimalPlaces] ELSE b.[TemperatureDecimalPlaces] END),
			a.[TemperatureUnitIndex] = (CASE e.[TemperatureUnitIndex_RVFlag] WHEN 1 THEN a.[TemperatureUnitIndex] ELSE b.[TemperatureUnitIndex] END),
			a.[Totalisor1] = (CASE e.[Totalisor1_RVFlag] WHEN 1 THEN a.[Totalisor1] ELSE b.[Totalisor1] END),
			a.[Totalisor2] = (CASE e.[Totalisor2_RVFlag] WHEN 1 THEN a.[Totalisor2] ELSE b.[Totalisor2] END),
			a.[TruckCardNumber] = (CASE e.[TruckCardNumber_RVFlag] WHEN 1 THEN a.[TruckCardNumber] ELSE b.[TruckCardNumber] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UserData1] = (CASE e.[UserData1_RVFlag] WHEN 1 THEN a.[UserData1] ELSE b.[UserData1] END),
			a.[UserData10] = (CASE e.[UserData10_RVFlag] WHEN 1 THEN a.[UserData10] ELSE b.[UserData10] END),
			a.[UserData11] = (CASE e.[UserData11_RVFlag] WHEN 1 THEN a.[UserData11] ELSE b.[UserData11] END),
			a.[UserData12] = (CASE e.[UserData12_RVFlag] WHEN 1 THEN a.[UserData12] ELSE b.[UserData12] END),
			a.[UserData13] = (CASE e.[UserData13_RVFlag] WHEN 1 THEN a.[UserData13] ELSE b.[UserData13] END),
			a.[UserData14] = (CASE e.[UserData14_RVFlag] WHEN 1 THEN a.[UserData14] ELSE b.[UserData14] END),
			a.[UserData15] = (CASE e.[UserData15_RVFlag] WHEN 1 THEN a.[UserData15] ELSE b.[UserData15] END),
			a.[UserData16] = (CASE e.[UserData16_RVFlag] WHEN 1 THEN a.[UserData16] ELSE b.[UserData16] END),
			a.[UserData17] = (CASE e.[UserData17_RVFlag] WHEN 1 THEN a.[UserData17] ELSE b.[UserData17] END),
			a.[UserData18] = (CASE e.[UserData18_RVFlag] WHEN 1 THEN a.[UserData18] ELSE b.[UserData18] END),
			a.[UserData19] = (CASE e.[UserData19_RVFlag] WHEN 1 THEN a.[UserData19] ELSE b.[UserData19] END),
			a.[UserData2] = (CASE e.[UserData2_RVFlag] WHEN 1 THEN a.[UserData2] ELSE b.[UserData2] END),
			a.[UserData20] = (CASE e.[UserData20_RVFlag] WHEN 1 THEN a.[UserData20] ELSE b.[UserData20] END),
			a.[UserData21] = (CASE e.[UserData21_RVFlag] WHEN 1 THEN a.[UserData21] ELSE b.[UserData21] END),
			a.[UserData22] = (CASE e.[UserData22_RVFlag] WHEN 1 THEN a.[UserData22] ELSE b.[UserData22] END),
			a.[UserData23] = (CASE e.[UserData23_RVFlag] WHEN 1 THEN a.[UserData23] ELSE b.[UserData23] END),
			a.[UserData24] = (CASE e.[UserData24_RVFlag] WHEN 1 THEN a.[UserData24] ELSE b.[UserData24] END),
			a.[UserData3] = (CASE e.[UserData3_RVFlag] WHEN 1 THEN a.[UserData3] ELSE b.[UserData3] END),
			a.[UserData4] = (CASE e.[UserData4_RVFlag] WHEN 1 THEN a.[UserData4] ELSE b.[UserData4] END),
			a.[UserData5] = (CASE e.[UserData5_RVFlag] WHEN 1 THEN a.[UserData5] ELSE b.[UserData5] END),
			a.[UserData6] = (CASE e.[UserData6_RVFlag] WHEN 1 THEN a.[UserData6] ELSE b.[UserData6] END),
			a.[UserData7] = (CASE e.[UserData7_RVFlag] WHEN 1 THEN a.[UserData7] ELSE b.[UserData7] END),
			a.[UserData8] = (CASE e.[UserData8_RVFlag] WHEN 1 THEN a.[UserData8] ELSE b.[UserData8] END),
			a.[UserData9] = (CASE e.[UserData9_RVFlag] WHEN 1 THEN a.[UserData9] ELSE b.[UserData9] END),
			a.[Volume] = (CASE e.[Volume_RVFlag] WHEN 1 THEN a.[Volume] ELSE b.[Volume] END),
			a.[VolumeDecimalPlaces] = (CASE e.[VolumeDecimalPlaces_RVFlag] WHEN 1 THEN a.[VolumeDecimalPlaces] ELSE b.[VolumeDecimalPlaces] END),
			a.[VolumeUnitIndex] = (CASE e.[VolumeUnitIndex_RVFlag] WHEN 1 THEN a.[VolumeUnitIndex] ELSE b.[VolumeUnitIndex] END),
			a.[Xref] = (CASE e.[Xref_RVFlag] WHEN 1 THEN a.[Xref] ELSE b.[Xref] END),
			a.[Year] = (CASE e.[Year_RVFlag] WHEN 1 THEN a.[Year] ELSE b.[Year] END)			
		FROM tblEquipment a
		INNER JOIN tblEquipment b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN erv.tblTempTargetEntitySite c
		ON c.EntityGuid = a.EquipmentGuid
		INNER JOIN erv.tblTempTargetEntitySite d
		ON d.ParentEntityGuid = b.EquipmentGuid
		INNER JOIN erv.tblTempEquipmentRecordVersioningFlag e
		ON e.EquipmentGuid = a._MasterRecordGuid
		WHERE e._CallingReferenceGuid = @callingRefGuid
		AND c._CallingReferenceGuid = @callingRefGuid
		AND d._CallingReferenceGuid = @callingRefGuid

		DELETE erv.tblTempEquipmentRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRefGuid 

		-- Process those ParentSpecific External fields whose propagation require custom handling.
		DECLARE @tblParentSpecificExternalFields TABLE
		(
			TargetField nvarchar(100)
		)

		/*Process those ParentSpecific External fields whose propagation require custom handling. */
		-- Process TagsAndLicences External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Tags and Licences') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.EquipmentGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] d				
				WHERE d.EquipmentGuid = b.ParentEntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.Instructor = d.Instructor,
			a.DateCompleted = d.DateCompleted,
			a.DateDue = d.DateDue,
			a.ExpirationDate = d.ExpirationDate,
			a.Id = d.Id,		
			a.Rating = d.Rating,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.EquipmentGuid
			INNER JOIN [map].[tblQualificationEquipmentTagAndLicenseToEquipment] d
			ON d.EquipmentGuid = b.ParentEntityGuid
			AND d.QualificationGuid = a.QualificationGuid
			AND d.HistoricalRecord = a.HistoricalRecord	
			WHERE b._CallingReferenceGuid = @callingRefGuid
			AND d.HistoricalRecord = 0

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblQualificationEquipmentTagAndLicenseToEquipment]
			(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.QualificationGuid, b.EntityGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.ID, a.Rating, a.HistoricalRecord, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.EquipmentGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] d
				WHERE d.EquipmentGuid = b.EntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process TestsAndInspections External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Tests and Inspections') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.EquipmentGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] d				
				WHERE d.EquipmentGuid = b.ParentEntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.Instructor = d.Instructor,
			a.DateCompleted = d.DateCompleted,
			a.DateDue = d.DateDue,
			a.ExpirationDate = d.ExpirationDate,
			a.Id = d.Id,
			a.Rating = d.Rating,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.EquipmentGuid
			INNER JOIN [map].[tblQualificationEquipmentTestAndInspectionToEquipment] d
			ON d.EquipmentGuid = b.ParentEntityGuid
			AND d.QualificationGuid = a.QualificationGuid
			AND d.HistoricalRecord = a.HistoricalRecord	
			WHERE b._CallingReferenceGuid = @callingRefGuid
			AND d.HistoricalRecord = 0

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblQualificationEquipmentTestAndInspectionToEquipment]
			(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.QualificationGuid, b.EntityGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue,a.ExpirationDate, a.ID, a.Rating, a.HistoricalRecord, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.EquipmentGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment]d
				WHERE d.EquipmentGuid = b.EntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END	

		DELETE erv.tblTempTargetEntitySite
		WHERE _CallingReferenceGuid = @callingRefGuid

		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		BEGIN
			COMMIT TRANSACTION --PropagateToChildRecordVersions
		END
	END TRY
	BEGIN CATCH        
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
			ROLLBACK TRANSACTION --PropagateToChildRecordVersions
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
						+ 'Procedure Name: [erv].usp_PropagateEquipmentRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
GO
