CREATE PROCEDURE [rpt].[usp_AdjustmentReport] 
(
	@Sites NVARCHAR(MAX),
	@Managers NVARCHAR(MAX),
	@Owners NVARCHAR(MAX),
	@Product UNIQUEIDENTIFIER,
	@FromDate DATETIMEOFFSET(7),
	@ToDate DATETIMEOFFSET(7),
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER
)

AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_AdjustmentReport] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Adjustment Report
	-- Notes:
	-- 1. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 2. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	-- 3. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 4. @Product: ProductGuid associated with the transactions in the transaction list.
	-- 5. @FromDate: Lower bound date to collect transactions meeting criteria
	-- 6. @ToDate: Upper bound date to collect transactions meeting criteria
	-- 7. @SiteGuid: Identifies the site the report is being run from
	-- 8. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		-- Trim off passed in hours and minute, add seconds until end of day
		Set @ToDate = DATEADD(second,86399,DATEADD(dd, DATEDIFF(dd, 0,@ToDate), 0))

		DECLARE @SiteGroupLevelVolumeUnitIndex INT
		DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

		SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
			@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
		FROM tblSites 
		WHERE SiteGuid = @SiteGuid

		SELECT CAST(t.InventoryDate AS Date) AS InventoryDate
			,l.Product
			,t.AliasName
 			,t.ManagerID
			,t.OwnerID
			,t.CarrierID
			,s.ID as Site
			,isnull(mgr.Address1, '') + ' ' + isnull(mgr.Address2, '') AS Address
			,isnull( mgr.city,'') + ', ' + isnull( mgr.[State],'') + ' ' + isnull( mgr.Zip,'') + ' ' AS CityStateZip
			-- ,s.City AS SiteCity
			-- ,s.State AS SiteState
			,mgr.EmergencyContact
			,l.Vcf
			,l.Temperature
			,l.Density
			,u.UserData2 AS SubtypeCode2
			,u.UserData3 AS SubtypeCode3
			,l.DocumentNumber AS ShipmentNumber
			,l.DestinationRegistrationID
			,t.RoutingID
			,l.SourceRegistrationID
			,l.DestinationEquipmentModel
			,l.SourceEquipmentModel
			,l.MeterStart
			,l.MeterStop 
			,dbo.udf_ConvertFromSIUnits(l.GrossQuantity, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity
			,dbo.udf_ConvertFromSIUnits(l.NetQuantity, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS NetQuantity
			,1 AS FlightCnt
			,t.UpdatedDate AS ModifiedDate
			,t.DeleteFlag AS IsDeleted
			,n.Notes
			,r.ReasonCode
			,lookup.tblEngineeringUnit.EngineeringUnitName AS VolumeUnitName
			,@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces
		FROM tblTransactions t 
		INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
		INNER JOIN tblSites s ON t.SiteGuid = s.SiteGuid
		INNER JOIN lookup.tblEngineeringUnit ON lookup.tblEngineeringUnit.EngineeringUnitIndex = @SiteGroupLevelVolumeUnitIndex
		INNER JOIN tblCompanies mgr on mgr.CompanyGuid=t.ManagerCompanyGuid
		INNER JOIN lookup.tblTransactionStatus ts on t.LookupTransactionStatusIndex = ts.TransactionStatusIndex
		LEFT JOIN tblTransactionNotes n ON t.TransactionGuid = n.TransactionGuid
		LEFT JOIN tblTransactionUserData u ON t.TransactionGuid = u.TransactionGuid
		LEFT JOIN tblAutoDistributionReasonCodes r ON t.ReasonCodeGuid = r.AutoDistributionReasonCodeGuid
		WHERE t.InventoryDate >= @FromDate AND t.InventoryDate < @ToDate 
			AND t.SiteGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
			AND l.ProductGuid = @Product
			AND t.OwnerCompanyGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Owners) c)
			AND t.ManagerCompanyGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Managers) c)
			AND t.LookupTransTypeIndex = 1 -- T1 PrimaryAdjustment
			AND t.DeleteFlag = CAST(0 AS BIT)
			AND EXISTS (SELECT *
				FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
				WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
				OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) 
			AND ts.TransactionStatusCode IN ('Closed', 'Enterprise')
		ORDER BY t.OwnerID, l.Product, t.InventoryDate, t.AliasName, t.RoutingID

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
						+ 'Procedure Name: [rpt].usp_AdjustmentReport' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END