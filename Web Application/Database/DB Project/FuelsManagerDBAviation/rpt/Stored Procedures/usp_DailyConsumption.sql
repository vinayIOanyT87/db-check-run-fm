
CREATE PROCEDURE [rpt].[usp_DailyConsumption] 
(
	@Sites NVARCHAR(MAX),
	@Managers NVARCHAR(MAX),
	@Owners NVARCHAR(MAX),
	@Consumers NVARCHAR(MAX),
	@Vendors NVARCHAR(MAX),
	@Product UNIQUEIDENTIFIER,
	@FromDate DATETIMEOFFSET(7),
	@ToDate DATETIMEOFFSET(7),
	@TransAliases NVARCHAR(100)
)

AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_DailyConsumption] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Daily Consumption Report
	-- Notes:
	-- 1. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 2. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	-- 3. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 4. @Consumers: List of company MasterRecordGuids assigned the role of consumer that the transactions list as the consumer for itself to be included in the results
	-- 5. @Vendors: List of company MasterRecordGuids assigned the role of vendor that the transactions list as the vendor for itself to be included in the results
	-- 6. @Product: Product MasterRecordGuid that the transactions list as the product for itself to be included in the results
	-- 7. @FromDate: Lower bound date to collect transactions meeting criteria
	-- 8. @ToDate: Upper bound date to collect transactions meeting criteria
	-- 9. @TransAliases: List of comma separated TransactionTypes
	------------------------------------------------------------------------------------------------------
	BEGIN TRY
		SET NOCOUNT ON

		Set @ToDate = DATEADD(second,86399,DATEADD(dd, DATEDIFF(dd, 0,@ToDate), 0))
		
		SELECT CAST(t.InventoryDate AS DATE) AS InventoryDate,t.ShipToID AS Consumer, l.Product AS Product,
			l.DocumentNumber AS MeterTicket,
			l.DestinationRegistrationID AS TailNum,
			t.RoutingID AS FlightNum,
			t.SourceRegistrationID1 AS VehicleID,
			l.MeterStart AS Start,
			l.MeterStop AS Stop,
			CASE t.LookupTransTypeIndex
				WHEN 4 THEN (-1 * (dbo.udf_ConvertFromSIUnits(ABS(l.GrossQuantity), tblSites.VolumeUnitIndex, tblSites.VolumeDecimalPlaces))) -- WHEN T4_SecondaryDefuel
				ELSE dbo.udf_ConvertFromSIUnits(ABS(l.GrossQuantity), tblSites.VolumeUnitIndex, tblSites.VolumeDecimalPlaces) 
				END AS Gross,
			CASE t.LookupTransTypeIndex
				WHEN 4 THEN (-1 * (dbo.udf_ConvertFromSIUnits(ABS(l.NetQuantity), tblSites.VolumeUnitIndex, tblSites.VolumeDecimalPlaces))) -- WHEN T4_SecondaryDefuel
				ELSE dbo.udf_ConvertFromSIUnits(ABS(l.NetQuantity), tblSites.VolumeUnitIndex, tblSites.VolumeDecimalPlaces) 
				END AS Net,
			l.Vcf AS VCF,
			ABS(dbo.udf_ConvertFromSIUnits(l.Temperature, tblSites.TemperatureUnitIndex, tblSites.TemperatureDecimalPlaces)) AS TankTemp,
			ABS(dbo.udf_ConvertFromSIUnits(l.Density, tblSites.DensityUnitIndex, tblSites.DensityDecimalPlaces)) AS CorrectedAPIGravity,
			t.OwnerID AS Owner,
			t.CarrierID AS IntoPlaneAgent,
			1 AS FltCnt
		FROM tblTransactionLineItems l
		INNER JOIN tblTransactions t ON t.TransactionGuid = l.TransactionGuid
		INNER JOIN tblSites ON tblSites.SiteGuid = t.SiteGuid
		WHERE t.InventoryDate >= @FromDate AND t.InventoryDate <= @ToDate
			AND t.LookupTransTypeIndex IN (SELECT Num AS LookupTransTypeIndex FROM rpt.udf_GetIntTableFromStringList(@TransAliases))
			AND t.OwnerCompanyGuid IN (SELECT o.Guid FROM rpt.udf_GetTableFromStringList(@Owners) o)
			AND t.ManagerCompanyGuid IN (SELECT m.Guid FROM rpt.udf_GetTableFromStringList(@Managers) m)
			AND l.ProductGuid = @Product
			AND t.SiteGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
			AND t.CarrierCompanyGuid IN (SELECT v.Guid FROM rpt.udf_GetTableFromStringList(@Vendors) v)
			AND t.ShipToCompanyGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Consumers) c)
			AND t.DeleteFlag = CAST(0 AS BIT) 
			AND (t.ReversalType IS NULL OR t.ReversalType = '' OR t.ReversalType = 'O')  
		ORDER BY Consumer, Product, InventoryDate

	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [rpt].usp_DailyConsumption' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END