
CREATE FUNCTION [dbo].[udf_GetCostCentreInfo]
(@TransTypeID INT, @SiteGuid UNIQUEIDENTIFIER, @CompanyGuid UNIQUEIDENTIFIER)
RETURNS 
    @CostCentreTable TABLE (
        [CostCentre]     NVARCHAR (60) NULL,
        [CostCentreDesc] NVARCHAR (60) NULL)
AS
BEGIN
	IF (@TransTypeID = 3) or (@TransTypeID = 4) or (@TransTypeID = 5) or  (@TransTypeID = 6)
	BEGIN
        INSERT INTO @CostCentreTable
		  SELECT UserData1 AS CostCentre, UserData2 AS CostCentreDesc
		  FROM (select ba.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) aa inner join tblCompanies ba on aa.CompanyGuid = ba.CompanyGuid) c where c.CompanyGuid = @CompanyGuid OR c._MasterRecordGuid = @CompanyGuid
	END
	ELSE IF (@TransTypeID = 8) -- receipts
	BEGIN
        INSERT INTO @CostCentreTable
		  SELECT UserData1 AS CostCentre, UserData2 AS CostCentreDesc
		  FROM dbo.tblSites WHERE SiteGuid = @SiteGuid
	END
	ELSE
	BEGIN
        INSERT INTO @CostCentreTable
		  SELECT '' AS CostCentre, '' AS CostCentreDesc
	END
    RETURN
END