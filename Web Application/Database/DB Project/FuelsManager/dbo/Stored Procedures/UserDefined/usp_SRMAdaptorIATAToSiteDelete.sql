
/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Delete a Service Request Messaging Adaptor IATA Code to Site Mapping record
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorIATAToSiteDelete]
(
	@SRMAdaptorIATAToSiteGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	DELETE FROM map.tblSRMAdaptorIATAToSite
	WHERE SRMAdaptorIATAToSiteGuid = @SRMAdaptorIATAToSiteGuid

END