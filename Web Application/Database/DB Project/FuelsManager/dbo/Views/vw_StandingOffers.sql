
CREATE VIEW [dbo].[vw_StandingOffers]
AS
SELECT     
	StandingOfferGuid, 
	dbo.udf_StandingOfferID(StandingOfferGuid) AS ID, 
	SiteGuid, 
	SupplierCompanyGuid, 
	ProductGuid, 
	StandingOfferPrice, 
	EffectiveDate, 
    ExpirationDate,
    LocationIATAGuid, 
    CreatedBy, 
    CreatedDate, 
    UpdatedBy, 
    UpdatedDate,
    (SELECT ID			FROM  dbo.tblCompanies  WHERE (CompanyGuid = dbo.tblStandingOffers.SupplierCompanyGuid)) AS SupplierID,
    (SELECT  TOP 1 ProductID	FROM  dbo.tblProducts WHERE (ProductGuid = dbo.tblStandingOffers.ProductGuid)) AS ProductID,
    (SELECT  IATAID		FROM  dbo.tblIATA WHERE  (IATAGuid = dbo.tblStandingOffers.LocationIATAGuid)) AS LocationID,
    (SELECT  Name		FROM  dbo.tblIATA AS tblIATA_1 WHERE(IATAGuid = dbo.tblStandingOffers.LocationIATAGuid)) AS LocationName, 
    LowerBound, 
    UpperBound, 
    ReferenceNumber
FROM         dbo.tblStandingOffers;