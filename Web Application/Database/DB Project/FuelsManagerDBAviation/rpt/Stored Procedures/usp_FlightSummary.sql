CREATE  Procedure [rpt].[usp_FlightSummary] 
(
	@Sites nvarchar(max),
	@Managers nvarchar(max),
	@Owners nvarchar(max),
	@Consumers nvarchar(max),
	@Vendors nvarchar(max),
	@Product uniqueidentifier,
	@FromDate nvarchar(30),
	@ToDate NVARCHAR(30),
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER,
	@EnterpriseStatus BIT
)
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_FlightSummary] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the Flight transaction records for the FlightSummary report.
	-- Notes:
	-- 1. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 2. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	-- 3. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 4. @Consumers: List of company MasterRecordGuids assigned the role of consumer that the transactions list as the consumer for itself to be included in the results
	-- 5. @Vendors: List of company MasterRecordGuids assigned the role of vendor that the transactions list as the vendor for itself to be included in the results
	-- 6. @Product: Product MasterRecordGuid that the transactions list as the product for itself to be included in the results
	-- 7. @FromDate: Lower bound date to collect transactions meeting criteria
	-- 8. @ToDate: Upper bound date to collect transactions meeting criteria
	-- 9. @SiteGuid: Identifies the site the report is being run from
	-- 10. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		
		Declare @ProductGuidParamValue uniqueidentifier
		Set @ProductGuidParamValue = @Product

		DECLARE @BeginDate datetimeoffset(7)
		SET @BeginDate = CAST('01 ' + @FromDate AS datetimeoffset)
		DECLARE @EndDate datetimeoffset(7)
		SET @EndDate = CAST('01 ' + @ToDate AS datetimeoffset)
		SET @EndDate = DATEADD(day,-1,DATEADD(month, 1, @EndDate))
		DECLARE @NewBeginDate datetimeoffset(7)
	
		DECLARE @DateRange TABLE (
			[EndDate] DATETIMEOFFSET(7) ,
			[BeginDate] DATETIMEOFFSET(7)
		);

		INSERT INTO @DateRange SELECT @EndDate EndDate, @BeginDate BeginDate 

		DECLARE @tblMonthYear TABLE(
			StartDate datetimeoffset(7)
			,EndDate datetimeoffset(7)
			,MonthYear nvarchar(30)
		);

		Declare @EndDateTemp datetimeoffset;
		Set @EndDateTemp = @EndDate;
		--The below code is populating a table for the specifically formatted month year information that is required by the report.
		WHILE(@EndDateTemp IS NOT NULL AND @EndDateTemp >= @BeginDate) --to include the year
		BEGIN
			--Get First Day of Current Month
			Set @NewBeginDate = DATEADD(mm, DATEDIFF(m,0,@EndDateTemp),0)
			IF(MONTH(@NewBeginDate) = 1)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'January ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 2)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'February ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 3)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'March ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 4)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'April ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 5)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'May ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 6)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'June ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 7)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'July ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 8)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'August ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 9)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'September ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 10)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'October ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 11)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'November ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			ELSE IF(MONTH(@NewBeginDate) = 12)
				INSERT INTO @tblMonthYear (StartDate,EndDate,MonthYear) VALUES (@NewBeginDate,@EndDateTemp,'December ' + CONVERT(nvarchar,YEAR(@NewBeginDate),4))
			
			--Get Last Day of Previous Month
			SET @EndDateTemp = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,@EndDateTemp),0)) --list for all years in database	
		END

		Declare @SiteTable Table
		(
			ID nvarchar(100),
			IdentityGuid uniqueidentifier,
			Address1 nvarchar(100),
			Address2 nvarchar(100),
			CityStateZip nvarchar(100),
			Country nvarchar(100),
			Phone nvarchar(100)
		)
		Insert into @SiteTable
		SELECT t.ID,
		 t.SiteGuid,
		 t.Address1,
		 t.Address2,
		 CASE WHEN CONCAT(t.City, t.State, t.Zip) = '' THEN NULL 
			ELSE CONCAT(t.City, ', ', t.State, ' ', t.Zip) 
			END AS CityStateZip,
		 t.Country,
		 t.Phone from rpt.udf_GetTableFromStringList(@Sites), tblSites t where SiteGuid = Guid

		Declare @ManagerTable Table
		(
			ID nvarchar(100),
			IdentityGuid uniqueidentifier
		)
		Insert into @ManagerTable
		Select ID,CompanyGuid from rpt.udf_GetTableFromStringList(@Managers), tblCompanies where CompanyGuid = Guid

		Declare @OwnerTable Table
		(
			ID nvarchar(100),
			IdentityGuid uniqueidentifier
		)
		Insert into @OwnerTable
		Select ID,CompanyGuid from rpt.udf_GetTableFromStringList(@Owners), tblCompanies where CompanyGuid = Guid

		Declare @ConsumerTable Table
		(
			ID nvarchar(100),
			IdentityGuid uniqueidentifier
		)
		Insert into @ConsumerTable
		Select ID,CompanyGuid from rpt.udf_GetTableFromStringList(@Consumers), tblCompanies where CompanyGuid = Guid

		Declare @VendorTable Table
		(
			ID nvarchar(100),
			IdentityGuid uniqueidentifier
		)
		Insert into @VendorTable
		Select ID,CompanyGuid from rpt.udf_GetTableFromStringList(@Vendors), tblCompanies where CompanyGuid = Guid
		
		DECLARE @tblResults TABLE
		(
			Consumer uniqueidentifier
			,ConsumerID nvarchar(100)
			,Manager uniqueidentifier
			,ManagerID nvarchar(100)
			,Owner uniqueidentifier
			,OwnerID nvarchar(100)
			,Vendor uniqueidentifier
			,VendorID nvarchar(100)
			,ProductID nvarchar(30)
			,StartDate datetimeoffset(7)
			,EndDate datetimeoffset(7)
			,MonthYear nvarchar(30)
			,IssueGross float
			,IssueNet float
			,IssueCount int
			,DefuelGross float
			,DefuelNet float
			,DefuelCount int
			,SiteID nvarchar(30)
			,SiteGuid uniqueidentifier
			,Address1 nvarchar(30)
			,Address2 nvarchar(30)
			,CityStateZip nvarchar(60)
			,Country nvarchar(30)
			,Phone nvarchar(20)
			,VolumeUnitName NVARCHAR(100)
			,VolumeUnitIndex int
			,VolumeDecimalPlaces int
		);

		DECLARE @ProductName nvarchar(30)
		SET @ProductName = (SELECT ProductID FROM tblProducts WHERE ProductGuid = @ProductGuidParamValue)			
			
		DECLARE @SiteGroupLevelVolumeUnitIndex INT
		DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

		SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
			@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
		FROM tblSites 
		WHERE SiteGuid = @SiteGuid
		
			DECLARE @Trx TABLE (
				[InventoryDate] [date] NULL,
				[LookupTransTypeIndex] [smallint] NOT NULL,
				[ProductGuid] [uniqueidentifier] NULL,
				[SiteGuid] [uniqueidentifier] NULL,
				SiteId NVARCHAR(30) NULL,
				[ManagerCompanyGuid] [uniqueidentifier] NULL,
				[OwnerCompanyGuid] [uniqueidentifier] NULL,
				[ShipperCompanyGuid] [uniqueidentifier] NULL,
				[ShipToCompanyGuid] [uniqueidentifier] NULL,
				[ReversalType] [nvarchar](2) NULL,
				[CarrierCompanyGuid] [uniqueidentifier] NULL,
				[BillToCompanyGuid] [uniqueidentifier] NULL,
				[SupplierCompanyGuid] [uniqueidentifier] NULL,
				[GrossQuantity] [float] NULL,
				[NetQuantity] [float] NULL,
				[Cnt] [int] NULL
			)

		-- Only create rows for combinations that actually have data
		INSERT INTO @Trx SELECT
			t.InventoryDate
			,t.LookupTransTypeIndex
			,l.ProductGuid
			,s.IdentityGuid
			,s.ID as SiteID 
			,t.ManagerCompanyGuid
			,t.OwnerCompanyGuid
			,t.ShipperCompanyGuid 
			,t.ShipToCompanyGuid
			,t.ReversalType
			, t.CarrierCompanyGuid
			, t.BillToCompanyGuid
			, t.SupplierCompanyGuid
			,SUM(ABS(ISNULL(l.GrossQuantity,0)))
			,SUM(ABS(ISNULL(l.NetQuantity,0)))
			,Count(t.TransactionGuid)
		from tblTransactions t
		inner join tblTransactionLineItems l on l.TransactionGuid=t.TransactionGuid
		inner join @SiteTable s on t.SiteGuid = s.IdentityGuid
	where t.DeleteFlag = cast(0 as bit)
		AND (t.ReversalType IS NULL OR t.ReversalType = '' OR t.ReversalType = 'O') 
		AND l.ProductGuid = @ProductGuidParamValue
		and InventoryDate between @BeginDate and @EndDate
		and t.SiteGuid in (SELECT IdentityGuid from @SiteTable)
		and t.LookupTransTypeIndex in (4,5) 
		AND t.LookupTransactionStatusIndex IN (select s.TransactionStatusIndex from [rpt].[udf_GetEnterpriseStatusTable](@EnterpriseStatus) s) 
		and ManagerCompanyGuid in (SELECT IdentityGuid from @ManagerTable)
		and OwnerCompanyGuid in (SELECT IdentityGuid from @OwnerTable)
		and ShipToCompanyGuid in (SELECT IdentityGuid from @ConsumerTable)
		and CarrierCompanyGuid in (SELECT IdentityGuid from @VendorTable)
		AND EXISTS (SELECT *
			FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
			WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
			OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid))
	Group By
			t.InventoryDate
			,t.LookupTransTypeIndex
			,l.ProductGuid
			,s.IdentityGuid
			,s.ID 
			,t.ManagerCompanyGuid
			,t.OwnerCompanyGuid
			,t.ShipperCompanyGuid 
			,t.ShipToCompanyGuid
			,t.ReversalType
			, t.CarrierCompanyGuid
			, t.BillToCompanyGuid
			, t.SupplierCompanyGuid

		INSERT INTO @tblResults 
		(Consumer,ConsumerID,Manager,ManagerID,Owner,OwnerID,Vendor,VendorID,
		ProductID,StartDate,EndDate,MonthYear,IssueGross,IssueNet,IssueCount,DefuelGross,DefuelNet,DefuelCount,SiteID,SiteGuid,Address1,Address2,CityStateZip,Country,Phone,VolumeUnitName,VolumeUnitIndex,VolumeDecimalPlaces) 
		SELECT distinct d.IdentityGuid As Consumer,d.ID AS ConsumerID, 
		g.IdentityGuid As Manager,g.ID AS ManagerID, 
		h.IdentityGuid As Owner,h.ID AS OwnerID, 
		i.IdentityGuid As Vendor,i.ID AS VendorID, 
		@ProductName,f.*,0,0,0,0,0,0, 
		y.ID AS SiteID,y.IdentityGuid,
		y.Address1,
		y.Address2,
		y.CityStateZip,
		y.Country,
		y.Phone,
		te.EngineeringUnitName AS VolumeUnitName,
		@SiteGroupLevelVolumeUnitIndex AS VolumeUnitIndex,
		@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces
		FROM 		@tblMonthYear f
		CROSS JOIN @TRX trx
		INNER JOIN lookup.tblEngineeringUnit te ON @SiteGroupLevelVolumeUnitIndex = te.EngineeringUnitIndex
		INNER JOIN @ConsumerTable d on d.IdentityGuid = trx.ShipToCompanyGuid
		INNER JOIN @ManagerTable g on g.IdentityGuid = trx.ManagerCompanyGuid
		INNER JOIN @OwnerTable h on h.IdentityGuid = trx.OwnerCompanyGuid
		INNER JOIN @VendorTable i on i.IdentityGuid = trx.CarrierCompanyGuid
		INNER JOIN @SiteTable y on y.IdentityGuid = trx.SiteGuid

		--Update month totals for T5 transactions for gross, net, and count
		UPDATE @tblResults SET IssueGross = transListSums.IssueGross,IssueNet = transListSums.IssueNet, IssueCount = transListSums.IssueCount
		FROM @tblResults r2
		INNER JOIN  
		(
			SELECT transList.Consumer AS Consumer,
			transList.Manager AS Manager,
			transList.Owner AS Owner,
			transList.Vendor AS Vendor,
			transList.SiteID AS SiteID,
			transList.SiteGuid,
			transList.StartDate AS StartDate,
			transList.EndDate AS EndDate,
			SUM(transList.IssueGross) AS IssueGross,
			SUM(transList.IssueNet) AS IssueNet,
			SUM(Cnt) AS IssueCount
			FROM 
			(
				SELECT 
				r.Consumer AS Consumer, 
				r.Manager AS Manager, 
				r.Owner AS Owner,
				r.Vendor AS Vendor,
				r.SiteID,
				r.SiteGuid,
				r.StartDate AS StartDate, 
				r.EndDate AS EndDate,
				dbo.udf_ConvertFromSIUnits(t.GrossQuantity, r.VolumeUnitIndex, r.VolumeDecimalPlaces) AS IssueGross,
				dbo.udf_ConvertFromSIUnits(t.NetQuantity, r.VolumeUnitIndex, r.VolumeDecimalPlaces) AS IssueNet,
				t.Cnt
				FROM @TRX t
				INNER JOIN @tblResults r
				ON t.ShipToCompanyGuid = r.Consumer AND t.ManagerCompanyGuid = r.Manager AND t.OwnerCompanyGuid = r.Owner AND t.SiteGuid = r.SiteGuid AND t.CarrierCompanyGuid = r.Vendor
				WHERE t.InventoryDate >= r.StartDate AND t.InventoryDate <= r.EndDate
				AND t.LookupTransTypeIndex = 5 -- T5_PrimaryDisbursement
				GROUP BY r.Consumer,r.Manager,r.Owner,r.Vendor,r.SiteID,r.SiteGuid, r.StartDate, r.EndDate, t.GrossQuantity, t.NetQuantity, r.VolumeUnitIndex, r.VolumeDecimalPlaces,t.Cnt
			) transList
			GROUP BY transList.Consumer, transList.Manager, transList.Owner, transList.Vendor, transList.SiteID, transList.SiteGuid,transList.StartDate, transList.EndDate
		) transListSums
		ON transListSums.Consumer = r2.Consumer AND 
		transListSums.Manager = r2.Manager AND 
		transListSums.Owner = r2.Owner AND 
		transListSums.Vendor = r2.Vendor AND 
		transListSums.SiteGuid = r2.SiteGuid AND 
		transListSums.StartDate = r2.StartDate AND 
		transListSums.EndDate = r2.EndDate

		--Update month totals for T4 transactions for gross, net, and count
		UPDATE @tblResults Set DefuelGross = transListSum.DefuelGross,DefuelNet = transListSum.DefuelNet, DefuelCount = transListSum.DefuelCount
		FROM @tblResults r2
		INNER JOIN
		(
			SELECT 
			transList.Consumer AS Consumer, 
			transList.Manager AS Manager, 
			transList.Owner AS Owner,
			transList.Vendor AS Vendor,
			transList.SiteID AS SiteID,
			transList.SiteGuid,
			transList.StartDate AS StartDate,
			transList.EndDate AS EndDate,
			SUM(transList.DefuelGross) AS DefuelGross,
			SUM(transList.DefuelNet) AS DefuelNet,
			SUM(Cnt) AS DefuelCount
			FROM 
			(
				SELECT r.Consumer AS Consumer,
				r.Manager AS Manager, 
				r.Owner AS Owner,
				r.Vendor AS Vendor,
				r.SiteID AS SiteID,
				r.SiteGuid,
				r.StartDate AS StartDate,
				r.EndDate AS EndDate,
				dbo.udf_ConvertFromSIUnits(t.GrossQuantity, r.VolumeUnitIndex, r.VolumeDecimalPlaces) AS DefuelGross,
				dbo.udf_ConvertFromSIUnits(t.NetQuantity, r.VolumeUnitIndex, r.VolumeDecimalPlaces) AS DefuelNet,
				t.Cnt
				FROM @TRX t
				INNER JOIN @tblResults r
				ON t.ShipToCompanyGuid = r.Consumer AND t.ManagerCompanyGuid = r.Manager AND t.OwnerCompanyGuid = r.Owner AND t.SiteGuid = r.SiteGuid AND t.CarrierCompanyGuid = r.Vendor 
				WHERE t.InventoryDate >= r.StartDate AND t.InventoryDate <= r.EndDate
				AND t.LookupTransTypeIndex = 4 -- T4_SecondaryDefuel
				GROUP BY r.Consumer,r.Manager,r.Owner,r.Vendor,r.SiteID, r.SiteGuid, r.StartDate, r.EndDate, t.GrossQuantity, t.NetQuantity, r.VolumeUnitIndex, r.VolumeDecimalPlaces, t.Cnt
			) transList
			GROUP BY transList.Consumer, transList.Manager, transList.Owner, transList.Vendor, transList.SiteID, transList.SiteGuid, transList.StartDate, transList.EndDate
		) transListSum
		ON 
		transListSum.Consumer = r2.Consumer AND 
		transListSum.Manager = r2.Manager AND 
		transListSum.Owner = r2.Owner AND 
		transListSum.Vendor = r2.Vendor AND 
		transListSum.SiteGuid = r2.SiteGuid AND 
		transListSum.StartDate = r2.StartDate AND 
		transListSum.EndDate = r2.EndDate

		SELECT * FROM @tblResults
		--WHERE IssueCount > 0 OR DefuelCount > 0
		ORDER BY SiteID,ManagerID,OwnerID,ConsumerID,VendorID,StartDAte
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
						+ 'Procedure Name: [rpt].usp_FlightSummary' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END