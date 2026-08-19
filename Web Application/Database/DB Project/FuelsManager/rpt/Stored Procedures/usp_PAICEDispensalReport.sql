CREATE PROCEDURE [rpt].[usp_PAICEDispensalReport]
 /*=============================================
 Author:	   Kimberly Foote
 Create date:  7/8/2010
 Description:  rpt_sp_FSM_PAICEDispensalReport
 Version:		7.0.1.0
 Execution: 

 Modification History:
 	Date		by		Description
	7/8/2010	KF		New Report
	2015-JAN-21		Paul Carpenter - rewrite for FuelsManagerDB 9.x
	
	
		-- Notes:
	-- 1. @SiteGuid: Site/SiteGroup that report is being executed at for the purpose of retrieving proper units and decimal places.
	-- 2. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 3. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list has its associated manager in to be included in the results.
	-- 4. @Vendors: List of company MasterRecordGuids assigned the role of carrier that the transactions list has its associated carrier in to be included in the results.
	-- 5. @Products: Product MasterRecordGuids that the transactions list has its associated product in to be included in the results
	-- 7. @UserGuid: Identifies the user running the report


 =============================================*/
	
	@SiteGuid uniqueidentifier,	
	@Sites uniqueidentifier,
	@UserGuid UNIQUEIDENTIFIER,
	@BeginDate datetime,
	@EndDate datetime,
	@Managers nvarchar(max),
	@Owners nvarchar(max),
	@Products nvarchar(max)

AS

	DECLARE @TEndDate DATETIME -- used for 	Trans Tracking Temp Table
	SET @TEndDate = convert(char(10),@EndDate,110) + ' 23:59:59'
	
	DECLARE @RoleOwner int = 1	
	DECLARE @DateNeverSet DATETIMEOFFSET(7) = '1/1/1899'
	DECLARE @BeginningOfTime DATETIMEOFFSET(7) = '1/1/2000'
	DECLARE @VolumeUnits int
	DECLARE @VolumeUnitName NVARCHAR(100)
	DECLARE @VolumeDecimalPlaces int
	SELECT 
		 @VolumeDecimalPlaces = s.VolumeDecimalPlaces 
		,@VolumeUnits = s.VolumeUnitIndex
		,@VolumeUnitName = eu.EngineeringUnitName
	FROM dbo.tblSites s
	INNER JOIN [lookup].[tblEngineeringUnit] eu on eu.EngineeringUnitIndex = s.VolumeUnitIndex
	WHERE s.SiteGuid = @SiteGuid
	
	DECLARE @AuthorizedCompanies TABLE (CompanyGuid UNIQUEIDENTIFIER)
	INSERT INTO @AuthorizedCompanies SELECT CompanyGuid FROM  [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)
	
	
	DECLARE @SiteList TABLE ( SiteGuid UNIQUEIDENTIFIER NOT NULL				)
	INSERT INTO @SiteList	SELECT Guid as siteGuid FROM  rpt.udf_GetTableFromStringList(@Sites )	
	
	DECLARE @OwnerList TABLE ( OwnerID nvarchar (100) NOT NULL
							 , OwnerCompanyGuid UNIQUEIDENTIFIER NOT NULL )

	INSERT INTO @OwnerList	
		SELECT distinct c.ID AS OwnerID, c._MasterRecordGuid as OwnerCompanyGuid
		FROM tblCompanies c				
		INNER JOIN  erv.udf_GetCompanyRecordVersions (@SiteGuid) aa ON aa.MasterRecordGuid = c._MasterRecordGuid
		INNER JOIN [map].[tblCompanyToRole] b 	ON b.CompanyGuid = c._MasterRecordGuid
		WHERE 1=1
		AND b.LookupCompanyRoleIndex = @RoleOwner
		AND c.LockedOut = 0
		AND c.CompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Owners ) c) 
		ORDER BY c.ID

-- END OWNER LIST

--	BEGIN	PRODUCT LIST
	
	DECLARE @ProductList TABLE ( ProductID nvarchar(30) NOT NULL
							   , ProductGuid UNIQUEIDENTIFIER NOT NULL)
	
	INSERT INTO @ProductList	
		SELECT  a.ProductID, a._MasterRecordGuid as ProductGuid
		FROM tblProducts a 
		INNER JOIN erv.udf_GetProductRecordVersions(@SiteGuid) b ON a._MasterRecordGuid = b.MasterRecordGuid
		WHERE a.ProductGuid 	in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Products ) c) 	
		ORDER BY ProductID
			
-- END PRODUCT LIST
	
	DECLARE @ManagerList TABLE ( ManagerCompanyGuid UNIQUEIDENTIFIER NOT NULL)
	INSERT INTO @ManagerList	
	SELECT m.Guid as ManagerCompanyGuid FROM rpt.udf_GetTableFromStringList(@Managers ) m

/*	
	select * from @SiteList
	select * from @AuthorizedCompanies
	select * from @OwnerList
	select * from @ProductList
	select * from @ManagerList
*/		 

IF OBJECT_ID('tempdb..#Master') IS NOT NULL DROP Table #Master
	
-- Begin Create Master Table
	Create Table #Master(
						[TransDate]			datetime,
						[TicketNo]			nvarchar(60),	
						[LocationCode]		nvarchar(60),
						[FuelOwner]			nvarchar(100),
						[Supplier] 			nvarchar(100),
						[FuelCode] 			nvarchar(60),
						[FuelVolume]		float,
						[Contract/AgreeNo]	nvarchar(60),
						[FlightOperNo]		nvarchar(60),
						[FlightDate]		datetime,
						[FlightNextDest]	nvarchar(8),
						[AircraftOwner]		nvarchar(60),
						[AircraftRegNo]		nvarchar(100),
						[GroupBy]			int,
						[GroupName]			nvarchar(100)
					)
	
-- End Create Master Table		
 
--	Begin Insert Dummy record (there will always be a record to start the table)	
-- print 'Insert Dummy record' ;
		INSERT INTO #Master
				SELECT 
					 [TransDate] = ''
					,[Ticket#] = ''
					,[LocationCode] = ''
					,[FuelOwner] = ''
					,[Supplier] = ''
					,[FuelCode] = ''
					,[FuelVolume] = ''
					,[Contract/Agree#] = ''
					,[FlightOper#] = ''
					,[FlightDate] = ''
					,[FlightNextDest] = ''
					,[AircraftOwner] = ''
					,[AircraftReg#] = ''
					,[GroupBy] = '0'
					,[GroupName] = ''
--	END 	Dummy(there will always be a record to start the table)	Table
	
-- Begin	Stock Gain
-- print 'Begin	Stock Gain';
	INSERT INTO #Master
				SELECT 			
					 [TransDate] = t.TransDateTime
					,[TicketNo] = t.DocumentNumber
					,[LocationCode] = t.Site
					,[FuelOwner] = t.OwnerID
					,[Supplier] = t.SupplierID
					,[FuelCode] = l.ProductCode
					,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
					,[Contract/AgreeNo] = tud.UserData16
					,[FlightOper#] = t.RoutingID
					,[FlightDate] = t.InventoryDate
					,[FlightNextDest] = t.NextStationIataID
					,[AircraftOwner] = t.OwnerID
					,[AircraftRegNo] = t.DestinationRegistrationID1
					,[GroupBy] = '1'
					,[GroupName] = 'Stock Gain'
		 
				From   tblTransactions t with(nolock)
				INNER JOIN tblTransactionLineItems l with(nolock) ON t.TransactionGuid= l.TransactionGuid	
				INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
				INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
				INNER JOIN @ManagerList m	                      ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
				INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
				LEFT JOIN tblTransactionUserData tud WITH(nolock) ON t.TransactionGuid= tud.TransactionGuid
				WHERE 
						InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList) AND t.SiteGuid in (select SiteGuid FROM @SiteList)
					AND a.AliasName = 'Adjustment'
					AND l.GrossQuantity > 0
					AND t.DeleteFlag = CAST(0 AS bit)
					AND l.DeleteFlag = CAST(0 AS bit)
					AND EXISTS (SELECT 1
					FROM ( SELECT * 
							FROM @AuthorizedCompanies authorizedCompaniesGuids 
							WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
								OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
						) a) 
 
				ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID
	-- End Stock Gain
	
	-- Begin	Uplift
-- print 'Begin Uplift';
	INSERT INTO #Master				
				SELECT 				
					 [TransDate] = t.TransDateTime
					,[TicketNo] = t.DocumentNumber
					,[LocationCode] = t.Site
					,[FuelOwner] = t.OwnerID
					,[Supplier] = t.SupplierID
					,[FuelCode] = l.ProductCode
					,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
					,[Contract/AgreeNo] = tud.UserData16
					,[FlightOper#] = t.RoutingID
					,[FlightDate] = t.InventoryDate
					,[FlightNextDest] = t.NextStationIataID
					,[AircraftOwner] = t.OwnerID
					,[AircraftRegNo] = t.DestinationRegistrationID1
					,[GroupBy] = '2'
					,[GroupName] = 'Uplift'								 
				From   tblTransactions t WITH(nolock)
						INNER JOIN tblTransactionLineItems l WITH(nolock) ON t.TransactionGuid= l.TransactionGuid	
						INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
						INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
						INNER JOIN @ManagerList m						  ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
						INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
						LEFT JOIN tblTransactionUserData tud WITH(nolock) ON t.TransactionGuid= tud.TransactionGuid
						Where 
								InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList)
							AND a.AliasName = 'Issue'
							--AND t.TransID NOT  IN (SELECT Transid FROM #TransTrack)
							AND ISNULL(T.DATE01, @DateNeverSet) < @BeginningOfTime
							AND t.DeleteFlag = CAST(0 AS bit)
							AND l.DeleteFlag = CAST(0 AS bit)
							AND EXISTS (SELECT 1
							FROM ( SELECT CompanyGuid 
								   FROM @AuthorizedCompanies authorizedCompaniesGuids 
								   WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
									 OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
								) a) 
						ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID				 				 				
-- END	Uplift

-- BEGIN Defuel
-- print 'BEGIN Defuel';
	INSERT INTO #Master
		SELECT 
			 [TransDate] = t.TransDateTime
			,[TicketNo] = t.DocumentNumber
			,[LocationCode] = t.Site
			,[FuelOwner] = t.OwnerID
			,[Supplier] = t.SupplierID
			,[FuelCode] = l.ProductCode
			,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
			,[Contract/AgreeNo] = tud.UserData16
			,[FlightOper#] = t.RoutingID
			,[FlightDate] = t.InventoryDate
			,[FlightNextDest] = t.NextStationIataID
			,[AircraftOwner] = t.OwnerID
			,[AircraftRegNo] = t.DestinationRegistrationID1
			,[GroupBy] = '3'
			,[GroupName] = 'Defuel'
		FROM   tblTransactions t with(nolock)
		INNER JOIN tblTransactionLineItems l WITH(nolock) ON t.TransactionGuid= l.TransactionGuid	
		INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
		INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
		INNER JOIN @ManagerList m						  ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
		INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
		LEFT JOIN tblTransactionUserData tud WITH(nolock) ON t.TransactionGuid= tud.TransactionGuid
		WHERE 
				InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList)
			AND a.AliasName = 'Issue'
			--AND t.TransID  IN (SELECT Transid FROM #TransTrack)
			AND ISNULL(T.DATE01, @DateNeverSet) > @BeginningOfTime
			AND t.DeleteFlag = CAST(0 AS bit)
			AND l.DeleteFlag = CAST(0 AS bit)
			AND EXISTS (SELECT 1
			FROM ( SELECT CompanyGuid 
				   FROM @AuthorizedCompanies authorizedCompaniesGuids 
				   WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
					 OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
				) a) 
		ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID	
-- END Defuel

-- BEGIN Mechanical
-- print 'BEGIN Mechanical';
	INSERT INTO #Master
		SELECT 
			 [TransDate] = t.TransDateTime
			,[TicketNo] = t.DocumentNumber
			,[LocationCode] = t.Site
			,[FuelOwner] = t.OwnerID
			,[Supplier] = t.SupplierID
			,[FuelCode] = l.ProductCode
			,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
			,[Contract/AgreeNo] = tud.UserData16
			,[FlightOper#] = t.RoutingID
			,[FlightDate] = t.InventoryDate
			,[FlightNextDest] = t.NextStationIataID
			,[AircraftOwner] = t.OwnerID
			,[AircraftRegNo] = t.DestinationRegistrationID1
			,[GroupBy] = '4'
			,[GroupName] = 'Mechanical'
		FROM   tblTransactions t with(nolock)
				INNER JOIN tblTransactionLineItems l with(nolock) ON t.TransactionGuid= l.TransactionGuid	
				INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
				INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
				INNER JOIN @ManagerList m						  ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
				INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
				LEFT JOIN tblTransactionUserData tud with(nolock) ON t.TransactionGuid= tud.TransactionGuid
				WHERE 
						InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList)
					AND a.AliasName = 'Issue'
					AND t.RoutingID in('ACMTC','QKMTC')
					AND t.DeleteFlag = CAST(0 as bit)
					AND l.DeleteFlag = CAST(0 as bit)
					AND EXISTS (SELECT 1
					FROM ( SELECT CompanyGuid 
						   FROM @AuthorizedCompanies authorizedCompaniesGuids 
						   WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
							 OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
						) a) 
				ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID	
-- END Mechanical

-- BEGIN UpLift 	Adjustment
-- print 'BEGIN UpLift 	Adjustment';
	INSERT INTO #Master
		Select 				
			 [TransDate] = t.TransDateTime
			,[TicketNo] = t.DocumentNumber
			,[LocationCode] = t.Site
			,[FuelOwner] = t.OwnerID
			,[Supplier] = t.SupplierID
			,[FuelCode] = l.ProductCode
			,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
			,[Contract/AgreeNo] = tud.UserData16
			,[FlightOper#] = t.RoutingID
			,[FlightDate] = t.InventoryDate
			,[FlightNextDest] = t.NextStationIataID
			,[AircraftOwner] = t.OwnerID
			,[AircraftRegNo] = t.DestinationRegistrationID1
			,[GroupBy] = '5'
			,[GroupName] = 'UpLift Adjustment'
		FROM   tblTransactions t WITH(nolock)
		INNER JOIN tblTransactionLineItems l WITH(nolock) ON t.TransactionGuid= l.TransactionGuid	
		INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
		INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
		INNER JOIN @ManagerList m						  ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
		INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
		LEFT JOIN tblTransactionUserData tud WITH(nolock) ON t.TransactionGuid= tud.TransactionGuid
		WHERE 
				InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList)
			AND a.AliasName = 'Issue'
			--AND t.TransID  IN (SELECT Transid FROM #TransTrack)
			AND ISNULL(T.DATE01, @DateNeverSet) > @BeginningOfTime
			AND t.DeleteFlag = CAST(0 AS bit)
			AND l.DeleteFlag = CAST(0 AS bit)
			AND EXISTS (SELECT 1
							FROM ( SELECT CompanyGuid 
								   FROM @AuthorizedCompanies authorizedCompaniesGuids 
								   WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
									 OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
								) a) 
		ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID					
		
-- END UpLift 	Adjustment

-- BEGIN UpLift Cancellation
-- print 'BEGIN UpLift Cancellation';
	INSERT INTO #Master				
		SELECT 				
			 [TransDate] = t.TransDateTime
			,[TicketNo] = t.DocumentNumber
			,[LocationCode] = t.Site
			,[FuelOwner] = t.OwnerID
			,[Supplier] = t.SupplierID
			,[FuelCode] = l.ProductCode
			,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
			,[Contract/AgreeNo] = tud.UserData16
			,[FlightOper#] = t.RoutingID
			,[FlightDate] = t.InventoryDate
			,[FlightNextDest] = t.NextStationIataID
			,[AircraftOwner] = t.OwnerID
			,[AircraftRegNo] = t.DestinationRegistrationID1
			,[GroupBy] = '6'
			,[GroupName] = 'UpLift Cancellation'
		FROM   tblTransactions t with(nolock)
		INNER JOIN tblTransactionLineItems l WITH(nolock) ON t.TransactionGuid= l.TransactionGuid	
		INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
		INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
		INNER JOIN @ManagerList m						  ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
		INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
		LEFT JOIN tblTransactionUserData tud WITH(nolock) ON t.TransactionGuid= tud.TransactionGuid
		WHERE 
				InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList)
			AND a.AliasName in('Issue','Defuel')
			--AND t.TransID  IN (SELECT Transid FROM #TransTrack)
			AND ISNULL(T.DATE01, @DateNeverSet) > @BeginningOfTime
			AND t.DeleteFlag = CAST(0 AS bit)
			AND l.DeleteFlag = CAST(0 AS bit)
			AND EXISTS (SELECT 1
							FROM ( SELECT CompanyGuid 
								   FROM @AuthorizedCompanies authorizedCompaniesGuids 
								   WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
									 OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
								) a)  
		ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID					
-- END UpLift Cancellation			 

-- BEGIN Fuel Exchange
-- print 'BEGIN Fuel Exchange';
	INSERT INTO #Master
		SELECT 
			 [TransDate] = t.TransDateTime
			,[TicketNo] = t.DocumentNumber
			,[LocationCode] = t.Site
			,[FuelOwner] = t.OwnerID
			,[Supplier] = t.SupplierID
			,[FuelCode] = l.ProductCode
			,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
			,[Contract/AgreeNo] = tud.UserData16
			,[FlightOper#] = t.RoutingID
			,[FlightDate] = t.InventoryDate
			,[FlightNextDest] = t.NextStationIataID
			,[AircraftOwner] = t.OwnerID
			,[AircraftRegNo] = t.DestinationRegistrationID1
			,[GroupBy] = '7'
			,[GroupName] = 'Fuel Exchange'
		FROM   tblTransactions t WITH(nolock)
		INNER JOIN tblTransactionLineItems l WITH(nolock) ON t.TransactionGuid= l.TransactionGuid	
		INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
		INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
		INNER JOIN @ManagerList m						  ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
		INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
		LEFT JOIN tblTransactionUserData tud WITH(nolock) ON t.TransactionGuid= tud.TransactionGuid
		WHERE 
				InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList)
			AND a.AliasName = 'Transfer'
			AND tud.UserData16 Like '5%'
			--AND t.TransID NOT IN (SELECT Transid FROM #TransTrack)
			AND ISNULL(T.DATE01, @DateNeverSet) < @BeginningOfTime
			AND t.DeleteFlag = CAST(0 as bit)
			AND l.DeleteFlag = CAST(0 as bit)
			AND EXISTS (SELECT 1
			FROM ( SELECT CompanyGuid 
				   FROM @AuthorizedCompanies authorizedCompaniesGuids 
				   WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
					 OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
				) a) 
		ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID
-- END Fuel Exchange

-- BEGIN Fuel Sale
-- print 'BEGIN Fuel Sale';
	INSERT INTO #Master
		SELECT 
			 [TransDate] = t.TransDateTime
			,[TicketNo] = t.DocumentNumber
			,[LocationCode] = t.Site
			,[FuelOwner] = t.OwnerID
			,[Supplier] = t.SupplierID
			,[FuelCode] = l.ProductCode
			,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
			,[Contract/AgreeNo] = tud.UserData16
			,[FlightOper#] = t.RoutingID
			,[FlightDate] = t.InventoryDate
			,[FlightNextDest] = t.NextStationIataID
			,[AircraftOwner] = t.OwnerID
			,[AircraftRegNo] = t.DestinationRegistrationID1
			,[GroupBy] = '8'
			,[GroupName] = 'Fuel Sale'
		FROM   tblTransactions t WITH(nolock)
		INNER JOIN tblTransactionLineItems l WITH(nolock) ON t.TransactionGuid= l.TransactionGuid	
		INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
		INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
		INNER JOIN @ManagerList m						  ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
		INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
		LEFT JOIN tblTransactionUserData tud WITH(nolock) ON t.TransactionGuid= tud.TransactionGuid
		WHERE 
				InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList)
			AND a.AliasName = 'Transfer'
			AND tud.UserData16 NOT LIKE '5%'
			--AND t.TransID NOT IN (SELECT Transid FROM #TransTrack)
			AND ISNULL(T.DATE01, @DateNeverSet) < @BeginningOfTime
			AND t.DeleteFlag = CAST(0 as bit)
			AND l.DeleteFlag = CAST(0 as bit)
			AND EXISTS (SELECT 1
							FROM ( SELECT CompanyGuid 
								   FROM @AuthorizedCompanies authorizedCompaniesGuids 
								   WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
									 OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
								) a) 
		ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID
-- END Fuel Sale						 

-- BEGIN Stock Loss
-- print 'BEGIN Stock Loss';
	INSERT INTO #Master
		SELECT 				
			 [TransDate] = t.TransDateTime
			,[TicketNo] = t.DocumentNumber
			,[LocationCode] = t.Site
			,[FuelOwner] = t.OwnerID
			,[Supplier] = t.SupplierID
			,[FuelCode] = l.ProductCode
			,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
			,[Contract/AgreeNo] = tud.UserData16
			,[FlightOper#] = t.RoutingID
			,[FlightDate] = t.InventoryDate
			,[FlightNextDest] = t.NextStationIataID
			,[AircraftOwner] = t.OwnerID
			,[AircraftRegNo] = t.DestinationRegistrationID1
			,[GroupBy] = '9'
			,[GroupName] = 'Stock Loss'
		FROM   tblTransactions t WITH(nolock)
		INNER JOIN tblTransactionLineItems l WITH(nolock) ON t.TransactionGuid= l.TransactionGuid	
		INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
		INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
		INNER JOIN @ManagerList m								  ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
		INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
		LEFT JOIN tblTransactionUserData tud WITH(nolock) ON t.TransactionGuid= tud.TransactionGuid
		WHERE 
				InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList)
			AND a.AliasName = 'Adjustment'
			AND l.GrossQuantity <= 0
			AND t.DeleteFlag = CAST(0 as bit)
			AND l.DeleteFlag = CAST(0 as bit)
						AND EXISTS (SELECT 1
						FROM ( SELECT CompanyGuid 
							   FROM @AuthorizedCompanies authorizedCompaniesGuids 
							   WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
								 OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
							) a) 
		ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID			
				 
-- END Stock Loss

-- BEGIN Dispensal Adjustment
-- print 'BEGIN Dispensal Adjustment';
	INSERT INTO #Master
		Select 
			 [TransDate] = t.TransDateTime
			,[TicketNo] = t.DocumentNumber
			,[LocationCode] = t.Site
			,[FuelOwner] = t.OwnerID
			,[Supplier] = t.SupplierID
			,[FuelCode] = l.ProductCode
			,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
			,[Contract/AgreeNo] = tud.UserData16
			,[FlightOper#] = t.RoutingID
			,[FlightDate] = t.InventoryDate
			,[FlightNextDest] = t.NextStationIataID
			,[AircraftOwner] = t.OwnerID
			,[AircraftRegNo] = t.DestinationRegistrationID1
			,[GroupBy] = '10'
			,[GroupName] = 'Dispensal Adjustment'			
		FROM   tblTransactions t WITH(nolock)
				INNER JOIN tblTransactionLineItems l WITH(nolock) ON t.TransactionGuid= l.TransactionGuid	
				INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
				INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
				INNER JOIN @ManagerList m						  ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
				INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
				LEFT JOIN tblTransactionUserData tud WITH(nolock) ON t.TransactionGuid= tud.TransactionGuid
		WHERE 
				InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList)
			AND a.AliasName = 'Transfer'
			--AND t.TransID  IN (SELECT Transid FROM #TransTrack)
			AND ISNULL(T.DATE01, @DateNeverSet) > @BeginningOfTime
			AND t.DeleteFlag = CAST(0 as bit)
			AND l.DeleteFlag = CAST(0 as bit)
			AND EXISTS (SELECT 1
							FROM ( SELECT CompanyGuid
								   FROM @AuthorizedCompanies authorizedCompaniesGuids 
								   WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
									 OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
								) a) 
		ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID				
-- END Dispensal Adjustment

-- BEGIN Dispensal Cancellation
-- print 'BEGIN Dispensal Cancellation';
	INSERT INTO #Master
		SELECT 
			 [TransDate] = t.TransDateTime
			,[TicketNo] = t.DocumentNumber
			,[LocationCode] = t.Site
			,[FuelOwner] = t.OwnerID
			,[Supplier] = t.SupplierID
			,[FuelCode] = l.ProductCode
			,[FuelVolume] = dbo.udf_ConvertFromSIUnits(l.GrossQuantity , @VolumeUnits, @VolumeDecimalPlaces)
			,[Contract/AgreeNo] = tud.UserData16
			,[FlightOper#] = t.RoutingID
			,[FlightDate] = t.InventoryDate
			,[FlightNextDest] = t.NextStationIataID
			,[AircraftOwner] = t.OwnerID
			,[AircraftRegNo] = t.DestinationRegistrationID1
			,[GroupBy] = '11'
			,[GroupName] = 'Dispensal Cancellation'
		FROM   tblTransactions t WITH(nolock)
		INNER JOIN tblTransactionLineItems l WITH(nolock) ON t.TransactionGuid= l.TransactionGuid	
		INNER JOIN @ProductList p                         ON p.ProductGuid = l.ProductGuid
		INNER JOIN @OwnerList o                           ON o.OwnerCompanyGuid = t.OwnerCompanyGuid
		INNER JOIN @ManagerList m						  ON m.ManagerCompanyGuid=t.ManagerCompanyGuid
		INNER JOIN tblTransactionAliases a                ON a.TransactionAliasGuid=t.TransactionAliasGuid 
		LEFT JOIN tblTransactionUserData tud WITH(nolock) ON t.TransactionGuid= tud.TransactionGuid						
		WHERE 
				InventoryDate BETWEEN @BeginDate AND @TEndDate AND t.SiteGuid in (select SiteGuid FROM @SiteList)
			AND a.AliasName = 'Transfer'
			--AND t.TransID  IN (SELECT Transid FROM #TransTrack)
			AND ISNULL(T.DATE01, @DateNeverSet) > @BeginningOfTime
			AND t.DeleteFlag = CAST(1 as bit)
			AND l.DeleteFlag = CAST(0 as bit)
			AND EXISTS (SELECT 1
							FROM ( SELECT CompanyGuid 
									FROM @AuthorizedCompanies authorizedCompaniesGuids 
									WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
										OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)
								) a) 
		ORDER BY [GroupBy], t.TransDateTime, t.DocumentNumber,  t.Site, t.OwnerID	

-- END Dispensal Cancellation

-- print 'FINAL RESULTS'
Select m.*, @VolumeDecimalPlaces as VolumeDecimalPlaces, @VolumeUnitName as VolumeUnitName from #Master m where [Groupby] <> '0'
ORDER BY [GroupBy], [TransDate], [TicketNo],  [LocationCode], [FuelOwner]



drop table #Master



GRANT EXECUTE ON rpt.[rpt_sp_FSM_PAICEDispensalReport] TO [public]
GO