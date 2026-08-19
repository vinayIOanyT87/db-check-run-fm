/*
    DROP PROCEDURE [Staging].[usp_PresetTransactionSubLineItems]

	EXEC [staging].[usp_PresetTransactionSubLineItems]
	
*/
CREATE PROCEDURE [staging].[usp_PresetTransactionSubLineItems]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_PresetTransactionLineItems]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: For table staging.tblTransactionLineSubItems, set all the extra fields that were added to the table and that are not populated 
  --          from the corresponding tblTransactioSubLineItems OLTP table.
  -- Notes:
  -- 1. The factTransaction (Level 3) is fed not only by the main measure tables: staging.tblTransactionLineItems and 
  --    staging.tblTransactionSubLineItems, but also from additional supporting tables: staging.tblTransactions, 
  --    staging.tblTransactionUserData, staging.tblTransactionLineItemUserData, staging.tblExportResults, and staging.tblExportInterfaceResult.
  --    Instead of having each record of the factTransaction to be populated by an Insert_Update operation (Insert from 
  --    staging.tblTransactionLineItems [or staging.tblTransactionSubLineItems] followed by an update from staging.tblTransactions, 
  --    staging.tblTransactionUserData, etc.), tables staging.tblTransactionLineItems and staging.tblTransactionSubLineItems have been 
  --    modified to include those fields of the supporting tables that are required by factTransaction. The procedure below is used to 
  --    load those fields for each record of staging.tblTransactionSubLineItems. 
  --    This approach allows each record of the factTransaction to be populated by a single Insert operation instead of an Insert+Update 
  --    combination.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    --Transaction Header Fields
    UPDATE a
    SET a.HeaderBillToCompanyKey = b.BillToCompanyKey,
    a.HeaderBillToCompanySKey = b.BillToCompanySKey,
    a.HeaderBillToCode = b.BillToCode,
    a.HeaderBillToId = b.BillToID,
    a.HeaderCardExpirationDateSKey = b.CardExpirationDateSKey,
    a.HeaderCardName = b.CardName,
    a.HeaderCardNumber = b.CardNumber,
    a.HeaderCardType = b.CardType,
    a.HeaderCarrierCompanyKey = b.CarrierCompanyKey,
    a.HeaderCarrierCompanySKey = b.CarrierCompanySKey,
    a.HeaderCarrierCode = b.CarrierCode,
    a.HeaderCarrierId = b.CarrierID,
    a.HeaderCashAmount = b.CashAmount,
    a.HeaderConjoinOwnerKey = b.ConjoinOwnerKey,
    a.HeaderConjoinOwnerId = b.ConjoinOwnerId,
    a.HeaderConjoinOwnerSKey = b.ConjoinOwnerSKey,
    a.HeaderConjoinTransID = b.ConjoinTransID,
    a.HeaderContactFirstName = b.ContactFirstName,
    a.HeaderContactInfo = b.ContactInfo,
    a.HeaderContactSurname = b.ContactSurname,
    a.HeaderCountry = b.Country,
    a.HeaderCreatedDateSKey = b.CreatedDateSKey,
    a.HeaderCreatedDate = b.CreatedDate,
    a.HeaderCreatedTimeSKey = b.CreatedTimeSKey,
    a.HeaderCreditAmount = b.CreditAmount,
    a.HeaderDate01 = b.Date01,
    a.HeaderDate02 = b.Date02,
    a.HeaderDate03 = b.Date03,
    a.HeaderDate04 = b.Date04,
    a.HeaderDeleteFlag = b.DeleteFlag,
    a.HeaderDestinationEquipment1Key = b.DestinationEquipment1Key,
	a.HeaderDestinationEquipment1SKey = b.DestinationEquipment1SKey,
    a.HeaderDispatchedDateSKey = b.DispatchedDateSKey,
    a.HeaderDispatchedTimeSKey = b.DispatchedTimeSKey,
    a.HeaderDocumentNumber = b.DocumentNumber,
    a.HeaderDriverIdentificationNumber = b.DriverIdentificationNumber,
    a.HeaderEffectiveDateSKey = b.EffectiveDateSKey,
    a.HeaderFlag01 = b.Flag01,
    a.HeaderFlag02 = b.Flag02,
    a.HeaderFlag03 = b.Flag03,
    a.HeaderFlag04 = b.Flag04,
    a.HeaderFlag05 = b.Flag05,
    a.HeaderFlag06 = b.Flag06,
    a.HeaderInternationalRouteIndicator = b.InternationalRouteIndicator,
    a.HeaderInventoryDate = b.InventoryDate,
    a.HeaderInventoryDateSKey = b.InventoryDateSKey,
    a.HeaderLegacyNumber = b.LegacyNumber,
    a.HeaderLinkedDocumentNumber = b.LinkedDocumentNumber,
    a.HeaderLoadID = b.LoadID,
    a.HeaderManagerCompanyKey = b.ManagerCompanyKey,
    a.HeaderManagerCompanySKey = b.ManagerCompanySKey,
    a.HeaderManagerCode = b.ManagerCode,
    a.HeaderManagerId = b.ManagerID,
    a.HeaderNumber01 = b.Number01,
    a.HeaderNumber02 = b.Number02,
    a.HeaderNumber03 = b.Number03,
    a.HeaderNumber04 = b.Number04,
    a.HeaderNumber05 = b.Number05,
    a.HeaderNumber06 = b.Number06,
    a.HeaderOperatorPersonnelKey = b.OperatorPersonnelKey,
    a.HeaderOperatorPersonnelSKey = b.OperatorPersonnelSKey,
    a.HeaderOperatorId = b.OperatorId,
    a.HeaderOperatorName = b.OperatorName,
    a.HeaderOwnerCompanyKey = b.OwnerCompanyKey,
    a.HeaderOwnerCompanySKey = b.OwnerCompanySKey,
    a.HeaderOwnerCode = b.OwnerCode,
    a.HeaderOwnerId = b.OwnerID,
    a.HeaderPONumber = b.PONumber,
    a.HeaderPreviousRoutingID = b.PreviousRoutingID,    
    a.HeaderReasonCode = b.ReasonCode,
    a.HeaderReasonCodeKey = b.ReasonCodeKey,
    a.HeaderReasonCodeSKey = b.ReasonCodeSKey,
    a.HeaderRequestedDateSKey = b.RequestedDateSKey,
    a.HeaderRequestedDateTime = b.RequestedDateTime,
    a.HeaderReversalType = b.ReversalType,
    a.HeaderReversedTransID = b.ReversedTransID,
    a.HeaderRouteOriginationDateSKey = b.RouteOriginationDateSKey,
    a.HeaderRoutingID = b.RoutingID,
    a.HeaderShipperCompanyKey = b.ShipperCompanyKey,
    a.HeaderShipperCompanySKey = b.ShipperCompanySKey,
    a.HeaderShipperCode = b.ShipperCode,
    a.HeaderShipperId = b.ShipperID,
    a.HeaderShipToCompanyKey = b.ShipToCompanyKey,
    a.HeaderShipToCompanySKey = b.ShipToCompanySKey,
    a.HeaderShipToCode = b.ShipToCode,
    a.HeaderShipToId = b.ShipToID,
    a.HeaderShipmentNumber = b.ShipmentNumber,       
    a.HeaderSourceEquipment1Key = b.SourceEquipment1Key,
	a.HeaderSourceEquipment1SKey = b.SourceEquipment1SKey,
    a.HeaderSubmittedToAccounting = b.SubmittedToAccounting,
    a.HeaderSubType = b.SubType,
    a.HeaderSupplierCompanyKey = b.SupplierCompanyKey,
    a.HeaderSupplierCompanySKey = b.SupplierCompanySKey,
    a.HeaderSupplierCode = b.SupplierCode,
    a.HeaderSupplierId = b.SupplierID,
    a.HeaderTicketMode = b.TicketMode,
    a.HeaderTicketSource = b.TicketSource,
    a.HeaderTimeEnd = b.TimeEnd,
    a.HeaderTimeEndDateSKey = b.TimeEndDateSKey,
    a.HeaderTimeEndTimeSKey = b.TimeEndTimeSKey,
    a.HeaderTimeIn = b.TimeIn,
    a.HeaderTimeInDateSKey = b.TimeInDateSKey,
    a.HeaderTimeInTimeSKey = b.TimeInTimeSKey,
    a.HeaderTimeOut = b.TimeOut,
    a.HeaderTimeOutDateSKey = b.TimeOutDateSKey,
    a.HeaderTimeOutTimeSKey = b.TimeOutTimeSKey,
    a.HeaderTransactionAliasKey = b.TransactionAliasKey,
	a.HeaderTransactionAliasName = b.TransactionAliasName,
    a.HeaderTransactionAliasSKey = b.TransactionAliasSKey,
    a.HeaderTransactionStatusIndex = b.TransactionStatusIndex,
    a.HeaderTransactionStatusName = b.TransactionStatusName,
    a.HeaderTransactionTypeKey = b.TransactionTypeKey,
    a.HeaderTransactionTypeSKey = b.TransactionTypeSKey,
    a.HeaderTransDateSKey = b.TransDateSKey,
    a.HeaderTransDateTime = b.TransDateTime,
    a.HeaderTransID = b.TransID,
    a.HeaderTransReferenceID = b.TransReferenceID,
    a.HeaderTransTimeSKey = b.TransTimeSKey,
    a.HeaderTransVersion = b.TransVersion
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey   
    WHERE a.IgnoreRecord = 0


     --Transaction LineItem Fields
    UPDATE a
    SET a.LineItemLoadArmNumber = b.LoadArmNumber,
    a.LineItemDestinationEquipmentKey = b.DestinationEquipmentKey,
    a.LineItemDestinationEquipmentID = b.DestinationEquipmentID,
	a.LineItemDestinationEquipmentSKey = b.DestinationEquipmentSKey,
	a.LineItemSourceEquipmentKey = b.SourceEquipmentKey,
	a.LineItemSourceEquipmentID = b.SourceEquipmentID,
	a.LineItemSourceEquipmentSKey = b.SourceEquipmentSKey,
	a.LineItemLoadingLocationStationKey = b.LoadingLocationStationKey,
    a.LineItemLoadingLocationStationSKey = b.LoadingLocationStationSKey,
    a.LineItemStorageLocationTankKey = b.StorageLocationTankKey,
    a.LineItemStorageLocationTankSKey = b.StorageLocationTankSKey
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactionLineItems b
    ON b.TransactionLineItemKey = a.TransactionLineItemKey   
    WHERE a.IgnoreRecord = 0


    UPDATE Staging.tblTransactionSubLineItems
    SET MeterStartStopTimeDiff = 0
    WHERE IgnoreRecord = 0
    AND (MeterStartDateTime IS NULL OR MeterStopDateTime IS NULL)


    UPDATE Staging.tblTransactionSubLineItems
    SET MeterStartStopTimeDiff = DATEDIFF(Minute, MeterStartDateTime, MeterStopDateTime)
    WHERE MeterStartDateTime IS NOT NULL
    AND MeterStopDateTime IS NOT NULL
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
    + 'Procedure Name: [staging].[usp_PresetTransactionLineItems]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
