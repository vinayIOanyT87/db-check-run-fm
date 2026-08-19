

/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Create a Service Request Messaging Adaptor IATA Code to Site Mapping record
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorIATAToSiteInsert]
(
	@SRMAdaptorGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@IATAGuid UNIQUEIDENTIFIER,
	@IsEnabled BIT,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@SRMAdaptorIATAToSiteGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	SET @SRMAdaptorIATAToSiteGuid = NEWID()

	INSERT INTO map.tblSRMAdaptorIATAToSite
	(
		SRMAdaptorIATAToSiteGuid,
		SRMAdaptorGuid,
		SiteGuid,
		IATAGuid,
		IsEnabled,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	)
	VALUES
	(
		@SRMAdaptorIATAToSiteGuid,
		@SRMAdaptorGuid,
		@SiteGuid,
		@IATAGuid,
		@IsEnabled,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)
END