
/*
=============================================
Author: Ryan Hill
Create date: 7/30/12
Description:

Delete old messages from tblSRMMessage.
Messages with a flight origination date older than the specified number of days
are removed.
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMMessageCleanup]
(
	@DaysOld INT
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Now DATETIMEOFFSET 
	SET @Now = SYSDATETIMEOFFSET()

	DELETE FROM tblSRMMessage
	WHERE DATEDIFF(DAY, FlightOriginationDate, @Now) > @DaysOld
	
END