
/*
=============================================
Author: Ryan Hill
Create date: 4/24/12
Description:

Delete a record in the meter table by the primary key
=============================================
*/
CREATE PROCEDURE [dbo].[usp_MeterDelete]
(
	@MeterGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT OFF

	DELETE FROM tblMeter
	WHERE MeterGuid = @MeterGuid

END