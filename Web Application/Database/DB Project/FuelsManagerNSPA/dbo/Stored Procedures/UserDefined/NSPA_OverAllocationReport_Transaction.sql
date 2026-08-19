CREATE PROCEDURE dbo.NSPA_OverAllocationReport_Transaction
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
	Installation		NVARCHAR(60),
	CardNumber			NVARCHAR(60),
	Customer			NVARCHAR(60),
	EquipmentNumber		NVARCHAR(60),
	EquipmentType		NVARCHAR(60),
	BillTo				NVARCHAR(60),
	TransactionLimit	FLOAT,
	QuantityDelivered	FLOAT
)

DECLARE @SiteGroupFlag BIT
SELECT @SiteGroupFlag = SiteGroupFlag FROM tblSites WHERE ID = @ViewingSiteID
IF (@SiteGroupFlag = 1)
BEGIN
	INSERT INTO @ResultTable
	SELECT	t.[Site],
			f.ID,
			(SELECT ID FROM tblCompanies WHERE CompanyGuid = f.ShipToCompanyGuid),
			t.DestinationRegistrationID1,
			t.DestinationEquipmentType1,
			(SELECT ID FROM tblCompanies WHERE CompanyGuid = f.BillToCompanyGuid),
			dbo.udf_ConvertFromSIUnits(ISNULL(a.AllocationLimit, 0), @VolumeUnits, @VolumeDecimalPlaces),
			-dbo.udf_ConvertFromSIUnits(ISNULL(a.QuantityDelivered, 0), @VolumeUnits, @VolumeDecimalPlaces)
	FROM	dbo.NSPA_OverAllocation_Transaction(@StartDate, @EndDate) a INNER JOIN tblFuelCards f ON f.FuelCardGuid = a.FuelCardGuid
																		INNER JOIN tblTransactions t ON t.TransactionGuid = a.TransactionGuid

	-- return results
	SELECT * FROM @ResultTable
END -- IF (@SiteGroupFlag)

END -- CREATE PROCEDURE