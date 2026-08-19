
CREATE Procedure [rpt].[usp_QueryIssueOrDefuelReport] 
(
	  @SiteGuid uniqueidentifier
	, @UserGuid uniqueidentifier
	, @BeginDate datetime 
	, @EndDate datetime 
	, @Sites nvarchar(max) 
	, @Managers nvarchar(max) 
	, @Owners nvarchar(max) 
	, @Vendors nvarchar(max) 
	, @Consumers nvarchar(max) 
	, @Product  nvarchar(max)
	, @IsDefuel BIT 
	, @TicketNum nvarchar(50) 
	, @FlightNum nvarchar(50) 
	, @IsInternational BIT 
	, @TailNum nvarchar(50) 
	, @AircraftType nvarchar(50) 
	, @Gross float 
	, @Net float 
	, @VehicleID nvarchar(50) 
	, @Notes nvarchar(max)
)
AS
BEGIN

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

Declare @AliasGuid uniqueidentifier;
Declare @SourceEquipmentID nvarchar(50);
Declare @DestinationEquipmentID nvarchar(50);

Set @SourceEquipmentID = @VehicleID
Set @DestinationEquipmentID = @TailNum


if(@IsDefuel = 0)
BEGIN
Set @AliasGuid = (Select top 1 TransactionAliasGuid from tblTransactionAliases where _MasterRecordGuid = TransactionAliasGuid and AliasName = 'Issue')
--Set @SourceEquipmentID = @VehicleID
--Set @DestinationEquipmentID = @TailNum
END
else
BEGIN
Set @AliasGuid = (Select top 1 TransactionAliasGuid from tblTransactionAliases where _MasterRecordGuid = TransactionAliasGuid and AliasName = 'Defuel')
--Set @SourceEquipmentID = @TailNum
--Set @DestinationEquipmentID = @VehicleID
END

if @TicketNum <> ''
	SET @TicketNum = '%' + @TicketNum + '%'

if @FlightNum <> ''
	SET @FlightNum = '%' + @FlightNum + '%'
 
if @SourceEquipmentID <> ''
	SET @SourceEquipmentID = '%' + @SourceEquipmentID + '%'

if @DestinationEquipmentID <> ''
	SET @DestinationEquipmentID = '%' + @DestinationEquipmentID + '%'

if @AircraftType <> ''
	SET @AircraftType = '%' + @AircraftType + '%'

if @Notes <> ''
	SET @Notes = '%' + @Notes + '%'

DECLARE @SiteGroupLevelVolumeUnitIndex INT
DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
	@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
FROM tblSites 
WHERE SiteGuid = @SiteGuid

SELECT	
	 InventoryDate 
	,Site
	,t.AliasName
	,t.DocumentNumber as ShipmentNumber 
	,Product  
	,RoutingID
	,CASE ISNULL(u.UserData2,'') WHEN '' THEN 'None' ELSE u.UserData2 END as SubType2
	,CASE IsNull(InternationalRouteIndicator,0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END as InternationalRouteIndicator
	,CASE ISNULL(t.SubType,'') WHEN '' THEN 'None' ELSE t.SubType END as SubType
	,DestinationRegistrationID1 as DestVehicleID
	,DestinationEquipmentModel1 as DestVehicleModel
	,dbo.udf_ConvertFromSIUnits(ABS(ISNULL(li.GrossQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity
	,dbo.udf_ConvertFromSIUnits(ABS(ISNULL(li.NetQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)  AS NetQuantity
	,SourceRegistrationID1 as SrcVehicleID
	,ManagerID
	,OwnerID
	,CarrierID 
	,ShipToID 
	,rpt.TRIMX(Isnull(tn.Notes,'')) as Notes
	,t.DeleteFlag
	,@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces
	,te.EngineeringUnitName as UnitsOfIssue
FROM	tblTransactions t 
INNER JOIN tblTransactionLineItems li on t.transactionGuid=li.transactionGuid
INNER JOIN lookup.tblEngineeringUnit te on @SiteGroupLevelVolumeUnitIndex = te.EngineeringUnitIndex
LEFT JOIN  tblTransactionNotes tn on t.transactionGuid=tn.transactionGuid
LEFT JOIN tblTransactionUserData u on t.TransactionGuid = u.TransactionGuid
WHERE  
	t.InventoryDate BETWEEN @BeginDate AND @EndDate
	AND (t.SiteGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)) 
	AND (@IsInternational is NULL OR (IsNull(InternationalRouteIndicator,0) = @IsInternational )) 
	AND (@TicketNum = '' OR (li.DocumentNumber LIKE @TicketNum)) 
	AND (@Product IS NULL OR (li.ProductGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Product) c))) 
	AND TransactionAliasGuid = @AliasGuid
	AND (@FlightNum = '' OR (RoutingID LIKE @FlightNum)) 
	AND (@DestinationEquipmentID = '' OR (DestinationRegistrationID1 LIKE @DestinationEquipmentID)) 
	AND (@AircraftType = '' OR (DestinationEquipmentModel1 LIKE @AircraftType)) 
	-- because floats are sometime a bit off from integer values, look for values within a unit quantity (gallon or liter)
	AND (@Gross IS NULL OR (1.0 > abs(@Gross - dbo.udf_ConvertFromSIUnits(ABS(ISNULL(li.GrossQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, 0))))
	AND (@Net   IS NULL OR (1.0 > abs(@Net - dbo.udf_ConvertFromSIUnits(ABS(ISNULL(li.NetQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, 0))))
	AND (@SourceEquipmentID = '' OR (SourceRegistrationID1 LIKE @SourceEquipmentID)) 
	AND (@Notes = '' OR (Notes LIKE @Notes)) 
	--AND Notes is not null
	AND t.ManagerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Managers) c)
	AND t.OwnerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Owners) c)
	AND t.CarrierCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Vendors) c)
	AND t.ShipToCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Consumers) c)
	AND EXISTS (SELECT *
		FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
		WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
		OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) 

	ORDER BY InventoryDate, Site, ManagerID, OwnerID, ShipToID, CarrierID, t.DocumentNumber
	
END