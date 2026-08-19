
/*
=============================================
Author: Ryan Hill
Create date: 4/24/12
Description:

Create a record in the meter table with the values provided, 
and return the identity guid of the new record
=============================================
*/
CREATE PROCEDURE [dbo].[usp_MeterInsert]
(
	@SiteGuid UNIQUEIDENTIFIER,
	@MeterID NVARCHAR(30),
	@NumberOfDigits TINYINT,
	@RotatesBackwardsFlag BIT,
	@ReceiptMeterFlag BIT,
	@MeterFactor FLOAT = 1.0,
	@FuelCompressionFactor FLOAT = 1.0,
	@DcuID NVARCHAR(50) = NULL,
	@DcuBatteryVoltage FLOAT = NULL,
	@DcuBatteryCurrent FLOAT = NULL,
	@DcuTemperature FLOAT = NULL,
	@DcuResets INT = NULL,
	@DcuUpdateDate DATETIMEOFFSET(7) = NULL,
	@DcuConfigurationDate DATETIMEOFFSET(7) = NULL,
	@DcuFirmwareVersion NVARCHAR(50) = NULL,
	@DcuBluetoothAddress NVARCHAR(50) = NULL,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@MeterGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	SET NOCOUNT OFF
	SET @MeterGuid = NEWID()

	INSERT INTO tblMeter
	(
		SiteGuid,
		MeterID,
		NumberOfDigits,
		RotatesBackwardsFlag,
		ReceiptMeterFlag, 
		MeterFactor,
		FuelCompressionFactor,
		DcuID,
		DcuBatteryVoltage,
		DcuBatteryCurrent,
		DcuTemperature,
		DcuResets,
		DcuUpdateDate,
		DcuConfigurationDate,
		DcuFirmwareVersion,
		DcuBluetoothAddress,
		CreatedDate,
		CreatedBy,
		UpdatedDate, 
		UpdatedBy,
		MeterGuid
	)
	VALUES
	(
		@SiteGuid,
		@MeterID,
		@NumberOfDigits,
		@RotatesBackwardsFlag,
		@ReceiptMeterFlag,
		@MeterFactor,
		@FuelCompressionFactor,
		@DcuID,
		@DcuBatteryVoltage,
		@DcuBatteryCurrent,
		@DcuTemperature,
		@DcuResets,
		@DcuUpdateDate,
		@DcuConfigurationDate,
		@DcuFirmwareVersion,
		@DcuBluetoothAddress,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy,
		@MeterGuid
	)
END