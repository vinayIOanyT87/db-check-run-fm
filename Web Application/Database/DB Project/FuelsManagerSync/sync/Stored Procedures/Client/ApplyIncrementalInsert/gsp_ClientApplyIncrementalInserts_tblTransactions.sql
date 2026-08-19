-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactions
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblTransactions]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@TransID nvarchar(64),
@AliasName nvarchar(32),
@SubType nvarchar(20),
@Site nvarchar(30),
@TransReferenceID nvarchar(64),
@InventoryDate date,
@ShipToID nvarchar(100),
@ShipToCode nvarchar(10),
@SupplierID nvarchar(100),
@SupplierCode nvarchar(10),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@RequestedDeliveryDate datetimeoffset(7),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@TransDateTime datetimeoffset(7),
@TransVersion bigint,
@SCACCode nvarchar(4),
@CardNumber nvarchar(30),
@ShipmentNumber nvarchar(30),
@ShipperID nvarchar(100),
@ShipperCode nvarchar(10),
@OwnerID nvarchar(100),
@OwnerCode nvarchar(10),
@ManagerID nvarchar(100),
@ManagerCode nvarchar(10),
@CarrierID nvarchar(100),
@CarrierCode nvarchar(10),
@ConjoinTransID nvarchar(64),
@ReversedTransID nvarchar(64),
@LinkedDocumentNumber nvarchar(64),
@ReversalType nvarchar(2),
@PONumber nvarchar(14),
@TimeIn datetimeoffset(7),
@TimeOut datetimeoffset(7),
@TimeEnd datetimeoffset(7),
@RoutingID nvarchar(30),
@TicketSource nvarchar(20),
@LoadID nvarchar(50),
@BillToID nvarchar(100),
@BillToCode nvarchar(10),
@DriverIdentificationNumber nvarchar(50),
@CreditAmount float,
@CardExpiration datetimeoffset(7),
@CardName nvarchar(30),
@CardType nvarchar(30),
@CashAmount float,
@RouteOriginationDate datetimeoffset(7),
@InternationalRouteIndicator bit,
@PreviousRoutingID nvarchar(30),
@ShippingDocumentNumber nvarchar(30),
@DocumentNumber nvarchar(30),
@STD datetimeoffset(7),
@ETD datetimeoffset(7),
@STA datetimeoffset(7),
@ETA datetimeoffset(7),
@SFT datetimeoffset(7),
@FST datetimeoffset(7),
@EstimatedFuelingDuration int,
@DeleteFlag bit,
@TicketMode nvarchar(15),
@DestinationRegistrationID1 nvarchar(30),
@DestinationSerialNumber1 nvarchar(10),
@DestinationEquipmentType1 nvarchar(50),
@DestinationEquipmentModel1 nvarchar(20),
@DestinationCompanyEquipmentID1 nvarchar(30),
@DestinationRegistrationID2 nvarchar(30),
@DestinationSerialNumber2 nvarchar(10),
@DestinationEquipmentType2 nvarchar(50),
@DestinationEquipmentModel2 nvarchar(20),
@DestinationCompanyEquipmentID2 nvarchar(30),
@DestinationRegistrationID3 nvarchar(30),
@DestinationSerialNumber3 nvarchar(10),
@DestinationEquipmentType3 nvarchar(50),
@DestinationEquipmentModel3 nvarchar(20),
@DestinationCompanyEquipmentID3 nvarchar(30),
@SourceRegistrationID1 nvarchar(30),
@SourceSerialNumber1 nvarchar(10),
@SourceEquipmentType1 nvarchar(50),
@SourceEquipmentModel1 nvarchar(20),
@SourceCompanyEquipmentID1 nvarchar(30),
@SourceRegistrationID2 nvarchar(30),
@SourceSerialNumber2 nvarchar(10),
@SourceEquipmentType2 nvarchar(50),
@SourceEquipmentModel2 nvarchar(20),
@SourceCompanyEquipmentID2 nvarchar(30),
@SourceRegistrationID3 nvarchar(30),
@SourceSerialNumber3 nvarchar(10),
@SourceEquipmentType3 nvarchar(50),
@SourceEquipmentModel3 nvarchar(20),
@SourceCompanyEquipmentID3 nvarchar(30),
@OperatorID nvarchar(50),
@EffectiveDate datetimeoffset(7),
@ExpirationDate datetimeoffset(7),
@ScheduledDate datetimeoffset(7),
@AutoComplete bit,
@Flag01 bit,
@Flag02 bit,
@Flag03 bit,
@Flag04 bit,
@Flag05 bit,
@Flag06 bit,
@Number01 float,
@Number02 float,
@Number03 float,
@Number04 float,
@Number05 float,
@Number06 float,
@ContactFirstName nvarchar(50),
@ContactSurname nvarchar(50),
@Date01 datetimeoffset(7),
@Date02 datetimeoffset(7),
@Date03 datetimeoffset(7),
@Date04 datetimeoffset(7),
@LegacyNumber nvarchar(50),
@Country nvarchar(50),
@ContactInfo nvarchar(50),
@AssociatedDocNumber nvarchar(30),
@AssociatedCLIN nvarchar(10),
@SubmittedToAccounting bit,
@FuelCardID nvarchar(50),
@AssociatedTransportOrderNumber nvarchar(30),
@RequestedDateTime datetimeoffset(7),
@DispatchedDateTime datetimeoffset(7),
@ErrorFlag bit,
@TransactionGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupTransTypeIndex smallint,
@LookupTransactionStatusIndex int,
@LookupOriginApplicationIndex int,
@TransactionAliasGuid uniqueidentifier,
@BillToCompanyGuid uniqueidentifier,
@Destination1EquipmentGuid uniqueidentifier,
@Destination2EquipmentGuid uniqueidentifier,
@Destination3EquipmentGuid uniqueidentifier,
@FinalStationIATAGuid uniqueidentifier,
@FuelCardGuid uniqueidentifier,
@ManagerCompanyGuid uniqueidentifier,
@NextStationIATAGuid uniqueidentifier,
@OperatorPersonnelGuid uniqueidentifier,
@OriginStationIATAGuid uniqueidentifier,
@OwnerCompanyGuid uniqueidentifier,
@PreviousStationIATAGuid uniqueidentifier,
@ShipperCompanyGuid uniqueidentifier,
@ShipToCompanyGuid uniqueidentifier,
@Source1EquipmentGuid uniqueidentifier,
@Source2EquipmentGuid uniqueidentifier,
@Source3EquipmentGuid uniqueidentifier,
@SupplierCompanyGuid uniqueidentifier,
@CarrierCompanyGuid uniqueidentifier,
@ReasonCodeGuid uniqueidentifier,
@OriginStationIATAID nvarchar(50),
@PreviousStationIATAID nvarchar(50),
@NextStationIATAID nvarchar(50),
@FinalStationIATAID nvarchar(50),
@OperatorName nvarchar(150),
@FuelAdditiveFlag bit,
@IssuePoint nvarchar(max),
@IssuePointNumber nvarchar(max),
@RadioNumber nvarchar(max),
@GateID nvarchar(10),
@GateGuid uniqueidentifier,
@ShippingMethod nvarchar(150),
@ReferencedTransactionGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTransactions] AS existingData
        USING (SELECT @TransID 'TransID',@AliasName 'AliasName',@SubType 'SubType',@Site 'Site',@TransReferenceID 'TransReferenceID',@InventoryDate 'InventoryDate',@ShipToID 'ShipToID',@ShipToCode 'ShipToCode',@SupplierID 'SupplierID',@SupplierCode 'SupplierCode',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@RequestedDeliveryDate 'RequestedDeliveryDate',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@TransDateTime 'TransDateTime',@TransVersion 'TransVersion',@SCACCode 'SCACCode',@CardNumber 'CardNumber',@ShipmentNumber 'ShipmentNumber',@ShipperID 'ShipperID',@ShipperCode 'ShipperCode',@OwnerID 'OwnerID',@OwnerCode 'OwnerCode',@ManagerID 'ManagerID',@ManagerCode 'ManagerCode',@CarrierID 'CarrierID',@CarrierCode 'CarrierCode',@ConjoinTransID 'ConjoinTransID',@ReversedTransID 'ReversedTransID',@LinkedDocumentNumber 'LinkedDocumentNumber',@ReversalType 'ReversalType',@PONumber 'PONumber',@TimeIn 'TimeIn',@TimeOut 'TimeOut',@TimeEnd 'TimeEnd',@RoutingID 'RoutingID',@TicketSource 'TicketSource',@LoadID 'LoadID',@BillToID 'BillToID',@BillToCode 'BillToCode',@DriverIdentificationNumber 'DriverIdentificationNumber',@CreditAmount 'CreditAmount',@CardExpiration 'CardExpiration',@CardName 'CardName',@CardType 'CardType',@CashAmount 'CashAmount',@RouteOriginationDate 'RouteOriginationDate',@InternationalRouteIndicator 'InternationalRouteIndicator',@PreviousRoutingID 'PreviousRoutingID',@ShippingDocumentNumber 'ShippingDocumentNumber',@DocumentNumber 'DocumentNumber',@STD 'STD',@ETD 'ETD',@STA 'STA',@ETA 'ETA',@SFT 'SFT',@FST 'FST',@EstimatedFuelingDuration 'EstimatedFuelingDuration',@DeleteFlag 'DeleteFlag',@TicketMode 'TicketMode',@DestinationRegistrationID1 'DestinationRegistrationID1',@DestinationSerialNumber1 'DestinationSerialNumber1',@DestinationEquipmentType1 'DestinationEquipmentType1',@DestinationEquipmentModel1 'DestinationEquipmentModel1',@DestinationCompanyEquipmentID1 'DestinationCompanyEquipmentID1',@DestinationRegistrationID2 'DestinationRegistrationID2',@DestinationSerialNumber2 'DestinationSerialNumber2',@DestinationEquipmentType2 'DestinationEquipmentType2',@DestinationEquipmentModel2 'DestinationEquipmentModel2',@DestinationCompanyEquipmentID2 'DestinationCompanyEquipmentID2',@DestinationRegistrationID3 'DestinationRegistrationID3',@DestinationSerialNumber3 'DestinationSerialNumber3',@DestinationEquipmentType3 'DestinationEquipmentType3',@DestinationEquipmentModel3 'DestinationEquipmentModel3',@DestinationCompanyEquipmentID3 'DestinationCompanyEquipmentID3',@SourceRegistrationID1 'SourceRegistrationID1',@SourceSerialNumber1 'SourceSerialNumber1',@SourceEquipmentType1 'SourceEquipmentType1',@SourceEquipmentModel1 'SourceEquipmentModel1',@SourceCompanyEquipmentID1 'SourceCompanyEquipmentID1',@SourceRegistrationID2 'SourceRegistrationID2',@SourceSerialNumber2 'SourceSerialNumber2',@SourceEquipmentType2 'SourceEquipmentType2',@SourceEquipmentModel2 'SourceEquipmentModel2',@SourceCompanyEquipmentID2 'SourceCompanyEquipmentID2',@SourceRegistrationID3 'SourceRegistrationID3',@SourceSerialNumber3 'SourceSerialNumber3',@SourceEquipmentType3 'SourceEquipmentType3',@SourceEquipmentModel3 'SourceEquipmentModel3',@SourceCompanyEquipmentID3 'SourceCompanyEquipmentID3',@OperatorID 'OperatorID',@EffectiveDate 'EffectiveDate',@ExpirationDate 'ExpirationDate',@ScheduledDate 'ScheduledDate',@AutoComplete 'AutoComplete',@Flag01 'Flag01',@Flag02 'Flag02',@Flag03 'Flag03',@Flag04 'Flag04',@Flag05 'Flag05',@Flag06 'Flag06',@Number01 'Number01',@Number02 'Number02',@Number03 'Number03',@Number04 'Number04',@Number05 'Number05',@Number06 'Number06',@ContactFirstName 'ContactFirstName',@ContactSurname 'ContactSurname',@Date01 'Date01',@Date02 'Date02',@Date03 'Date03',@Date04 'Date04',@LegacyNumber 'LegacyNumber',@Country 'Country',@ContactInfo 'ContactInfo',@AssociatedDocNumber 'AssociatedDocNumber',@AssociatedCLIN 'AssociatedCLIN',@SubmittedToAccounting 'SubmittedToAccounting',@FuelCardID 'FuelCardID',@AssociatedTransportOrderNumber 'AssociatedTransportOrderNumber',@RequestedDateTime 'RequestedDateTime',@DispatchedDateTime 'DispatchedDateTime',@ErrorFlag 'ErrorFlag',@TransactionGuid 'TransactionGuid',@SiteGuid 'SiteGuid',@LookupTransTypeIndex 'LookupTransTypeIndex',@LookupTransactionStatusIndex 'LookupTransactionStatusIndex',@LookupOriginApplicationIndex 'LookupOriginApplicationIndex',@TransactionAliasGuid 'TransactionAliasGuid',@BillToCompanyGuid 'BillToCompanyGuid',@Destination1EquipmentGuid 'Destination1EquipmentGuid',@Destination2EquipmentGuid 'Destination2EquipmentGuid',@Destination3EquipmentGuid 'Destination3EquipmentGuid',@FinalStationIATAGuid 'FinalStationIATAGuid',@FuelCardGuid 'FuelCardGuid',@ManagerCompanyGuid 'ManagerCompanyGuid',@NextStationIATAGuid 'NextStationIATAGuid',@OperatorPersonnelGuid 'OperatorPersonnelGuid',@OriginStationIATAGuid 'OriginStationIATAGuid',@OwnerCompanyGuid 'OwnerCompanyGuid',@PreviousStationIATAGuid 'PreviousStationIATAGuid',@ShipperCompanyGuid 'ShipperCompanyGuid',@ShipToCompanyGuid 'ShipToCompanyGuid',@Source1EquipmentGuid 'Source1EquipmentGuid',@Source2EquipmentGuid 'Source2EquipmentGuid',@Source3EquipmentGuid 'Source3EquipmentGuid',@SupplierCompanyGuid 'SupplierCompanyGuid',@CarrierCompanyGuid 'CarrierCompanyGuid',@ReasonCodeGuid 'ReasonCodeGuid',@OriginStationIATAID 'OriginStationIATAID',@PreviousStationIATAID 'PreviousStationIATAID',@NextStationIATAID 'NextStationIATAID',@FinalStationIATAID 'FinalStationIATAID',@OperatorName 'OperatorName',@FuelAdditiveFlag 'FuelAdditiveFlag',@IssuePoint 'IssuePoint',@IssuePointNumber 'IssuePointNumber',@RadioNumber 'RadioNumber',@GateID 'GateID',@GateGuid 'GateGuid',@ShippingMethod 'ShippingMethod',@ReferencedTransactionGuid 'ReferencedTransactionGuid'
                ) AS remoteChanges ([TransID],[AliasName],[SubType],[Site],[TransReferenceID],[InventoryDate],[ShipToID],[ShipToCode],[SupplierID],[SupplierCode],[CreatedDate],[CreatedBy],[RequestedDeliveryDate],[UpdatedDate],[UpdatedBy],[TransDateTime],[TransVersion],[SCACCode],[CardNumber],[ShipmentNumber],[ShipperID],[ShipperCode],[OwnerID],[OwnerCode],[ManagerID],[ManagerCode],[CarrierID],[CarrierCode],[ConjoinTransID],[ReversedTransID],[LinkedDocumentNumber],[ReversalType],[PONumber],[TimeIn],[TimeOut],[TimeEnd],[RoutingID],[TicketSource],[LoadID],[BillToID],[BillToCode],[DriverIdentificationNumber],[CreditAmount],[CardExpiration],[CardName],[CardType],[CashAmount],[RouteOriginationDate],[InternationalRouteIndicator],[PreviousRoutingID],[ShippingDocumentNumber],[DocumentNumber],[STD],[ETD],[STA],[ETA],[SFT],[FST],[EstimatedFuelingDuration],[DeleteFlag],[TicketMode],[DestinationRegistrationID1],[DestinationSerialNumber1],[DestinationEquipmentType1],[DestinationEquipmentModel1],[DestinationCompanyEquipmentID1],[DestinationRegistrationID2],[DestinationSerialNumber2],[DestinationEquipmentType2],[DestinationEquipmentModel2],[DestinationCompanyEquipmentID2],[DestinationRegistrationID3],[DestinationSerialNumber3],[DestinationEquipmentType3],[DestinationEquipmentModel3],[DestinationCompanyEquipmentID3],[SourceRegistrationID1],[SourceSerialNumber1],[SourceEquipmentType1],[SourceEquipmentModel1],[SourceCompanyEquipmentID1],[SourceRegistrationID2],[SourceSerialNumber2],[SourceEquipmentType2],[SourceEquipmentModel2],[SourceCompanyEquipmentID2],[SourceRegistrationID3],[SourceSerialNumber3],[SourceEquipmentType3],[SourceEquipmentModel3],[SourceCompanyEquipmentID3],[OperatorID],[EffectiveDate],[ExpirationDate],[ScheduledDate],[AutoComplete],[Flag01],[Flag02],[Flag03],[Flag04],[Flag05],[Flag06],[Number01],[Number02],[Number03],[Number04],[Number05],[Number06],[ContactFirstName],[ContactSurname],[Date01],[Date02],[Date03],[Date04],[LegacyNumber],[Country],[ContactInfo],[AssociatedDocNumber],[AssociatedCLIN],[SubmittedToAccounting],[FuelCardID],[AssociatedTransportOrderNumber],[RequestedDateTime],[DispatchedDateTime],[ErrorFlag],[TransactionGuid],[SiteGuid],[LookupTransTypeIndex],[LookupTransactionStatusIndex],[LookupOriginApplicationIndex],[TransactionAliasGuid],[BillToCompanyGuid],[Destination1EquipmentGuid],[Destination2EquipmentGuid],[Destination3EquipmentGuid],[FinalStationIATAGuid],[FuelCardGuid],[ManagerCompanyGuid],[NextStationIATAGuid],[OperatorPersonnelGuid],[OriginStationIATAGuid],[OwnerCompanyGuid],[PreviousStationIATAGuid],[ShipperCompanyGuid],[ShipToCompanyGuid],[Source1EquipmentGuid],[Source2EquipmentGuid],[Source3EquipmentGuid],[SupplierCompanyGuid],[CarrierCompanyGuid],[ReasonCodeGuid],[OriginStationIATAID],[PreviousStationIATAID],[NextStationIATAID],[FinalStationIATAID],[OperatorName],[FuelAdditiveFlag],[IssuePoint],[IssuePointNumber],[RadioNumber],[GateID],[GateGuid],[ShippingMethod],[ReferencedTransactionGuid])
        ON (existingData.[TransactionGuid] = remoteChanges.[TransactionGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [TransID] = remoteChanges.[TransID]
                       ,[AliasName] = remoteChanges.[AliasName]
                       ,[SubType] = remoteChanges.[SubType]
                       ,[Site] = remoteChanges.[Site]
                       ,[TransReferenceID] = remoteChanges.[TransReferenceID]
                       ,[InventoryDate] = remoteChanges.[InventoryDate]
                       ,[ShipToID] = remoteChanges.[ShipToID]
                       ,[ShipToCode] = remoteChanges.[ShipToCode]
                       ,[SupplierID] = remoteChanges.[SupplierID]
                       ,[SupplierCode] = remoteChanges.[SupplierCode]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[RequestedDeliveryDate] = remoteChanges.[RequestedDeliveryDate]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[TransDateTime] = remoteChanges.[TransDateTime]
                       ,[TransVersion] = remoteChanges.[TransVersion]
                       ,[SCACCode] = remoteChanges.[SCACCode]
                       ,[CardNumber] = remoteChanges.[CardNumber]
                       ,[ShipmentNumber] = remoteChanges.[ShipmentNumber]
                       ,[ShipperID] = remoteChanges.[ShipperID]
                       ,[ShipperCode] = remoteChanges.[ShipperCode]
                       ,[OwnerID] = remoteChanges.[OwnerID]
                       ,[OwnerCode] = remoteChanges.[OwnerCode]
                       ,[ManagerID] = remoteChanges.[ManagerID]
                       ,[ManagerCode] = remoteChanges.[ManagerCode]
                       ,[CarrierID] = remoteChanges.[CarrierID]
                       ,[CarrierCode] = remoteChanges.[CarrierCode]
                       ,[ConjoinTransID] = remoteChanges.[ConjoinTransID]
                       ,[ReversedTransID] = remoteChanges.[ReversedTransID]
                       ,[LinkedDocumentNumber] = remoteChanges.[LinkedDocumentNumber]
                       ,[ReversalType] = remoteChanges.[ReversalType]
                       ,[PONumber] = remoteChanges.[PONumber]
                       ,[TimeIn] = remoteChanges.[TimeIn]
                       ,[TimeOut] = remoteChanges.[TimeOut]
                       ,[TimeEnd] = remoteChanges.[TimeEnd]
                       ,[RoutingID] = remoteChanges.[RoutingID]
                       ,[TicketSource] = remoteChanges.[TicketSource]
                       ,[LoadID] = remoteChanges.[LoadID]
                       ,[BillToID] = remoteChanges.[BillToID]
                       ,[BillToCode] = remoteChanges.[BillToCode]
                       ,[DriverIdentificationNumber] = remoteChanges.[DriverIdentificationNumber]
                       ,[CreditAmount] = remoteChanges.[CreditAmount]
                       ,[CardExpiration] = remoteChanges.[CardExpiration]
                       ,[CardName] = remoteChanges.[CardName]
                       ,[CardType] = remoteChanges.[CardType]
                       ,[CashAmount] = remoteChanges.[CashAmount]
                       ,[RouteOriginationDate] = remoteChanges.[RouteOriginationDate]
                       ,[InternationalRouteIndicator] = remoteChanges.[InternationalRouteIndicator]
                       ,[PreviousRoutingID] = remoteChanges.[PreviousRoutingID]
                       ,[ShippingDocumentNumber] = remoteChanges.[ShippingDocumentNumber]
                       ,[DocumentNumber] = remoteChanges.[DocumentNumber]
                       ,[STD] = remoteChanges.[STD]
                       ,[ETD] = remoteChanges.[ETD]
                       ,[STA] = remoteChanges.[STA]
                       ,[ETA] = remoteChanges.[ETA]
                       ,[SFT] = remoteChanges.[SFT]
                       ,[FST] = remoteChanges.[FST]
                       ,[EstimatedFuelingDuration] = remoteChanges.[EstimatedFuelingDuration]
                       ,[DeleteFlag] = remoteChanges.[DeleteFlag]
                       ,[TicketMode] = remoteChanges.[TicketMode]
                       ,[DestinationRegistrationID1] = remoteChanges.[DestinationRegistrationID1]
                       ,[DestinationSerialNumber1] = remoteChanges.[DestinationSerialNumber1]
                       ,[DestinationEquipmentType1] = remoteChanges.[DestinationEquipmentType1]
                       ,[DestinationEquipmentModel1] = remoteChanges.[DestinationEquipmentModel1]
                       ,[DestinationCompanyEquipmentID1] = remoteChanges.[DestinationCompanyEquipmentID1]
                       ,[DestinationRegistrationID2] = remoteChanges.[DestinationRegistrationID2]
                       ,[DestinationSerialNumber2] = remoteChanges.[DestinationSerialNumber2]
                       ,[DestinationEquipmentType2] = remoteChanges.[DestinationEquipmentType2]
                       ,[DestinationEquipmentModel2] = remoteChanges.[DestinationEquipmentModel2]
                       ,[DestinationCompanyEquipmentID2] = remoteChanges.[DestinationCompanyEquipmentID2]
                       ,[DestinationRegistrationID3] = remoteChanges.[DestinationRegistrationID3]
                       ,[DestinationSerialNumber3] = remoteChanges.[DestinationSerialNumber3]
                       ,[DestinationEquipmentType3] = remoteChanges.[DestinationEquipmentType3]
                       ,[DestinationEquipmentModel3] = remoteChanges.[DestinationEquipmentModel3]
                       ,[DestinationCompanyEquipmentID3] = remoteChanges.[DestinationCompanyEquipmentID3]
                       ,[SourceRegistrationID1] = remoteChanges.[SourceRegistrationID1]
                       ,[SourceSerialNumber1] = remoteChanges.[SourceSerialNumber1]
                       ,[SourceEquipmentType1] = remoteChanges.[SourceEquipmentType1]
                       ,[SourceEquipmentModel1] = remoteChanges.[SourceEquipmentModel1]
                       ,[SourceCompanyEquipmentID1] = remoteChanges.[SourceCompanyEquipmentID1]
                       ,[SourceRegistrationID2] = remoteChanges.[SourceRegistrationID2]
                       ,[SourceSerialNumber2] = remoteChanges.[SourceSerialNumber2]
                       ,[SourceEquipmentType2] = remoteChanges.[SourceEquipmentType2]
                       ,[SourceEquipmentModel2] = remoteChanges.[SourceEquipmentModel2]
                       ,[SourceCompanyEquipmentID2] = remoteChanges.[SourceCompanyEquipmentID2]
                       ,[SourceRegistrationID3] = remoteChanges.[SourceRegistrationID3]
                       ,[SourceSerialNumber3] = remoteChanges.[SourceSerialNumber3]
                       ,[SourceEquipmentType3] = remoteChanges.[SourceEquipmentType3]
                       ,[SourceEquipmentModel3] = remoteChanges.[SourceEquipmentModel3]
                       ,[SourceCompanyEquipmentID3] = remoteChanges.[SourceCompanyEquipmentID3]
                       ,[OperatorID] = remoteChanges.[OperatorID]
                       ,[EffectiveDate] = remoteChanges.[EffectiveDate]
                       ,[ExpirationDate] = remoteChanges.[ExpirationDate]
                       ,[ScheduledDate] = remoteChanges.[ScheduledDate]
                       ,[AutoComplete] = remoteChanges.[AutoComplete]
                       ,[Flag01] = remoteChanges.[Flag01]
                       ,[Flag02] = remoteChanges.[Flag02]
                       ,[Flag03] = remoteChanges.[Flag03]
                       ,[Flag04] = remoteChanges.[Flag04]
                       ,[Flag05] = remoteChanges.[Flag05]
                       ,[Flag06] = remoteChanges.[Flag06]
                       ,[Number01] = remoteChanges.[Number01]
                       ,[Number02] = remoteChanges.[Number02]
                       ,[Number03] = remoteChanges.[Number03]
                       ,[Number04] = remoteChanges.[Number04]
                       ,[Number05] = remoteChanges.[Number05]
                       ,[Number06] = remoteChanges.[Number06]
                       ,[ContactFirstName] = remoteChanges.[ContactFirstName]
                       ,[ContactSurname] = remoteChanges.[ContactSurname]
                       ,[Date01] = remoteChanges.[Date01]
                       ,[Date02] = remoteChanges.[Date02]
                       ,[Date03] = remoteChanges.[Date03]
                       ,[Date04] = remoteChanges.[Date04]
                       ,[LegacyNumber] = remoteChanges.[LegacyNumber]
                       ,[Country] = remoteChanges.[Country]
                       ,[ContactInfo] = remoteChanges.[ContactInfo]
                       ,[AssociatedDocNumber] = remoteChanges.[AssociatedDocNumber]
                       ,[AssociatedCLIN] = remoteChanges.[AssociatedCLIN]
                       ,[SubmittedToAccounting] = remoteChanges.[SubmittedToAccounting]
                       ,[FuelCardID] = remoteChanges.[FuelCardID]
                       ,[AssociatedTransportOrderNumber] = remoteChanges.[AssociatedTransportOrderNumber]
                       ,[RequestedDateTime] = remoteChanges.[RequestedDateTime]
                       ,[DispatchedDateTime] = remoteChanges.[DispatchedDateTime]
                       ,[ErrorFlag] = remoteChanges.[ErrorFlag]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[LookupTransTypeIndex] = remoteChanges.[LookupTransTypeIndex]
                       ,[LookupTransactionStatusIndex] = remoteChanges.[LookupTransactionStatusIndex]
                       ,[LookupOriginApplicationIndex] = remoteChanges.[LookupOriginApplicationIndex]
                       ,[TransactionAliasGuid] = remoteChanges.[TransactionAliasGuid]
                       ,[BillToCompanyGuid] = remoteChanges.[BillToCompanyGuid]
                       ,[Destination1EquipmentGuid] = remoteChanges.[Destination1EquipmentGuid]
                       ,[Destination2EquipmentGuid] = remoteChanges.[Destination2EquipmentGuid]
                       ,[Destination3EquipmentGuid] = remoteChanges.[Destination3EquipmentGuid]
                       ,[FinalStationIATAGuid] = remoteChanges.[FinalStationIATAGuid]
                       ,[FuelCardGuid] = remoteChanges.[FuelCardGuid]
                       ,[ManagerCompanyGuid] = remoteChanges.[ManagerCompanyGuid]
                       ,[NextStationIATAGuid] = remoteChanges.[NextStationIATAGuid]
                       ,[OperatorPersonnelGuid] = remoteChanges.[OperatorPersonnelGuid]
                       ,[OriginStationIATAGuid] = remoteChanges.[OriginStationIATAGuid]
                       ,[OwnerCompanyGuid] = remoteChanges.[OwnerCompanyGuid]
                       ,[PreviousStationIATAGuid] = remoteChanges.[PreviousStationIATAGuid]
                       ,[ShipperCompanyGuid] = remoteChanges.[ShipperCompanyGuid]
                       ,[ShipToCompanyGuid] = remoteChanges.[ShipToCompanyGuid]
                       ,[Source1EquipmentGuid] = remoteChanges.[Source1EquipmentGuid]
                       ,[Source2EquipmentGuid] = remoteChanges.[Source2EquipmentGuid]
                       ,[Source3EquipmentGuid] = remoteChanges.[Source3EquipmentGuid]
                       ,[SupplierCompanyGuid] = remoteChanges.[SupplierCompanyGuid]
                       ,[CarrierCompanyGuid] = remoteChanges.[CarrierCompanyGuid]
                       ,[ReasonCodeGuid] = remoteChanges.[ReasonCodeGuid]
                       ,[OriginStationIATAID] = remoteChanges.[OriginStationIATAID]
                       ,[PreviousStationIATAID] = remoteChanges.[PreviousStationIATAID]
                       ,[NextStationIATAID] = remoteChanges.[NextStationIATAID]
                       ,[FinalStationIATAID] = remoteChanges.[FinalStationIATAID]
                       ,[OperatorName] = remoteChanges.[OperatorName]
                       ,[FuelAdditiveFlag] = remoteChanges.[FuelAdditiveFlag]
                       ,[IssuePoint] = remoteChanges.[IssuePoint]
                       ,[IssuePointNumber] = remoteChanges.[IssuePointNumber]
                       ,[RadioNumber] = remoteChanges.[RadioNumber]
                       ,[GateID] = remoteChanges.[GateID]
                       ,[GateGuid] = remoteChanges.[GateGuid]
                       ,[ShippingMethod] = remoteChanges.[ShippingMethod]
                       ,[ReferencedTransactionGuid] = remoteChanges.[ReferencedTransactionGuid]

        WHEN NOT MATCHED THEN
            INSERT ([TransID],[AliasName],[SubType],[Site],[TransReferenceID],[InventoryDate],[ShipToID],[ShipToCode],[SupplierID],[SupplierCode],[CreatedDate],[CreatedBy],[RequestedDeliveryDate],[UpdatedDate],[UpdatedBy],[TransDateTime],[TransVersion],[SCACCode],[CardNumber],[ShipmentNumber],[ShipperID],[ShipperCode],[OwnerID],[OwnerCode],[ManagerID],[ManagerCode],[CarrierID],[CarrierCode],[ConjoinTransID],[ReversedTransID],[LinkedDocumentNumber],[ReversalType],[PONumber],[TimeIn],[TimeOut],[TimeEnd],[RoutingID],[TicketSource],[LoadID],[BillToID],[BillToCode],[DriverIdentificationNumber],[CreditAmount],[CardExpiration],[CardName],[CardType],[CashAmount],[RouteOriginationDate],[InternationalRouteIndicator],[PreviousRoutingID],[ShippingDocumentNumber],[DocumentNumber],[STD],[ETD],[STA],[ETA],[SFT],[FST],[EstimatedFuelingDuration],[DeleteFlag],[TicketMode],[DestinationRegistrationID1],[DestinationSerialNumber1],[DestinationEquipmentType1],[DestinationEquipmentModel1],[DestinationCompanyEquipmentID1],[DestinationRegistrationID2],[DestinationSerialNumber2],[DestinationEquipmentType2],[DestinationEquipmentModel2],[DestinationCompanyEquipmentID2],[DestinationRegistrationID3],[DestinationSerialNumber3],[DestinationEquipmentType3],[DestinationEquipmentModel3],[DestinationCompanyEquipmentID3],[SourceRegistrationID1],[SourceSerialNumber1],[SourceEquipmentType1],[SourceEquipmentModel1],[SourceCompanyEquipmentID1],[SourceRegistrationID2],[SourceSerialNumber2],[SourceEquipmentType2],[SourceEquipmentModel2],[SourceCompanyEquipmentID2],[SourceRegistrationID3],[SourceSerialNumber3],[SourceEquipmentType3],[SourceEquipmentModel3],[SourceCompanyEquipmentID3],[OperatorID],[EffectiveDate],[ExpirationDate],[ScheduledDate],[AutoComplete],[Flag01],[Flag02],[Flag03],[Flag04],[Flag05],[Flag06],[Number01],[Number02],[Number03],[Number04],[Number05],[Number06],[ContactFirstName],[ContactSurname],[Date01],[Date02],[Date03],[Date04],[LegacyNumber],[Country],[ContactInfo],[AssociatedDocNumber],[AssociatedCLIN],[SubmittedToAccounting],[FuelCardID],[AssociatedTransportOrderNumber],[RequestedDateTime],[DispatchedDateTime],[ErrorFlag],[TransactionGuid],[SiteGuid],[LookupTransTypeIndex],[LookupTransactionStatusIndex],[LookupOriginApplicationIndex],[TransactionAliasGuid],[BillToCompanyGuid],[Destination1EquipmentGuid],[Destination2EquipmentGuid],[Destination3EquipmentGuid],[FinalStationIATAGuid],[FuelCardGuid],[ManagerCompanyGuid],[NextStationIATAGuid],[OperatorPersonnelGuid],[OriginStationIATAGuid],[OwnerCompanyGuid],[PreviousStationIATAGuid],[ShipperCompanyGuid],[ShipToCompanyGuid],[Source1EquipmentGuid],[Source2EquipmentGuid],[Source3EquipmentGuid],[SupplierCompanyGuid],[CarrierCompanyGuid],[ReasonCodeGuid],[OriginStationIATAID],[PreviousStationIATAID],[NextStationIATAID],[FinalStationIATAID],[OperatorName],[FuelAdditiveFlag],[IssuePoint],[IssuePointNumber],[RadioNumber],[GateID],[GateGuid],[ShippingMethod],[ReferencedTransactionGuid])
                VALUES (@TransID,@AliasName,@SubType,@Site,@TransReferenceID,@InventoryDate,@ShipToID,@ShipToCode,@SupplierID,@SupplierCode,@CreatedDate,@CreatedBy,@RequestedDeliveryDate,@UpdatedDate,@UpdatedBy,@TransDateTime,@TransVersion,@SCACCode,@CardNumber,@ShipmentNumber,@ShipperID,@ShipperCode,@OwnerID,@OwnerCode,@ManagerID,@ManagerCode,@CarrierID,@CarrierCode,@ConjoinTransID,@ReversedTransID,@LinkedDocumentNumber,@ReversalType,@PONumber,@TimeIn,@TimeOut,@TimeEnd,@RoutingID,@TicketSource,@LoadID,@BillToID,@BillToCode,@DriverIdentificationNumber,@CreditAmount,@CardExpiration,@CardName,@CardType,@CashAmount,@RouteOriginationDate,@InternationalRouteIndicator,@PreviousRoutingID,@ShippingDocumentNumber,@DocumentNumber,@STD,@ETD,@STA,@ETA,@SFT,@FST,@EstimatedFuelingDuration,@DeleteFlag,@TicketMode,@DestinationRegistrationID1,@DestinationSerialNumber1,@DestinationEquipmentType1,@DestinationEquipmentModel1,@DestinationCompanyEquipmentID1,@DestinationRegistrationID2,@DestinationSerialNumber2,@DestinationEquipmentType2,@DestinationEquipmentModel2,@DestinationCompanyEquipmentID2,@DestinationRegistrationID3,@DestinationSerialNumber3,@DestinationEquipmentType3,@DestinationEquipmentModel3,@DestinationCompanyEquipmentID3,@SourceRegistrationID1,@SourceSerialNumber1,@SourceEquipmentType1,@SourceEquipmentModel1,@SourceCompanyEquipmentID1,@SourceRegistrationID2,@SourceSerialNumber2,@SourceEquipmentType2,@SourceEquipmentModel2,@SourceCompanyEquipmentID2,@SourceRegistrationID3,@SourceSerialNumber3,@SourceEquipmentType3,@SourceEquipmentModel3,@SourceCompanyEquipmentID3,@OperatorID,@EffectiveDate,@ExpirationDate,@ScheduledDate,@AutoComplete,@Flag01,@Flag02,@Flag03,@Flag04,@Flag05,@Flag06,@Number01,@Number02,@Number03,@Number04,@Number05,@Number06,@ContactFirstName,@ContactSurname,@Date01,@Date02,@Date03,@Date04,@LegacyNumber,@Country,@ContactInfo,@AssociatedDocNumber,@AssociatedCLIN,@SubmittedToAccounting,@FuelCardID,@AssociatedTransportOrderNumber,@RequestedDateTime,@DispatchedDateTime,@ErrorFlag,@TransactionGuid,@SiteGuid,@LookupTransTypeIndex,@LookupTransactionStatusIndex,@LookupOriginApplicationIndex,@TransactionAliasGuid,@BillToCompanyGuid,@Destination1EquipmentGuid,@Destination2EquipmentGuid,@Destination3EquipmentGuid,@FinalStationIATAGuid,@FuelCardGuid,@ManagerCompanyGuid,@NextStationIATAGuid,@OperatorPersonnelGuid,@OriginStationIATAGuid,@OwnerCompanyGuid,@PreviousStationIATAGuid,@ShipperCompanyGuid,@ShipToCompanyGuid,@Source1EquipmentGuid,@Source2EquipmentGuid,@Source3EquipmentGuid,@SupplierCompanyGuid,@CarrierCompanyGuid,@ReasonCodeGuid,@OriginStationIATAID,@PreviousStationIATAID,@NextStationIATAID,@FinalStationIATAID,@OperatorName,@FuelAdditiveFlag,@IssuePoint,@IssuePointNumber,@RadioNumber,@GateID,@GateGuid,@ShippingMethod,@ReferencedTransactionGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTransactions] WHERE TransactionGuid = @TransactionGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
