CREATE Procedure [rpt].[usp_ConsumptionSummaryReport] 
(
	@Sites NVARCHAR(MAX),
	@Managers NVARCHAR(MAX),
	@Owners NVARCHAR(MAX),
	@Consumers NVARCHAR(MAX),
	@Vendors NVARCHAR(MAX),
	@Product UNIQUEIDENTIFIER,
	@FromDate DATETIMEOFFSET(7),
	@ToDate DATETIMEOFFSET(7),
	@TransAliases NVARCHAR(100),
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER,
	@EnterpriseStatus BIT
)
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_ConsumptionSummaryReport] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.005
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
	-- 10. @SiteGuid: Identifies the site the report is being run from
	-- 11. @UserGuid: Identifies the user running the report
    -- Updates:
    -- Date         Version     User        Description
    -- ----         -------     ----        -----------
    -- 10-09-2019   1.0.004     Jay R       Updated stored procedure to return specific product's volume decimal place
    --                                      GSE Consumption report required specific demial point value for some clients
    --                                      If a product's decimal value exists, it uses that, otherwise it defaults
    --                                      to the site's defined value.
    -- 11-03-2019   1.0.005     Jay R       Get DocumentNumber from Transaction and not from LineItems
	------------------------------------------------------------------------------------------------------
	BEGIN TRY
	SET NOCOUNT ON
	
		Set @ToDate = DATEADD(second,86399,DATEADD(dd, DATEDIFF(dd, 0,@ToDate), 0))

		DECLARE @SiteGroupLevelVolumeUnitIndex INT
		DECLARE @SiteGroupLevelVolumeDecimalPlaces INT
		DECLARE @SiteGroupLevelDensityUnitIndex INT
		DECLARE @SiteGroupLevelDensityDecimalPlaces INT
		DECLARE @SiteGroupLevelTemperatureUnitIndex INT
		DECLARE @SiteGroupLevelTemperatureDecimalPlaces INT

		SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
			@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0),
			@SiteGroupLevelDensityUnitIndex = ISNULL(DensityUnitIndex, 187),
			@SiteGroupLevelDensityDecimalPlaces = ISNULL(DensityDecimalPlaces, 0),
			@SiteGroupLevelTemperatureUnitIndex = ISNULL(TemperatureUnitIndex, 2),
			@SiteGroupLevelTemperatureDecimalPlaces = ISNULL(TemperatureDecimalPlaces, 1)
		FROM tblSites 
		WHERE SiteGuid = @SiteGuid

		SELECT 
			CAST(t.InventoryDate AS DATE) AS InventoryDate,
			t.LookupTransTypeIndex,		
			t.ReversalType,
			t.ShipToID AS Consumer, 
			l.Product AS Product,
			t.DocumentNumber AS MeterTicket,
			l.DestinationRegistrationID AS TailNum,
			l.DestinationEquipmentModel as DestEqType,
			t.NextStationIATAID as Destination,
			t.RoutingID AS FlightNum,
			t.SourceRegistrationID1 AS VehicleID,
			l.MeterStart AS Start,
			l.MeterStop AS Stop,
            (-1 * dbo.udf_ConvertFromSIUnits(ISNULL(l.GrossQuantity,0.0),
                @SiteGroupLevelVolumeUnitIndex,
                ISNULL(p.VolumeDecimalPlaces, @SiteGroupLevelVolumeDecimalPlaces))) AS Gross,
			(-1 * dbo.udf_ConvertFromSIUnits(ISNULL(l.NetQuantity, 0.0),
                @SiteGroupLevelVolumeUnitIndex,
                ISNULL(p.VolumeDecimalPlaces, @SiteGroupLevelVolumeDecimalPlaces))) AS Net,
			l.Vcf AS VCF,
			CASE WHEN l.Temperature IS NULL THEN NULL 
			ELSE 
				ABS(dbo.udf_ConvertFromSIUnits(ISNULL(l.Temperature, 0.0), @SiteGroupLevelTemperatureUnitIndex, @SiteGroupLevelTemperatureDecimalPlaces)) 
			END AS TankTemp,
			CASE WHEN l.Density IS NULL THEN NULL
			ELSE
				ABS(dbo.udf_ConvertFromSIUnits(ISNULL(l.Density, 0.0), @SiteGroupLevelDensityUnitIndex, @SiteGroupLevelDensityDecimalPlaces)) 
			END AS CorrectedAPIGravity,
			t.OwnerID AS Owner,
			t.CarrierID AS IntoPlaneAgent,
			t.ManagerID AS Manager,
			1 AS FltCnt,
			s.ID AS SiteID,
			s.Phone,
			s.Address1,
			s.Address2,
			CASE WHEN CONCAT(s.City, s.State, s.Zip) = '' THEN NULL 
				ELSE CONCAT(s.City, ', ', s.State, ' ', s.Zip) 
				END AS CityStateZip,
			s.Country,
			CASE t.LookupTransTypeIndex
				WHEN 4 then 'Defuel'
				WHEN 5 then 'Issue'
				WHEN 6 then 'Bulk Issue'
				ELSE 'Invalid' 
				END AS AliasName,
			lookup.tblEngineeringUnit.EngineeringUnitName AS VolumeUnitName,
			t.DeleteFlag,
			ISNULL(p.VolumeDecimalPlaces, @SiteGroupLevelVolumeDecimalPlaces) AS VolumeDecimalPlaces,
			@SiteGroupLevelDensityDecimalPlaces AS DensityDecimalPlaces,
			@SiteGroupLevelTemperatureDecimalPlaces AS TemperatureDecimalPlaces,
			tud.UserData2 AS SubType2
		FROM tblTransactionLineItems l
		INNER JOIN tblTransactions t ON t.TransactionGuid = l.TransactionGuid
		LEFT JOIN tblTransactionUserData tud ON l.TransactionGuid = tud.TransactionGuid 
		INNER JOIN tblSites s ON s.SiteGuid = t.SiteGuid
		INNER JOIN lookup.tblEngineeringUnit ON lookup.tblEngineeringUnit.EngineeringUnitIndex = @SiteGroupLevelVolumeUnitIndex
        INNER JOIN erv.udf_GetProductRecordVersions(@SiteGuid) rv ON rv.MasterRecordGuid = @Product
        INNER JOIN tblProducts p ON p.ProductGuid = rv.ProductGuid
		WHERE 
		    t.SiteGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
			AND t.ManagerCompanyGuid IN (SELECT m.Guid FROM rpt.udf_GetTableFromStringList(@Managers) m)			
			AND t.OwnerCompanyGuid IN (SELECT o.Guid FROM rpt.udf_GetTableFromStringList(@Owners) o)
			AND t.CarrierCompanyGuid IN (SELECT v.Guid FROM rpt.udf_GetTableFromStringList(@Vendors) v)
			AND t.ShipToCompanyGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Consumers) c)
			AND (t.DeleteFlag = CAST(0 AS BIT))
			AND t.InventoryDate >= @FromDate AND t.InventoryDate <= @ToDate			
			AND t.LookupTransTypeIndex IN (SELECT Num AS LookupTransTypeIndex FROM rpt.udf_GetIntTableFromStringList(@TransAliases))
			AND (t.ReversalType IS NULL OR t.ReversalType = '' OR t.ReversalType = 'O')
			AND l.ProductGuid = @Product
			AND t.LookupTransactionStatusIndex IN (select s.TransactionStatusIndex from [rpt].[udf_GetEnterpriseStatusTable](@EnterpriseStatus) s) 
			AND EXISTS (SELECT *
				FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
				WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
				OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) 
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
						+ 'Procedure Name: [rpt].usp_ConsumptionSummaryReport' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END
GO
