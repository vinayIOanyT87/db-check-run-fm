CREATE Procedure [rpt].[usp_GlycolIssue] 
(
	@Sites nvarchar(max),
	@Managers nvarchar(max),
	@Owners nvarchar(max),
	@Vendors nvarchar(max),
	@Consumers nvarchar(max),
	@FromDate DATETIMEOFFSET(7),
	@ToDate DATETIMEOFFSET(7),
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER
)

AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_GlycolIssue] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Glycol Issue Report
	-- Notes:
	-- 1. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 2. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	-- 3. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 4. @Vendors: List of company MasterRecordGuids assigned the role of carrier that the transactions list as the carrier for itself to be included in the results
	-- 5. @Consumers: List of company MasterRecordGuids assigned the role of shipto that the transactions list as the shipto company for itself to be included in the results
	-- 6. @FromDate: Lower bound date to collect transactions meeting criteria
	-- 7. @ToDate: Upper bound date to collect transactions meeting criteria
	-- 8. @SiteGuid: Identifies the site the report is being run from
	-- 9. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT	CAST(b.InventoryDate AS DATE) AS InventoryDate,
		b.Site,
		b.AliasName,
		a.DocumentNumber AS ShipmentNumber, 
		a.Product,
		e.UserData23 AS RoutingID,
		e.UserData3 AS Blend,
		CASE IsNull(b.InternationalRouteIndicator,0) WHEN 0 
		THEN 'DOM' 
		ELSE 'FTZ' 
		END AS InternationalRouteIndicator,
		b.DestinationRegistrationID1,
		b.DestinationEquipmentModel1,
		dbo.udf_ConvertFromSIUnits(abs(a.GrossQuantity), s.VolumeUnitIndex, s.VolumeDecimalPlaces) AS GrossQuantity,
		dbo.udf_ConvertFromSIUnits(abs(a.NetQuantity), s.VolumeUnitIndex, s.VolumeDecimalPlaces) AS NetQuantity,
		b.SourceRegistrationID1,
		b.ManagerID,
		b.OwnerID, 
		b.CarrierID, 
		b.ShipToID, 
		d.Notes,
		s.City as SiteCity,
		s.[State] as SiteState,
		lookup.tblEngineeringUnit.EngineeringUnitName AS VolumeUnitName
		FROM tblTransactionLineItems a 
		INNER JOIN tblTransactions b ON b.TransactionGuid = a.TransactionGuid
		LEFT JOIN dbo.tblTransactionNotes d ON b.TransactionGuid = d.TransactionGuid
		LEFT JOIN dbo.tblTransactionUserData e ON b.TransactionGuid = e.TransactionGuid
		LEFT JOIN tblSites s on b.SiteGuid = s.SiteGuid
		INNER JOIN lookup.tblEngineeringUnit ON lookup.tblEngineeringUnit.EngineeringUnitIndex = s.VolumeUnitIndex
		WHERE b.InventoryDate >= @FromDate AND b.InventoryDate < @ToDate 
		AND b.LookupTransTypeIndex = 12 -- T12_InventoryNotAffected
		AND Product LIKE ('%GL%')  
		AND b.OwnerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Owners) c)
		AND b.ManagerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Managers) c)
		AND b.SiteGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
		AND b.ShipToCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Consumers) c)
		AND b.CarrierCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Vendors) c)
		AND EXISTS (SELECT *
			FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
			WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
			OR authorizedCompaniesGuids.CompanyGuid IN (b.ShipToCompanyGuid, b.SupplierCompanyGuid, b.ShipperCompanyGuid, b.OwnerCompanyGuid, b.ManagerCompanyGuid, b.CarrierCompanyGuid, b.BillToCompanyGuid)) 
		ORDER BY InventoryDate, Site, ShipToID
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
						+ 'Procedure Name: [rpt].usp_GlycolIssue' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END