/*
	DROP PROCEDURE [erv].[usp_PropagateEquipmentRevisionByEntityRecordChange]

	EXEC [erv].[usp_PropagateEquipmentRevisionByEntityRecordChange] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_PropagateEquipmentRevisionByEntityRecordChange] 'F94D0DAB-8C85-4A73-830E-A8168078B6AD'
	EXEC [erv].[usp_PropagateEquipmentRevisionByEntityRecordChange] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
*/

CREATE PROCEDURE [erv].[usp_PropagateEquipmentRevisionByEntityRecordChange]
(
	@SourceEquipmentGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagateEquipmentRevisionByEntityRecordChange] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate the current revision of a given equipment entity record down the site hierarchy, according to the rules established by the Field Level Control configurations.
	-- This Stored Procedure is to be used to propagate the effect of an entity record change down to all its children record versions.
	-- Notes:
	-- 1. @SourceEquipmentGuid: Guid of the Equipment record that needs to be propagated down the site hierarchy. This should correspond to the exact record version that has been 
	--    changed (and not the parent record of the entity record).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
/*
	DECLARE @SourceEquipmentGuid uniqueidentifier
	SET @SourceEquipmentGuid = '886AA683-C97D-461C-AFB6-AD9A4579E51D'
*/
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Equipment'

		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		SELECT @ownerSiteGuid = SiteGuid, @masterRecordGuid = _MasterRecordGuid FROM tblEquipment
		WHERE EquipmentGuid = @SourceEquipmentGuid

		IF ((@masterRecordGuid IS NULL) OR (@ownerSiteGuid IS NULL))
		BEGIN
			RAISERROR('Cannot locate the source record for data propagation.',16,1); 
			RETURN;
		END

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

		DECLARE @assignedFromSiteGroupGuid uniqueidentifier
		IF (@SourceEquipmentGuid = @masterRecordGuid)
		BEGIN
			SET @assignedFromSiteGroupGuid = @ownerSiteGuid
		END
		ELSE
		BEGIN
			SET @assignedFromSiteGroupGuid = (SELECT [erv].[udf_GetEntityAssignedFromSite] (@EntityTypeId, @SourceEquipmentGuid, Null))
			IF (@assignedFromSiteGroupGuid IS NULL)
			BEGIN
				RAISERROR('Cannot locate the assignment information for the selected child record version.',16,1); 
				RETURN;
			END
		END				

		-- Retrieve the Entity To Site hierarchy below the owner sitegroup of the entity record whose changes are to be propagated
		-- This corresponds to all the child record versions who derives, directly or indirectly, from the given record version.
		DECLARE @tblEntityToSiteHierarchy TABLE
		(
			SiteGuid uniqueidentifier
			, SiteId nvarchar(30)
			, HierarchyLevel int
			, Processed bit
		);

		INSERT INTO @tblEntityToSiteHierarchy
		(SiteGuid, SiteId, HierarchyLevel, Processed)
		SELECT SiteGuid, SiteId, HierarchyLevel, 0
		FROM [erv].[udf_GetEquipmentToSiteHierarchyByRecordVersionGuid](@SourceEquipmentGuid)
		WHERE HierarchyLevel > 0
		ORDER BY HierarchyLevel, SiteGuid

		--Retrieve the VersionSpecific fields for the owner sitegroup of the entity record whose changes need to be propagated
		DECLARE @tblSourceVersionSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			Processed bit
		)
		DECLARE @callingRef2Guid uniqueidentifier
		SET @callingRef2Guid = NEWID()

		EXEC erv.usp_GetRecordVersioningFields @EntityTypeId, @masterRecordGuid, @ownerSiteGuid, 'VersionSpecific', @callingRef2Guid 

		INSERT @tblSourceVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName)
		SELECT TargetField, IsExternalAttribute, InternalFieldName FROM erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid
		
		--Build a table that has one flag column for each column of the tblEquipment table, and set the flag according to whether the field is VersionSpecific or not.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempEquipmentRecordVersioningFlag
		(EquipmentGuid, SiteGuid, _CallingReferenceGuid)
		SELECT a.EquipmentGuid, a.SiteGuid, @callingRef1Guid FROM tblEquipment a
		INNER JOIN @tblEntityToSiteHierarchy b
		ON b.SiteGuid = a.SiteGuid
		WHERE a._MasterRecordGuid = @masterRecordGuid

		DECLARE @tblTargetChildRecordVersions TABLE
		(
			EquipmentGuid uniqueidentifier,
			SiteGuid uniqueidentifier,
			HierarchyLevel int,
			Processed bit
		)

		INSERT INTO @tblTargetChildRecordVersions
		(EquipmentGuid, SiteGuid, HierarchyLevel, Processed)
		SELECT a.EquipmentGuid, b.SiteGuid, c.HierarchyLevel, 0 FROM erv.tblTempEquipmentRecordVersioningFlag a
		INNER JOIN tblEquipment b
		ON b.EquipmentGuid = a.EquipmentGuid
		INNER JOIN @tblEntityToSiteHierarchy c
		ON c.SiteGuid = b.SiteGuid
		WHERE b._MasterRecordGuid = @masterRecordGuid
		AND a._CallingReferenceGuid = @callingRef1Guid
			

		IF (NOT EXISTS (SELECT * FROM erv.tblTempEquipmentRecordVersioningFlag WHERE _CallingReferenceGuid = @callingRef1Guid))
		BEGIN				
			/*	No child record versions to update.	*/
			RETURN;
		END

		EXEC [erv].[usp_PivotFLCConfigurationsForEntityRecord] @EntityTypeId, @masterRecordGuid, @ownerSiteGuid, @callingRef2Guid, @callingRef1Guid

		DELETE erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid
		

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --PropagateToChildRecordVersions
            SET @BeginTran = 1   
		END  	
		

		-- Update all the internal non-VersionSpecific fields for all applicable child record versions
		UPDATE a
		SET a.[ActualGPM] = (CASE d.[ActualGPM_RVFlag] WHEN 1 THEN a.[ActualGPM] ELSE b.[ActualGPM] END),
			a.[AssetTrackingDeviceGuid] = (CASE d.[AssetTrackingDeviceGuid_RVFlag] WHEN 1 THEN a.[AssetTrackingDeviceGuid] ELSE b.[AssetTrackingDeviceGuid] END),
			a.[AssignedToMeterGuid] = (CASE d.[AssignedToMeterGuid_RVFlag] WHEN 1 THEN a.[AssignedToMeterGuid] ELSE b.[AssignedToMeterGuid] END),
			a.[AttachedTo] = (CASE d.[AttachedTo_RVFlag] WHEN 1 THEN a.[AttachedTo] ELSE b.[AttachedTo] END),
			a.[CalibrationDate] = (CASE d.[CalibrationDate_RVFlag] WHEN 1 THEN a.[CalibrationDate] ELSE b.[CalibrationDate] END),
			a.[Capacity] = (CASE d.[Capacity_RVFlag] WHEN 1 THEN a.[Capacity] ELSE b.[Capacity] END),
			a.[CompanyEquipmentID] = (CASE d.[CompanyEquipmentID_RVFlag] WHEN 1 THEN a.[CompanyEquipmentID] ELSE b.[CompanyEquipmentID] END),
			a.[CompanyGuid] = (CASE d.[CompanyGuid_RVFlag] WHEN 1 THEN a.[CompanyGuid] ELSE b.[CompanyGuid] END),
			a.[Consecutive_OOS_Variance] = (CASE d.[Consecutive_OOS_Variance_RVFlag] WHEN 1 THEN a.[Consecutive_OOS_Variance] ELSE b.[Consecutive_OOS_Variance] END),
			a.[DefuelMeterForwards] = (CASE d.[DefuelMeterForwards_RVFlag] WHEN 1 THEN a.[DefuelMeterForwards] ELSE b.[DefuelMeterForwards] END),
			a.[DensityDecimalPlaces] = (CASE d.[DensityDecimalPlaces_RVFlag] WHEN 1 THEN a.[DensityDecimalPlaces] ELSE b.[DensityDecimalPlaces] END),
			a.[DensityUnitIndex] = (CASE d.[DensityUnitIndex_RVFlag] WHEN 1 THEN a.[DensityUnitIndex] ELSE b.[DensityUnitIndex] END),
			a.[Description] = (CASE d.[Description_RVFlag] WHEN 1 THEN a.[Description] ELSE b.[Description] END),
			a.[EquipmentSequence] = (CASE d.[EquipmentSequence_RVFlag] WHEN 1 THEN a.[EquipmentSequence] ELSE b.[EquipmentSequence] END),
			a.[EquipmentTypeGuid] = (CASE d.[EquipmentTypeGuid_RVFlag] WHEN 1 THEN a.[EquipmentTypeGuid] ELSE b.[EquipmentTypeGuid] END),
			a.[Fixed] = (CASE d.[Fixed_RVFlag] WHEN 1 THEN a.[Fixed] ELSE b.[Fixed] END),
			a.[FixedVolume] = (CASE d.[FixedVolume_RVFlag] WHEN 1 THEN a.[FixedVolume] ELSE b.[FixedVolume] END),
			a.[FuelAdditiveFlag] = (CASE d.[FuelAdditiveFlag_RVFlag] WHEN 1 THEN a.[FuelAdditiveFlag] ELSE b.[FuelAdditiveFlag] END),
			a.[FuelCardGuid] = (CASE d.[FuelCardGuid_RVFlag] WHEN 1 THEN a.[FuelCardGuid] ELSE b.[FuelCardGuid] END),
			a.[FuelingState] = (CASE d.[FuelingState_RVFlag] WHEN 1 THEN a.[FuelingState] ELSE b.[FuelingState] END),
			a.[FuelingType] = (CASE d.[FuelingType_RVFlag] WHEN 1 THEN a.[FuelingType] ELSE b.[FuelingType] END),
			a.[HiddenDate] = (CASE d.[HiddenDate_RVFlag] WHEN 1 THEN a.[HiddenDate] ELSE b.[HiddenDate] END),
			a.[ID] = (CASE d.[ID_RVFlag] WHEN 1 THEN a.[ID] ELSE b.[ID] END),
			a.[InspectionDate] = (CASE d.[InspectionDate_RVFlag] WHEN 1 THEN a.[InspectionDate] ELSE b.[InspectionDate] END),
			a.[InstallationDate] = (CASE d.[InstallationDate_RVFlag] WHEN 1 THEN a.[InstallationDate] ELSE b.[InstallationDate] END),
			a.[IntoPlane] = (CASE d.[IntoPlane_RVFlag] WHEN 1 THEN a.[IntoPlane] ELSE b.[IntoPlane] END),
			a.[InUse] = (CASE d.[InUse_RVFlag] WHEN 1 THEN a.[InUse] ELSE b.[InUse] END),
			a.[IssPtNum] = (CASE d.[IssPtNum_RVFlag] WHEN 1 THEN a.[IssPtNum] ELSE b.[IssPtNum] END),
			a.[LockedOut] = (CASE d.[LockedOut_RVFlag] WHEN 1 THEN a.[LockedOut] ELSE b.[LockedOut] END),
			a.[LockedOutDate] = (CASE d.[LockedOutDate_RVFlag] WHEN 1 THEN a.[LockedOutDate] ELSE b.[LockedOutDate] END),
			a.[LockedOutReason] = (CASE d.[LockedOutReason_RVFlag] WHEN 1 THEN a.[LockedOutReason] ELSE b.[LockedOutReason] END),
			a.[LowStockWarning] = (CASE d.[LowStockWarning_RVFlag] WHEN 1 THEN a.[LowStockWarning] ELSE b.[LowStockWarning] END),
			a.[Make] = (CASE d.[Make_RVFlag] WHEN 1 THEN a.[Make] ELSE b.[Make] END),
			a.[ManagedEquipmentFlag] = (CASE d.[ManagedEquipmentFlag_RVFlag] WHEN 1 THEN a.[ManagedEquipmentFlag] ELSE b.[ManagedEquipmentFlag] END),
			a.[ManufactureDate] = (CASE d.[ManufactureDate_RVFlag] WHEN 1 THEN a.[ManufactureDate] ELSE b.[ManufactureDate] END),
			a.[MassDecimalPlaces] = (CASE d.[MassDecimalPlaces_RVFlag] WHEN 1 THEN a.[MassDecimalPlaces] ELSE b.[MassDecimalPlaces] END),
			a.[MassUnitIndex] = (CASE d.[MassUnitIndex_RVFlag] WHEN 1 THEN a.[MassUnitIndex] ELSE b.[MassUnitIndex] END),
			a.[MediaType] = (CASE d.[MediaType_RVFlag] WHEN 1 THEN a.[MediaType] ELSE b.[MediaType] END),
			a.[MeterReading] = (CASE d.[MeterReading_RVFlag] WHEN 1 THEN a.[MeterReading] ELSE b.[MeterReading] END),
			a.[Meters] = (CASE d.[Meters_RVFlag] WHEN 1 THEN a.[Meters] ELSE b.[Meters] END),
			a.[Mobile] = (CASE d.[Mobile_RVFlag] WHEN 1 THEN a.[Mobile] ELSE b.[Mobile] END),
			a.[Model] = (CASE d.[Model_RVFlag] WHEN 1 THEN a.[Model] ELSE b.[Model] END),
			a.[Notes] = (CASE d.[Notes_RVFlag] WHEN 1 THEN a.[Notes] ELSE b.[Notes] END),
			a.[ProductGuid] = (CASE d.[ProductGuid_RVFlag] WHEN 1 THEN a.[ProductGuid] ELSE b.[ProductGuid] END),
			a.[PulseRatio] = (CASE d.[PulseRatio_RVFlag] WHEN 1 THEN a.[PulseRatio] ELSE b.[PulseRatio] END),
			a.[QCDate] = (CASE d.[QCDate_RVFlag] WHEN 1 THEN a.[QCDate] ELSE b.[QCDate] END),
			a.[RatedGPM] = (CASE d.[RatedGPM_RVFlag] WHEN 1 THEN a.[RatedGPM] ELSE b.[RatedGPM] END),
			a.[Round] = (CASE d.[Round_RVFlag] WHEN 1 THEN a.[Round] ELSE b.[Round] END),
			a.[SafeFill] = (CASE d.[SafeFill_RVFlag] WHEN 1 THEN a.[SafeFill] ELSE b.[SafeFill] END),
			a.[ScullyRequired] = (CASE d.[ScullyRequired_RVFlag] WHEN 1 THEN a.[ScullyRequired] ELSE b.[ScullyRequired] END),
			a.[SecondaryStorageFlag] = (CASE d.[SecondaryStorageFlag_RVFlag] WHEN 1 THEN a.[SecondaryStorageFlag] ELSE b.[SecondaryStorageFlag] END),
			a.[SerialNumber] = (CASE d.[SerialNumber_RVFlag] WHEN 1 THEN a.[SerialNumber] ELSE b.[SerialNumber] END),
			a.[StockTrack] = (CASE d.[StockTrack_RVFlag] WHEN 1 THEN a.[StockTrack] ELSE b.[StockTrack] END),
			a.[StorageType] = (CASE d.[StorageType_RVFlag] WHEN 1 THEN a.[StorageType] ELSE b.[StorageType] END),
			a.[TemperatureDecimalPlaces] = (CASE d.[TemperatureDecimalPlaces_RVFlag] WHEN 1 THEN a.[TemperatureDecimalPlaces] ELSE b.[TemperatureDecimalPlaces] END),
			a.[TemperatureUnitIndex] = (CASE d.[TemperatureUnitIndex_RVFlag] WHEN 1 THEN a.[TemperatureUnitIndex] ELSE b.[TemperatureUnitIndex] END),
			a.[Totalisor1] = (CASE d.[Totalisor1_RVFlag] WHEN 1 THEN a.[Totalisor1] ELSE b.[Totalisor1] END),
			a.[Totalisor2] = (CASE d.[Totalisor2_RVFlag] WHEN 1 THEN a.[Totalisor2] ELSE b.[Totalisor2] END),
			a.[TruckCardNumber] = (CASE d.[TruckCardNumber_RVFlag] WHEN 1 THEN a.[TruckCardNumber] ELSE b.[TruckCardNumber] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UserData1] = (CASE d.[UserData1_RVFlag] WHEN 1 THEN a.[UserData1] ELSE b.[UserData1] END),
			a.[UserData10] = (CASE d.[UserData10_RVFlag] WHEN 1 THEN a.[UserData10] ELSE b.[UserData10] END),
			a.[UserData11] = (CASE d.[UserData11_RVFlag] WHEN 1 THEN a.[UserData11] ELSE b.[UserData11] END),
			a.[UserData12] = (CASE d.[UserData12_RVFlag] WHEN 1 THEN a.[UserData12] ELSE b.[UserData12] END),
			a.[UserData13] = (CASE d.[UserData13_RVFlag] WHEN 1 THEN a.[UserData13] ELSE b.[UserData13] END),
			a.[UserData14] = (CASE d.[UserData14_RVFlag] WHEN 1 THEN a.[UserData14] ELSE b.[UserData14] END),
			a.[UserData15] = (CASE d.[UserData15_RVFlag] WHEN 1 THEN a.[UserData15] ELSE b.[UserData15] END),
			a.[UserData16] = (CASE d.[UserData16_RVFlag] WHEN 1 THEN a.[UserData16] ELSE b.[UserData16] END),
			a.[UserData17] = (CASE d.[UserData17_RVFlag] WHEN 1 THEN a.[UserData17] ELSE b.[UserData17] END),
			a.[UserData18] = (CASE d.[UserData18_RVFlag] WHEN 1 THEN a.[UserData18] ELSE b.[UserData18] END),
			a.[UserData19] = (CASE d.[UserData19_RVFlag] WHEN 1 THEN a.[UserData19] ELSE b.[UserData19] END),
			a.[UserData2] = (CASE d.[UserData2_RVFlag] WHEN 1 THEN a.[UserData2] ELSE b.[UserData2] END),
			a.[UserData20] = (CASE d.[UserData20_RVFlag] WHEN 1 THEN a.[UserData20] ELSE b.[UserData20] END),
			a.[UserData21] = (CASE d.[UserData21_RVFlag] WHEN 1 THEN a.[UserData21] ELSE b.[UserData21] END),
			a.[UserData22] = (CASE d.[UserData22_RVFlag] WHEN 1 THEN a.[UserData22] ELSE b.[UserData22] END),
			a.[UserData23] = (CASE d.[UserData23_RVFlag] WHEN 1 THEN a.[UserData23] ELSE b.[UserData23] END),
			a.[UserData24] = (CASE d.[UserData24_RVFlag] WHEN 1 THEN a.[UserData24] ELSE b.[UserData24] END),
			a.[UserData3] = (CASE d.[UserData3_RVFlag] WHEN 1 THEN a.[UserData3] ELSE b.[UserData3] END),
			a.[UserData4] = (CASE d.[UserData4_RVFlag] WHEN 1 THEN a.[UserData4] ELSE b.[UserData4] END),
			a.[UserData5] = (CASE d.[UserData5_RVFlag] WHEN 1 THEN a.[UserData5] ELSE b.[UserData5] END),
			a.[UserData6] = (CASE d.[UserData6_RVFlag] WHEN 1 THEN a.[UserData6] ELSE b.[UserData6] END),
			a.[UserData7] = (CASE d.[UserData7_RVFlag] WHEN 1 THEN a.[UserData7] ELSE b.[UserData7] END),
			a.[UserData8] = (CASE d.[UserData8_RVFlag] WHEN 1 THEN a.[UserData8] ELSE b.[UserData8] END),
			a.[UserData9] = (CASE d.[UserData9_RVFlag] WHEN 1 THEN a.[UserData9] ELSE b.[UserData9] END),
			a.[Volume] = (CASE d.[Volume_RVFlag] WHEN 1 THEN a.[Volume] ELSE b.[Volume] END),
			a.[VolumeDecimalPlaces] = (CASE d.[VolumeDecimalPlaces_RVFlag] WHEN 1 THEN a.[VolumeDecimalPlaces] ELSE b.[VolumeDecimalPlaces] END),
			a.[VolumeUnitIndex] = (CASE d.[VolumeUnitIndex_RVFlag] WHEN 1 THEN a.[VolumeUnitIndex] ELSE b.[VolumeUnitIndex] END),
			a.[Xref] = (CASE d.[Xref_RVFlag] WHEN 1 THEN a.[Xref] ELSE b.[Xref] END),
			a.[Year] = (CASE d.[Year_RVFlag] WHEN 1 THEN a.[Year] ELSE b.[Year] END)			
		FROM tblEquipment a
		INNER JOIN tblEquipment b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN @tblEntityToSiteHierarchy c
		ON a.SiteGuid = c.SiteGuid
		INNER JOIN erv.tblTempEquipmentRecordVersioningFlag d
		ON d.EquipmentGuid = a.EquipmentGuid
		WHERE b.EquipmentGuid = @SourceEquipmentGuid
		AND d._CallingReferenceGuid = @callingRef1Guid

		DELETE erv.tblTempEquipmentRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRef1Guid 

		
		/*Process those non-VersionSpecific External fields whose propagation require custom handling. */
		-- Process TagsAndLicences External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Tags and Licences') = 0)
		BEGIN
			DECLARE @tblTagAndLicenseMappings TABLE
			(
				EquipmentGuid uniqueidentifier,
				QualificationGuid uniqueidentifier,
				Sequence int,
				Instructor nvarchar(50),
				DateCompleted [datetimeoffset](7),
				DateDue [datetimeoffset](7),
				ExpirationDate [datetimeoffset](7),
				ID nvarchar(50),
				Rating nvarchar(20),
				HistoricalRecord bit,
				CreatedDate [datetimeoffset](7),
				CreatedBy [dbo].[udtUserID],
				UpdatedDate [datetimeoffset](7),
				UpdatedBy [dbo].[udtUserID]
			)
			INSERT INTO @tblTagAndLicenseMappings
			(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.QualificationGuid, a.EquipmentGuid, b.Sequence, b.Instructor, b.DateCompleted, b.DateDue, b.ExpirationDate, b.ID, b.Rating, b.HistoricalRecord, b.CreatedDate, b.CreatedBy, b.UpdatedDate, b.UpdatedBy 
			FROM @tblTargetChildRecordVersions a
			CROSS JOIN (SELECT * FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment]  WHERE EquipmentGuid = @SourceEquipmentGuid) b

			DELETE a FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.EquipmentGuid = a.EquipmentGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM @tblTagAndLicenseMappings c
				WHERE c.EquipmentGuid = a.EquipmentGuid
				AND c.QualificationGuid = a.QualificationGuid
			)
						
			--Update the active (non-historical) child record version mappings that have been modified in the parent Equipment
			UPDATE a
			SET a.Sequence = b.Sequence, 
			a.Instructor = b.Instructor,
			a.DateCompleted = b.DateCompleted,
			a.DateDue = b.DateDue,
			a.ExpirationDate = b.ExpirationDate,
			a.Id = b.Id,		
			a.Rating = b.Rating,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = b.UpdatedBy
			FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] a
			INNER JOIN @tblTagAndLicenseMappings b
			ON b.EquipmentGuid = a.EquipmentGuid
			AND b.QualificationGuid = a.QualificationGuid
			AND b.HistoricalRecord = a.HistoricalRecord			
			WHERE b.HistoricalRecord = 0

			INSERT INTO [map].[tblQualificationEquipmentTagAndLicenseToEquipment]
			(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.QualificationGuid, a.EquipmentGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.ID, a.Rating, a.HistoricalRecord, GETDATE(), CreatedBy, GETDATE(), UpdatedBy
			FROM @tblTagAndLicenseMappings a
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment] b 
				WHERE b.EquipmentGuid = a.EquipmentGuid
				AND b.QualificationGuid = a.QualificationGuid
			)
		END

		-- Process TestsAndInspections External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Tests and Inspections') = 0)
		BEGIN
			DECLARE @tblTestAndInspectionMappings TABLE
			(
				EquipmentGuid uniqueidentifier,
				QualificationGuid uniqueidentifier,
				Sequence int,
				Instructor nvarchar(50),
				DateCompleted [datetimeoffset](7),
				DateDue [datetimeoffset](7),
				ExpirationDate [datetimeoffset](7),
				ID nvarchar(50),
				Rating nvarchar(20),
				HistoricalRecord bit,
				CreatedDate [datetimeoffset](7),
				CreatedBy [dbo].[udtUserID],
				UpdatedDate [datetimeoffset](7),
				UpdatedBy [dbo].[udtUserID]
			)
			INSERT INTO @tblTestAndInspectionMappings
			(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.QualificationGuid, a.EquipmentGuid, b.Sequence, b.Instructor, b.DateCompleted, b.DateDue, b.ExpirationDate, b.ID, b.Rating, b.HistoricalRecord, GETDATE(), b.CreatedBy, GETDATE(), b.UpdatedBy 
			FROM @tblTargetChildRecordVersions a
			CROSS JOIN (SELECT * FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment]  WHERE EquipmentGuid = @SourceEquipmentGuid) b

			DELETE a FROM  [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.EquipmentGuid = a.EquipmentGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM @tblTestAndInspectionMappings c
				WHERE c.EquipmentGuid = a.EquipmentGuid
				AND c.QualificationGuid = a.QualificationGuid
			)

			--Update the active (non-historical) child record version mappings that have been modified in the parent Equipment
			UPDATE a
			SET a.Sequence = b.Sequence, 
			a.Instructor = b.Instructor,
			a.DateCompleted = b.DateCompleted,
			a.DateDue = b.DateDue,
			a.ExpirationDate = b.ExpirationDate,
			a.Id = b.Id,
			a.Rating = b.Rating,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = b.UpdatedBy
			FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] a
			INNER JOIN @tblTestAndInspectionMappings b
			ON b.EquipmentGuid = a.EquipmentGuid
			AND b.QualificationGuid = a.QualificationGuid
			AND b.HistoricalRecord = a.HistoricalRecord			
			AND b.HistoricalRecord = 0

			INSERT INTO [map].[tblQualificationEquipmentTestAndInspectionToEquipment]
			(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.QualificationGuid, a.EquipmentGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.ID, a.Rating, a.HistoricalRecord, GETDATE(), CreatedBy, GETDATE(), UpdatedBy
			FROM @tblTestAndInspectionMappings a
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment] b 
				WHERE b.EquipmentGuid = a.EquipmentGuid
				AND b.QualificationGuid = a.QualificationGuid
			)
			
		END
		
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
						+ 'Procedure Name: [erv].usp_PropagateEquipmentRevisionByEntityRecordChange' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
