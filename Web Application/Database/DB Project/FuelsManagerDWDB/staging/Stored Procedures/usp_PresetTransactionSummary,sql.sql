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
  --    i.e. it provides a consolidated set of values for each transaction guid.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY
    DECLARE @dummyDate datetime = '1/1/1900'

    --Load the transaction summary table from the transaction header table, taking advantage of all the references and ETL operations that have already been applied to the header table.
    INSERT INTO staging.tblTransactionSummary
    (
        BillToCompanyGuid,
        BillToCompanySKey,
        CarrierCompanyGuid,
        CarrierCompanySKey,
        DeleteFlag,
        DestinationEquipment1Guid,
        DestinationEquipment1SKey,
        DocumentNumber,
        InventoryDate,
        InventoryDateSKey,
        ManagerCompanyGuid,
        ManagerCompanySKey,
        OperatorPersonnelGuid,
        OperatorPersonnelSKey,
        OwnerCompanyGuid,
        OwnerCompanySKey,
        ReasonCodeGuid,
        ReasonCodeSKey,
        ReversalType,
        ShipperCompanyGuid,
        ShipperCompanySKey,
        ShipToCompanyGuid,
        ShipToCompanySKey,
        SiteGuid,
        SiteSKey,
        SourceEquipment1Guid,
	    SourceEquipment1SKey,
        SubType,
        SupplierCompanyGuid,
        SupplierCompanySKey,
        TimeIn,
        TimeOut,
        TransactionAliasGuid,
        TransactionAliasSKey,
        TransactionStatusIndex,
        TransactionStatusName,
        TransactionTypeIndex,
        TransactionTypeSKey,
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
        BillToCompanyGuid,
        BillToCompanySKey,
        CarrierCompanyGuid,
        CarrierCompanySKey,
        DeleteFlag,
        DestinationEquipment1Guid,
        DestinationEquipment1SKey,
        DocumentNumber,
        InventoryDate,
        InventoryDateSKey,
        ManagerCompanyGuid,
        ManagerCompanySKey,
        OperatorPersonnelGuid,
        OperatorPersonnelSKey,
        OwnerCompanyGuid,
        OwnerCompanySKey,
        ReasonCodeGuid,
        ReasonCodeSKey,
        ReversalType,
        ShipperCompanyGuid,
        ShipperCompanySKey,
        ShipToCompanyGuid,
        ShipToCompanySKey,
        SiteGuid,
        SiteSKey,
        SourceEquipment1Guid,
	    SourceEquipment1SKey,
        SubType,
        SupplierCompanyGuid,
        SupplierCompanySKey,
        TimeIn,
        TimeOut,
        TransactionAliasGuid,
        TransactionAliasSKey,
        TransactionStatusIndex,
        TransactionStatusName,
        TransactionTypeIndex,
        TransactionTypeSKey,
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


    --Set the transaction summary measures

    --Fetch the existing MeterMinStartTime and MeterMaxStopTime from the FactTransactionSummary table if they exist
    UPDATE a
    SET a.Line_MeterMinStartTime = b.Line_MeterMinStartTime,
    a.Line_MeterMaxStopTime = b.Line_MeterMaxStopTime
    FROM staging.tblTransactionSummary a
    INNER JOIN dbo.FactTransactionSummary b
    ON b.TransactionGuid = b.TransactionGuid
    WHERE EXISTS
    (
        SELECT * FROM staging.tblTransactions c
        WHERE c.TransactionGuid = a.TransactionGuid
        AND c.IgnoreRecord = 0
    )
    AND a.IgnoreRecord = 0    

    -- Update/set the MeterMinStartTime and MeterMaxStopTime 
    UPDATE a
    SET a.Line_MeterMinStartTime = (CASE WHEN ISNULL(b.MeterMinStartTime, @dummyDate) < ISNULL(a.Line_MeterMinStartTime, @dummyDate) THEN b.MeterMinStartTime ELSE a.Line_MeterMinStartTime END),
    a.Line_MeterMaxStopTime = (CASE WHEN ISNULL(b.MeterMaxStopTime, @dummyDate) > ISNULL(a.Line_MeterMaxStopTime, @dummyDate) THEN b.MeterMaxStopTime ELSE a.Line_MeterMaxStopTime END)
    FROM staging.tblTransactionSummary a
    INNER JOIN
    (
        SELECT TransactionGuid, MIN(MeterStartDateTime) MeterMinStartTime, MAX(MeterStopDateTime) MeterMaxStopTime
        FROM staging.tblTransactionLineItems
        WHERE IgnoreRecord = 0
        AND DeleteFlag = 0
        AND IsRecordDeleted = 0
        GROUP BY TransactionGuid
    ) b
    ON b.TransactionGuid = a.TransactionGuid
    WHERE a.IgnoreRecord = 0
    AND a.DeleteFlag = 0
    AND a.IsRecordDeleted = 0

    --Set the transaction summary terminal time measures
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