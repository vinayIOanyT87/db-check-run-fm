/*
=============================================
Author: John Aguirre
Create date: 10/18/2018
Description:
Stored procedure for use with Equipment Utilization SSRS Report
                
    Modification History:
    Date        Version     By          Description
    ----------  -------     ----        -------------
    10/18/2018  1.0.0.0     John A      Initial
    08/06/2019  1.0.0.1     Jay R       Added EnterpriseStatus parameter
=============================================
*/
CREATE PROCEDURE [rpt].[usp_MeterReconciliationReport] 
    @InventoryDate DATE
	,@SiteGuidStr UNIQUEIDENTIFIER
    ,@EnterpriseStatus BIT

AS
BEGIN
	SET NOCOUNT ON;

	WITH cteAllTransactionsForTheDay(TransactionGuid, meter, meterStart, meterStop, AliasName, GrossQuantity, Customer, FlightNumber, LookupTransTypeIndex) AS (
		SELECT t.TransactionGuid
			,tli.meterid AS meter
			,tli.meterStart
			,tli.meterStop
			,t.AliasName
			,CASE 
				WHEN t.LookupTransTypeIndex = 5
					THEN -- T5 PrimaryDisbursement
						dbo.udf_ConvertFromSIUnits(tli.GrossQuantity, s.VolumeUnitIndex, s.VolumeDecimalPlaces) * - 1
				ELSE dbo.udf_ConvertFromSIUnits(tli.GrossQuantity, s.VolumeUnitIndex, s.VolumeDecimalPlaces)
				END AS GrossQuantity
			,t.ShipToID AS Customer
			,t.RoutingID AS FlightNumber
			,t.LookupTransTypeIndex
		FROM tblTransactions t
		INNER JOIN tblTransactionLineItems tli ON t.TransactionGuid = tli.TransactionGuid
		INNER JOIN tblSites s ON t.SiteGuid = s.SiteGuid
		WHERE t.InventoryDate = @InventoryDate
			AND t.DeleteFlag != 1
			AND s.SiteGuid = @SiteGuidStr
			AND tli.MeterID IS NOT NULL
            AND t.LookupTransactionStatusIndex IN (SELECT s.TransactionStatusIndex FROM [rpt].[udf_GetEnterpriseStatusTable](@EnterpriseStatus) s)
		)

	SELECT m.meterID
		,t.TransactionGuid
		,t.meterStart
		,t.meterStop
		,t.AliasName
		,t.Customer
		,t.FlightNumber
		,t.GrossQuantity AS GrossQuantity
		,(
			SELECT SUM(a.GrossQuantity)
			FROM cteAllTransactionsForTheDay a
			WHERE a.AliasName != '24 Hour Closeout'
				AND a.meter = m.MeterID
			) AS TotalTickets
		,(
			SELECT TOP 1 b.MeterStart
			FROM cteAllTransactionsForTheDay b
			WHERE b.AliasName = '24 Hour Closeout'
				AND b.meter = m.MeterID
			ORDER BY b.meterStart
			) AS MeterStart24Hour
		,(
			SELECT TOP 1 c.meterStop
			FROM cteAllTransactionsForTheDay c
			WHERE c.AliasName = '24 Hour Closeout'
				AND c.meter = m.MeterID
			ORDER BY c.meterStop DESC
			) AS MeterStop24Hour
		,(
			SELECT COUNT(*)
			FROM cteAllTransactionsForTheDay d
			WHERE d.AliasName != '24 Hour Closeout'
				AND d.meter = m.MeterID
			) AS TotalEntries
		,CASE 
			WHEN t.AliasName IS NULL
				THEN 0
			ELSE 1
			END AS HasEntries
		,t.LookupTransTypeIndex
	FROM tblMeter m
	LEFT JOIN cteAllTransactionsForTheDay t ON m.meterID = t.meter
	WHERE m.SiteGuid = @SiteGuidStr
	ORDER BY m.MeterID
		,meterStart
END
GO
