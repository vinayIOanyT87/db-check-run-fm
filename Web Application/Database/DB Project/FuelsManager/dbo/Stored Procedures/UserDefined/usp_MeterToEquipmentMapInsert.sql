/*
=============================================
Author: Gregory Lybanon
Create date: 8/23/2018
Description:

Create a record in map.MeterToEquipment to indicate a relationship
between a meter and Equipment
=============================================
*/
CREATE PROCEDURE [dbo].[usp_MeterToEquipmentMapInsert]
(
	@MeterGuid UNIQUEIDENTIFIER,
	@EquipmentGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID
)
AS
BEGIN
	SET NOCOUNT OFF

	INSERT INTO map.tblMeterToEquipment
	(
		MeterGuid,
		EquipmentGuid,
		CreatedDate,
		CreatedBy,
		UpdatedDate, 
		UpdatedBy
	)
	VALUES
	(
		@MeterGuid,
		@EquipmentGuid,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)

END
GO


