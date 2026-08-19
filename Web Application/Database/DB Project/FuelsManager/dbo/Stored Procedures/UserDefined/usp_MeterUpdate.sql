
/*
=============================================
Author: Ryan Hill
Create date: 4/24/12
Description:

Update a meter with the information provided
=============================================
*/
CREATE PROCEDURE [dbo].[usp_MeterUpdate]
(
	@MeterGuid UNIQUEIDENTIFIER,
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
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID
)
AS
BEGIN
	SET NOCOUNT OFF

	UPDATE tblMeter
	SET
		SiteGuid = @SiteGuid,
		MeterID = @MeterID,
		NumberOfDigits = @NumberOfDigits,
		RotatesBackwardsFlag = @RotatesBackwardsFlag,
		ReceiptMeterFlag = @ReceiptMeterFlag, 
		MeterFactor = @MeterFactor,
		FuelCompressionFactor = @FuelCompressionFactor,
		DcuID = @DcuID,
		DcuBatteryVoltage = @DcuBatteryVoltage,
		DcuBatteryCurrent = @DcuBatteryCurrent,
		DcuTemperature = @DcuTemperature,
		DcuResets = @DcuResets,
		DcuUpdateDate = @DcuUpdateDate,
		DcuConfigurationDate = @DcuConfigurationDate,
		DcuFirmwareVersion = @DcuFirmwareVersion,
		DcuBluetoothAddress = @DcuBluetoothAddress,
		UpdatedDate = @UpdatedDate, 
		UpdatedBy = @UpdatedBy
	WHERE MeterGuid = @MeterGuid

END