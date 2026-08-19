CREATE Procedure [rpt].[usp_OwnerConsumptionSummary] 
(
	@Sites nvarchar(max),
	@Managers nvarchar(max),
	@Owners nvarchar(max),
	@Consumers nvarchar(max),
	@Product uniqueidentifier,
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
	-- Stored Procedure: [rpt].[usp_OwnerConsumptionSummary] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.004 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Owner Consumption Summary Report
	-- Notes:
	-- 1. @SiteGuid: Site/SiteGroup that report is being executed at for the purpose of retrieving proper units and decimal places.
	-- 2. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 3. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	-- 3. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 4. @Consumers: List of company MasterRecordGuids assigned the role of consumer that the transactions list as the consumer for itself to be included in the results
	-- 5. @Product: Product MasterRecordGuid that the transactions list as the product for itself to be included in the results
	-- 6. @FromDate: Lower bound date to collect transactions meeting criteria
	-- 7. @ToDate: Upper bound date to collect transactions meeting criteria
	-- 8. @TransAliases: List of comma separated TransactionTypes
	-- 9. @SiteGuid: Identifies the site the report is being run from
	-- 10. @UserGuid: Identifies the user running the report
    -- Updates:
    -- Date         Version     User        Description
    -- ----         -------     ----        -----------
    -- 11-03-2019   1.0.004     Jay R       Get DocumentNumber from Transaction and not from LineItems
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		-- Trim off passed in hours and minute, add seconds until end of day
	    SET @ToDate = DATEADD(second,86399,DATEADD(dd, DATEDIFF(dd, 0,@ToDate), 0))

		DECLARE @SiteGroupLevelVolumeUnitIndex INT
		DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

		SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
			@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
		FROM tblSites 
		WHERE SiteGuid = @SiteGuid

		SELECT
		CAST(b.InventoryDate AS DATE) AS InventoryDate,
		b.Site,
		CASE b.LookupTransTypeIndex
			WHEN 4 THEN 'Defuel' -- WHEN T4_SecondaryDefuel
			WHEN 5 THEN 'Issue'  -- WHEN T5_PrimaryDisbursement
			WHEN 6 THEN 'Bulk Issue' -- WHEN T6_SecondaryDisbursement
			ELSE 'UNKNOWN' 
			END AS AliasName,
		b.ManagerID,
		b.OwnerID, 
		b.CarrierID, 
		b.ShipToID, 
		a.Product,  
		s.City as SiteCity,
		s.State as SiteState,
		b.DocumentNumber AS ShipmentNumber, 
		b.DestinationRegistrationID1, 
		b.RoutingID, 
		b.SourceRegistrationID1, 
		b.DestinationEquipmentModel1,
		b.SourceEquipmentModel1,
		a.MeterStart, 
		a.MeterStop, 
		CASE b.LookupTransTypeIndex
			WHEN 4 then (-1 * (dbo.udf_ConvertFromSIUnits(ABS(a.GrossQuantity), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces))) -- WHEN T4_SecondaryDefuel
			ELSE dbo.udf_ConvertFromSIUnits(ABS(a.GrossQuantity), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) 
			END AS GrossQuantity,
		CASE b.LookupTransTypeIndex
			WHEN 4 then (-1 * (dbo.udf_ConvertFromSIUnits(ABS(a.NetQuantity), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces))) -- WHEN T4_SecondaryDefuel
			ELSE  dbo.udf_ConvertFromSIUnits(ABS(a.NetQuantity), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) 
			END AS NetQuantity,
		1 AS FlightCnt,
		d.Notes,
		b.DeleteFlag,
		lookup.tblEngineeringUnit.EngineeringUnitName AS VolumeUnitName,
		@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces
		FROM tblTransactionLineItems a
		INNER JOIN tblTransactions b 		ON b.TransactionGuid = a.TransactionGuid
		INNER JOIN tblSites s				ON b.SiteGuid = s.SiteGuid
		LEFT JOIN dbo.tblTransactionNotes d ON b.TransactionGuid = d.TransactionGuid
		INNER JOIN lookup.tblEngineeringUnit ON lookup.tblEngineeringUnit.EngineeringUnitIndex = @SiteGroupLevelVolumeUnitIndex
		WHERE b.InventoryDate BETWEEN @FromDate AND @ToDate
		AND b.LookupTransTypeIndex in (SELECT Num AS LookupTransTypeIndex FROM rpt.udf_GetIntTableFromStringList(@TransAliases))
		AND b.LookupTransactionStatusIndex IN (select s.TransactionStatusIndex from [rpt].[udf_GetEnterpriseStatusTable](@EnterpriseStatus) s) 
		AND b.OwnerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Owners) c)
		AND b.ManagerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Managers) c)
		AND a.ProductGuid = @Product
		AND b.SiteGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
		AND b.ShipToCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Consumers) c)
		AND (b.DeleteFlag = CAST(0 AS BIT))
		AND (b.ReversalType IS NULL OR b.ReversalType = '' OR b.ReversalType = 'O')  
		AND EXISTS (SELECT *
			FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
			WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
			OR authorizedCompaniesGuids.CompanyGuid IN (b.ShipToCompanyGuid, b.SupplierCompanyGuid, b.ShipperCompanyGuid, b.OwnerCompanyGuid, b.ManagerCompanyGuid, b.CarrierCompanyGuid, b.BillToCompanyGuid)) 
		ORDER BY Product, OwnerID, ShipToID, InventoryDate, AliasName, RoutingID
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
						+ 'Procedure Name: [rpt].usp_OwnerConsumptionSummary' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END