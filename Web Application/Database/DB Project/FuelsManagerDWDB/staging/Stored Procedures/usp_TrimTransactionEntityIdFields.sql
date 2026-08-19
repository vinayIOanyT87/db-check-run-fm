/*
  DROP PROCEDURE [staging].[usp_TrimTransactionEntityIdFields]

	EXEC [staging].[usp_TrimTransactionEntityIdFields]
	
*/
CREATE PROCEDURE [staging].[usp_TrimTransactionEntityIdFields]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_TrimTransactionEntityIdFields]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Trim the Entity ID field (e.g. BillToId) found in tblTransactions, tblTransactionLineItems, and tblTransactionSubLineItems.
  -- Notes:
  -- 1. The values of the ID fields are trimmed first before comparison against the respective entity tables as multiple cases were found
  --    of ID fields that only differ by prefix and/or suffix whitespaces.
  -- 2. A copy of the original version of the Entity Id field, before it is trimmed, is captured for reference/debugging purposes in a field 
  --    with the same name but with an "_Orig" suffix, e.g. BillToIdId_Orig.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @openEndedDate datetimeoffset(7)
    SELECT
      @openEndedDate = DATEADD(YEAR, 100, GETDATE())

    -- Trim Transaction Header Entity Id fields
    UPDATE staging.tblTransactions
    SET SiteId_Orig = SiteId,
        SiteId = TRIM(SiteId),
        BillToId_Orig = BillToId,
        BillToId = TRIM(BillToId),
        CarrierId_Orig = CarrierId,
        CarrierId = TRIM(CarrierId),
        ManagerId_Orig = ManagerId,
        ManagerId = TRIM(ManagerId),
        OwnerId_Orig = OwnerId,
        OwnerId = TRIM(OwnerId),
        ShipperId_Orig = ShipperId,
        ShipperId = TRIM(ShipperId),
        ShipToId_Orig = ShipToId,
        ShipToId = TRIM(ShipToId),
        SupplierId_Orig = SupplierId,
        SupplierId = TRIM(SupplierId),
        DestinationCompanyEquipmentID1_Orig = DestinationCompanyEquipmentID1,
        DestinationCompanyEquipmentID1 = TRIM(DestinationCompanyEquipmentID1),
        DestinationCompanyEquipmentID2_Orig = DestinationCompanyEquipmentID2,
        DestinationCompanyEquipmentID2 = TRIM(DestinationCompanyEquipmentID2),
        DestinationCompanyEquipmentID3_Orig = DestinationCompanyEquipmentID3,
        DestinationCompanyEquipmentID3 = TRIM(DestinationCompanyEquipmentID3),
        SourceCompanyEquipmentID1_Orig = SourceCompanyEquipmentID1,
        SourceCompanyEquipmentID1 = TRIM(SourceCompanyEquipmentID1),
        SourceCompanyEquipmentID2_Orig = SourceCompanyEquipmentID2,
        SourceCompanyEquipmentID2 = TRIM(SourceCompanyEquipmentID2),
        SourceCompanyEquipmentID3_Orig = SourceCompanyEquipmentID3,
        SourceCompanyEquipmentID3 = TRIM(SourceCompanyEquipmentID3),
        OperatorID_Orig = OperatorID,
        OperatorID = TRIM(OperatorID),
        ReasonCode_Orig = ReasonCode,
        ReasonCode = TRIM(ReasonCode),
        TransactionAliasName_Orig = TransactionAliasName,
        TransactionAliasName = TRIM(TransactionAliasName)



    -- Trim TransactionLineItem Entity Id fields
    UPDATE staging.tblTransactionLineItems
    SET DestinationCompartmentId_Orig = DestinationCompartmentID,
        DestinationCompartmentId = TRIM(DestinationCompartmentId),
        DestinationCompanyEquipmentID_Orig = DestinationCompanyEquipmentID,
        DestinationCompanyEquipmentID = TRIM(DestinationCompanyEquipmentID),
        Product_Orig = ProductId,
        ProductId = TRIM(ProductId),
        SourceCompartmentId_Orig = SourceCompartmentID,
        SourceCompartmentId = TRIM(SourceCompartmentId),
        SourceCompanyEquipmentID_Orig = SourceCompanyEquipmentID,
        SourceCompanyEquipmentID = TRIM(SourceCompanyEquipmentID),
        LoadingLocationID_Orig = LoadingLocationID,
        LoadingLocationID = TRIM(LoadingLocationId)



    -- Trim TransactionSubLineItem Entity Id fields
    UPDATE staging.tblTransactionSubLineItems
    SET Product_Orig = ProductId,
        ProductId = TRIM(ProductId)


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
    + 'Procedure Name: [staging].[usp_TrimTransactionEntityIdFields]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END