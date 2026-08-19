

/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Update a Service Request Messaging Adaptor IATA Code to Site Mapping record
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorIATAToSiteUpdate]
(
	@SRMAdaptorIATAToSiteGuid UNIQUEIDENTIFIER,
	@SRMAdaptorGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@IATAGuid UNIQUEIDENTIFIER,
	@IsEnabled BIT,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE map.tblSRMAdaptorIATAToSite
	SET SRMAdaptorGuid = @SRMAdaptorGuid,
	    SiteGuid = @SiteGuid,
		IATAGuid = @IATAGuid,
		IsEnabled = @IsEnabled,
		UpdatedDate = @UpdatedDate,
		UpdatedBy = @UpdatedBy
	WHERE SRMAdaptorIATAToSiteGuid = @SRMAdaptorIATAToSiteGuid 

END