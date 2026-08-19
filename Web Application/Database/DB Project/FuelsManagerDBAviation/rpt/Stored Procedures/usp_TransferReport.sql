CREATE Procedure [rpt].[usp_TransferReport] 
(
	@SiteGuid uniqueidentifier,
	@Sites nvarchar(max),
	@FromManagers nvarchar(max),
	@ToManagers nvarchar(max),
	@FromOwners nvarchar(max),
	@ToOwners nvarchar(max),
	@Product uniqueidentifier,
	@FromDate DATETIMEOFFSET(7),
	@ToDate DATETIMEOFFSET(7),
	@UserGuid UNIQUEIDENTIFIER
)

AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_TransferReport] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Transfer Report
	-- Notes:
	-- 1. @SiteGuid: Site/SiteGroup that report is being executed at for the purpose of retrieving proper units and decimal places.
	-- 2. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 3. @FromManagers: List of company MasterRecordGuids assigned the role of manager that the transactions list has the manager for itself to be included in the results
	-- 4. @ToManagers: List of company MasterRecordGuids assigned the role of manager that the conjoined transactions list has the manager for itself to be included in the results
	-- 5. @FromOwners: List of company MasterRecordGuids assigned the role of owner that the transactions list AS the owner for itself to be included in the results
	-- 5. @ToOwners: List of company MasterRecordGuids assigned the role of owner that the conjoined transactions list AS the owner for itself to be included in the results
	-- 6. @Product: Product MasterRecordGuid that the transactions list AS the product for itself to be included in the results
	-- 7. @FromDate: Lower bound date to collect transactions meeting criteria
	-- 8. @ToDate: Upper bound date to collect transactions meeting criteria
	-- 9. @UserGuid: Identifies the user running the report

	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		
	-- Trim off passed in hours and minute, add seconds until end of day
	SET @ToDate = DATEADD(second,86399,DATEADD(dd, DATEDIFF(dd, 0,@ToDate), 0))

	-- Trim off passed in hours and minute, add seconds until end of day
	SET @ToDate = DATEADD(second,86399,DATEADD(dd, DATEDIFF(dd, 0,@ToDate), 0))

	DECLARE @SiteGroupLevelVolumeUnitIndex INT
	DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

	SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
		@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
	FROM tblSites 
	WHERE SiteGuid = @SiteGuid

		declare @BarrelsVolumeIndex int;
		declare @BarrelsUnitAbbrev nvarchar(10);
		Set @BarrelsVolumeIndex = (Select top 1 ISNULL(EngineeringUnitIndex,48) from lookup.tblEngineeringUnit where EngineeringUnitName = 'Barrels (Oil)')
		Set @BarrelsUnitAbbrev = (Select top 1 ISNULL(EngineeringUnitAbbreviation,'bbl') from lookup.tblEngineeringUnit where EngineeringUnitName = 'Barrels (Oil)')

		SELECT	
		t.TransID
		,CAST(t.InventoryDate AS Date) AS InventoryDate
		,l.Product
		,l.DocumentNumber AS Batch  
		,dbo.udf_ConvertFromSIUnits(abs(ISNULL(l.GrossQuantity,0.0)),@SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity
		,dbo.udf_ConvertFromSIUnits(abs(ISNULL(l.NetQuantity,0.0)),@SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS NetQuantity
		,dbo.udf_ConvertFromSIUnits(abs(ISNULL(l.GrossQuantity,0.0)),@BarrelsVolumeIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantityBBL
		,dbo.udf_ConvertFromSIUnits(abs(ISNULL(l.NetQuantity,0.0)),@BarrelsVolumeIndex, @SiteGroupLevelVolumeDecimalPlaces) AS NetQuantityBBL
		, t.Site
		,t.ManagerID AS FromManager
		,ttSub.ManagerID AS ToManager
		,t.OwnerID AS FromOwner
		,ttSub.OwnerID AS ToOwner
		,t.ShipToID AS FromConsumer
		,ttSub.ShipToID AS ToConsumer
		,l.StorageLocationID AS FromTank
		,llSub.StorageLocationID AS ToTank
		,@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces
		,te.EngineeringUnitName as VolumeEngUnitName
		,te.EngineeringUnitAbbreviation as VolumeEngUnitAbbrev
		,@BarrelsUnitAbbrev as BarrelsEngUnitAbbrev
		FROM	tblTransactions t 
		INNER JOIN tblTransactionLineItems l  ON t.TransactionGuid = l.TransactionGuid 
		INNER JOIN lookup.tblEngineeringUnit te on @SiteGroupLevelVolumeUnitIndex = te.EngineeringUnitIndex
		LEFT JOIN tblTransactions ttSub ON ttSub.TransID = t.ConjoinTransID
		LEFT JOIN tblTransactionLineItems llSub ON ttSub.TransactionGuid = llSub.TransactionGuid 
		WHERE	
		t.InventoryDate >= @FromDate AND t.InventoryDate < @ToDate 
		AND t.LookupTransTypeIndex = 13 -- T13_OwnerTransfer
		AND l.ProductGuid = @Product
		AND t.SubType = 'D'
		AND ttSub.SubType = 'C'
		AND ISNULL(t.DeleteFlag,0) = 0
		AND t.OwnerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@FromOwners) c)
		AND ttSub.OwnerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@ToOwners) c)
		AND t.ManagerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@FromManagers) c)
		AND ttSub.ManagerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@ToManagers) c)
		AND t.SiteGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
		AND EXISTS (SELECT *
			FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
			WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
			OR authorizedCompaniesGuids.CompanyGuid IN 
			(t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid
			, ttSub.ShipToCompanyGuid, ttSub.SupplierCompanyGuid, ttSub.ShipperCompanyGuid, ttSub.OwnerCompanyGuid, ttSub.ManagerCompanyGuid, ttSub.CarrierCompanyGuid, ttSub.BillToCompanyGuid))
		ORDER BY   l.Product, t.OwnerID, t.Site, t.ManagerID, t.InventoryDate
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
						+ 'Procedure Name: [rpt].usp_TransferReport' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END