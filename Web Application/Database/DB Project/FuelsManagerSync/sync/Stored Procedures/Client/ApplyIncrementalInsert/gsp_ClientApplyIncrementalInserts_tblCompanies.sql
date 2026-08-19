-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblCompanies
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblCompanies]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
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
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblCompanies] AS existingData
        USING (SELECT @ID 'ID',@Code 'Code',@Name 'Name',@ShortName 'ShortName',@Address1 'Address1',@Address2 'Address2',@City 'City',@State 'State',@Zip 'Zip',@Country 'Country',@Phone 'Phone',@FAX 'FAX',@EmergencyContact 'EmergencyContact',@EmergencyPhone 'EmergencyPhone',@FlightPrefix 'FlightPrefix',@EffectiveDate 'EffectiveDate',@ExpirationDate 'ExpirationDate',@OnHold 'OnHold',@PickupFLights 'PickupFLights',@StockTrack 'StockTrack',@SufferLossGain 'SufferLossGain',@LowStockWarning 'LowStockWarning',@LockedOut 'LockedOut',@LockedOutReason 'LockedOutReason',@LockedOutDate 'LockedOutDate',@ReceivableAccount 'ReceivableAccount',@RefinerCode 'RefinerCode',@LastActivityDate 'LastActivityDate',@CreditOK 'CreditOK',@AdditiveAccounting 'AdditiveAccounting',@PurchaseOrderRequired 'PurchaseOrderRequired',@EPANumber 'EPANumber',@FederalID 'FederalID',@FederalID2 'FederalID2',@FederalID3 'FederalID3',@FederalID4 'FederalID4',@FederalID5 'FederalID5',@StateID 'StateID',@TaxNumber 'TaxNumber',@FlushPermitted 'FlushPermitted',@PumpOffPermitted 'PumpOffPermitted',@DeliveryToTerminalPermitted 'DeliveryToTerminalPermitted',@LicenseNumber 'LicenseNumber',@LicenseExpiration 'LicenseExpiration',@InsuranceCompany 'InsuranceCompany',@InsurancePolicy 'InsurancePolicy',@LiabilityAmount 'LiabilityAmount',@HazardousMaterialExclusion 'HazardousMaterialExclusion',@InsuranceExpiration 'InsuranceExpiration',@AllowDriverEntry 'AllowDriverEntry',@PINRequired 'PINRequired',@MaximumVehicleWeight 'MaximumVehicleWeight',@WeightUnits 'WeightUnits',@AccountNumber 'AccountNumber',@SCACCode 'SCACCode',@DisableOwnerAllocationsCheck 'DisableOwnerAllocationsCheck',@DisableShipperAllocationsCheck 'DisableShipperAllocationsCheck',@DisableBillToAllocationsCheck 'DisableBillToAllocationsCheck',@DisableShipToAllocationsCheck 'DisableShipToAllocationsCheck',@LoadRackDisplayText 'LoadRackDisplayText',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@CompanyGuid 'CompanyGuid',@SiteGuid 'SiteGuid',@IATAGuid 'IATAGuid',@ShipperTypeApplicationStringGuid 'ShipperTypeApplicationStringGuid',@CustomerBillToTypeApplicationStringGuid 'CustomerBillToTypeApplicationStringGuid',@CustomerShipToTypeApplicationStringGuid 'CustomerShipToTypeApplicationStringGuid',@Contact1Name 'Contact1Name',@Contact1Address1 'Contact1Address1',@Contact1Address2 'Contact1Address2',@Contact1City 'Contact1City',@Contact1State 'Contact1State',@Contact1Zip 'Contact1Zip',@Contact1Country 'Contact1Country',@Contact1PhoneOffice 'Contact1PhoneOffice',@Contact1Fax 'Contact1Fax',@Contact1EmailAddress 'Contact1EmailAddress',@Contact2Name 'Contact2Name',@Contact2Address1 'Contact2Address1',@Contact2Address2 'Contact2Address2',@Contact2City 'Contact2City',@Contact2State 'Contact2State',@Contact2Zip 'Contact2Zip',@Contact2Country 'Contact2Country',@Contact2PhoneOffice 'Contact2PhoneOffice',@Contact2Fax 'Contact2Fax',@Contact2EmailAddress 'Contact2EmailAddress',@Contact1PhoneMobile 'Contact1PhoneMobile',@Contact2PhoneMobile 'Contact2PhoneMobile',@_MasterRecordGuid '_MasterRecordGuid',@Note 'Note',@HiddenDate 'HiddenDate',@ScullyRequired 'ScullyRequired',@ConsortiumTypeIndex 'ConsortiumTypeIndex',@CompanyIATACode 'CompanyIATACode',@CompanyICAOCode 'CompanyICAOCode'
                ) AS remoteChanges ([ID],[Code],[Name],[ShortName],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone],[FAX],[EmergencyContact],[EmergencyPhone],[FlightPrefix],[EffectiveDate],[ExpirationDate],[OnHold],[PickupFLights],[StockTrack],[SufferLossGain],[LowStockWarning],[LockedOut],[LockedOutReason],[LockedOutDate],[ReceivableAccount],[RefinerCode],[LastActivityDate],[CreditOK],[AdditiveAccounting],[PurchaseOrderRequired],[EPANumber],[FederalID],[FederalID2],[FederalID3],[FederalID4],[FederalID5],[StateID],[TaxNumber],[FlushPermitted],[PumpOffPermitted],[DeliveryToTerminalPermitted],[LicenseNumber],[LicenseExpiration],[InsuranceCompany],[InsurancePolicy],[LiabilityAmount],[HazardousMaterialExclusion],[InsuranceExpiration],[AllowDriverEntry],[PINRequired],[MaximumVehicleWeight],[WeightUnits],[AccountNumber],[SCACCode],[DisableOwnerAllocationsCheck],[DisableShipperAllocationsCheck],[DisableBillToAllocationsCheck],[DisableShipToAllocationsCheck],[LoadRackDisplayText],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[CompanyGuid],[SiteGuid],[IATAGuid],[ShipperTypeApplicationStringGuid],[CustomerBillToTypeApplicationStringGuid],[CustomerShipToTypeApplicationStringGuid],[Contact1Name],[Contact1Address1],[Contact1Address2],[Contact1City],[Contact1State],[Contact1Zip],[Contact1Country],[Contact1PhoneOffice],[Contact1Fax],[Contact1EmailAddress],[Contact2Name],[Contact2Address1],[Contact2Address2],[Contact2City],[Contact2State],[Contact2Zip],[Contact2Country],[Contact2PhoneOffice],[Contact2Fax],[Contact2EmailAddress],[Contact1PhoneMobile],[Contact2PhoneMobile],[_MasterRecordGuid],[Note],[HiddenDate],[ScullyRequired],[ConsortiumTypeIndex],[CompanyIATACode],[CompanyICAOCode])
        ON (existingData.[CompanyGuid] = remoteChanges.[CompanyGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ID] = remoteChanges.[ID]
                       ,[Code] = remoteChanges.[Code]
                       ,[Name] = remoteChanges.[Name]
                       ,[ShortName] = remoteChanges.[ShortName]
                       ,[Address1] = remoteChanges.[Address1]
                       ,[Address2] = remoteChanges.[Address2]
                       ,[City] = remoteChanges.[City]
                       ,[State] = remoteChanges.[State]
                       ,[Zip] = remoteChanges.[Zip]
                       ,[Country] = remoteChanges.[Country]
                       ,[Phone] = remoteChanges.[Phone]
                       ,[FAX] = remoteChanges.[FAX]
                       ,[EmergencyContact] = remoteChanges.[EmergencyContact]
                       ,[EmergencyPhone] = remoteChanges.[EmergencyPhone]
                       ,[FlightPrefix] = remoteChanges.[FlightPrefix]
                       ,[EffectiveDate] = remoteChanges.[EffectiveDate]
                       ,[ExpirationDate] = remoteChanges.[ExpirationDate]
                       ,[OnHold] = remoteChanges.[OnHold]
                       ,[PickupFLights] = remoteChanges.[PickupFLights]
                       ,[StockTrack] = remoteChanges.[StockTrack]
                       ,[SufferLossGain] = remoteChanges.[SufferLossGain]
                       ,[LowStockWarning] = remoteChanges.[LowStockWarning]
                       ,[LockedOut] = remoteChanges.[LockedOut]
                       ,[LockedOutReason] = remoteChanges.[LockedOutReason]
                       ,[LockedOutDate] = remoteChanges.[LockedOutDate]
                       ,[ReceivableAccount] = remoteChanges.[ReceivableAccount]
                       ,[RefinerCode] = remoteChanges.[RefinerCode]
                       ,[LastActivityDate] = remoteChanges.[LastActivityDate]
                       ,[CreditOK] = remoteChanges.[CreditOK]
                       ,[AdditiveAccounting] = remoteChanges.[AdditiveAccounting]
                       ,[PurchaseOrderRequired] = remoteChanges.[PurchaseOrderRequired]
                       ,[EPANumber] = remoteChanges.[EPANumber]
                       ,[FederalID] = remoteChanges.[FederalID]
                       ,[FederalID2] = remoteChanges.[FederalID2]
                       ,[FederalID3] = remoteChanges.[FederalID3]
                       ,[FederalID4] = remoteChanges.[FederalID4]
                       ,[FederalID5] = remoteChanges.[FederalID5]
                       ,[StateID] = remoteChanges.[StateID]
                       ,[TaxNumber] = remoteChanges.[TaxNumber]
                       ,[FlushPermitted] = remoteChanges.[FlushPermitted]
                       ,[PumpOffPermitted] = remoteChanges.[PumpOffPermitted]
                       ,[DeliveryToTerminalPermitted] = remoteChanges.[DeliveryToTerminalPermitted]
                       ,[LicenseNumber] = remoteChanges.[LicenseNumber]
                       ,[LicenseExpiration] = remoteChanges.[LicenseExpiration]
                       ,[InsuranceCompany] = remoteChanges.[InsuranceCompany]
                       ,[InsurancePolicy] = remoteChanges.[InsurancePolicy]
                       ,[LiabilityAmount] = remoteChanges.[LiabilityAmount]
                       ,[HazardousMaterialExclusion] = remoteChanges.[HazardousMaterialExclusion]
                       ,[InsuranceExpiration] = remoteChanges.[InsuranceExpiration]
                       ,[AllowDriverEntry] = remoteChanges.[AllowDriverEntry]
                       ,[PINRequired] = remoteChanges.[PINRequired]
                       ,[MaximumVehicleWeight] = remoteChanges.[MaximumVehicleWeight]
                       ,[WeightUnits] = remoteChanges.[WeightUnits]
                       ,[AccountNumber] = remoteChanges.[AccountNumber]
                       ,[SCACCode] = remoteChanges.[SCACCode]
                       ,[DisableOwnerAllocationsCheck] = remoteChanges.[DisableOwnerAllocationsCheck]
                       ,[DisableShipperAllocationsCheck] = remoteChanges.[DisableShipperAllocationsCheck]
                       ,[DisableBillToAllocationsCheck] = remoteChanges.[DisableBillToAllocationsCheck]
                       ,[DisableShipToAllocationsCheck] = remoteChanges.[DisableShipToAllocationsCheck]
                       ,[LoadRackDisplayText] = remoteChanges.[LoadRackDisplayText]
                       ,[UserData1] = remoteChanges.[UserData1]
                       ,[UserData2] = remoteChanges.[UserData2]
                       ,[UserData3] = remoteChanges.[UserData3]
                       ,[UserData4] = remoteChanges.[UserData4]
                       ,[UserData5] = remoteChanges.[UserData5]
                       ,[UserData6] = remoteChanges.[UserData6]
                       ,[UserData7] = remoteChanges.[UserData7]
                       ,[UserData8] = remoteChanges.[UserData8]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[IATAGuid] = remoteChanges.[IATAGuid]
                       ,[ShipperTypeApplicationStringGuid] = remoteChanges.[ShipperTypeApplicationStringGuid]
                       ,[CustomerBillToTypeApplicationStringGuid] = remoteChanges.[CustomerBillToTypeApplicationStringGuid]
                       ,[CustomerShipToTypeApplicationStringGuid] = remoteChanges.[CustomerShipToTypeApplicationStringGuid]
                       ,[Contact1Name] = remoteChanges.[Contact1Name]
                       ,[Contact1Address1] = remoteChanges.[Contact1Address1]
                       ,[Contact1Address2] = remoteChanges.[Contact1Address2]
                       ,[Contact1City] = remoteChanges.[Contact1City]
                       ,[Contact1State] = remoteChanges.[Contact1State]
                       ,[Contact1Zip] = remoteChanges.[Contact1Zip]
                       ,[Contact1Country] = remoteChanges.[Contact1Country]
                       ,[Contact1PhoneOffice] = remoteChanges.[Contact1PhoneOffice]
                       ,[Contact1Fax] = remoteChanges.[Contact1Fax]
                       ,[Contact1EmailAddress] = remoteChanges.[Contact1EmailAddress]
                       ,[Contact2Name] = remoteChanges.[Contact2Name]
                       ,[Contact2Address1] = remoteChanges.[Contact2Address1]
                       ,[Contact2Address2] = remoteChanges.[Contact2Address2]
                       ,[Contact2City] = remoteChanges.[Contact2City]
                       ,[Contact2State] = remoteChanges.[Contact2State]
                       ,[Contact2Zip] = remoteChanges.[Contact2Zip]
                       ,[Contact2Country] = remoteChanges.[Contact2Country]
                       ,[Contact2PhoneOffice] = remoteChanges.[Contact2PhoneOffice]
                       ,[Contact2Fax] = remoteChanges.[Contact2Fax]
                       ,[Contact2EmailAddress] = remoteChanges.[Contact2EmailAddress]
                       ,[Contact1PhoneMobile] = remoteChanges.[Contact1PhoneMobile]
                       ,[Contact2PhoneMobile] = remoteChanges.[Contact2PhoneMobile]
                       ,[_MasterRecordGuid] = remoteChanges.[_MasterRecordGuid]
                       ,[Note] = remoteChanges.[Note]
                       ,[HiddenDate] = remoteChanges.[HiddenDate]
                       ,[ScullyRequired] = remoteChanges.[ScullyRequired]
                       ,[ConsortiumTypeIndex] = remoteChanges.[ConsortiumTypeIndex]
                       ,[CompanyIATACode] = remoteChanges.[CompanyIATACode]
                       ,[CompanyICAOCode] = remoteChanges.[CompanyICAOCode]

        WHEN NOT MATCHED THEN
            INSERT ([ID],[Code],[Name],[ShortName],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone],[FAX],[EmergencyContact],[EmergencyPhone],[FlightPrefix],[EffectiveDate],[ExpirationDate],[OnHold],[PickupFLights],[StockTrack],[SufferLossGain],[LowStockWarning],[LockedOut],[LockedOutReason],[LockedOutDate],[ReceivableAccount],[RefinerCode],[LastActivityDate],[CreditOK],[AdditiveAccounting],[PurchaseOrderRequired],[EPANumber],[FederalID],[FederalID2],[FederalID3],[FederalID4],[FederalID5],[StateID],[TaxNumber],[FlushPermitted],[PumpOffPermitted],[DeliveryToTerminalPermitted],[LicenseNumber],[LicenseExpiration],[InsuranceCompany],[InsurancePolicy],[LiabilityAmount],[HazardousMaterialExclusion],[InsuranceExpiration],[AllowDriverEntry],[PINRequired],[MaximumVehicleWeight],[WeightUnits],[AccountNumber],[SCACCode],[DisableOwnerAllocationsCheck],[DisableShipperAllocationsCheck],[DisableBillToAllocationsCheck],[DisableShipToAllocationsCheck],[LoadRackDisplayText],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[CompanyGuid],[SiteGuid],[IATAGuid],[ShipperTypeApplicationStringGuid],[CustomerBillToTypeApplicationStringGuid],[CustomerShipToTypeApplicationStringGuid],[Contact1Name],[Contact1Address1],[Contact1Address2],[Contact1City],[Contact1State],[Contact1Zip],[Contact1Country],[Contact1PhoneOffice],[Contact1Fax],[Contact1EmailAddress],[Contact2Name],[Contact2Address1],[Contact2Address2],[Contact2City],[Contact2State],[Contact2Zip],[Contact2Country],[Contact2PhoneOffice],[Contact2Fax],[Contact2EmailAddress],[Contact1PhoneMobile],[Contact2PhoneMobile],[_MasterRecordGuid],[Note],[HiddenDate],[ScullyRequired],[ConsortiumTypeIndex],[CompanyIATACode],[CompanyICAOCode])
                VALUES (@ID,@Code,@Name,@ShortName,@Address1,@Address2,@City,@State,@Zip,@Country,@Phone,@FAX,@EmergencyContact,@EmergencyPhone,@FlightPrefix,@EffectiveDate,@ExpirationDate,@OnHold,@PickupFLights,@StockTrack,@SufferLossGain,@LowStockWarning,@LockedOut,@LockedOutReason,@LockedOutDate,@ReceivableAccount,@RefinerCode,@LastActivityDate,@CreditOK,@AdditiveAccounting,@PurchaseOrderRequired,@EPANumber,@FederalID,@FederalID2,@FederalID3,@FederalID4,@FederalID5,@StateID,@TaxNumber,@FlushPermitted,@PumpOffPermitted,@DeliveryToTerminalPermitted,@LicenseNumber,@LicenseExpiration,@InsuranceCompany,@InsurancePolicy,@LiabilityAmount,@HazardousMaterialExclusion,@InsuranceExpiration,@AllowDriverEntry,@PINRequired,@MaximumVehicleWeight,@WeightUnits,@AccountNumber,@SCACCode,@DisableOwnerAllocationsCheck,@DisableShipperAllocationsCheck,@DisableBillToAllocationsCheck,@DisableShipToAllocationsCheck,@LoadRackDisplayText,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@CompanyGuid,@SiteGuid,@IATAGuid,@ShipperTypeApplicationStringGuid,@CustomerBillToTypeApplicationStringGuid,@CustomerShipToTypeApplicationStringGuid,@Contact1Name,@Contact1Address1,@Contact1Address2,@Contact1City,@Contact1State,@Contact1Zip,@Contact1Country,@Contact1PhoneOffice,@Contact1Fax,@Contact1EmailAddress,@Contact2Name,@Contact2Address1,@Contact2Address2,@Contact2City,@Contact2State,@Contact2Zip,@Contact2Country,@Contact2PhoneOffice,@Contact2Fax,@Contact2EmailAddress,@Contact1PhoneMobile,@Contact2PhoneMobile,@_MasterRecordGuid,@Note,@HiddenDate,@ScullyRequired,@ConsortiumTypeIndex,@CompanyIATACode,@CompanyICAOCode)
        ;
    
    SET @sync_row_count = @@rowcount;
    
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
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
