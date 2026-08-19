/*
=============================================
Author: George Peters
Create date: 3/25/14
Description:

Associate a Fuel Card to a piece of Equipment
=============================================
*/
CREATE PROCEDURE [dbo].[usp_AssignFuelCardToEquipment]
(
	@EquipmentGuid UNIQUEIDENTIFIER,
	@FuelCardGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT OFF

	IF (@EquipmentGuid IS NOT NULL AND @FuelCardGuid IS NOT NULL)
	BEGIN
		-- Currently FuelsManager only allows a single Fuel Card to be associated with a piece of equipment.  This should be changed in the
		-- future which would convert this stored procedure into an Insert on an intersection table.
		UPDATE [dbo].[tblEquipment] SET [FuelCardGuid] = @FuelCardGuid WHERE [EquipmentGuid] = @EquipmentGuid;
	END
END