-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblCompanies
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblCompanies]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(100),
@Code nvarchar(10),
@Name nvarchar(100),
@ShortName nvarchar(4),
@Address1 nvarchar(60),
@Address2 nvarchar(60),
@City nvarchar(60),
@State nvarchar(20),
@Zip nvarchar(11),
@Country nvarchar(30),
@Phone nvarchar(20),
@FAX nvarchar(20),
@EmergencyContact nvarchar(30),
@EmergencyPhone nvarchar(20),
@FlightPrefix nvarchar(5),
@EffectiveDate datetimeoffset(7),
@ExpirationDate datetimeoffset(7),
@OnHold bit,
@PickupFLights bit,
@StockTrack bit,
@SufferLossGain bit,
@LowStockWarning float,
@LockedOut bit,
@LockedOutReason nvarchar(80),
@LockedOutDate datetimeoffset(7),
@ReceivableAccount nvarchar(20),
@RefinerCode nvarchar(20),
@LastActivityDate datetimeoffset(7),
@CreditOK bit,
@AdditiveAccounting bit,
@PurchaseOrderRequired bit,
@EPANumber nvarchar(20),
@FederalID nvarchar(20),
@FederalID2 nvarchar(20),
@FederalID3 nvarchar(20),
@FederalID4 nvarchar(20),
@FederalID5 nvarchar(20),
@StateID nvarchar(20),
@TaxNumber nvarchar(20),
@FlushPermitted bit,
@PumpOffPermitted bit,
@DeliveryToTerminalPermitted bit,
@LicenseNumber nvarchar(20),
@LicenseExpiration datetimeoffset(7),
@InsuranceCompany nvarchar(20),
@InsurancePolicy nvarchar(20),
@LiabilityAmount money,
@HazardousMaterialExclusion bit,
@InsuranceExpiration datetimeoffset(7),
@AllowDriverEntry bit,
@PINRequired bit,
@MaximumVehicleWeight float,
@WeightUnits smallint,
@AccountNumber nvarchar(30),
@SCACCode nvarchar(4),
@DisableOwnerAllocationsCheck bit,
@DisableShipperAllocationsCheck bit,
@DisableBillToAllocationsCheck bit,
@DisableShipToAllocationsCheck bit,
@LoadRackDisplayText nvarchar(30),
@UserData1 nvarchar(60),
@UserData2 nvarchar(60),
@UserData3 nvarchar(60),
@UserData4 nvarchar(60),
@UserData5 nvarchar(60),
@UserData6 nvarchar(60),
@UserData7 nvarchar(60),
@UserData8 nvarchar(60),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@CompanyGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@IATAGuid uniqueidentifier,
@ShipperTypeApplicationStringGuid uniqueidentifier,
@CustomerBillToTypeApplicationStringGuid uniqueidentifier,
@CustomerShipToTypeApplicationStringGuid uniqueidentifier,
@Contact1Name nvarchar(30),
@Contact1Address1 nvarchar(30),
@Contact1Address2 nvarchar(30),
@Contact1City nvarchar(60),
@Contact1State nvarchar(20),
@Contact1Zip nvarchar(11),
@Contact1Country nvarchar(30),
@Contact1PhoneOffice nvarchar(20),
@Contact1Fax nvarchar(20),
@Contact1EmailAddress nvarchar(30),
@Contact2Name nvarchar(30),
@Contact2Address1 nvarchar(30),
@Contact2Address2 nvarchar(30),
@Contact2City nvarchar(60),
@Contact2State nvarchar(20),
@Contact2Zip nvarchar(11),
@Contact2Country nvarchar(30),
@Contact2PhoneOffice nvarchar(20),
@Contact2Fax nvarchar(20),
@Contact2EmailAddress nvarchar(30),
@Contact1PhoneMobile nvarchar(20),
@Contact2PhoneMobile nvarchar(20),
@_MasterRecordGuid uniqueidentifier,
@Note nvarchar(2000),
@HiddenDate datetimeoffset(7),
@ScullyRequired bit,
@ConsortiumTypeIndex int,
@CompanyIATACode nvarchar(50),
@CompanyICAOCode nvarchar(50),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblCompanies varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblCompanies] CT
                        WHERE CT.PK_CompanyGuid = @CompanyGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblCompanies].[ID],[dbo].[tblCompanies].[Code],[dbo].[tblCompanies].[Name],[dbo].[tblCompanies].[ShortName],[dbo].[tblCompanies].[Address1],[dbo].[tblCompanies].[Address2],[dbo].[tblCompanies].[City],[dbo].[tblCompanies].[State],[dbo].[tblCompanies].[Zip],[dbo].[tblCompanies].[Country],[dbo].[tblCompanies].[Phone],[dbo].[tblCompanies].[FAX],[dbo].[tblCompanies].[EmergencyContact],[dbo].[tblCompanies].[EmergencyPhone],[dbo].[tblCompanies].[FlightPrefix],[dbo].[tblCompanies].[EffectiveDate],[dbo].[tblCompanies].[ExpirationDate],[dbo].[tblCompanies].[OnHold],[dbo].[tblCompanies].[PickupFLights],[dbo].[tblCompanies].[StockTrack],[dbo].[tblCompanies].[SufferLossGain],[dbo].[tblCompanies].[LowStockWarning],[dbo].[tblCompanies].[LockedOut],[dbo].[tblCompanies].[LockedOutReason],[dbo].[tblCompanies].[LockedOutDate],[dbo].[tblCompanies].[ReceivableAccount],[dbo].[tblCompanies].[RefinerCode],[dbo].[tblCompanies].[LastActivityDate],[dbo].[tblCompanies].[CreditOK],[dbo].[tblCompanies].[AdditiveAccounting],[dbo].[tblCompanies].[PurchaseOrderRequired],[dbo].[tblCompanies].[EPANumber],[dbo].[tblCompanies].[FederalID],[dbo].[tblCompanies].[FederalID2],[dbo].[tblCompanies].[FederalID3],[dbo].[tblCompanies].[FederalID4],[dbo].[tblCompanies].[FederalID5],[dbo].[tblCompanies].[StateID],[dbo].[tblCompanies].[TaxNumber],[dbo].[tblCompanies].[FlushPermitted],[dbo].[tblCompanies].[PumpOffPermitted],[dbo].[tblCompanies].[DeliveryToTerminalPermitted],[dbo].[tblCompanies].[LicenseNumber],[dbo].[tblCompanies].[LicenseExpiration],[dbo].[tblCompanies].[InsuranceCompany],[dbo].[tblCompanies].[InsurancePolicy],[dbo].[tblCompanies].[LiabilityAmount],[dbo].[tblCompanies].[HazardousMaterialExclusion],[dbo].[tblCompanies].[InsuranceExpiration],[dbo].[tblCompanies].[AllowDriverEntry],[dbo].[tblCompanies].[PINRequired],[dbo].[tblCompanies].[MaximumVehicleWeight],[dbo].[tblCompanies].[WeightUnits],[dbo].[tblCompanies].[AccountNumber],[dbo].[tblCompanies].[SCACCode],[dbo].[tblCompanies].[DisableOwnerAllocationsCheck],[dbo].[tblCompanies].[DisableShipperAllocationsCheck],[dbo].[tblCompanies].[DisableBillToAllocationsCheck],[dbo].[tblCompanies].[DisableShipToAllocationsCheck],[dbo].[tblCompanies].[LoadRackDisplayText],[dbo].[tblCompanies].[UserData1],[dbo].[tblCompanies].[UserData2],[dbo].[tblCompanies].[UserData3],[dbo].[tblCompanies].[UserData4],[dbo].[tblCompanies].[UserData5],[dbo].[tblCompanies].[UserData6],[dbo].[tblCompanies].[UserData7],[dbo].[tblCompanies].[UserData8],[dbo].[tblCompanies].[CreatedDate],[dbo].[tblCompanies].[CreatedBy],[dbo].[tblCompanies].[UpdatedDate],[dbo].[tblCompanies].[UpdatedBy],[dbo].[tblCompanies].[CompanyGuid],[dbo].[tblCompanies].[SiteGuid],[dbo].[tblCompanies].[IATAGuid],[dbo].[tblCompanies].[ShipperTypeApplicationStringGuid],[dbo].[tblCompanies].[CustomerBillToTypeApplicationStringGuid],[dbo].[tblCompanies].[CustomerShipToTypeApplicationStringGuid],[dbo].[tblCompanies].[Contact1Name],[dbo].[tblCompanies].[Contact1Address1],[dbo].[tblCompanies].[Contact1Address2],[dbo].[tblCompanies].[Contact1City],[dbo].[tblCompanies].[Contact1State],[dbo].[tblCompanies].[Contact1Zip],[dbo].[tblCompanies].[Contact1Country],[dbo].[tblCompanies].[Contact1PhoneOffice],[dbo].[tblCompanies].[Contact1Fax],[dbo].[tblCompanies].[Contact1EmailAddress],[dbo].[tblCompanies].[Contact2Name],[dbo].[tblCompanies].[Contact2Address1],[dbo].[tblCompanies].[Contact2Address2],[dbo].[tblCompanies].[Contact2City],[dbo].[tblCompanies].[Contact2State],[dbo].[tblCompanies].[Contact2Zip],[dbo].[tblCompanies].[Contact2Country],[dbo].[tblCompanies].[Contact2PhoneOffice],[dbo].[tblCompanies].[Contact2Fax],[dbo].[tblCompanies].[Contact2EmailAddress],[dbo].[tblCompanies].[Contact1PhoneMobile],[dbo].[tblCompanies].[Contact2PhoneMobile],[dbo].[tblCompanies].[_MasterRecordGuid],[dbo].[tblCompanies].[Note],[dbo].[tblCompanies].[HiddenDate],[dbo].[tblCompanies].[ScullyRequired],[dbo].[tblCompanies].[ConsortiumTypeIndex],[dbo].[tblCompanies].[CompanyIATACode],[dbo].[tblCompanies].[CompanyICAOCode]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblCompanies]
                        INNER JOIN [track].[tblCompanies] CT
                            ON CT.PK_CompanyGuid = [dbo].[tblCompanies].[CompanyGuid] 
                    WHERE CT.PK_CompanyGuid = @CompanyGuid
            ) MERGE existingData
            USING (SELECT @ID,@Code,@Name,@ShortName,@Address1,@Address2,@City,@State,@Zip,@Country,@Phone,@FAX,@EmergencyContact,@EmergencyPhone,@FlightPrefix,@EffectiveDate,@ExpirationDate,@OnHold,@PickupFLights,@StockTrack,@SufferLossGain,@LowStockWarning,@LockedOut,@LockedOutReason,@LockedOutDate,@ReceivableAccount,@RefinerCode,@LastActivityDate,@CreditOK,@AdditiveAccounting,@PurchaseOrderRequired,@EPANumber,@FederalID,@FederalID2,@FederalID3,@FederalID4,@FederalID5,@StateID,@TaxNumber,@FlushPermitted,@PumpOffPermitted,@DeliveryToTerminalPermitted,@LicenseNumber,@LicenseExpiration,@InsuranceCompany,@InsurancePolicy,@LiabilityAmount,@HazardousMaterialExclusion,@InsuranceExpiration,@AllowDriverEntry,@PINRequired,@MaximumVehicleWeight,@WeightUnits,@AccountNumber,@SCACCode,@DisableOwnerAllocationsCheck,@DisableShipperAllocationsCheck,@DisableBillToAllocationsCheck,@DisableShipToAllocationsCheck,@LoadRackDisplayText,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@CompanyGuid,@SiteGuid,@IATAGuid,@ShipperTypeApplicationStringGuid,@CustomerBillToTypeApplicationStringGuid,@CustomerShipToTypeApplicationStringGuid,@Contact1Name,@Contact1Address1,@Contact1Address2,@Contact1City,@Contact1State,@Contact1Zip,@Contact1Country,@Contact1PhoneOffice,@Contact1Fax,@Contact1EmailAddress,@Contact2Name,@Contact2Address1,@Contact2Address2,@Contact2City,@Contact2State,@Contact2Zip,@Contact2Country,@Contact2PhoneOffice,@Contact2Fax,@Contact2EmailAddress,@Contact1PhoneMobile,@Contact2PhoneMobile,@_MasterRecordGuid,@Note,@HiddenDate,@ScullyRequired,@ConsortiumTypeIndex,@CompanyIATACode,@CompanyICAOCode
                    ) AS remoteChanges ([ID],[Code],[Name],[ShortName],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone],[FAX],[EmergencyContact],[EmergencyPhone],[FlightPrefix],[EffectiveDate],[ExpirationDate],[OnHold],[PickupFLights],[StockTrack],[SufferLossGain],[LowStockWarning],[LockedOut],[LockedOutReason],[LockedOutDate],[ReceivableAccount],[RefinerCode],[LastActivityDate],[CreditOK],[AdditiveAccounting],[PurchaseOrderRequired],[EPANumber],[FederalID],[FederalID2],[FederalID3],[FederalID4],[FederalID5],[StateID],[TaxNumber],[FlushPermitted],[PumpOffPermitted],[DeliveryToTerminalPermitted],[LicenseNumber],[LicenseExpiration],[InsuranceCompany],[InsurancePolicy],[LiabilityAmount],[HazardousMaterialExclusion],[InsuranceExpiration],[AllowDriverEntry],[PINRequired],[MaximumVehicleWeight],[WeightUnits],[AccountNumber],[SCACCode],[DisableOwnerAllocationsCheck],[DisableShipperAllocationsCheck],[DisableBillToAllocationsCheck],[DisableShipToAllocationsCheck],[LoadRackDisplayText],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[CompanyGuid],[SiteGuid],[IATAGuid],[ShipperTypeApplicationStringGuid],[CustomerBillToTypeApplicationStringGuid],[CustomerShipToTypeApplicationStringGuid],[Contact1Name],[Contact1Address1],[Contact1Address2],[Contact1City],[Contact1State],[Contact1Zip],[Contact1Country],[Contact1PhoneOffice],[Contact1Fax],[Contact1EmailAddress],[Contact2Name],[Contact2Address1],[Contact2Address2],[Contact2City],[Contact2State],[Contact2Zip],[Contact2Country],[Contact2PhoneOffice],[Contact2Fax],[Contact2EmailAddress],[Contact1PhoneMobile],[Contact2PhoneMobile],[_MasterRecordGuid],[Note],[HiddenDate],[ScullyRequired],[ConsortiumTypeIndex],[CompanyIATACode],[CompanyICAOCode])
            ON (existingData.[CompanyGuid] = remoteChanges.[CompanyGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[Code] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Code'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Code] ELSE remoteChanges.[Code] END
                       ,[Name] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Name'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Name] ELSE remoteChanges.[Name] END
                       ,[ShortName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShortName'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[ShortName] ELSE remoteChanges.[ShortName] END
                       ,[Address1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Address1'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Address1] ELSE remoteChanges.[Address1] END
                       ,[Address2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Address2'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Address2] ELSE remoteChanges.[Address2] END
                       ,[City] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('City'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[City] ELSE remoteChanges.[City] END
                       ,[State] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('State'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[State] ELSE remoteChanges.[State] END
                       ,[Zip] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Zip'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Zip] ELSE remoteChanges.[Zip] END
                       ,[Country] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Country'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Country] ELSE remoteChanges.[Country] END
                       ,[Phone] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Phone'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Phone] ELSE remoteChanges.[Phone] END
                       ,[FAX] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FAX'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[FAX] ELSE remoteChanges.[FAX] END
                       ,[EmergencyContact] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EmergencyContact'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[EmergencyContact] ELSE remoteChanges.[EmergencyContact] END
                       ,[EmergencyPhone] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EmergencyPhone'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[EmergencyPhone] ELSE remoteChanges.[EmergencyPhone] END
                       ,[FlightPrefix] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlightPrefix'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[FlightPrefix] ELSE remoteChanges.[FlightPrefix] END
                       ,[EffectiveDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EffectiveDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[EffectiveDate] ELSE remoteChanges.[EffectiveDate] END
                       ,[ExpirationDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExpirationDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[ExpirationDate] ELSE remoteChanges.[ExpirationDate] END
                       ,[OnHold] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OnHold'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[OnHold] ELSE remoteChanges.[OnHold] END
                       ,[PickupFLights] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PickupFLights'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[PickupFLights] ELSE remoteChanges.[PickupFLights] END
                       ,[StockTrack] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StockTrack'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[StockTrack] ELSE remoteChanges.[StockTrack] END
                       ,[SufferLossGain] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SufferLossGain'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[SufferLossGain] ELSE remoteChanges.[SufferLossGain] END
                       ,[LowStockWarning] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LowStockWarning'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[LowStockWarning] ELSE remoteChanges.[LowStockWarning] END
                       ,[LockedOut] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOut'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[LockedOut] ELSE remoteChanges.[LockedOut] END
                       ,[LockedOutReason] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutReason'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[LockedOutReason] ELSE remoteChanges.[LockedOutReason] END
                       ,[LockedOutDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[LockedOutDate] ELSE remoteChanges.[LockedOutDate] END
                       ,[ReceivableAccount] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReceivableAccount'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[ReceivableAccount] ELSE remoteChanges.[ReceivableAccount] END
                       ,[RefinerCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RefinerCode'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[RefinerCode] ELSE remoteChanges.[RefinerCode] END
                       ,[LastActivityDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastActivityDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[LastActivityDate] ELSE remoteChanges.[LastActivityDate] END
                       ,[CreditOK] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreditOK'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[CreditOK] ELSE remoteChanges.[CreditOK] END
                       ,[AdditiveAccounting] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveAccounting'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[AdditiveAccounting] ELSE remoteChanges.[AdditiveAccounting] END
                       ,[PurchaseOrderRequired] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PurchaseOrderRequired'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[PurchaseOrderRequired] ELSE remoteChanges.[PurchaseOrderRequired] END
                       ,[EPANumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EPANumber'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[EPANumber] ELSE remoteChanges.[EPANumber] END
                       ,[FederalID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FederalID'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[FederalID] ELSE remoteChanges.[FederalID] END
                       ,[FederalID2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FederalID2'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[FederalID2] ELSE remoteChanges.[FederalID2] END
                       ,[FederalID3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FederalID3'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[FederalID3] ELSE remoteChanges.[FederalID3] END
                       ,[FederalID4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FederalID4'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[FederalID4] ELSE remoteChanges.[FederalID4] END
                       ,[FederalID5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FederalID5'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[FederalID5] ELSE remoteChanges.[FederalID5] END
                       ,[StateID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StateID'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[StateID] ELSE remoteChanges.[StateID] END
                       ,[TaxNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TaxNumber'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[TaxNumber] ELSE remoteChanges.[TaxNumber] END
                       ,[FlushPermitted] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlushPermitted'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[FlushPermitted] ELSE remoteChanges.[FlushPermitted] END
                       ,[PumpOffPermitted] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PumpOffPermitted'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[PumpOffPermitted] ELSE remoteChanges.[PumpOffPermitted] END
                       ,[DeliveryToTerminalPermitted] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeliveryToTerminalPermitted'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[DeliveryToTerminalPermitted] ELSE remoteChanges.[DeliveryToTerminalPermitted] END
                       ,[LicenseNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LicenseNumber'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[LicenseNumber] ELSE remoteChanges.[LicenseNumber] END
                       ,[LicenseExpiration] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LicenseExpiration'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[LicenseExpiration] ELSE remoteChanges.[LicenseExpiration] END
                       ,[InsuranceCompany] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InsuranceCompany'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[InsuranceCompany] ELSE remoteChanges.[InsuranceCompany] END
                       ,[InsurancePolicy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InsurancePolicy'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[InsurancePolicy] ELSE remoteChanges.[InsurancePolicy] END
                       ,[LiabilityAmount] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LiabilityAmount'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[LiabilityAmount] ELSE remoteChanges.[LiabilityAmount] END
                       ,[HazardousMaterialExclusion] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HazardousMaterialExclusion'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[HazardousMaterialExclusion] ELSE remoteChanges.[HazardousMaterialExclusion] END
                       ,[InsuranceExpiration] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InsuranceExpiration'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[InsuranceExpiration] ELSE remoteChanges.[InsuranceExpiration] END
                       ,[AllowDriverEntry] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AllowDriverEntry'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[AllowDriverEntry] ELSE remoteChanges.[AllowDriverEntry] END
                       ,[PINRequired] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PINRequired'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[PINRequired] ELSE remoteChanges.[PINRequired] END
                       ,[MaximumVehicleWeight] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MaximumVehicleWeight'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[MaximumVehicleWeight] ELSE remoteChanges.[MaximumVehicleWeight] END
                       ,[WeightUnits] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WeightUnits'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[WeightUnits] ELSE remoteChanges.[WeightUnits] END
                       ,[AccountNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AccountNumber'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[AccountNumber] ELSE remoteChanges.[AccountNumber] END
                       ,[SCACCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SCACCode'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[SCACCode] ELSE remoteChanges.[SCACCode] END
                       ,[DisableOwnerAllocationsCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DisableOwnerAllocationsCheck'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[DisableOwnerAllocationsCheck] ELSE remoteChanges.[DisableOwnerAllocationsCheck] END
                       ,[DisableShipperAllocationsCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DisableShipperAllocationsCheck'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[DisableShipperAllocationsCheck] ELSE remoteChanges.[DisableShipperAllocationsCheck] END
                       ,[DisableBillToAllocationsCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DisableBillToAllocationsCheck'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[DisableBillToAllocationsCheck] ELSE remoteChanges.[DisableBillToAllocationsCheck] END
                       ,[DisableShipToAllocationsCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DisableShipToAllocationsCheck'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[DisableShipToAllocationsCheck] ELSE remoteChanges.[DisableShipToAllocationsCheck] END
                       ,[LoadRackDisplayText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadRackDisplayText'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[LoadRackDisplayText] ELSE remoteChanges.[LoadRackDisplayText] END
                       ,[UserData1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[UserData1] ELSE remoteChanges.[UserData1] END
                       ,[UserData2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[UserData2] ELSE remoteChanges.[UserData2] END
                       ,[UserData3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[UserData3] ELSE remoteChanges.[UserData3] END
                       ,[UserData4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[UserData4] ELSE remoteChanges.[UserData4] END
                       ,[UserData5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[UserData5] ELSE remoteChanges.[UserData5] END
                       ,[UserData6] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[UserData6] ELSE remoteChanges.[UserData6] END
                       ,[UserData7] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[UserData7] ELSE remoteChanges.[UserData7] END
                       ,[UserData8] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[UserData8] ELSE remoteChanges.[UserData8] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[IATAGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IATAGuid'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[IATAGuid] ELSE remoteChanges.[IATAGuid] END
                       ,[ShipperTypeApplicationStringGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipperTypeApplicationStringGuid'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[ShipperTypeApplicationStringGuid] ELSE remoteChanges.[ShipperTypeApplicationStringGuid] END
                       ,[CustomerBillToTypeApplicationStringGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CustomerBillToTypeApplicationStringGuid'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[CustomerBillToTypeApplicationStringGuid] ELSE remoteChanges.[CustomerBillToTypeApplicationStringGuid] END
                       ,[CustomerShipToTypeApplicationStringGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CustomerShipToTypeApplicationStringGuid'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[CustomerShipToTypeApplicationStringGuid] ELSE remoteChanges.[CustomerShipToTypeApplicationStringGuid] END
                       ,[Contact1Name] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Name'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1Name] ELSE remoteChanges.[Contact1Name] END
                       ,[Contact1Address1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Address1'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1Address1] ELSE remoteChanges.[Contact1Address1] END
                       ,[Contact1Address2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Address2'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1Address2] ELSE remoteChanges.[Contact1Address2] END
                       ,[Contact1City] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1City'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1City] ELSE remoteChanges.[Contact1City] END
                       ,[Contact1State] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1State'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1State] ELSE remoteChanges.[Contact1State] END
                       ,[Contact1Zip] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Zip'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1Zip] ELSE remoteChanges.[Contact1Zip] END
                       ,[Contact1Country] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Country'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1Country] ELSE remoteChanges.[Contact1Country] END
                       ,[Contact1PhoneOffice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1PhoneOffice'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1PhoneOffice] ELSE remoteChanges.[Contact1PhoneOffice] END
                       ,[Contact1Fax] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Fax'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1Fax] ELSE remoteChanges.[Contact1Fax] END
                       ,[Contact1EmailAddress] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1EmailAddress'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1EmailAddress] ELSE remoteChanges.[Contact1EmailAddress] END
                       ,[Contact2Name] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Name'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2Name] ELSE remoteChanges.[Contact2Name] END
                       ,[Contact2Address1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Address1'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2Address1] ELSE remoteChanges.[Contact2Address1] END
                       ,[Contact2Address2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Address2'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2Address2] ELSE remoteChanges.[Contact2Address2] END
                       ,[Contact2City] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2City'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2City] ELSE remoteChanges.[Contact2City] END
                       ,[Contact2State] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2State'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2State] ELSE remoteChanges.[Contact2State] END
                       ,[Contact2Zip] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Zip'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2Zip] ELSE remoteChanges.[Contact2Zip] END
                       ,[Contact2Country] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Country'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2Country] ELSE remoteChanges.[Contact2Country] END
                       ,[Contact2PhoneOffice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2PhoneOffice'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2PhoneOffice] ELSE remoteChanges.[Contact2PhoneOffice] END
                       ,[Contact2Fax] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Fax'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2Fax] ELSE remoteChanges.[Contact2Fax] END
                       ,[Contact2EmailAddress] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2EmailAddress'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2EmailAddress] ELSE remoteChanges.[Contact2EmailAddress] END
                       ,[Contact1PhoneMobile] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1PhoneMobile'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact1PhoneMobile] ELSE remoteChanges.[Contact1PhoneMobile] END
                       ,[Contact2PhoneMobile] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2PhoneMobile'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Contact2PhoneMobile] ELSE remoteChanges.[Contact2PhoneMobile] END
                       ,[_MasterRecordGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('_MasterRecordGuid'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[_MasterRecordGuid] ELSE remoteChanges.[_MasterRecordGuid] END
                       ,[Note] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Note'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[Note] ELSE remoteChanges.[Note] END
                       ,[HiddenDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[HiddenDate] ELSE remoteChanges.[HiddenDate] END
                       ,[ScullyRequired] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ScullyRequired'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[ScullyRequired] ELSE remoteChanges.[ScullyRequired] END
                       ,[ConsortiumTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ConsortiumTypeIndex'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[ConsortiumTypeIndex] ELSE remoteChanges.[ConsortiumTypeIndex] END
                       ,[CompanyIATACode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyIATACode'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[CompanyIATACode] ELSE remoteChanges.[CompanyIATACode] END
                       ,[CompanyICAOCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyICAOCode'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN existingData.[CompanyICAOCode] ELSE remoteChanges.[CompanyICAOCode] END

            WHEN NOT MATCHED THEN
                INSERT ([ID],[Code],[Name],[ShortName],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone],[FAX],[EmergencyContact],[EmergencyPhone],[FlightPrefix],[EffectiveDate],[ExpirationDate],[OnHold],[PickupFLights],[StockTrack],[SufferLossGain],[LowStockWarning],[LockedOut],[LockedOutReason],[LockedOutDate],[ReceivableAccount],[RefinerCode],[LastActivityDate],[CreditOK],[AdditiveAccounting],[PurchaseOrderRequired],[EPANumber],[FederalID],[FederalID2],[FederalID3],[FederalID4],[FederalID5],[StateID],[TaxNumber],[FlushPermitted],[PumpOffPermitted],[DeliveryToTerminalPermitted],[LicenseNumber],[LicenseExpiration],[InsuranceCompany],[InsurancePolicy],[LiabilityAmount],[HazardousMaterialExclusion],[InsuranceExpiration],[AllowDriverEntry],[PINRequired],[MaximumVehicleWeight],[WeightUnits],[AccountNumber],[SCACCode],[DisableOwnerAllocationsCheck],[DisableShipperAllocationsCheck],[DisableBillToAllocationsCheck],[DisableShipToAllocationsCheck],[LoadRackDisplayText],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[CompanyGuid],[SiteGuid],[IATAGuid],[ShipperTypeApplicationStringGuid],[CustomerBillToTypeApplicationStringGuid],[CustomerShipToTypeApplicationStringGuid],[Contact1Name],[Contact1Address1],[Contact1Address2],[Contact1City],[Contact1State],[Contact1Zip],[Contact1Country],[Contact1PhoneOffice],[Contact1Fax],[Contact1EmailAddress],[Contact2Name],[Contact2Address1],[Contact2Address2],[Contact2City],[Contact2State],[Contact2Zip],[Contact2Country],[Contact2PhoneOffice],[Contact2Fax],[Contact2EmailAddress],[Contact1PhoneMobile],[Contact2PhoneMobile],[_MasterRecordGuid],[Note],[HiddenDate],[ScullyRequired],[ConsortiumTypeIndex],[CompanyIATACode],[CompanyICAOCode])
                    VALUES (@ID,@Code,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Name'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Name END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShortName'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @ShortName END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Address1'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Address1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Address2'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Address2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('City'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @City END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('State'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @State END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Zip'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Zip END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Country'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Country END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Phone'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Phone END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FAX'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @FAX END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EmergencyContact'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @EmergencyContact END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EmergencyPhone'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @EmergencyPhone END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlightPrefix'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @FlightPrefix END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EffectiveDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @EffectiveDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExpirationDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @ExpirationDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OnHold'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @OnHold END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PickupFLights'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @PickupFLights END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StockTrack'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @StockTrack END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SufferLossGain'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @SufferLossGain END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LowStockWarning'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @LowStockWarning END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOut'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @LockedOut END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutReason'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @LockedOutReason END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @LockedOutDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReceivableAccount'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @ReceivableAccount END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RefinerCode'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @RefinerCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastActivityDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @LastActivityDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreditOK'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @CreditOK END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveAccounting'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @AdditiveAccounting END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PurchaseOrderRequired'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @PurchaseOrderRequired END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EPANumber'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @EPANumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FederalID'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @FederalID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FederalID2'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @FederalID2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FederalID3'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @FederalID3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FederalID4'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @FederalID4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FederalID5'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @FederalID5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StateID'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @StateID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TaxNumber'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @TaxNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlushPermitted'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @FlushPermitted END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PumpOffPermitted'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @PumpOffPermitted END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeliveryToTerminalPermitted'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @DeliveryToTerminalPermitted END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LicenseNumber'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @LicenseNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LicenseExpiration'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @LicenseExpiration END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InsuranceCompany'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @InsuranceCompany END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InsurancePolicy'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @InsurancePolicy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LiabilityAmount'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @LiabilityAmount END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HazardousMaterialExclusion'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @HazardousMaterialExclusion END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InsuranceExpiration'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @InsuranceExpiration END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AllowDriverEntry'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @AllowDriverEntry END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PINRequired'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @PINRequired END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MaximumVehicleWeight'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @MaximumVehicleWeight END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WeightUnits'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @WeightUnits END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AccountNumber'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @AccountNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SCACCode'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @SCACCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DisableOwnerAllocationsCheck'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @DisableOwnerAllocationsCheck END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DisableShipperAllocationsCheck'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @DisableShipperAllocationsCheck END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DisableBillToAllocationsCheck'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @DisableBillToAllocationsCheck END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DisableShipToAllocationsCheck'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @DisableShipToAllocationsCheck END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadRackDisplayText'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @LoadRackDisplayText END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @UserData1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @UserData2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @UserData3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @UserData4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @UserData5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @UserData6 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @UserData7 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @UserData8 END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@CompanyGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IATAGuid'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @IATAGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipperTypeApplicationStringGuid'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @ShipperTypeApplicationStringGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CustomerBillToTypeApplicationStringGuid'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @CustomerBillToTypeApplicationStringGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CustomerShipToTypeApplicationStringGuid'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @CustomerShipToTypeApplicationStringGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Name'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1Name END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Address1'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1Address1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Address2'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1Address2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1City'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1City END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1State'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1State END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Zip'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1Zip END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Country'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1Country END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1PhoneOffice'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1PhoneOffice END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1Fax'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1Fax END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1EmailAddress'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1EmailAddress END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Name'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2Name END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Address1'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2Address1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Address2'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2Address2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2City'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2City END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2State'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2State END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Zip'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2Zip END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Country'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2Country END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2PhoneOffice'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2PhoneOffice END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2Fax'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2Fax END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2EmailAddress'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2EmailAddress END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact1PhoneMobile'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact1PhoneMobile END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Contact2PhoneMobile'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Contact2PhoneMobile END),@_MasterRecordGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Note'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @Note END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @HiddenDate END),@ScullyRequired,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ConsortiumTypeIndex'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @ConsortiumTypeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyIATACode'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @CompanyIATACode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyICAOCode'), @sync_supported_columns_tblCompanies)) WHEN 0 THEN NULL ELSE @CompanyICAOCode END))
            ;
         SET @sync_row_count = @@rowcount;
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END

    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @CompanyGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @CompanyGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @CompanyGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblCompanies] WHERE CompanyGuid = @CompanyGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
