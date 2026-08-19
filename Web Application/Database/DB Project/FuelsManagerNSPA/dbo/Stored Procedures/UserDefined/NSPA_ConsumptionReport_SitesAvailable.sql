CREATE PROCEDURE dbo.NSPA_ConsumptionReport_SitesAvailable
(
	@SiteID NVARCHAR(60),
	@Label	NVARCHAR(60)
)
AS
BEGIN
SET NOCOUNT ON

DECLARE @SiteTable TABLE
(SiteID NVARCHAR(60), Label NVARCHAR(60), Period DATETIME)

INSERT INTO @SiteTable
select * from dbo.NSPA_ConsumptionReport_MonthParam(@SiteID)

SELECT SiteID FROM @SiteTable WHERE Label = @Label

END