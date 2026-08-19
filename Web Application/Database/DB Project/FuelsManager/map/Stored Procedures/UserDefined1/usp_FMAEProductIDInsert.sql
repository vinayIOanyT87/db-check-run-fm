
/*
=============================================
Author: Ryan Hill
Create date: 03/19/2013
Description:
	Insert a translation for product records
	imported through the FMAE interface
=============================================
*/
CREATE PROCEDURE [map].[usp_FMAEProductIDInsert]
(
	@FMAEProductID NVARCHAR(30),
	@ProductGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@FMAEProductIDMapGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	SET @FMAEProductIDMapGuid = NEWID()

	INSERT INTO map.tblFMAEProductID
	(
		FMAEProductIDMapGuid,
		FMAEProductID,
		ProductGuid,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	)
	VALUES
	(
		@FMAEProductIDMapGuid,
		@FMAEProductID,
		@ProductGuid,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)
END