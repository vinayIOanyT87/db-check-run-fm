
/*
=============================================
Author: Ryan Hill
Create date: 03/19/2013
Description:
	Delete a translation for product records
	imported through the FMAE interface
=============================================
*/
CREATE PROCEDURE [map].[usp_FMAEProductIDDelete]
(
	@FMAEProductIDMapGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	DELETE FROM map.tblFMAEProductID
	WHERE FMAEProductIDMapGuid = @FMAEProductIDMapGuid
END