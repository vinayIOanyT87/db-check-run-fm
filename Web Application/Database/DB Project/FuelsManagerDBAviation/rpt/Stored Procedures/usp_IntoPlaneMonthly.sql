CREATE Procedure [rpt].[usp_IntoPlaneMonthly] 
(
	@SiteGuid uniqueidentifier,
	@Sites nvarchar(max),
	@Managers nvarchar(max),
	@Consumers nvarchar(max),
	@Vendors nvarchar(max),
	@Product uniqueidentifier,
	@MonthYear NVARCHAR(20),
	@UserGuid UNIQUEIDENTIFIER
)

AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_IntoPlaneMonthly] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- 2013-Dec-03 TFS Bug 44088 pcarpenter: Remove ShipToID from output columns because this field is not displayed and prevents correct grouping on the report
	-- Purpose: Retrieve the transaction records for the Into Plane Monthly Report
	-- Notes:
	-- 1. @SiteGuid: Site/SiteGroup that report is being executed at for the purpose of retrieving proper units and decimal places.
	-- 2. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 3. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list has its associated manager in to be included in the results.
	-- 4. @Vendors: List of company MasterRecordGuids assigned the role of carrier that the transactions list has its associated carrier in to be included in the results.
	-- 5. @Product: Product MasterRecordGuids that the transactions list has its associated product in to be included in the results
	-- 6. @MonthYear: Month and year to filter the date of transactions in the results
	-- 7. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	DECLARE @BeginDate datetimeoffset(7)
	
	SET @BeginDate = rpt.udf_MonthYearToTimestamp(@MonthYear)

	DECLARE @EndDate datetimeoffset(7)
	SET @EndDate = DATEADD(month,1,@BeginDate)

	Declare @ManagersTable TABLE (IdentityGuid uniqueIdentifier)
	Declare @SitesTable TABLE (IdentityGuid uniqueIdentifier)
	Declare @VendorsTable TABLE (IdentityGuid uniqueIdentifier)
	Declare @ConsumersTable TABLE (IdentityGuid uniqueIdentifier)


	Insert Into @ManagersTable SELECT Guid FROM rpt.udf_GetTableFromStringList(@Managers)
	Insert Into @SitesTable SELECT Guid FROM rpt.udf_GetTableFromStringList(@Sites)
	Insert Into @VendorsTable SELECT Guid FROM rpt.udf_GetTableFromStringList(@Vendors)
	Insert Into @ConsumersTable SELECT Guid FROM rpt.udf_GetTableFromStringList(@Consumers)
						
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @SiteGroupLevelVolumeUnitIndex INT
	DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

	SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
		@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
	FROM tblSites 
	WHERE SiteGuid = @SiteGuid
	

		SELECT CAST(b.InventoryDate AS DATE) AS Date,
			s.ID as SiteID, 
			b.ManagerID, 
			b.IntoPlaneAgent, 
			tblProducts.ProductID AS Product, 
			b.ShipToID, 
			b.TypeofFuel,
			SUM(b.Total_TransactionCount) AS TotalIssueCount,
			ABS(SUM(b.Total_Issues_Gross)) AS GrossGallonsUplifted,
			ABS(SUM(b.Total_Issues_Net)) AS NetGallonsUplifted,
			ABS(SUM(b.Total_Adjustments)) AS TotalAdjustments,
			SUM(b.Total_DefuelCount) AS TotalDefeulCount,
			ABS(SUM(b.Total_Defuels_Gross)) AS TotalDefuelsGross,
			ABS(SUM(b.Total_Defuels_Net)) AS TotalDefuelsNet,
			@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces,
			te.EngineeringUnitName
		FROM 
		(
			SELECT a.ManagerID,a.InventoryDate, a.IntoPlaneAgent, a.ShipToID, a.SubType AS TypeofFuel,
				SUM(ISNULL(a.Transaction_Count,0)) AS Total_TransactionCount, 
				SUM(ISNULL(Gross_Total,0.00)) AS Total_Issues_Gross,
				SUM(ISNULL(Net_Total,0.00)) AS Total_Issues_Net,
				0.00 AS Total_Adjustments,
				0.00 AS Total_Defuels_Gross,
				0.0 as Total_Defuels_Net,
				0 AS Total_DefuelCount,
				a.SiteGuid,
				a.ProductGuid
			FROM
			(
					SELECT ManagerID,InventoryDate, CarrierID AS IntoPlaneAgent, CASE ISNULL(InternationalRouteIndicator,0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END AS SubType,
						ShipToID,
						CONVERT(float,SUM(dbo.udf_ConvertFromSIUnits(ISNULL(GrossQuantity,0), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces))) AS Gross_Total,
						CONVERT(float,SUM(dbo.udf_ConvertFromSIUnits(ISNULL(NetQuantity,0), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces))) AS Net_Total,
						COUNT(Product) AS Transaction_Count,
						t.SiteGuid,
						li.ProductGuid
					FROM tblTransactions t
					INNER JOIN tblTransactionLineItems li ON (t.TransactionGuid = li.TransactionGuid)
					WHERE 
						t.LookupTransTypeIndex = 5 -- T5_PrimaryDisbursement
						AND t.InventoryDate >= @BeginDate AND InventoryDate < @EndDate
						AND li.ProductGuid	= @Product
						AND t.ManagerCompanyGuid in (Select IdentityGuid from @ManagersTable)
						AND t.SiteGuid in (Select IdentityGuid from @SitesTable)
						AND t.CarrierCompanyGuid in (Select IdentityGuid from @VendorsTable)
						AND t.ShipToCompanyGuid in (Select IdentityGuid from @ConsumersTable)
						AND t.DeleteFlag = CAST(0 AS bit)
						AND li.DeleteFlag = CAST(0 AS bit) 
						
						AND EXISTS (SELECT *
							FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
							WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
							OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) 
							
					GROUP BY t.SiteGuid,ManagerID,InventoryDate,CarrierID,li.ProductGuid,ShipToID, CASE ISNULL(InternationalRouteIndicator,0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END
			) a
			GROUP BY a.SiteGuid,a.ManagerID,a.InventoryDate,a.IntoPlaneAgent,a.ProductGuid, a.ShipToID, a.SubType
			UNION
			SELECT a.ManagerID,a.InventoryDate, a.IntoPlaneAgent, a.ShipToID, a.SubType AS TypeofFuel,
				0 AS Total_TransactionCount,
				0.00 AS Total_Issues_Gross,
				0.00 AS Total_IssuesNet, 
				0.00 AS Total_Adjustments,
				SUM(ISNULL(Gross_Total,0.00)) AS Total_Defuels_Gross,
				SUM(ISNULL(Net_Total,0.00)) AS Total_Defuels_Net,
				SUM(ISNULL(a.Transaction_Count,0)) AS Total_DefuelCount,
				a.SiteGuid,
				a.ProductGuid
			FROM
			(
					SELECT ManagerID,InventoryDate, CarrierID AS IntoPlaneAgent, CASE ISNULL(InternationalRouteIndicator,0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END AS SubType,
						ShipToID,
						SUM(dbo.udf_ConvertFromSIUnits(ISNULL(GrossQuantity,0.0), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)) AS Gross_Total,
						SUM(dbo.udf_ConvertFromSIUnits(ISNULL(NetQuantity,0.0), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)) AS Net_Total,
						COUNT(Product) AS Transaction_Count,
						t.SiteGuid,
						li.ProductGuid
					FROM tblTransactions t
					INNER JOIN tblTransactionLineItems li ON (t.TransactionGuid = li.TransactionGuid)
					WHERE 
					    t.LookupTransTypeIndex = 4 -- T4_SecondaryDefuel
						AND t.InventoryDate >= @BeginDate AND InventoryDate < @EndDate
						AND li.ProductGuid	= @Product
						AND t.ManagerCompanyGuid in (Select IdentityGuid from @ManagersTable)
						AND t.SiteGuid in (Select IdentityGuid from @SitesTable)
						AND t.CarrierCompanyGuid in (Select IdentityGuid from @VendorsTable)
						AND t.ShipToCompanyGuid in (Select IdentityGuid from @ConsumersTable)
						AND t.DeleteFlag = CAST(0 AS bit)
						AND li.DeleteFlag = CAST(0 AS bit) 
						
						AND EXISTS (SELECT *
							FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
							WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
							OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) 
							
					GROUP BY t.SiteGuid,ManagerID,InventoryDate,CarrierID,li.ProductGuid,ShipToID, CASE ISNULL(InternationalRouteIndicator,0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END
			) a
			GROUP BY a.SiteGuid,a.ManagerID,a.InventoryDate,a.IntoPlaneAgent,a.ProductGuid, a.ShipToID, a.SubType
		) b 
		INNER JOIN tblSites s on s.SiteGuid = b.SiteGuid
		INNER JOIN lookup.tblEngineeringUnit te on @SiteGroupLevelVolumeUnitIndex = te.EngineeringUnitIndex
		INNER JOIN tblProducts ON tblProducts._MasterRecordGuid = b.ProductGuid AND tblProducts._MasterRecordGuid = tblProducts.ProductGuid -- Joining with products through MasterRecordGuid is OK if we are only looking for the ID. ID is not record versioned.
		GROUP BY s.ID, ManagerID, InventoryDate, [IntoPlaneAgent], tblProducts.[ProductID], ShipToID, [TypeofFuel], te.EngineeringUnitName
		ORDER BY SiteID, ManagerID, InventoryDate, [IntoPlaneAgent], tblProducts.[ProductID], ShipToID, [TypeofFuel]
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
						+ 'Procedure Name: [rpt].usp_IntoPlaneMonthly' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    

END