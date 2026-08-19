-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblCompanies
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblCompanies]
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
@sync_table_name nvarchar(512)
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
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
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
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
