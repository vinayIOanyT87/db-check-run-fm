USE [FuelsManagerDB]
GO

-- Create Index on ConjoinTransID for searching associated owners on Transfer transactions
IF EXISTS (SELECT 1 FROM sys.indexes AS si JOIN sys.objects AS so on si.object_id=so.object_id JOIN sys.schemas AS sc on so.schema_id=sc.schema_id
        WHERE sc.name='dbo' AND so.name ='tblTransactions' AND si.name='IXU_tblTransactions_ConjoinTransID')
    DROP INDEX [IXU_tblTransactions_ConjoinTransID] ON [dbo].[tblTransactions]
GO

SET ANSI_PADDING ON
GO

CREATE NONCLUSTERED INDEX [IXU_tblTransactions_ConjoinTransID] ON [dbo].[tblTransactions]
(
	[ConjoinTransID] ASC,
	[ManagerCompanyGuid] ASC
)
INCLUDE ( 	[TransactionGuid],
	[LookupTransTypeIndex],
	[InventoryDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO
-- End Index

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[rpt].[usp_ConsumptionDetailReport]') AND type IN (N'P', N'PC'))
	DROP PROCEDURE [rpt].[usp_ConsumptionDetailReport]
GO

CREATE PROCEDURE [rpt].[usp_ConsumptionDetailReport] 
(
	@Sites NVARCHAR(MAX),
	@Managers NVARCHAR(MAX),
	@Owners NVARCHAR(MAX),
	@Consumers NVARCHAR(MAX),
	@Vendors NVARCHAR(MAX),
	@Product UNIQUEIDENTIFIER,
	@FromDate DATETIMEOFFSET(7),
	@ToDate DATETIMEOFFSET(7),
	@TransAliases NVARCHAR(100),
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER,
    @LastRunTime DATETIME = NULL
)
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	/*------------------------------------------------------------------------------------------------------
	 Stored Procedure: [rpt].[usp_ConsumptionDetailReport] 
	 Author: Shawn Marlin
	 Purpose: Retrieve the transaction records for the Daily Consumption Report
	 Notes:
	 1. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	 2. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	 3. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	 4. @Consumers: List of company MasterRecordGuids assigned the role of consumer that the transactions list as the consumer for itself to be included in the results
	 5. @Vendors: List of company MasterRecordGuids assigned the role of vendor that the transactions list as the vendor for itself to be included in the results
	 6. @Product: Product MasterRecordGuid that the transactions list as the product for itself to be included in the results
	 7. @FromDate: Lower bound date to collect transactions meeting criteria
	 8. @ToDate: Upper bound date to collect transactions meeting criteria
	 9. @TransAliases: List of comma separated TransactionTypes
	 10. @SiteGuid: Identifies the site the report is being run from
	 11. @UserGuid: Identifies the user running the report
    
    Modification History:
    Date        Version     By          Description
    ----------  -------     ----        -------------
    04/02/2013  1.0.003     Shawn M     --
    07/01/2019  1.0.004     Jay R       Fixed calculation of Gross and Net Quantities when value is negative
                                        Added support for BBL (barrels) and receipt notes
                                        Added @LastRunTime parameter used for Managed report subscriptions
                                            If value is not empty, the transaction's UpdatedDate is used for date range,
                                            not the InventoryDate. The report passes the LastRunTime of the report's
                                            subscription if the report's ManagedReport parameter is set to 'True'. If 'False',
                                            a Null value is passed and the InventoryDate value is used. Uses efficient Boolean
                                            logic to check for the above (https://weblogs.sqlteam.com/jeffs/2003/11/14/513/)
                                            This should not affect existing reports since this parameter was defined with a 
                                            default Null value.
                                        Added condition to ignore Vendors and Consumers for the 2 new transaction
                                            types, Adjustments and Transfers.
                                        Added join to tblTransactions on ConjoinTransID to get Associated Owner on Transfer transactions
                                        Added column for Supplier on Receipts transactions
    08/02/2019  1.0.005     Jay R       Added Flight date column
    11/03/2019  1.0.005     Jay R       Get DocumentNumber from Transaction and not from LineItems
	------------------------------------------------------------------------------------------------------*/

	BEGIN TRY
	SET NOCOUNT ON
	
		Set @ToDate = DATEADD(second,86399,DATEADD(dd, DATEDIFF(dd, 0,@ToDate), 0))

		DECLARE @SiteGroupLevelVolumeUnitIndex INT
		DECLARE @SiteGroupLevelVolumeDecimalPlaces INT
		DECLARE @SiteGroupLevelDensityUnitIndex INT
		DECLARE @SiteGroupLevelDensityDecimalPlaces INT
		DECLARE @SiteGroupLevelTemperatureUnitIndex INT
		DECLARE @SiteGroupLevelTemperatureDecimalPlaces INT
        DECLARE @SiteGroupLevelBarrelUnitIndex INT

		SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
			@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0),
			@SiteGroupLevelDensityUnitIndex = ISNULL(DensityUnitIndex, 187),
			@SiteGroupLevelDensityDecimalPlaces = ISNULL(DensityDecimalPlaces, 0),
			@SiteGroupLevelTemperatureUnitIndex = ISNULL(TemperatureUnitIndex, 2),
			@SiteGroupLevelTemperatureDecimalPlaces = ISNULL(TemperatureDecimalPlaces, 1)
		FROM tblSites 
		WHERE SiteGuid = @SiteGuid

        -- Barrel units
        SELECT @SiteGroupLevelBarrelUnitIndex = 48

		SELECT 
            CAST(t.InventoryDate AS DATE) AS InventoryDate,
			t.LookupTransTypeIndex,		
			t.ReversalType,
			t.ShipToID AS Consumer, 
			l.Product AS Product,
			t.DocumentNumber AS MeterTicket,
			-- Tail # is DestinationSerialNumber2 for issue, DestinationSerialNumber3 for defuel
			(CASE WHEN t.LookupTransTypeIndex = 4 THEN t.DestinationSerialNumber3
			WHEN t.LookupTransTypeIndex = 5 THEN t.DestinationSerialNumber2 ELSE NULL END)AS TailNum,
			l.DestinationEquipmentModel as DestEqType,
			t.RoutingID AS FlightNum,
			t.SourceRegistrationID1 AS VehicleID,
			l.MeterStart AS Start,
			l.MeterStop AS Stop,
			(-1 * dbo.udf_ConvertFromSIUnits(ISNULL(l.GrossQuantity,0.0), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)) AS Gross,
			(-1 * dbo.udf_ConvertFromSIUnits(ISNULL(l.NetQuantity, 0.0), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)) AS Net,
			g.EngineeringUnitName AS VolumeUnitName,
            (-1 * dbo.udf_ConvertFromSIUnits(ISNULL(l.GrossQuantity,0.0), @SiteGroupLevelBarrelUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)) AS GrossBBL,
			(-1 * dbo.udf_ConvertFromSIUnits(ISNULL(l.NetQuantity, 0.0), @SiteGroupLevelBarrelUnitIndex, @SiteGroupLevelVolumeDecimalPlaces)) AS NetBBL,
			b.EngineeringUnitName AS BarrelUnitName,
            l.Vcf AS VCF,
			u.UserData2 as SubType2Code,
			t.NextStationIATAID as Destination,
			t.OwnerID AS Owner,
            t2.OwnerID AS AssociatedOwner,
			t.CarrierID AS IntoPlaneAgent,
			t.ManagerID AS Manager,
            t.SupplierID AS Supplier,
			1 AS FltCnt,
            uli.UserData13 AS FlightDate,
			s.ID AS SiteID,
			s.Phone,
			s.Address1,
			s.Address2,
			CASE WHEN CONCAT(s.City, s.State, s.Zip) = '' THEN NULL 
				ELSE CONCAT(s.City, ', ', s.State, ' ', s.Zip) 
			END AS CityStateZip,
			s.Country,
			CASE t.LookupTransTypeIndex
                WHEN 1 THEN 'Adjustment'
				WHEN 4 THEN 'Defuel'
				WHEN 5 THEN 'Issue'
				WHEN 6 THEN 'Bulk Issue'
                WHEN 8 THEN 'Receipt'
                WHEN 13 THEN 'Transfer'
				ELSE 'Invalid' 
			END AS AliasName,	
            n.Notes,
			@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces,
			@SiteGroupLevelDensityDecimalPlaces AS DensityDecimalPlaces,
			@SiteGroupLevelTemperatureDecimalPlaces AS TemperatureDecimalPlaces,
            t.DeleteFlag
		FROM tblTransactionLineItems l
		INNER JOIN tblTransactions t ON t.TransactionGuid = l.TransactionGuid
		LEFT JOIN tblTransactionUserData u ON t.TransactionGuid = u.TransactionGuid
        LEFT JOIN tblTransactionLineItemUserData uli ON uli.TransactionLineItemGuid = l.TransactionLineItemGuid
        LEFT JOIN tblTransactionNotes n ON n.TransactionGuid = t.TransactionGuid
        LEFT JOIN tblTransactions t2 ON t2.ConjoinTransID = t.TransID
		INNER JOIN tblSites s ON s.SiteGuid = t.SiteGuid
		INNER JOIN lookup.tblEngineeringUnit g ON g.EngineeringUnitIndex = @SiteGroupLevelVolumeUnitIndex
        INNER JOIN lookup.tblEngineeringUnit b ON b.EngineeringUnitIndex = @SiteGroupLevelBarrelUnitIndex
		WHERE 
		    t.SiteGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
			AND t.ManagerCompanyGuid IN (SELECT m.Guid FROM rpt.udf_GetTableFromStringList(@Managers) m)			
			AND t.OwnerCompanyGuid IN (SELECT o.Guid FROM rpt.udf_GetTableFromStringList(@Owners) o)
			AND ((t.CarrierCompanyGuid IN (SELECT v.Guid FROM rpt.udf_GetTableFromStringList(@Vendors) v))
                OR (t.LookupTransTypeIndex IN (1, 8, 13) AND t.CarrierCompanyGuid IS NULL))
			AND ((t.ShipToCompanyGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Consumers) c))
                OR (t.LookupTransTypeIndex IN (1, 8, 13) AND t.ShipToCompanyGuid IS NULL))
			AND (t.DeleteFlag = CAST(0 AS BIT))
            AND (@LastRunTime IS NOT NULL OR t.InventoryDate BETWEEN @FromDate AND @ToDate)
            AND (@LastRunTime IS NULL OR t.UpdatedDate BETWEEN @LastRunTime AND @ToDate)
			AND t.LookupTransTypeIndex IN (SELECT Num AS LookupTransTypeIndex FROM rpt.udf_GetIntTableFromStringList(@TransAliases))
			AND (t.ReversalType IS NULL OR t.ReversalType = '' OR t.ReversalType = 'O')
			AND l.ProductGuid = @Product
			AND EXISTS (SELECT *
				FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
				WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
				OR authorizedCompaniesGuids.CompanyGuid IN (t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid)) 
		ORDER BY Consumer, Product, InventoryDate, FlightNum

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
						+ 'Procedure Name: [rpt].usp_ConsumptionSummaryReport' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END
GO
