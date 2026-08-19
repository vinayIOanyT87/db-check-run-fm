/*
    DROP PROCEDURE [Staging].[usp_PresetTransactionSummary]

	EXEC [staging].[usp_PresetTransactionLineSummary]
	
*/
CREATE PROCEDURE [staging].[usp_PresetTransactionSummary]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_PresetTransactionSummary]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Load the staging.tblTransactionSummary, and set all the extra fields that were added to the table and that are not populated 
  --          from the corresponding tblTransactions OLTP table.
  -- Notes:
  -- 1. The factTransactionSummary (Level 3) table captures summary measures (e.g. Line_MeterMinStartMaxStopTimeDiff) at the transaction header level,
  --    i.e. it provides a consolidated set of values for each TransactionKey.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY
    DECLARE @dummyDate datetime = '1/1/1900'
    DECLARE @currentDate datetime = GetDate()

    DECLARE @tblSegmentFactSKey TABLE
	(
        [TransactionKey] [nvarchar](50) NULL,
		[CloneMasterFactTransactionSKey] int NULL
	);

    --Load the transaction summary table from the staging transaction header table, taking advantage of all the references and ETL operations that have already been applied to the header table.
    INSERT INTO staging.tblTransactionSummary
    (
        BillToCompanyKey,
        BillToCompanySKey,
        CarrierCompanyKey,
        CarrierCompanySKey,
        DeleteFlag,
        DestinationEquipment1Key,
        DestinationEquipment1SKey,
        DocumentNumber,
        InventoryDate,
        InventoryDateSKey,
        ManagerCompanyKey,
        ManagerCompanySKey,
        OperatorPersonnelKey,
        OperatorPersonnelSKey,
        OwnerCompanyKey,
        OwnerCompanySKey,
        ReasonCodeKey,
        ReasonCodeSKey,
        ReversalType,
        ShipperCompanyKey,
        ShipperCompanySKey,
        ShipToCompanyKey,
        ShipToCompanySKey,
        SiteKey,
        SiteSKey,
        SourceEquipment1Key,
	    SourceEquipment1SKey,
        SubType,
        SupplierCompanyKey,
        SupplierCompanySKey,
        TimeIn,
        TimeInDateSKey,
        TimeInTimeSKey,
        TimeOut,
        TimeOutDateSKey,
        TimeOutTimeSKey,
        TransactionAliasKey,
        TransactionAliasSKey,
        TransactionKey,
        TransactionStatusIndex,
        TransactionStatusName,
        TransactionTypeKey,
        TransactionTypeSKey,        
        TransDateTime,
        TransDateSKey,
        TransTimeSKey,
        TransID,

        RecordUpdatedDate,
	    CombinedUpdatedDate,
	    CombinedUpdatedDateSKey,
	    IsRecordDeleted,
	    IsRecordAddedByETL,
	    SourceFactSKey,
	    IgnoreRecord,
	    CDCSKey,
	    SourceRowVersion,
	    CDCRowVersion
    )
    SELECT
        BillToCompanyKey,
        BillToCompanySKey,
        CarrierCompanyKey,
        CarrierCompanySKey,
        DeleteFlag,
        DestinationEquipment1Key,
        DestinationEquipment1SKey,
        DocumentNumber,
        InventoryDate,
        InventoryDateSKey,
        ManagerCompanyKey,
        ManagerCompanySKey,
        OperatorPersonnelKey,
        OperatorPersonnelSKey,
        OwnerCompanyKey,
        OwnerCompanySKey,
        ReasonCodeKey,
        ReasonCodeSKey,
        ReversalType,
        ShipperCompanyKey,
        ShipperCompanySKey,
        ShipToCompanyKey,
        ShipToCompanySKey,
        SiteKey,
        SiteSKey,
        SourceEquipment1Key,
	    SourceEquipment1SKey,
        SubType,
        SupplierCompanyKey,
        SupplierCompanySKey,
        TimeIn,
        TimeInDateSKey,
        TimeInTimeSKey,
        TimeOut,
        TimeOutDateSKey,
        TimeOutTimeSKey,
        TransactionAliasKey,
        TransactionAliasSKey,
        TransactionKey,
        TransactionStatusIndex,
        TransactionStatusName,
        TransactionTypeKey,
        TransactionTypeSKey,
        TransDateTime,
        TransDateSKey,
        TransTimeSKey,
        TransID,

        RecordUpdatedDate,
	    CombinedUpdatedDate,
	    CombinedUpdatedDateSKey,
	    IsRecordDeleted,
	    IsRecordAddedByETL,
	    SourceFactSKey,
	    IgnoreRecord,
	    CDCSKey,
	    SourceRowVersion,
	    CDCRowVersion
    FROM staging.tblTransactions
    WHERE IgnoreRecord = 0



    --For partial LineItem and SubLineItem segments with missing headers, load the staging transaction summary table from the FactTransactionSummary table, using the source FactTransaction record that was identified to be used to build/clone the record for the partial segment.
    INSERT INTO @tblSegmentFactSKey
	(TransactionKey, CloneMasterFactTransactionSKey)
	SELECT b.TransactionKey, b.SKey
	FROM
	(
		SELECT DISTINCT a.SourceFactTransactionSKey SourceFactTransactionSKey
		FROM
		(
			SELECT MAX(SourceFactTransactionSKey) SourceFactTransactionSKey FROM staging.tblPartialTransactionSegment 
			WHERE SegmentType IN ('LineItem', 'SubLineItem')
			AND IsNewMainSegment = 1 
            AND MissingSegmentType = 'Header'
			GROUP BY RecordKey
		) a
	) x
	INNER JOIN dbo.FactTransaction b
	ON b.SKey = x.SourceFactTransactionSKey

	
	INSERT INTO staging.tblTransactionSummary
    (
        Line_MeterMaxStopTime,
        Line_MeterMinStartTime,

        BillToCompanyKey,
        BillToCompanySKey,
        CarrierCompanyKey,
        CarrierCompanySKey,
        DeleteFlag,
        DestinationEquipment1Key,
        DestinationEquipment1SKey,
        DocumentNumber,
        InventoryDate,
        InventoryDateSKey,
        ManagerCompanyKey,
        ManagerCompanySKey,
        OperatorPersonnelKey,
        OperatorPersonnelSKey,
        OwnerCompanyKey,
        OwnerCompanySKey,
        ReasonCodeKey,
        ReasonCodeSKey,
        ReversalType,
        ShipperCompanyKey,
        ShipperCompanySKey,
        ShipToCompanyKey,
        ShipToCompanySKey,
        SiteKey,
        SiteSKey,
        SourceEquipment1Key,
	    SourceEquipment1SKey,
        SubType,
        SupplierCompanyKey,
        SupplierCompanySKey,
        TimeIn,
        TimeInDateSKey,
        TimeInTimeSKey,
        TimeOut,
        TimeOutDateSKey,
        TimeOutTimeSKey,
        TransactionAliasKey,
        TransactionAliasSKey,
        TransactionKey,
        TransactionStatusIndex,
        TransactionStatusName,
        TransactionTypeKey,
        TransactionTypeSKey,        
        TransDateTime,
        TransDateSKey,
        TransTimeSKey,
        TransID,

        RecordUpdatedDate,
	    CombinedUpdatedDate,
	    CombinedUpdatedDateSKey,
	    IsRecordDeleted,
	    IsRecordAddedByETL,
	    SourceFactSKey,
	    IgnoreRecord,
	    CDCSKey,
	    SourceRowVersion,
	    CDCRowVersion
    )
    SELECT
        a.Line_MeterMaxStopTime,
        a.Line_MeterMinStartTime,

        NULL BillToCompanyKey,
        a.BillToCompanySKey,
        NULL CarrierCompanyKey,
        a.CarrierCompanySKey,
        a.DeleteFlag,
        NULL DestinationEquipment1Key,
        a.DestinationEquipment1SKey,
        NULL DocumentNumber,
        NULL InventoryDate,
        a.InventoryDateSKey,
        NULL ManagerCompanyKey,
        a.ManagerCompanySKey,
        NULL OperatorPersonnelKey,
        a.OperatorPersonnelSKey,
        NULL OwnerCompanyKey,
        a.OwnerCompanySKey,
        NULL ReasonCodeKey,
        a.ReasonCodeSKey,
        a.ReversalType,
        NULL ShipperCompanyKey,
        a.ShipperCompanySKey,
        NULL ShipToCompanyKey,
        a.ShipToCompanySKey,
        NULL SiteKey,
        a.SiteSKey,
        NULL SourceEquipment1Key,
	    a.SourceEquipment1SKey,
        a.SubType,
        NULL SupplierCompanyKey,
        a.SupplierCompanySKey,
        a.TimeIn,
        a.TimeInDateSKey,
        a.TimeInTimeSKey,
        a.TimeOut,
        a.TimeOutDateSKey,
        a.TimeOutTimeSKey,
        NULL TransactionAliasKey,
        a.TransactionAliasSKey,
        a.TransactionKey,
        a.TransactionStatusIndex,
        a.TransactionStatusName,
        NULL TransactionTypeKey,
        a.TransactionTypeSKey,
        a.TransDateTime,
        a.TransDateSKey,
        a.TransTimeSKey,
        a.TransID,

        a._RecordUpdatedDate,
	    a._RecordUpdatedDate CombinedUpdatedDate,
	    a._RecordUpdatedDateSKey,
	    a.DeleteFlag,
	    0 IsRecordAddedByETL,
	    NULL SourceFactSKey,
	    0 IgnoreRecord,
	    NULL CDCSKey,
	    NULL SourceRowVersion,
	    NULL CDCRowVersion
    FROM FactTransactionSummary a
    INNER JOIN @tblSegmentFactSKey b
    ON b.TransactionKey = a.TransactionKey
    WHERE NOT EXISTS
    (
        SELECT * FROM staging.tblTransactionSummary c
        WHERE c.TransactionKey = a.TransactionKey
    )




    --Set the transaction summary measures

    --Fetch the existing MeterMinStartTime and MeterMaxStopTime from the FactTransactionSummary table if they exist
    UPDATE a
    SET a.Line_MeterMinStartTime = b.Line_MeterMinStartTime,
    a.Line_MeterMaxStopTime = b.Line_MeterMaxStopTime
    FROM staging.tblTransactionSummary a
    INNER JOIN dbo.FactTransactionSummary b
    ON b.TransactionKey = a.TransactionKey
    WHERE EXISTS
    (
        SELECT * FROM staging.tblTransactions c
        WHERE c.TransactionKey = a.TransactionKey
        AND c.IgnoreRecord = 0
    )
    AND a.IgnoreRecord = 0    

    -- Update/set the MeterMinStartTime and MeterMaxStopTime 
    UPDATE a
    SET a.Line_MeterMinStartTime = (CASE WHEN ISNULL(b.MeterMinStartTime, @dummyDate) < ISNULL(a.Line_MeterMinStartTime, @currentDate) THEN b.MeterMinStartTime ELSE a.Line_MeterMinStartTime END),
    a.Line_MeterMaxStopTime = (CASE WHEN ISNULL(b.MeterMaxStopTime, @dummyDate) > ISNULL(a.Line_MeterMaxStopTime, @dummyDate) THEN b.MeterMaxStopTime ELSE a.Line_MeterMaxStopTime END)
    FROM staging.tblTransactionSummary a
    INNER JOIN
    (
        SELECT x.TransactionKey, MIN(x.MeterMinStartTime) MeterMinStartTime, MAX(x.MeterMaxStopTime) MeterMaxStopTime
        FROM
        (
            SELECT TransactionKey, MIN(MeterStartDateTime) MeterMinStartTime, MAX(MeterStopDateTime) MeterMaxStopTime
            FROM staging.tblTransactionLineItems
            WHERE IgnoreRecord = 0
            AND DeleteFlag = 0
            AND IsRecordDeleted = 0
            GROUP BY TransactionKey
            UNION ALL
            SELECT TransactionKey, MIN(MeterStartDateTime) MeterMinStartTime, MAX(MeterStopDateTime) MeterMaxStopTime
            FROM staging.tblTransactionSubLineItems
            WHERE IgnoreRecord = 0
            AND DeleteFlag = 0
            AND IsRecordDeleted = 0
            GROUP BY TransactionKey
        ) x
        GROUP BY x.TransactionKey
    ) b
    ON b.TransactionKey = a.TransactionKey
    WHERE a.IgnoreRecord = 0
    AND a.DeleteFlag = 0
    AND a.IsRecordDeleted = 0



    --Set the transaction summary terminal time measures
    UPDATE staging.tblTransactionSummary
    SET [TimeInTimeOutDiff] = 0
    WHERE IgnoreRecord = 0
    AND (TimeIn IS NULL OR TimeOut IS NULL)

    UPDATE staging.tblTransactionSummary
    SET [TimeInTimeOutDiff] = DATEDIFF(Minute, TimeIn, TimeOut)
    WHERE IgnoreRecord = 0
    AND TimeIn IS NOT NULL
    AND TimeOut IS NOT NULL
    AND IgnoreRecord = 0

    UPDATE Staging.tblTransactionSummary
    SET Line_TimeInMinMeterStartDiff = 0
    WHERE IgnoreRecord = 0
    AND (Line_MeterMinStartTime IS NULL OR TimeIn IS NULL)

    UPDATE Staging.tblTransactionSummary
    SET Line_MeterMinStartMaxStopTimeDiff = 0
    WHERE IgnoreRecord = 0
    AND (Line_MeterMinStartTime IS NULL OR Line_MeterMaxStopTime IS NULL)

    UPDATE Staging.tblTransactionSummary
    SET Line_MaxMeterStopTimeOutDiff = 0
    WHERE IgnoreRecord = 0
    AND (Line_MeterMaxStopTime IS NULL OR TimeOut IS NULL)

    UPDATE Staging.tblTransactionSummary
    SET Line_TimeInMinMeterStartDiff = DATEDIFF(Minute, TimeIn, Line_MeterMinStartTime)
    WHERE Line_MeterMinStartTime IS NOT NULL
    AND TimeIn IS NOT NULL
    AND IgnoreRecord = 0

    UPDATE Staging.tblTransactionSummary
    SET Line_MeterMinStartMaxStopTimeDiff = DATEDIFF(Minute, Line_MeterMinStartTime, Line_MeterMaxStopTime)
    WHERE Line_MeterMinStartTime IS NOT NULL
    AND Line_MeterMaxStopTime IS NOT NULL
    AND IgnoreRecord = 0

    UPDATE Staging.tblTransactionSummary
    SET Line_MaxMeterStopTimeOutDiff = DATEDIFF(Minute, Line_MeterMaxStopTime, TimeOut)
    WHERE Line_MeterMaxStopTime IS NOT NULL
    AND TimeOut IS NOT NULL
    AND IgnoreRecord = 0


  END TRY
  BEGIN CATCH
    DECLARE @_ErrMessage nvarchar(2048),
            @_ErrNumber int,
            @_ErrProcName nvarchar(126),
            @_ErrLineNumber int;
    SET @_ErrMessage = ERROR_MESSAGE();
    SET @_ErrNumber = ERROR_NUMBER();
    SET @_ErrProcName = ERROR_PROCEDURE();
    SET @_ErrLineNumber = ERROR_LINE();
    SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
    + 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
    + 'Procedure Name: [staging].[usp_PresetTransactionSummary]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END