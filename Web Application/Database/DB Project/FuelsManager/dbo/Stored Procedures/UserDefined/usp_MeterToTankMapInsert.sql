
/*
=============================================
Author: Ryan Hill
Create date: 4/24/12
Description:

Create a record in map.MeterToTank to indicate a relationship
between a meter and a tank
=============================================
*/
CREATE PROCEDURE [dbo].[usp_MeterToTankMapInsert]
(
	@MeterGuid UNIQUEIDENTIFIER,
	@TankGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID
)
AS
BEGIN
	SET NOCOUNT OFF

	INSERT INTO map.tblMeterToTank
	(
		TankGuid,
		MeterGuid,
		CreatedDate,
		CreatedBy,
		UpdatedDate, 
		UpdatedBy
	)
	VALUES
	(
		@TankGuid,
		@MeterGuid,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)

END