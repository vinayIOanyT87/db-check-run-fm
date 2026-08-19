
CREATE FUNCTION [dbo].[udf_BuildStandingOfferID]
(	@siteGuid UNIQUEIDENTIFIER, 
	@supplierCompanyGuid UNIQUEIDENTIFIER, 
	@productGuid UNIQUEIDENTIFIER, 
	@locationIATAGuid UNIQUEIDENTIFIER, 
	@effectiveDate DATETIMEOFFSET(7), 
	@expriationDate DATETIMEOFFSET(7), 
	@lowerBound INT,
	@upperBound INT)
RETURNS NVARCHAR (150)
AS
BEGIN
	DECLARE @siteID     nvarchar(50);
	DECLARE @supplierID nvarchar(50);
	DECLARE @productID  nvarchar(50);
	DECLARE @locationID nvarchar(50);

	SELECT @siteID     = ID        FROM dbo.tblSites     WHERE siteGuid    = @siteGuid;
	SELECT @supplierID = ID        FROM (select ba.* from erv.udf_GetCompanyRecordVersions (@siteGuid) aa inner join tblCompanies ba on aa.CompanyGuid = ba.CompanyGuid) c WHERE c.CompanyGuid = @supplierCompanyGuid OR c._MasterRecordGuid = @supplierCompanyGuid;
	SELECT @productID  = (Select top 1 ProductID FROM dbo.tblProducts  WHERE productGuid = @productGuid);
	SELECT @locationID = IATAID    FROM dbo.tblIATA      WHERE IATAGuid    = @locationIATAGuid;
	
	IF (@lowerBound IS NULL)
	BEGIN
	   SELECT @lowerBound = 0 
	END

	IF (@upperBound IS NULL)
	BEGIN
	   SELECT @upperBound = 0 
	END
 
	RETURN 'ID=(' + ISNULL(@siteID,' ')    + 
			', ' + ISNULL(@supplierID,' ')    + 
			', ' + ISNULL(@productID,' ') +
			', ' + ISNULL(@locationID,' ') + 
			', ' + CONVERT(nvarchar, @effectiveDate, 112) + 
			', ' + CONVERT(nvarchar, @expriationDate, 112) + 
			', ' + CONVERT(nvarchar, @lowerBound) +
			', ' + CONVERT(nvarchar, @upperBound) +
			')';

END