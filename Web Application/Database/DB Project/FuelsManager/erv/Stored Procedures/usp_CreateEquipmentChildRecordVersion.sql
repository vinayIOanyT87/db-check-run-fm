/*
	DROP PROCEDURE [erv].[usp_CreateEquipmentChildRecordVersion]

	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	--EXEC [erv].[usp_CreateEquipmentChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '0F7228B9-D8E4-41C8-A862-B71FB3F38763', @dt, 'HB'
	EXEC [erv].[usp_CreateEquipmentChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '3D95FDFA-3D72-4E4B-9264-B8E068ECD364', @dt, 'HB'

	SELECT EquipmentGuid, Id, _MasterRecordGuid, SiteGuid, * FROM tblEquipment WHERE _MasterRecordGuid = 'F5EA57B8-2CFB-4605-9B55-8850199671C7'	
*/

CREATE PROCEDURE [erv].[usp_CreateEquipmentChildRecordVersion]
(
	@ParentEntityGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_CreateEquipmentChildRecordVersion] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new Equipment record version for a target site/sitegroup, off a parent record version 
	-- Notes:
	-- 1. @ParentEntityGuid: Entity Guid of the record to be cloned.
	-- 2. @TargetSiteGuid: Site/SiteGroup for which the new clone needs to be created.
	-- 3. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @childRecordVersionGuid uniqueidentifier
		SET @childRecordVersionGuid = NEWID()

		DECLARE @masterRecGuid uniqueidentifier
		SELECT @masterRecGuid = _MasterRecordGuid FROM tblEquipment
		WHERE EquipmentGuid = @ParentEntityGuid

		IF NOT EXISTS
		(
			SELECT * FROM map.tblEntityEquipmentToSite
			WHERE EquipmentGuid = @masterRecGuid
			AND SiteGuid = @TargetSiteGuid
		)
		BEGIN
			RETURN
		END

		IF EXISTS
		(
			SELECT * FROM tblEquipment
			WHERE _MasterRecordGuid = @masterRecGuid
			AND SiteGuid = @TargetSiteGuid
		)
		BEGIN
			RETURN
		END

		--Create the child record version by cloning the internal fields of the parent record version
		INSERT INTO tblEquipment
		(EquipmentGuid,ID,SiteGuid,Description,Make,Model,Year,IssPtNum,Fixed,StorageType,InUse,FixedVolume,IntoPlane,Mobile,AttachedTo,MediaType,Meters,DefuelMeterForwards,PulseRatio,Round,Xref,LowStockWarning,StockTrack,Totalisor1,Totalisor2,FuelingState,Volume,MeterReading,Consecutive_OOS_Variance,Notes,Capacity,SafeFill,VolumeUnitIndex,TemperatureUnitIndex,DensityUnitIndex,MassUnitIndex,VolumeDecimalPlaces,TemperatureDecimalPlaces,DensityDecimalPlaces,MassDecimalPlaces,EquipmentSequence,LockedOut,LockedOutReason,LockedOutDate,SerialNumber,CompanyEquipmentID,TruckCardNumber,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,RatedGPM,ActualGPM,FuelAdditiveFlag,ManufactureDate,InstallationDate,InspectionDate,CalibrationDate,QCDate,SecondaryStorageFlag,ManagedEquipmentFlag,FuelingType,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,UserData9,UserData10,UserData11,UserData12,UserData13,UserData14,UserData15,UserData16,UserData17,UserData18,UserData19,UserData20,UserData21,UserData22,UserData23,UserData24,CompanyGuid,ParentEquipmentGuid,EquipmentTypeGuid,FuelCardGuid,ProductGuid,AssignedToMeterGuid, _MasterRecordGuid,HiddenDate,AssetTrackingDeviceGuid,ScullyRequired)
		SELECT @childRecordVersionGuid,ID,@TargetSiteGuid,Description,Make,Model,Year,IssPtNum,Fixed,StorageType,InUse,FixedVolume,IntoPlane,Mobile,AttachedTo,MediaType,Meters,DefuelMeterForwards,PulseRatio,Round,Xref,LowStockWarning,StockTrack,Totalisor1,Totalisor2,FuelingState,Volume,MeterReading,Consecutive_OOS_Variance,Notes,Capacity,SafeFill,VolumeUnitIndex,TemperatureUnitIndex,DensityUnitIndex,MassUnitIndex,VolumeDecimalPlaces,TemperatureDecimalPlaces,DensityDecimalPlaces,MassDecimalPlaces,EquipmentSequence,LockedOut,LockedOutReason,LockedOutDate,SerialNumber,CompanyEquipmentID,TruckCardNumber,@CreatedDate,@CreatedBy,@CreatedDate,@CreatedBy,RatedGPM,ActualGPM,FuelAdditiveFlag,ManufactureDate,InstallationDate,InspectionDate,CalibrationDate,QCDate,SecondaryStorageFlag,ManagedEquipmentFlag,FuelingType,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,UserData9,UserData10,UserData11,UserData12,UserData13,UserData14,UserData15,UserData16,UserData17,UserData18,UserData19,UserData20,UserData21,UserData22,UserData23,UserData24,CompanyGuid,ParentEquipmentGuid,EquipmentTypeGuid,FuelCardGuid,ProductGuid,AssignedToMeterGuid,_MasterRecordGuid,HiddenDate,AssetTrackingDeviceGuid,ScullyRequired
		FROM tblEquipment
		WHERE EquipmentGuid = @ParentEntityGuid

		--Clone the external attributes of the parent record version
		--Tags and Licenses
		INSERT INTO [map].[tblQualificationEquipmentTagAndLicenseToEquipment]
		(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT QualificationGuid, @childRecordVersionGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment]
		WHERE EquipmentGuid = @ParentEntityGuid

		--Tests and Equipments
		INSERT INTO [map].[tblQualificationEquipmentTestAndInspectionToEquipment]
		(QualificationGuid, EquipmentGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT QualificationGuid, @childRecordVersionGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment]
		WHERE EquipmentGuid = @ParentEntityGuid

		--Compartments
		/*
		INSERT INTO [dbo].[tblEquipment]
		(ID,Description,Make,Model,Year,IssPtNum,Fixed,StorageType,InUse,FixedVolume,IntoPlane,Mobile,AttachedTo,MediaType,Meters,DefuelMeterForwards,PulseRatio,Round,Xref,LowStockWarning,StockTrack,Totalisor1,Totalisor2,FuelingState,Volume,MeterReading,Consecutive_OOS_Variance,Notes,Capacity,SafeFill,VolumeUnitIndex,TemperatureUnitIndex,DensityUnitIndex,MassUnitIndex,VolumeDecimalPlaces,TemperatureDecimalPlaces,DensityDecimalPlaces,MassDecimalPlaces,EquipmentSequence,LockedOut,LockedOutReason,LockedOutDate,SerialNumber,CompanyEquipmentID,TruckCardNumber,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,RatedGPM,ActualGPM,FuelAdditiveFlag,ManufactureDate,InstallationDate,InspectionDate,CalibrationDate,QCDate,SecondaryStorageFlag,ManagedEquipmentFlag,FuelingType,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,UserData9,UserData10,UserData11,UserData12,UserData13,UserData14,UserData15,UserData16,UserData17,UserData18,UserData19,UserData20,UserData21,UserData22,UserData23,UserData24,EquipmentGuid,SiteGuid,CompanyGuid,ParentEquipmentGuid,EquipmentTypeGuid,FuelCardGuid,ProductGuid,AssignedToMeterGuid,_MasterRecordGuid)
		SELECT ID,Description,Make,Model,Year,IssPtNum,Fixed,StorageType,InUse,FixedVolume,IntoPlane,Mobile,AttachedTo,MediaType,Meters,DefuelMeterForwards,PulseRatio,Round,Xref,LowStockWarning,StockTrack,Totalisor1,Totalisor2,FuelingState,Volume,MeterReading,Consecutive_OOS_Variance,Notes,Capacity,SafeFill,VolumeUnitIndex,TemperatureUnitIndex,DensityUnitIndex,MassUnitIndex,VolumeDecimalPlaces,TemperatureDecimalPlaces,DensityDecimalPlaces,MassDecimalPlaces,EquipmentSequence,LockedOut,LockedOutReason,LockedOutDate,SerialNumber,CompanyEquipmentID,TruckCardNumber,@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy,RatedGPM,ActualGPM,FuelAdditiveFlag,ManufactureDate,InstallationDate,InspectionDate,CalibrationDate,QCDate,SecondaryStorageFlag,ManagedEquipmentFlag,FuelingType,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,UserData9,UserData10,UserData11,UserData12,UserData13,UserData14,UserData15,UserData16,UserData17,UserData18,UserData19,UserData20,UserData21,UserData22,UserData23,UserData24, NEWID(),@TargetSiteGuid,CompanyGuid,@childRecordVersionGuid,EquipmentTypeGuid,FuelCardGuid,ProductGuid,AssignedToMeterGuid,_MasterRecordGuid
		FROM [dbo].[tblEquipment]
		WHERE ParentEquipmentGuid = @ParentEntityGuid
		*/

		/*
		--Process Variable
		INSERT INTO tblProcessVariableEquipment
		(LookupProcessVariableTypeIndex, InstanceNumber, EquipmentGuid, OPCConnectionGuid, OPCItemID, DataType, ServerEngineeringUnitsIndex, Quality, SIValue, LookupSIValueVariantTypeIndex, DateTimeStamp, Maximum, LookupMaximumVariantTypeIndex, Minimum, LookupMinimumVariantTypeIndex, DataTypeEnabled, Input, InputEnabled, MessageApplicationStringGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT LookupProcessVariableTypeIndex, InstanceNumber, @childRecordVersionGuid, OPCConnectionGuid, OPCItemID, DataType, ServerEngineeringUnitsIndex, Quality, SIValue, LookupSIValueVariantTypeIndex, DateTimeStamp, Maximum, LookupMaximumVariantTypeIndex, Minimum, LookupMinimumVariantTypeIndex, DataTypeEnabled, Input, InputEnabled, MessageApplicationStringGuid, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM tblProcessVariableEquipment
		WHERE EquipmentGuid = @ParentEntityGuid
		*/

		/*
		--Meter
		DECLARE @assignedToMeterGuid uniqueidentifier
		SELECT @assignedToMeterGuid = AssignedToMeterGuid FROM tblEquipment WHERE EquipmentGuid = @ParentEntityGuid
		IF (@assignedToMeterGuid IS NOT NULL)
		BEGIN
			DECLARE @childRecordVersionMeterGuid uniqueidentifier
			SET @childRecordVersionMeterGuid = NEWID()

			INSERT INTO tblMeter
			(MeterGuid, SiteGuid, MeterId, NumberOfDigits, RotatesBackwardsFlag, ReceiptMeterFlag, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT @childRecordVersionMeterGuid, @TargetSiteGuid, a.MeterId, a.NumberOfDigits, a.RotatesBackwardsFlag, a.ReceiptMeterFlag, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
			FROM tblMeter a
			INNER JOIN tblEquipment b
			ON b.AssignedToMeterGuid = a.MeterGuid
			WHERE b.EquipmentGuid = @ParentEntityGuid		

			UPDATE tblEquipment
			SET AssignedToMeterGuid = @childRecordVersionMeterGuid
			WHERE EquipmentGuid = @childRecordVersionGuid
		END
		*/
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
						+ 'Procedure Name: [erv].usp_CreateEquipmentChildRecordVersion' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
