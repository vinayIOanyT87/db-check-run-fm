
/*
=============================================
Author: Ryan Hill
Create date: 03/19/2013
Description:
	Update a translation for product records
	imported through the FMAE interface
=============================================
*/
CREATE PROCEDURE [map].[usp_FMAEProductIDUpdate]
(
	@FMAEProductIDMapGuid UNIQUEIDENTIFIER,
	@FMAEProductID NVARCHAR(30),
	@ProductGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE map.tblFMAEProductID 
	SET
		FMAEProductID =  @FMAEProductID,
		ProductGuid = @ProductGuid,
		UpdatedDate = @UpdatedDate,
		UpdatedBy = @UpdatedBy
	WHERE FMAEProductIDMapGuid = @FMAEProductIDMapGuid
END