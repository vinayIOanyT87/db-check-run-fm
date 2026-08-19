CREATE PROCEDURE dbo.usp_FuelCardLimitEnumerateForMobile (@SiteGuid AS UNIQUEIDENTIFIER)
AS
BEGIN
	SELECT fcl.ID + '_' + cast(ROW_NUMBER() OVER (
				ORDER BY fc.ID
					,p.ProductID
					,lp.FuelCardLimitPeriodName
				) AS VARCHAR(10)) AS FuelCardLimitId
		,fcl.FuelCardLimitGuid
		,fcli.limit * 1000 AS LimitAmount
		,p.ProductID
		,lp.FuelCardLimitPeriodName AS LimitPeriod
		,fc.ID AS FuelCardId
	FROM tblFuelCardLimit fcl
	INNER JOIN tblFuelCardLimitLineItem fcli ON fcl.FuelCardLimitGuid = fcli.FuelCardLimitGuid
	INNER JOIN lookup.tblFuelCardLimitPeriod lp ON lp.FuelCardLimitPeriodIndex = fcli.Period
	INNER JOIN map.tblFuelCardLimitToFuelCard fcmap ON fcmap.FuelCardLimitGuid = fcl.FuelCardLimitGuid
	INNER JOIN map.tblEntityFuelCardLimitToSite fcls ON fcls.FuelCardLimitGuid = fcl.FuelCardLimitGuid
	INNER JOIN tblFuelCards fc ON fc.FuelCardGuid = fcmap.FuelCardGuid
	INNER JOIN map.tblEntityFuelCardToSite fcs ON fcs.FuelCardGuid = fc.FuelCardGuid
	INNER JOIN tblProducts p ON p.ProductGuid = fcli.ProductGuid
	WHERE fcls.SiteGuid = @SiteGuid
		AND fcs.SiteGuid = @SiteGuid
	ORDER BY fc.ID
		,ProductID
		,LimitPeriod
END