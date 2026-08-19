CREATE PROCEDURE [dbo].[usp_GetMeterForeignKeyReferences]
(
	@MeterGuid uniqueidentifier
)
AS
BEGIN
BEGIN TRY
	SELECT DISTINCT EquipmentGuid AS PrimaryKeyGuid, 'dbo.tblEquipment' AS TableName FROM dbo.tblEquipment WHERE AssignedToMeterGuid = @MeterGuid
	UNION SELECT DISTINCT MeterToEquipmentGuid, 'map.tblMeterToEquipment' FROM map.tblMeterToEquipment WHERE MeterGuid = @MeterGuid
	UNION SELECT DISTINCT MeterToTankGuid, 'map.tblMeterToTank' FROM map.tblMeterToTank WHERE MeterGuid = @MeterGuid
	UNION SELECT DISTINCT ProductToOffloadExternalMeterGuid, 'map.tblProductToOffloadExternalMeter' FROM map.tblProductToOffloadExternalMeter WHERE AssignedToMeterGuid = @MeterGuid
	UNION SELECT DISTINCT ProductToPresetComponentTankOrTankGroupGuid,'map.tblProductToPresetComponentTankOrTankGroup' FROM map.tblProductToPresetComponentTankOrTankGroup WHERE AssignedToMeterGuid = @MeterGuid
	UNION SELECT DISTINCT ProductToPresetFlowControlledAdditiveGuid, 'map.tblProductToPresetFlowControlledAdditive' FROM map.tblProductToPresetFlowControlledAdditive WHERE AssignedToMeterGuid = @MeterGuid
	UNION SELECT DISTINCT ProductToPresetInjectorGuid, 'map.tblProductToPresetInjector' FROM map.tblProductToPresetInjector WHERE AssignedToMeterGuid = @MeterGuid
END TRY
BEGIN CATCH
	DECLARE	@_ErrMessage NVARCHAR(2048)
			,@_ErrNumber INT
			,@_ErrProcName NVARCHAR(126)
			,@_ErrLineNumber INT;
	SET @_ErrMessage = ERROR_MESSAGE();
	SET @_ErrNumber = ERROR_NUMBER();
	SET @_ErrProcName= ERROR_PROCEDURE();
	SET @_ErrLineNumber = ERROR_LINE();
	SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
				+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13) + CHAR(10)
				+ 'Procedure Name: [dbo].usp_IsMeterReferencedByForeignKey' + CHAR(13) + CHAR(10)
				+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)), '') + CHAR(13) + CHAR(10);
	RAISERROR(@_ErrMessage,18,1);
END CATCH
	
END
