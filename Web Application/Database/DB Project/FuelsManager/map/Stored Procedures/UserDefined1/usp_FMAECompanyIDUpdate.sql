
/*
=============================================
Author: Ryan Hill
Create date: 03/19/2013
Description:
	Update a translation for company records
	imported through the FMAE interface
=============================================
*/
CREATE PROCEDURE [map].[usp_FMAECompanyIDUpdate]
(
	@FMAECompanyIDMapGuid UNIQUEIDENTIFIER,
	@FMAECompanyID NVARCHAR(100),
	@CompanyGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE map.tblFMAECompanyID 
	SET
		FMAECompanyID =  @FMAECompanyID,
		CompanyGuid = @CompanyGuid,
		UpdatedDate = @UpdatedDate,
		UpdatedBy = @UpdatedBy
	WHERE FMAECompanyIDMapGuid = @FMAECompanyIDMapGuid
END