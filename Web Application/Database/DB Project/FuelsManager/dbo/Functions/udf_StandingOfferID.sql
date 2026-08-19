

CREATE FUNCTION [dbo].[udf_StandingOfferID]
(@StandingOfferGuid UNIQUEIDENTIFIER)
RETURNS NVARCHAR (150)
AS
BEGIN
	DECLARE @ID nvarchar(150);

	SELECT @ID = dbo.udf_BuildStandingOfferID(	siteGuid, 
										supplierCompanyGuid, 
										productGuid, 
										locationIATAGuid, 
										effectiveDate, 
										expirationDate,
										lowerBound,
										upperBound) 
	FROM dbo.tblStandingOffers 
	WHERE standingOfferGuid = @StandingOfferGuid;
 
	RETURN @ID;

END