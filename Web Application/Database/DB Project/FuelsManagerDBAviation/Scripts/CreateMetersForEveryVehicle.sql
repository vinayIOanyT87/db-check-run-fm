DECLARE @MetersCreated TABLE
(
	VehicleID NVARCHAR(100),
	VehicleGUID UNIQUEIDENTIFIER,
	SiteGuid UNIQUEIDENTIFIER,
	MeterID NVARCHAR(100),
	MeterGUID UNIQUEIDENTIFIER
);

WITH VehiclesWithoutMeters(VehicleID, VehicleGuid, SiteGuid) AS (
SELECT e.ID, e.EquipmentGuid, e.SiteGuid FROM tblEquipment e
LEFT JOIN map.tblMeterToEquipment mte ON e.EquipmentGuid = mte.EquipmentGuid
LEFT JOIN tblMeter m ON mte.MeterGuid = m.MeterGuid
WHERE m.MeterGuid is null
)

INSERT INTO @MetersCreated (VehicleID, VehicleGUID, SiteGuid, MeterID, MeterGUID)
SELECT VehicleID, VehicleGuid, SiteGuid, VehicleID, NewID() from VehiclesWithoutMeters

INSERT INTO tblMeter (MeterGuid, SiteGuid, MeterID, NumberOfDigits, RotatesBackwardsFlag, ReceiptMeterFlag, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
SELECT MeterGUID, SiteGuid, VehicleID, 8, 0, 0, GETUTCDATE(), 'SYSTEM', GETUTCDATE(), 'SYSTEM' FROM @MetersCreated

INSERT INTO map.tblMeterToEquipment (MeterToEquipmentGuid, MeterGuid, EquipmentGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
SELECT NEWID(), MeterGUID, VehicleGUID, GETUTCDATE(), 'SYSTEM', GETUTCDATE(), 'SYSTEM' FROM @MetersCreated
