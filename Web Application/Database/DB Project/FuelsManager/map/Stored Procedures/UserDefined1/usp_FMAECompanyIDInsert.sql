
/*
=============================================
Author: Ryan Hill
Create date: 03/19/2013
Description:
	Insert a translation for company records
	imported through the FMAE interface
=============================================
*/
CREATE PROCEDURE [map].[usp_FMAECompanyIDInsert]
(
	@FMAECompanyID NVARCHAR(100),
	@CompanyGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@FMAECompanyIDMapGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	SET @FMAECompanyIDMapGuid = NEWID()

	INSERT INTO map.tblFMAECompanyID
	(
		FMAECompanyIDMapGuid,
		FMAECompanyID,
		CompanyGuid,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	)
	VALUES
	(
		@FMAECompanyIDMapGuid,
		@FMAECompanyID,
		@CompanyGuid,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)
END