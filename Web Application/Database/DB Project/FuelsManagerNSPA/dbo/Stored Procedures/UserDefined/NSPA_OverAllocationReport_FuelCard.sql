CREATE PROCEDURE dbo.NSPA_OverAllocationReport_FuelCard
(
	@ViewingSiteID	NVARCHAR(60),
	@StartDate		DATETIME,
	@EndDate		DATETIME
)
AS
BEGIN
SET NOCOUNT ON

DECLARE @VolumeUnits int
SET		@VolumeUnits = (SELECT tblSites.VolumeUnitIndex FROM tblSites WHERE tblSites.ID = @ViewingSiteID)

DECLARE @VolumeDecimalPlaces int
SET		@VolumeDecimalPlaces = (SELECT tblSites.VolumeDecimalPlaces FROM tblSites WHERE tblSites.ID = @ViewingSiteID)

DECLARE @ResultTable TABLE
(
	CardNumber			NVARCHAR(60),
	Customer			NVARCHAR(60),
	BillTo				NVARCHAR(60),
	AllocationType		NVARCHAR(12),
	AllocationLimit		FLOAT,
	QuantityDelivered	FLOAT
)

DECLARE @SiteGroupFlag BIT
SELECT @SiteGroupFlag = SiteGroupFlag FROM tblSites WHERE ID = @ViewingSiteID
IF (@SiteGroupFlag = 1)
BEGIN

	-- put results into results table
	INSERT INTO @ResultTable
	SELECT	f.ID,
			(SELECT ID FROM tblCompanies WHERE CompanyGuid = f.ShipToCompanyGuid),
			(SELECT ID FROM tblCompanies WHERE CompanyGuid = f.BillToCompanyGuid),
			'Transaction',
			a.AllocationLimit,
			a.QuantityDelivered
	FROM	dbo.NSPA_OverAllocation_Transaction(@StartDate, @EndDate) a INNER JOIN tblFuelCards f ON f.FuelCardGuid = a.FuelCardGuid

	INSERT INTO @ResultTable
	SELECT	f.ID,
			(SELECT ID FROM tblCompanies WHERE CompanyGuid = f.ShipToCompanyGuid),
			(SELECT ID FROM tblCompanies WHERE CompanyGuid = f.BillToCompanyGuid),
			'Monthly',
			a.AllocationLimit,
			a.QuantityDelivered
	FROM	dbo.NSPA_OverAllocation_Monthly(@StartDate, @EndDate) a INNER JOIN tblFuelCards f ON f.FuelCardGuid = a.FuelCardGuid

	INSERT INTO @ResultTable
	SELECT	f.ID,
			(SELECT ID FROM tblCompanies WHERE CompanyGuid = f.ShipToCompanyGuid),
			(SELECT ID FROM tblCompanies WHERE CompanyGuid = f.BillToCompanyGuid),
			'Daily',
			a.AllocationLimit,
			a.QuantityDelivered
	FROM	dbo.NSPA_OverAllocation_Daily(@StartDate, @EndDate) a INNER JOIN tblFuelCards f ON f.FuelCardGuid = a.FuelCardGuid

	-- Return the aggregated results
	SELECT	CardNumber,
			Customer,
			BillTo,
			AllocationType,
			dbo.udf_ConvertFromSIUnits(MIN(ISNULL(AllocationLimit,0)), @VolumeUnits, @VolumeDecimalPlaces) AS AllocationLimit,
			-dbo.udf_ConvertFromSIUnits(SUM(ISNULL(QuantityDelivered,0)), @VolumeUnits, @VolumeDecimalPlaces) AS QuantityDelivered
	FROM	@ResultTable
	GROUP BY CardNumber, Customer, BillTo, AllocationType
	ORDER BY SUM(ISNULL(AllocationLimit,0)) - -SUM(ISNULL(QuantityDelivered,0)), CardNumber, Customer, BillTo, AllocationType
END -- IF (@SiteGroupFlag)
END -- CREATE PROCEDURE