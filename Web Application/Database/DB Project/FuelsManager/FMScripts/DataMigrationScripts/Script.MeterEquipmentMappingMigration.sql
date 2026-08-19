/*
Meter-Equipment Mapping Migration Script							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Meter-Equipment mapping is being moved to a new location.  Instead of using the 
 AssignedToMeterGuid field of tblEquipment, the data will now be stored in 
 [map].[tblMeterToEquipment].  This will allow multiple meters to be mapped to a 
 single piece of equipment.
 
 Do not deprecate the field [dbo].[tblEquipment].[AssignedToMeterGuid] until all 
 programs have adopted this change.
--------------------------------------------------------------------------------------
*/

/*
************************************
	DATA INSERT SECTION:
************************************
*/

IF EXISTS (SELECT * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tblMeterToEquipment' AND TABLE_SCHEMA = 'map' )
BEGIN
	--PRINT 'TABLE EXISTS'
	DECLARE @records BIGINT
	SELECT @records = COUNT(*) from [map].[tblMeterToEquipment]
	IF(@records = 0)
	BEGIN
		--PRINT 'EMPTY'
		INSERT INTO [map].[tblMeterToEquipment] 
		(	[MeterGuid],
			[EquipmentGuid],
			[CreatedDate], 
			[CreatedBy], 
			[UpdatedDate],
			[UpdatedBy])
		(SELECT 
		AssignedToMeterGuid, 
		EquipmentGuid, 
		SYSDATETIMEOFFSET(),
		'administrator', --'enterprise'
		SYSDATETIMEOFFSET(),
		'administrator' --'enterprise'
		FROM [dbo].tblEquipment 
		WHERE AssignedToMeterGuid IS NOT NULL)
	END

END

GO