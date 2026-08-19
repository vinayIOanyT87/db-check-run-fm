CREATE Procedure [rpt].[usp_GSEConsumption] 
(
	@Sites nvarchar(max),
	@Managers nvarchar(max),
	@Owners nvarchar(max),
	@Consumers nvarchar(max),
	@Product uniqueidentifier,
	@FromDate DATETIMEOFFSET(7),
	@ToDate DATETIMEOFFSET(7),
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER
)

AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_GSEConsumption] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the GSE Consumption Report
	-- Notes:
	-- 1. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 2. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list AS the manager for itself to be included in the results
	-- 3. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list AS the owner for itself to be included in the results
	-- 4. @Consumers: List of company MasterRecordGuids assigned the role of consumer that the transactions list AS the consumer for itself to be included in the results
	-- 5. @Product: Product MasterRecordGuid that the transactions list AS the product for itself to be included in the results
	-- 6. @FromDate: Lower bound date to collect transactions meeting criteria
	-- 7. @ToDate: Upper bound date to collect transactions meeting criteria
	-- 8. @SiteGuid: Identifies the site the report is being run from
	-- 9. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------
	BEGIN TRY		
		DECLARE @MasterProductGuid UNIQUEIDENTIFIER
		SELECT @MasterProductGuid=_MasterRecordGuid FROM tblProducts WHERE ProductGuid=@Product

		SELECT
			CAST(b.InventoryDate AS DATE) AS InventoryDate,
			b.Site,
			'Issue' AS AliasName,
			b.ManagerID,
			b.OwnerID, 
			b.CarrierID, 
			b.ShipToID, 
			a.Vcf AS VCF,
			dbo.udf_ConvertFromSIUnits(a.Temperature, s.TemperatureUnitIndex, s.TemperatureDecimalPlaces) AS Temperature,
			dbo.udf_ConvertFromSIUnits(a.Density, s.DensityUnitIndex, s.DensityDecimalPlaces) AS Density,
			a.Product,  
			a.DocumentNumber AS ShipmentNumber, 
			b.DestinationRegistrationID1, 
			b.DestinationEquipmentType1 AS RoutingID, 
			b.SourceRegistrationID1, 
			b.DestinationEquipmentModel1,
			b.SourceEquipmentModel1,
			a.MeterStart, 
			a.MeterStop, 
			dbo.udf_ConvertFromSIUnits(abs(a.GrossQuantity), s.VolumeUnitIndex, s.VolumeDecimalPlaces) AS GrossQuantity,
			dbo.udf_ConvertFromSIUnits(abs(a.NetQuantity), s.VolumeUnitIndex, s.VolumeDecimalPlaces) AS NetQuantity,
			1 AS FlightCnt,
			b.UpdatedDate AS ModifiedDate,
			b.DeleteFlag AS IsDeleted,
			d.Notes,
			s.City as SiteCity,
			s.[State] as SiteState,
			lookup.tblEngineeringUnit.EngineeringUnitName AS VolumeUnitName
		FROM tblTransactionLineItems a 
		INNER JOIN tblTransactions b  		   ON b.TransactionGuid = a.TransactionGuid
		INNER JOIN tblProducts p_trx  		   ON a.ProductGuid=p_trx.ProductGuid
		LEFT JOIN dbo.tblTransactionNotes d    ON b.TransactionGuid = d.TransactionGuid
		LEFT JOIN dbo.tblTransactionUserData e ON b.TransactionGuid = e.TransactionGuid
		LEFT JOIN tblSites s                   ON b.SiteGuid = s.SiteGuid
		INNER JOIN lookup.tblEngineeringUnit   ON lookup.tblEngineeringUnit.EngineeringUnitIndex = s.VolumeUnitIndex
		WHERE b.InventoryDate >= @FromDate AND b.InventoryDate < @ToDate
		AND p_trx._MasterRecordGuid = @MasterProductGuid
		AND b.LookupTransTypeIndex = 5 -- T5_PrimaryDisbursement
		AND b.OwnerCompanyGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Owners) c)
		AND b.ManagerCompanyGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Managers) c)
		AND (e.UserData2 = 'GSE' OR b.RoutingID = 'GSE')
		AND b.SiteGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
		AND b.ShipToCompanyGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Consumers) c)
		AND EXISTS (SELECT *
			FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
			WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
			OR authorizedCompaniesGuids.CompanyGuid IN (b.ShipToCompanyGuid, b.SupplierCompanyGuid, b.ShipperCompanyGuid, b.OwnerCompanyGuid, b.ManagerCompanyGuid, b.CarrierCompanyGuid, b.BillToCompanyGuid)) 
		ORDER BY site, ManagerID, OwnerID, CarrierID, ShipToID, Product, InventoryDate, AliasName, RoutingID
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
						+ 'Procedure Name: [rpt].usp_GSEConsumption' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END