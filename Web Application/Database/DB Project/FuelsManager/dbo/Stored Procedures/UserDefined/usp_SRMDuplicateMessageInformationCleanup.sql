
/*
=============================================
Author: Ryan Hill
Create date: 9/7/12
Description:

Delete SRM duplicate message information that is older than a provided number of hours
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMDuplicateMessageInformationCleanup]
(
	@HoursOld INT
)
AS
BEGIN
	SET NOCOUNT ON

	SET @HoursOld = @HoursOld * -1

	DELETE
	FROM tblSRMDuplicateMessageInformation
	WHERE CreatedDate < DATEADD(HOUR, @HoursOld, SYSDATETIMEOFFSET())
END