/*
	DROP PROCEDURE [erv].[usp_ReplicateEquipmentGSChangesOnMaster]

	EXEC [erv].[usp_ReplicateEquipmentGSChangesOnMaster] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_ReplicateEquipmentGSChangesOnMaster] 'F94D0DAB-8C85-4A73-830E-A8168078B6AD'
	EXEC [erv].[usp_ReplicateEquipmentGSChangesOnMaster] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
*/

CREATE PROCEDURE [erv].[usp_ReplicateEquipmentGSChangesOnMaster]
(
	@SourceEquipmentGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_ReplicateEquipmentGSChangesOnMaster] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Replicate the Global Specific field values of an Equipment child record version onto the Master Record copy.
	--          By replicating those field values onto the master record, we ensure that when the non-VersionSpecific
	--          fields of the master record are propagated down the site hierarchy, that all the GlobalSpecific changes made onto the
	--          the child record version will get propagated onto all the sitegroups and sites where the master record is assigned.
	-- Notes:
	-- 1. @SourceEquipmentGuid: Guid of the Equipment child record version record whose GlobalSpecific fields needs to be replicated to its local Master Record copy 
	--    (and not the parent record of the entity record).
	-- 2. Whereas RecordVersioning propagation is limited to child record versions, the GlobalSpecific field replication targets the master records and allows
	--    modifications to the master records. This also applies to external attributres that represent a reference to another RecordVersioning entity (e.g. Product).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Equipment'

		DECLARE @masterSiteGuid uniqueidentifier
		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		DECLARE @assignedFromSiteGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid, @ownerSiteGuid = SiteGuid FROM dbo.tblEquipment
		WHERE EquipmentGuid = @SourceEquipmentGuid
		AND EquipmentGuid <> _MasterRecordGuid

		IF (@masterRecordGuid IS NULL)
		BEGIN
			RAISERROR('Cannot locate the source child record for data replication.',16,1); 
			RETURN;
		END

		IF ((SELECT COUNT(*) FROM dbo.tblEquipment WHERE EquipmentGuid = @masterRecordGuid AND _MasterRecordGuid = @masterRecordGuid) = 0)
		BEGIN
			RAISERROR('Cannot locate the target master record for data replication.',16,1); 
			RETURN;
		END

		SELECT @masterSiteGuid = SiteGuid FROM dbo.tblEquipment
		WHERE EquipmentGuid = @masterRecordGuid
		AND EquipmentGuid = _MasterRecordGuid

		SELECT @assignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityEquipmentToSite 
		WHERE EquipmentGuid = @masterRecordGuid 
		AND SiteGuid = @ownerSiteGuid


		DECLARE @tblSegmentInfo TABLE
		(
			FilterValueGuid uniqueidentifier NULL,
			EntitySegmentTemplateGuid uniqueidentifier NOT NULL
		);		
		--Fetch all the entity segments that apply to the record. This query will usually return a single record.
		--The only situation where the query can return more than one record is that there is more than one entity segment (i.e. more than one filter field) are defined for 
		--the entity type of the entity record.
		INSERT INTO @tblSegmentInfo
		(FilterValueGuid, EntitySegmentTemplateGuid)
		SELECT FilterValueGuid, EntitySegmentTemplateGuid
		FROM [erv].[udf_GetEntitySegmentsByEntityGuid] (@EntityTypeId, @SourceEquipmentGuid)
		
		IF NOT EXISTS (SELECT * FROM @tblSegmentInfo)
		BEGIN
			RAISERROR('Cannot locate the segment information for the selected entity record.',16,1); 
			RETURN;
		END
		

		--Retrieve the GlobalSpecific fields for the AssignedFrom sitegroup of the child record version whose changes need to be replicated
		DECLARE @tblSourceGlobalSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL,
			Processed bit
		)
		DECLARE @callingRef2Guid uniqueidentifier
		SET @callingRef2Guid = NEWID()

		EXEC erv.usp_GetRecordVersioningFields @EntityTypeId, @masterRecordGuid, @assignedFromSiteGuid, 'GlobalSpecific', @callingRef2Guid 

		INSERT @tblSourceGlobalSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		SELECT TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode FROM erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid

		IF (NOT EXISTS (SELECT * FROM @tblSourceGlobalSpecificFields))
		BEGIN				
			/*	No GlobalSpecific fields to update.	*/
			RETURN;
		END

		--Build a table that has one flag column for each column of the tblEquipment table, and set the flag according to whether the field is GlobalSpecific or not.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempEquipmentRecordVersioningFlag
		(EquipmentGuid, SiteGuid, _CallingReferenceGuid)
		SELECT a.EquipmentGuid, a.SiteGuid, @callingRef1Guid FROM tblEquipment a
		WHERE a._MasterRecordGuid = @masterRecordGuid
		AND a.EquipmentGuid = a._MasterRecordGuid

		EXEC [erv].[usp_PivotFLCConfigurationsForEntityRecord] @EntityTypeId, @masterRecordGuid, @assignedFromSiteGuid, @callingRef2Guid, @callingRef1Guid

		DELETE erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid
		

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --ReplicateToMasterRecord
            SET @BeginTran = 1   
		END  	
		

		-- Update all the GlobalSpecific fields of the Master record version
		UPDATE a
		SET a.[ActualGPM] = (CASE d.[ActualGPM_RVFlag] WHEN 1 THEN b.[ActualGPM] ELSE a.[ActualGPM] END),
			a.[AssetTrackingDeviceGuid] = (CASE d.[AssetTrackingDeviceGuid_RVFlag] WHEN 1 THEN b.[AssetTrackingDeviceGuid] ELSE a.[AssetTrackingDeviceGuid] END),
			a.[AssignedToMeterGuid] = (CASE d.[AssignedToMeterGuid_RVFlag] WHEN 1 THEN b.[AssignedToMeterGuid] ELSE a.[AssignedToMeterGuid] END),
			a.[AttachedTo] = (CASE d.[AttachedTo_RVFlag] WHEN 1 THEN b.[AttachedTo] ELSE a.[AttachedTo] END),
			a.[CalibrationDate] = (CASE d.[CalibrationDate_RVFlag] WHEN 1 THEN b.[CalibrationDate] ELSE a.[CalibrationDate] END),
			a.[Capacity] = (CASE d.[Capacity_RVFlag] WHEN 1 THEN b.[Capacity] ELSE a.[Capacity] END),
			a.[CompanyEquipmentID] = (CASE d.[CompanyEquipmentID_RVFlag] WHEN 1 THEN b.[CompanyEquipmentID] ELSE a.[CompanyEquipmentID] END),
			a.[CompanyGuid] = (CASE d.[CompanyGuid_RVFlag] WHEN 1 THEN b.[CompanyGuid] ELSE a.[CompanyGuid] END),
			a.[Consecutive_OOS_Variance] = (CASE d.[Consecutive_OOS_Variance_RVFlag] WHEN 1 THEN b.[Consecutive_OOS_Variance] ELSE a.[Consecutive_OOS_Variance] END),
			a.[DefuelMeterForwards] = (CASE d.[DefuelMeterForwards_RVFlag] WHEN 1 THEN b.[DefuelMeterForwards] ELSE a.[DefuelMeterForwards] END),
			a.[DensityDecimalPlaces] = (CASE d.[DensityDecimalPlaces_RVFlag] WHEN 1 THEN b.[DensityDecimalPlaces] ELSE a.[DensityDecimalPlaces] END),
			a.[DensityUnitIndex] = (CASE d.[DensityUnitIndex_RVFlag] WHEN 1 THEN b.[DensityUnitIndex] ELSE a.[DensityUnitIndex] END),
			a.[Description] = (CASE d.[Description_RVFlag] WHEN 1 THEN b.[Description] ELSE a.[Description] END),
			a.[EquipmentSequence] = (CASE d.[EquipmentSequence_RVFlag] WHEN 1 THEN b.[EquipmentSequence] ELSE a.[EquipmentSequence] END),
			a.[EquipmentTypeGuid] = (CASE d.[EquipmentTypeGuid_RVFlag] WHEN 1 THEN b.[EquipmentTypeGuid] ELSE a.[EquipmentTypeGuid] END),
			a.[Fixed] = (CASE d.[Fixed_RVFlag] WHEN 1 THEN b.[Fixed] ELSE a.[Fixed] END),
			a.[FixedVolume] = (CASE d.[FixedVolume_RVFlag] WHEN 1 THEN b.[FixedVolume] ELSE a.[FixedVolume] END),
			a.[FuelAdditiveFlag] = (CASE d.[FuelAdditiveFlag_RVFlag] WHEN 1 THEN b.[FuelAdditiveFlag] ELSE a.[FuelAdditiveFlag] END),
			a.[FuelCardGuid] = (CASE d.[FuelCardGuid_RVFlag] WHEN 1 THEN b.[FuelCardGuid] ELSE a.[FuelCardGuid] END),
			a.[FuelingState] = (CASE d.[FuelingState_RVFlag] WHEN 1 THEN b.[FuelingState] ELSE a.[FuelingState] END),
			a.[FuelingType] = (CASE d.[FuelingType_RVFlag] WHEN 1 THEN b.[FuelingType] ELSE a.[FuelingType] END),
			a.[HiddenDate] = (CASE d.[HiddenDate_RVFlag] WHEN 1 THEN b.[HiddenDate] ELSE a.[HiddenDate] END),
			a.[ID] = (CASE d.[ID_RVFlag] WHEN 1 THEN b.[ID] ELSE a.[ID] END),
			a.[InspectionDate] = (CASE d.[InspectionDate_RVFlag] WHEN 1 THEN b.[InspectionDate] ELSE a.[InspectionDate] END),
			a.[InstallationDate] = (CASE d.[InstallationDate_RVFlag] WHEN 1 THEN b.[InstallationDate] ELSE a.[InstallationDate] END),
			a.[IntoPlane] = (CASE d.[IntoPlane_RVFlag] WHEN 1 THEN b.[IntoPlane] ELSE a.[IntoPlane] END),
			a.[InUse] = (CASE d.[InUse_RVFlag] WHEN 1 THEN b.[InUse] ELSE a.[InUse] END),
			a.[IssPtNum] = (CASE d.[IssPtNum_RVFlag] WHEN 1 THEN b.[IssPtNum] ELSE a.[IssPtNum] END),
			a.[LockedOut] = (CASE d.[LockedOut_RVFlag] WHEN 1 THEN b.[LockedOut] ELSE a.[LockedOut] END),
			a.[LockedOutDate] = (CASE d.[LockedOutDate_RVFlag] WHEN 1 THEN b.[LockedOutDate] ELSE a.[LockedOutDate] END),
			a.[LockedOutReason] = (CASE d.[LockedOutReason_RVFlag] WHEN 1 THEN b.[LockedOutReason] ELSE a.[LockedOutReason] END),
			a.[LowStockWarning] = (CASE d.[LowStockWarning_RVFlag] WHEN 1 THEN b.[LowStockWarning] ELSE a.[LowStockWarning] END),
			a.[Make] = (CASE d.[Make_RVFlag] WHEN 1 THEN b.[Make] ELSE a.[Make] END),
			a.[ManagedEquipmentFlag] = (CASE d.[ManagedEquipmentFlag_RVFlag] WHEN 1 THEN b.[ManagedEquipmentFlag] ELSE a.[ManagedEquipmentFlag] END),
			a.[ManufactureDate] = (CASE d.[ManufactureDate_RVFlag] WHEN 1 THEN b.[ManufactureDate] ELSE a.[ManufactureDate] END),
			a.[MassDecimalPlaces] = (CASE d.[MassDecimalPlaces_RVFlag] WHEN 1 THEN b.[MassDecimalPlaces] ELSE a.[MassDecimalPlaces] END),
			a.[MassUnitIndex] = (CASE d.[MassUnitIndex_RVFlag] WHEN 1 THEN b.[MassUnitIndex] ELSE a.[MassUnitIndex] END),
			a.[MediaType] = (CASE d.[MediaType_RVFlag] WHEN 1 THEN b.[MediaType] ELSE a.[MediaType] END),
			a.[MeterReading] = (CASE d.[MeterReading_RVFlag] WHEN 1 THEN b.[MeterReading] ELSE a.[MeterReading] END),
			a.[Meters] = (CASE d.[Meters_RVFlag] WHEN 1 THEN b.[Meters] ELSE a.[Meters] END),
			a.[Mobile] = (CASE d.[Mobile_RVFlag] WHEN 1 THEN b.[Mobile] ELSE a.[Mobile] END),
			a.[Model] = (CASE d.[Model_RVFlag] WHEN 1 THEN b.[Model] ELSE a.[Model] END),
			a.[Notes] = (CASE d.[Notes_RVFlag] WHEN 1 THEN b.[Notes] ELSE a.[Notes] END),
			a.[ProductGuid] = (CASE d.[ProductGuid_RVFlag] WHEN 1 THEN b.[ProductGuid] ELSE a.[ProductGuid] END),
			a.[PulseRatio] = (CASE d.[PulseRatio_RVFlag] WHEN 1 THEN b.[PulseRatio] ELSE a.[PulseRatio] END),
			a.[QCDate] = (CASE d.[QCDate_RVFlag] WHEN 1 THEN b.[QCDate] ELSE a.[QCDate] END),
			a.[RatedGPM] = (CASE d.[RatedGPM_RVFlag] WHEN 1 THEN b.[RatedGPM] ELSE a.[RatedGPM] END),
			a.[Round] = (CASE d.[Round_RVFlag] WHEN 1 THEN b.[Round] ELSE a.[Round] END),
			a.[SafeFill] = (CASE d.[SafeFill_RVFlag] WHEN 1 THEN b.[SafeFill] ELSE a.[SafeFill] END),
			a.[ScullyRequired] = (CASE d.[ScullyRequired_RVFlag] WHEN 1 THEN b.[ScullyRequired] ELSE a.[ScullyRequired] END),
			a.[SecondaryStorageFlag] = (CASE d.[SecondaryStorageFlag_RVFlag] WHEN 1 THEN b.[SecondaryStorageFlag] ELSE a.[SecondaryStorageFlag] END),
			a.[SerialNumber] = (CASE d.[SerialNumber_RVFlag] WHEN 1 THEN b.[SerialNumber] ELSE a.[SerialNumber] END),
			a.[StockTrack] = (CASE d.[StockTrack_RVFlag] WHEN 1 THEN b.[StockTrack] ELSE a.[StockTrack] END),
			a.[StorageType] = (CASE d.[StorageType_RVFlag] WHEN 1 THEN b.[StorageType] ELSE a.[StorageType] END),
			a.[TemperatureDecimalPlaces] = (CASE d.[TemperatureDecimalPlaces_RVFlag] WHEN 1 THEN b.[TemperatureDecimalPlaces] ELSE a.[TemperatureDecimalPlaces] END),
			a.[TemperatureUnitIndex] = (CASE d.[TemperatureUnitIndex_RVFlag] WHEN 1 THEN b.[TemperatureUnitIndex] ELSE a.[TemperatureUnitIndex] END),
			a.[Totalisor1] = (CASE d.[Totalisor1_RVFlag] WHEN 1 THEN b.[Totalisor1] ELSE a.[Totalisor1] END),
			a.[Totalisor2] = (CASE d.[Totalisor2_RVFlag] WHEN 1 THEN b.[Totalisor2] ELSE a.[Totalisor2] END),
			a.[TruckCardNumber] = (CASE d.[TruckCardNumber_RVFlag] WHEN 1 THEN b.[TruckCardNumber] ELSE a.[TruckCardNumber] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UserData1] = (CASE d.[UserData1_RVFlag] WHEN 1 THEN b.[UserData1] ELSE a.[UserData1] END),
			a.[UserData10] = (CASE d.[UserData10_RVFlag] WHEN 1 THEN b.[UserData10] ELSE a.[UserData10] END),
			a.[UserData11] = (CASE d.[UserData11_RVFlag] WHEN 1 THEN b.[UserData11] ELSE a.[UserData11] END),
			a.[UserData12] = (CASE d.[UserData12_RVFlag] WHEN 1 THEN b.[UserData12] ELSE a.[UserData12] END),
			a.[UserData13] = (CASE d.[UserData13_RVFlag] WHEN 1 THEN b.[UserData13] ELSE a.[UserData13] END),
			a.[UserData14] = (CASE d.[UserData14_RVFlag] WHEN 1 THEN b.[UserData14] ELSE a.[UserData14] END),
			a.[UserData15] = (CASE d.[UserData15_RVFlag] WHEN 1 THEN b.[UserData15] ELSE a.[UserData15] END),
			a.[UserData16] = (CASE d.[UserData16_RVFlag] WHEN 1 THEN b.[UserData16] ELSE a.[UserData16] END),
			a.[UserData17] = (CASE d.[UserData17_RVFlag] WHEN 1 THEN b.[UserData17] ELSE a.[UserData17] END),
			a.[UserData18] = (CASE d.[UserData18_RVFlag] WHEN 1 THEN b.[UserData18] ELSE a.[UserData18] END),
			a.[UserData19] = (CASE d.[UserData19_RVFlag] WHEN 1 THEN b.[UserData19] ELSE a.[UserData19] END),
			a.[UserData2] = (CASE d.[UserData2_RVFlag] WHEN 1 THEN b.[UserData2] ELSE a.[UserData2] END),
			a.[UserData20] = (CASE d.[UserData20_RVFlag] WHEN 1 THEN b.[UserData20] ELSE a.[UserData20] END),
			a.[UserData21] = (CASE d.[UserData21_RVFlag] WHEN 1 THEN b.[UserData21] ELSE a.[UserData21] END),
			a.[UserData22] = (CASE d.[UserData22_RVFlag] WHEN 1 THEN b.[UserData22] ELSE a.[UserData22] END),
			a.[UserData23] = (CASE d.[UserData23_RVFlag] WHEN 1 THEN b.[UserData23] ELSE a.[UserData23] END),
			a.[UserData24] = (CASE d.[UserData24_RVFlag] WHEN 1 THEN b.[UserData24] ELSE a.[UserData24] END),
			a.[UserData3] = (CASE d.[UserData3_RVFlag] WHEN 1 THEN b.[UserData3] ELSE a.[UserData3] END),
			a.[UserData4] = (CASE d.[UserData4_RVFlag] WHEN 1 THEN b.[UserData4] ELSE a.[UserData4] END),
			a.[UserData5] = (CASE d.[UserData5_RVFlag] WHEN 1 THEN b.[UserData5] ELSE a.[UserData5] END),
			a.[UserData6] = (CASE d.[UserData6_RVFlag] WHEN 1 THEN b.[UserData6] ELSE a.[UserData6] END),
			a.[UserData7] = (CASE d.[UserData7_RVFlag] WHEN 1 THEN b.[UserData7] ELSE a.[UserData7] END),
			a.[UserData8] = (CASE d.[UserData8_RVFlag] WHEN 1 THEN b.[UserData8] ELSE a.[UserData8] END),
			a.[UserData9] = (CASE d.[UserData9_RVFlag] WHEN 1 THEN b.[UserData9] ELSE a.[UserData9] END),
			a.[Volume] = (CASE d.[Volume_RVFlag] WHEN 1 THEN b.[Volume] ELSE a.[Volume] END),
			a.[VolumeDecimalPlaces] = (CASE d.[VolumeDecimalPlaces_RVFlag] WHEN 1 THEN b.[VolumeDecimalPlaces] ELSE a.[VolumeDecimalPlaces] END),
			a.[VolumeUnitIndex] = (CASE d.[VolumeUnitIndex_RVFlag] WHEN 1 THEN b.[VolumeUnitIndex] ELSE a.[VolumeUnitIndex] END),
			a.[Xref] = (CASE d.[Xref_RVFlag] WHEN 1 THEN b.[Xref] ELSE a.[Xref] END),
			a.[Year] = (CASE d.[Year_RVFlag] WHEN 1 THEN b.[Year] ELSE a.[Year] END)			
		FROM tblEquipment a
		INNER JOIN tblEquipment b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN erv.tblTempEquipmentRecordVersioningFlag d
		ON d.EquipmentGuid = a.EquipmentGuid
		WHERE b.EquipmentGuid = @SourceEquipmentGuid
		AND d._CallingReferenceGuid = @callingRef1Guid
		AND a.EquipmentGuid = a._MasterRecordGuid
		
		DELETE erv.tblTempEquipmentRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRef1Guid 

		
		/*Process those ParentSpecific External fields whose propagation require custom handling. */
		-- Process TagsAndLicences External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Tags and Licences') > 0)
		BEGIN
			--Delete the master record version TagsAndLicences mappings that are not supported anymore in the child Equipment record 			
			DELETE a FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
			INNER JOIN [dbo].tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			WHERE b.EquipmentGuid = @masterRecordGuid
			AND b.EquipmentGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] d
				INNER JOIN [dbo].tblEquipment e
				ON e.EquipmentGuid = d.EquipmentGuid			
				WHERE d.EquipmentGuid = @SourceEquipmentGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)	
						
			--Update the master record version mappings that have been modified in the child Equipment record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.Instructor = a.Instructor,
			d.DateCompleted = a.DateCompleted,
			d.DateDue = a.DateDue,
			d.ExpirationDate = a.ExpirationDate,
			d.Id = a.Id,		
			d.Rating = a.Rating,
			d.HistoricalRecord = a.HistoricalRecord,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
			INNER JOIN [dbo].[tblEquipment] b
			ON b.EquipmentGuid = a.EquipmentGuid
			INNER JOIN [map].[tblQualificationEquipmentTagAndLicenseToEquipment] d
			ON d.EquipmentGuid = b._MasterRecordGuid
			AND d.QualificationGuid = a.QualificationGuid
			WHERE a.EquipmentGuid = @SourceEquipmentGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblQualificationEquipmentTagAndLicenseToEquipment]
			(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.QualificationGuid, b._MasterRecordGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.ID, 
			a.Rating, a.HistoricalRecord, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
			INNER JOIN dbo.tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			WHERE a.EquipmentGuid = @SourceEquipmentGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] d
				WHERE d.EquipmentGuid = b._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
		END

		-- Process TestsAndInspections External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Tests and Inspections') > 0)
		BEGIN
			--Delete the master record version TestsAndInspections mappings that are not supported anymore in the child Equipment record 			
			DELETE a FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
			INNER JOIN [dbo].tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			WHERE b.EquipmentGuid = @masterRecordGuid
			AND b.EquipmentGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] d
				INNER JOIN [dbo].tblEquipment e
				ON e.EquipmentGuid = d.EquipmentGuid			
				WHERE d.EquipmentGuid = @SourceEquipmentGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)


			--Update the master record version mappings that have been modified in the child Equipment record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.Instructor = a.Instructor,
			d.DateCompleted = a.DateCompleted,
			d.DateDue = a.DateDue,
			d.ExpirationDate = a.ExpirationDate,
			d.Id = a.Id,		
			d.Rating = a.Rating,
			d.HistoricalRecord = a.HistoricalRecord,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
			INNER JOIN [dbo].[tblEquipment] b
			ON b.EquipmentGuid = a.EquipmentGuid
			INNER JOIN [map].[tblQualificationEquipmentTestAndInspectionToEquipment] d
			ON d.EquipmentGuid = b._MasterRecordGuid
			AND d.QualificationGuid = a.QualificationGuid
			WHERE a.EquipmentGuid = @SourceEquipmentGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblQualificationEquipmentTestAndInspectionToEquipment]
			(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.QualificationGuid, b._MasterRecordGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.ID, 
			a.Rating, a.HistoricalRecord, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
			INNER JOIN dbo.tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			WHERE a.EquipmentGuid = @SourceEquipmentGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] d
				WHERE d.EquipmentGuid = b._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
		END
		
		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		BEGIN
			COMMIT TRANSACTION --ReplicateToMasterRecord
		END
	END TRY
	BEGIN CATCH        
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
			ROLLBACK TRANSACTION --ReplicateToMasterRecord
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
						+ 'Procedure Name: [erv].usp_ReplicateEquipmentGSChangesOnMaster' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END