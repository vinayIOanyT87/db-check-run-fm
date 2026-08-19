CREATE PROCEDURE [rpt].[usp_EquipmentConsumptionReport]
(
	@BeginDate DATETIME,
	@EndDate DATETIME,
	@Managers NVARCHAR(MAX),
	@Sites NVARCHAR(MAX),
	@Owners NVARCHAR(MAX),
	@Consumers NVARCHAR(MAX),
	@Product UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,		
	@UserGuid UNIQUEIDENTIFIER,
	@EnterpriseStatus BIT
)
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_EquipmentConsumptionReport]
	-- Author: Paul Carpenter
	-- Version/Date:
	-- 2015-Apr-28 TFS 49896 - Equipment Consumption Report
	-- Purpose: Retrieve the individual IntoPlane transaction records 
	-- Notes:
	-- 1. @BeginDate: Start day to filter the date of transactions in the results
	-- 2. @EndDate: End day to filter the date of transactions in the results
	-- 3. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list has its associated manager in to be included in the results.
	-- 4. @Vendors: List of company MasterRecordGuids assigned the role of carrier that the transactions list has its associated carrier in to be included in the results.
	-- 5. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list has its associated owner in to be included in the results.
	-- 6. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 7. @Consumers: List of company MasterRecordGuids assigned the role of ShipTo that the transactions list has its associated ShipTo in to be included in the results.
	-- 8. @Product: Single Product MasterRecordGuid that the transactions list has its associated product in to be included in the results
	-- 9. @SiteGuid: Site/SiteGroup that report is being executed at for the purpose of retrieving proper units and decimal places.
	--10. @UserGuid: Identifies the user running the report
    -- Updates:
    -- Date         Version     User        Description
    -- ----         -------     ----        -----------
    -- 11-01-2019   1.0.001     Jay R       Get DocumentNumber from Transaction and not from LineItems
	------------------------------------------------------------------------------------------------------
AS
BEGIN
	-- Don't hold onto DB pages, minimize impact on other users
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	-- Trim off passed in hours and minute, add seconds until end of day
	Set @EndDate = DATEADD(second,86399,DATEADD(dd, DATEDIFF(dd, 0,@EndDate), 0))

	DECLARE @SiteGroupLevelVolumeUnitIndex INT
	DECLARE @SiteGroupLevelVolumeDecimalPlaces INT
	DECLARE @SiteGroupLevelDensityUnitIndex INT
	DECLARE @SiteGroupLevelDensityDecimalPlaces INT

	SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
		@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0),
		@SiteGroupLevelDensityUnitIndex = ISNULL(DensityUnitIndex, 187),
		@SiteGroupLevelDensityDecimalPlaces = ISNULL(DensityDecimalPlaces, 0)
	FROM tblSites 
	WHERE SiteGuid = @SiteGuid

	SELECT 
	  t.InventoryDate
	 ,t.AliasName
 	 ,t.ManagerID
	 ,t.OwnerID
	 ,t.CarrierID 
	 ,t.[Site]
	 ,t.ShipToID AS ConsumerID
	 ,l.VCF
	 ,l.Temperature
	 ,ABS(dbo.udf_ConvertFromSIUnits(l.Density, @SiteGroupLevelDensityUnitIndex, @SiteGroupLevelDensityDecimalPlaces)) AS Density	 	 	 
	 ,l.Product
	 ,t.DocumentNumber as ShipmentNumber
	 ,ISNULL(t.DestinationRegistrationID1, '') as DestinationRegistrationID1
	 ,t.RoutingID
	 ,t.SourceRegistrationID1 as SourceRegistrationID1
	 ,t.DestinationEquipmentModel1
	 ,t.SourceEquipmentModel1
	 ,l.MeterStart
	 ,l.MeterStop 
	 ,dbo.udf_ConvertFromSIUnits(ABS(ISNULL(l.GrossQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity
	 ,dbo.udf_ConvertFromSIUnits(ABS(ISNULL(l.NetQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)  AS NetQuantity	 
	 ,1 AS FlightCnt
	 ,t.UpdatedDate AS ModifiedDate
	 ,t.DeleteFlag AS IsDeleted
	 ,mgr.EmergencyContact
	 ,ISNULL(mgr.Address1, '') + ' ' + ISNULL(mgr.Address2, '') AS Address
	 ,ISNULL( mgr.city,'') + ', ' + ISNULL( mgr.[State],'') + ' ' + isnull( mgr.Zip,'') + ' ' AS CityStateZip
	 ,@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces
	 ,@SiteGroupLevelDensityDecimalPlaces AS DensityDecimalPlaces
	 ,Eng.EngineeringUnitName AS VolumeUnitName
	FROM   tblTransactions t WITH(nolock)
	INNER JOIN tblTransactionLineItems l WITH(nolock) ON  t.transactionGuid=l.transactionGuid
	INNER JOIN tblCompanies mgr on mgr.CompanyGuid=t.ManagerCompanyGuid
	INNER JOIN lookup.tblEngineeringUnit Eng ON Eng.EngineeringUnitIndex = @SiteGroupLevelVolumeUnitIndex
	WHERE 1=1
		AND (t.DeleteFlag is NULL OR t.DeleteFlag = 0)
		AND InventoryDate BETWEEN @BeginDate AND @EndDate 
		AND LookupTransTypeIndex IN( 6,5,4) -- 'Bulk Issue','Defuel','Issue'
		AND ProductGuid          IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Product) c)	
		AND ManagerCompanyGuid	 IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Managers) c)
		AND t.SiteGuid			 IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
		AND ( ShipToCompanyGuid  IS NULL OR ShipToCompanyGuid	IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Consumers) c))
		AND ( OwnerCompanyGuid	 IS NULL OR OwnerCompanyGuid	IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Owners) c))		
		AND EXISTS (SELECT *
						FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
						WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
						OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid))
		AND t.LookupTransactionStatusIndex IN (select s.TransactionStatusIndex from [rpt].[udf_GetEnterpriseStatusTable](@EnterpriseStatus) s) 
		ORDER BY 
		  t.ManagerID
		, t.SourceRegistrationID1
		, ISNULL(t.DestinationRegistrationID1, '')
		, t.InventoryDate
		, t.AliasName

END