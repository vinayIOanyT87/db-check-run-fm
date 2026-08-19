
CREATE PROCEDURE [rpt].[usp_IntoPlaneDailyReport]
(
		@BeginDate datetimeoffset(7),
		@Managers nvarchar(max),
		@Vendors nvarchar(max), 
		@Owners nvarchar(max), 
		@Sites nvarchar(max),
		@Consumers nvarchar(max),
		@Product uniqueidentifier,
		@SiteGuid uniqueidentifier,
		@UserGuid UNIQUEIDENTIFIER,
		@EnterpriseStatus BIT
)
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[sp_IntoPlaneDailyReport]
	-- Author: Paul Carpenter
	-- Version/Date:
	-- 2015-Apr-06 TFS 49861, 49862, 50466 Create report and suporting SQL for Into Plane Daily Report
	-- Purpose: Retrieve the individual IntoPlane transaction records 
	-- Notes:
	-- 1. @BeginDate: Single day to filter the date of transactions in the results
	-- 2. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list has its associated manager in to be included in the results.
	-- 3. @Vendors: List of company MasterRecordGuids assigned the role of carrier that the transactions list has its associated carrier in to be included in the results.
	-- 4. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list has its associated owner in to be included in the results.
	-- 5. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 6. @Consumers: List of company MasterRecordGuids assigned the role of ShipTo that the transactions list has its associated ShipTo in to be included in the results.
	-- 7. @Product: Product MasterRecordGuids that the transactions list has its associated product in to be included in the results
	-- 8. @SiteGuid: Site/SiteGroup that report is being executed at for the purpose of retrieving proper units and decimal places.
	-- 9. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	
	DECLARE @SiteGroupLevelVolumeUnitIndex INT
	DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

	SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
		@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
	FROM tblSites 
	WHERE SiteGuid = @SiteGuid

	SELECT 
		CAST(t.InventoryDate AS DATE) AS InventoryDate
		,t.AliasName
		,t.Site
		,t.ManagerID
		,t.OwnerID
		,t.CarrierID
		,t.ShipToID as Consumer
		,l.Product
		,l.DocumentNumber as ShipmentNumber
		,l.DestinationRegistrationID
		,t.RoutingID
		,l.SourceRegistrationID
		,l.MeterStart
		,l.MeterStop
		,l.DestinationEquipmentModel
		,l.SourceEquipmentModel
		,CASE t.LookupTransTypeIndex
			WHEN 4 then (-1 * (dbo.udf_ConvertFromSIUnits(ABS(isnull(l.GrossQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces))) -- WHEN T4_SecondaryDefuel
			ELSE dbo.udf_ConvertFromSIUnits(ABS(isnull(l.GrossQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)
			END AS GrossQuantity
		,CASE t.LookupTransTypeIndex
			WHEN 4 then (-1 * (dbo.udf_ConvertFromSIUnits(ABS(isnull(l.NetQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces))) -- WHEN T4_SecondaryDefuel
			ELSE  dbo.udf_ConvertFromSIUnits(ABS(isnull(l.NetQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)
			END AS NetQuantity
		,CASE IsNull(t.InternationalRouteIndicator,0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END as InternationalRouteIndicator
		,t.NextStationIATAID as NextStation
		,1 as FlightCnt 
		,@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces
		,t.DeleteFlag
		,lookup.tblEngineeringUnit.EngineeringUnitName AS VolumeUnitName
	FROM tblTransactions t
	INNER JOIN tblSites s ON s.SiteGuid = t.SiteGuid
	INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
	INNER JOIN lookup.tblEngineeringUnit ON lookup.tblEngineeringUnit.EngineeringUnitIndex = @SiteGroupLevelVolumeUnitIndex

	WHERE 
	    t.InventoryDate = @BeginDate 
		AND ProductGuid	= @Product
		AND ManagerCompanyGuid	in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Managers) c)
		AND t.SiteGuid			in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
		AND ( CarrierCompanyGuid is null OR CarrierCompanyGuid	in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Vendors) c))
		AND ( ShipToCompanyGuid  is null OR ShipToCompanyGuid	in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Consumers) c))
		AND ( OwnerCompanyGuid	 is null OR OwnerCompanyGuid	in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Owners) c))
		AND t.DeleteFlag = CAST(0 AS bit)
		AND l.DeleteFlag = CAST(0 AS bit) 		
		AND t.LookupTransactionStatusIndex IN (select s.TransactionStatusIndex from [rpt].[udf_GetEnterpriseStatusTable](@EnterpriseStatus) s) 
		--AND t.AliasName IN ('Bulk Issue','Defuel','Issue') 
		AND t.LookupTransTypeIndex in(6,5,4) -- T5_PrimaryDisbursement, T4_SecondaryDefuel			
		AND EXISTS (SELECT *
						FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
						WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
						OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid))
	ORDER BY
		t.ManagerID
		,t.OwnerId
		,CASE IsNull(t.InternationalRouteIndicator,0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END
		,t.ShipToID
		,t.InventoryDate
		,t.AliasName
END