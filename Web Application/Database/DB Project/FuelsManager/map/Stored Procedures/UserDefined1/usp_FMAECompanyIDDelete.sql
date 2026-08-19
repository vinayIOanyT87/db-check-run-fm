
/*
=============================================
Author: Ryan Hill
Create date: 03/19/2013
Description:
	Delete a translation for company records
	imported through the FMAE interface
=============================================
*/
CREATE PROCEDURE [map].[usp_FMAECompanyIDDelete]
(
	@FMAECompanyIDMapGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	DELETE FROM map.tblFMAECompanyID
	WHERE FMAECompanyIDMapGuid = @FMAECompanyIDMapGuid
END