USE [FuelsManagerDB]
GO

-- check for current procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[rpt].[usp_MonthEndFuelReport]') AND type IN (N'P', N'PC'))
	DROP PROCEDURE [rpt].[usp_MonthEndFuelReport]
GO

/*=============================================
 Author:		Jay Reina
 Create date:	09/25/2019
 Description:	This stored procedure is used by the Aviation to display Month-End Fuel Summary report
 Version:		9.2.0.0

 Modification History:
 DATE           VER         USER        COMMENT
 03/15/2019     9.2.0.0     Jay R       Initial version of Month End Fuel Summary Report
=============================================**/
CREATE PROCEDURE [rpt].[usp_MonthEndFuelReport] (
	@SiteGuid NVARCHAR(50)
	,@ManagerGuid NVARCHAR(50)
	,@OwnerGuid NVARCHAR(50) = NULL
	,@ProductGuid NVARCHAR(50)
	,@MonthYear NVARCHAR(20)
	)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @BeginDate DATETIMEOFFSET(7)
	SET @BeginDate = rpt.udf_MonthYearToTimestamp(@MonthYear)

	DECLARE @EndDate DATETIMEOFFSET(7)
	SET @EndDate = DATEADD(month, 1, @BeginDate)

    DECLARE @SiteGroupLevelVolumeUnitIndex INT
    DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

    SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
        @SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
    FROM tblSites 
    WHERE SiteGuid = @SiteGuid

    -- if @OwnerGuid parameter is NULL, build list 
    -- of all Owner GUIDs based on parameters 
    DECLARE @OwnerGuids NVARCHAR(4000)
    IF @OwnerGuid IS NULL
    BEGIN
        SELECT @OwnerGuids = (
		    SELECT SUBSTRING((
		        SELECT DISTINCT ',' + CONVERT(NVARCHAR(50), t.OwnerCompanyGuid) AS 'text()'
		        FROM tblTransactions t
		        INNER JOIN tblTransactionLineItems tli ON tli.TransactionGuid = t.TransactionGuid
		        WHERE t.SiteGuid = @SiteGuid
			        AND t.ManagerCompanyGuid = @ManagerGuid
			        AND t.OwnerID IS NOT NULL
			        AND (
				        t.InventoryDate >= @BeginDate
				        AND t.InventoryDate < @EndDate
				        )
			        AND tli.ProductGuid = @ProductGuid
			        AND t.DeleteFlag = 0
		        FOR XML PATH('')
		        ), 2, 4000) AS OwnerGuids
		    )
    END
    ELSE
    -- otherwise, use the @OwnerGuid passed in
    BEGIN
        SET @OwnerGuids = @OwnerGuid
    END

    -- temp table used to store Ledger details
    DECLARE @JournalResults TABLE (
	    OwnerID NVARCHAR(100)
	    ,ManagerID NVARCHAR(100)
	    ,SiteID NVARCHAR(100)
	    ,Address1 NVARCHAR(30)
	    ,Address2 NVARCHAR(30)
	    ,CityStateZip NVARCHAR(60)
	    ,Country NVARCHAR(30)
	    ,Phone NVARCHAR(20)
	    ,Issue_Total FLOAT
	    ,ID_24_Hr_Total FLOAT
	    ,Adjustment_Total FLOAT
	    ,Bulk_Issue_Total FLOAT
	    ,Defuel_Total FLOAT
	    ,Load_Rack_Total FLOAT
	    ,LR_Receipt_Total FLOAT
	    ,Receipt_Total FLOAT
	    ,Rotation_Total FLOAT
	    ,Transfer_Total FLOAT
	    ,[Begin Inventory] FLOAT
	    ,[Book Inventory] FLOAT
	    ,VolumeUnitName NVARCHAR(50)
	    ,VolumeDecimalPlaces TINYINT
	    ,ProductID NVARCHAR(30)
	)

    -- temp table to store Gross/Net details
    DECLARE @LocalResults TABLE (
	    OwnerID NVARCHAR(100)
	    ,GrossDefuel FLOAT
	    ,NetDefuel FLOAT
	    ,GrossBeginInventory FLOAT
	    ,NetBeginInventory FLOAT
	    ,GrossBookInventory FLOAT
	    ,NetBookInventory FLOAT
	)

    -- grab Gross details for owner(s)
    INSERT INTO @JournalResults
    EXEC rpt.usp_SummaryJournal @SiteGuid, @ManagerGuid, @OwnerGuids, @ProductGuid, @MonthYear, 1, @SiteGuid, NULL

    -- insert into Gross values only into temp table
    INSERT INTO @LocalResults
    SELECT OwnerID
        ,Defuel_Total
        ,NULL
        ,[Begin Inventory]
        ,NULL
        ,[Book Inventory]
        ,NULL
    FROM @JournalResults

    DELETE FROM @JournalResults

    -- grab Net details for owner(s)
    INSERT INTO @JournalResults
    EXEC rpt.usp_SummaryJournal @SiteGuid, @ManagerGuid, @OwnerGuids, @ProductGuid, @MonthYear, 0, @SiteGuid, NULL

    -- update Net values only in temp table based on OwnerID
    UPDATE lr
    SET lr.NetDefuel = j.Defuel_Total
        ,lr.NetBeginInventory = j.[Begin Inventory]
        ,lr.NetBookInventory = j.[Book Inventory]
    FROM @LocalResults lr
    INNER JOIN @JournalResults j ON j.OwnerID = lr.OwnerID  
    
    -- Owner detail
    SELECT OwnerID
		,NULL AS SupplierID
        ,GrossBeginInventory
        ,NetBeginInventory
        ,GrossBookInventory
        ,NetBookInventory
		,NULL AS GrossQuantity
		,NULL NetQuantity
        ,GrossDefuel
        ,NetDefuel
        ,NULL AS SubType2
        ,NULL AS Notes
        ,1 AS Header
	FROM @LocalResults

    UNION ALL

    -- Receipts
    SELECT t.OwnerID
		,t.SupplierID
        ,NULL AS GrossBeginInventory
        ,NULL AS NetBeginInventory
        ,NULL AS GrossBookInventory
        ,NULL AS NetBookInventory
		,dbo.udf_ConvertFromSIUnits(tli.GrossQuantity, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity
		,dbo.udf_ConvertFromSIUnits(tli.NetQuantity, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS NetQuantity
        ,NULL AS GrossDefuel
        ,NULL AS NetDefuel
        ,NULL AS SubType2
        ,NULL AS Notes
        ,2 AS Header
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems tli ON tli.TransactionGuid = t.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid
		AND t.ManagerCompanyGuid = @ManagerGuid
		AND t.OwnerCompanyGuid = COALESCE(@OwnerGuid, t.OwnerCompanyGuid)
		AND (t.InventoryDate >= @BeginDate AND t.InventoryDate < @EndDate)
		AND t.AliasName = 'Receipt'
		AND tli.ProductGuid = @ProductGuid
		AND t.DeleteFlag = 0

    UNION ALL

    -- Issues
	SELECT t.OwnerID
        ,NULL AS SupplierID
        ,NULL AS GrossBeginInventory
        ,NULL AS NetBeginInventory
        ,NULL AS GrossBookInventory
        ,NULL AS NetBookInventory
		,dbo.udf_ConvertFromSIUnits(tli.GrossQuantity * -1, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity
		,dbo.udf_ConvertFromSIUnits(tli.NetQuantity * -1, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS NetQuantity
        ,NULL AS GrossDefuel
        ,NULL AS NetDefuel
        ,tud.UserData2 AS SubType2
        ,NULL AS Notes
        ,3 AS Header
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems tli ON tli.TransactionGuid = t.TransactionGuid
    INNER JOIN tblTransactionUserData tud ON tud.TransactionGuid = t.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid
		AND t.ManagerCompanyGuid = @ManagerGuid
		AND t.OwnerCompanyGuid = COALESCE(@OwnerGuid, t.OwnerCompanyGuid)
		AND (t.InventoryDate >= @BeginDate AND t.InventoryDate < @EndDate)
		AND t.AliasName = 'Issue'
		AND tli.ProductGuid = @ProductGuid
		AND t.DeleteFlag = 0

    UNION ALL

    -- Bulk Issues
	SELECT t.OwnerID
        ,NULL AS SupplierID
        ,NULL AS GrossBeginInventory
        ,NULL AS NetBeginInventory
        ,NULL AS GrossBookInventory
        ,NULL AS NetBookInventory
		,dbo.udf_ConvertFromSIUnits(tli.GrossQuantity * -1, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity
		,dbo.udf_ConvertFromSIUnits(tli.NetQuantity * -1, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS NetQuantity
        ,NULL AS GrossDefuel
        ,NULL AS NetDefuel
        ,NULL AS SubType2
        ,NULL AS Notes
        ,4 AS Header
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems tli ON tli.TransactionGuid = t.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid
		AND t.ManagerCompanyGuid = @ManagerGuid
		AND t.OwnerCompanyGuid = COALESCE(@OwnerGuid, t.OwnerCompanyGuid)
		AND (t.InventoryDate >= @BeginDate AND t.InventoryDate < @EndDate)
		AND t.AliasName = 'Bulk Issue'
		AND tli.ProductGuid = @ProductGuid
		AND t.DeleteFlag = 0
    
    UNION ALL

    -- Adjustments
	SELECT t.OwnerID
        ,NULL AS SupplierID
        ,NULL AS GrossBeginInventory
        ,NULL AS NetBeginInventory
        ,NULL AS GrossBookInventory
        ,NULL AS NetBookInventory
		,dbo.udf_ConvertFromSIUnits(tli.GrossQuantity, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity
		,dbo.udf_ConvertFromSIUnits(tli.NetQuantity, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS NetQuantity
        ,NULL AS GrossDefuel
        ,NULL AS NetDefuel
        ,NULL AS SubType2
        ,tn.Notes
        ,5 AS Header
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems tli ON tli.TransactionGuid = t.TransactionGuid
    INNER JOIN tblTransactionNotes tn ON tn.TransactionGuid = t.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid
		AND t.ManagerCompanyGuid = @ManagerGuid
		AND t.OwnerCompanyGuid = COALESCE(@OwnerGuid, t.OwnerCompanyGuid)
		AND (t.InventoryDate >= @BeginDate AND t.InventoryDate < @EndDate)
		AND t.AliasName = 'Adjustment'
		AND tli.ProductGuid = @ProductGuid
		AND t.DeleteFlag = 0

    UNION ALL

    -- Transfers
	SELECT t.OwnerID
        ,NULL AS SupplierID
        ,NULL AS GrossBeginInventory
        ,NULL AS NetBeginInventory
        ,NULL AS GrossBookInventory
        ,NULL AS NetBookInventory
		,dbo.udf_ConvertFromSIUnits(tli.GrossQuantity, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity
		,dbo.udf_ConvertFromSIUnits(tli.NetQuantity, @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS NetQuantity
        ,NULL AS GrossDefuel
        ,NULL AS NetDefuel
        ,NULL AS SubType2
        ,NULL AS Notes
        ,6 AS Header
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems tli ON tli.TransactionGuid = t.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid
		AND t.ManagerCompanyGuid = @ManagerGuid
		AND t.OwnerCompanyGuid = COALESCE(@OwnerGuid, t.OwnerCompanyGuid)
		AND (t.InventoryDate >= @BeginDate AND t.InventoryDate < @EndDate)
		AND t.AliasName = 'Transfer'
		AND tli.ProductGuid = @ProductGuid
		AND t.DeleteFlag = 0

    ORDER BY OwnerID, Header

END
GO

GRANT EXECUTE
	ON [rpt].[usp_MonthEndFuelReport]
	TO [public]
GO
