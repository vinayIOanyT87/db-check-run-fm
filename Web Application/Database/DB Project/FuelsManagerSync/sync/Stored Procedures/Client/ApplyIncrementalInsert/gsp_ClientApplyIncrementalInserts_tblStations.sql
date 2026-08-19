-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblStations
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblStations]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(50),
@SwingArmPosition bit,
@VaporRecovery bit,
@Enabled bit,
@BOLPrinter nvarchar(80),
@PreloadPrinter nvarchar(80),
@BOLAgeInMinutes int,
@CardReader bit,
@ThirtyFiveBitCardSupport bit,
@NumberOfCopies int,
@NumberOfPreloadCopies int,
@InhibitLoadingByLoadID bit,
@InhibitOperatingModePrompt bit,
@SynchronizeReferenceDensity bit,
@SignatureDevice nvarchar(20),
@SetDefaultPresetToZero bit,
@ArmsServiced nvarchar(100),
@InhibitSettingRecipeNames bit,
@SignatureDevicePort int,
@SignatureDeviceBaudRate int,
@MeterRecircCardNumber nvarchar(30),
@TouchKeyReader bit,
@OffLoadByOffLoadID bit,
@UseManualMeterData bit,
@PromptForBOLNumber bit,
@QueryForTrailers bit,
@PromptForGravityCaptured bit,
@PromptForTemperatureCaptured bit,
@LastTransactionNumber int,
@LastTransactionNumberDateTime datetimeoffset(7),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@StationGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupStationTypeIndex int,
@LookupStationInterfaceTypeIndex int,
@TankGuid uniqueidentifier,
@IssueByVolumeTransactionAliasGuid uniqueidentifier,
@IssueByWeightTransactionAliasGuid uniqueidentifier,
@ReceiptByVolumeTransactionAliasGuid uniqueidentifier,
@ReceiptByWeightTransactionAliasGuid uniqueidentifier,
@RecircTransactionAliasGuid uniqueidentifier,
@LogCommunications bit,
@LogCommPath nvarchar(255),
@EnableScully bit,
@EnableEquipmentValidate bit,
@StationPromptTimeout int,
@StationMessageTimeout int,
@AssignedMeterGuid uniqueidentifier,
@EnableDynamicRecipes bit,
@EthanolExcess bit,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    ;   MERGE [dbo].[tblStations] AS existingData
        USING (SELECT @ID 'ID',@SwingArmPosition 'SwingArmPosition',@VaporRecovery 'VaporRecovery',@Enabled 'Enabled',@BOLPrinter 'BOLPrinter',@PreloadPrinter 'PreloadPrinter',@BOLAgeInMinutes 'BOLAgeInMinutes',@CardReader 'CardReader',@ThirtyFiveBitCardSupport 'ThirtyFiveBitCardSupport',@NumberOfCopies 'NumberOfCopies',@NumberOfPreloadCopies 'NumberOfPreloadCopies',@InhibitLoadingByLoadID 'InhibitLoadingByLoadID',@InhibitOperatingModePrompt 'InhibitOperatingModePrompt',@SynchronizeReferenceDensity 'SynchronizeReferenceDensity',@SignatureDevice 'SignatureDevice',@SetDefaultPresetToZero 'SetDefaultPresetToZero',@ArmsServiced 'ArmsServiced',@InhibitSettingRecipeNames 'InhibitSettingRecipeNames',@SignatureDevicePort 'SignatureDevicePort',@SignatureDeviceBaudRate 'SignatureDeviceBaudRate',@MeterRecircCardNumber 'MeterRecircCardNumber',@TouchKeyReader 'TouchKeyReader',@OffLoadByOffLoadID 'OffLoadByOffLoadID',@UseManualMeterData 'UseManualMeterData',@PromptForBOLNumber 'PromptForBOLNumber',@QueryForTrailers 'QueryForTrailers',@PromptForGravityCaptured 'PromptForGravityCaptured',@PromptForTemperatureCaptured 'PromptForTemperatureCaptured',@LastTransactionNumber 'LastTransactionNumber',@LastTransactionNumberDateTime 'LastTransactionNumberDateTime',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@StationGuid 'StationGuid',@SiteGuid 'SiteGuid',@LookupStationTypeIndex 'LookupStationTypeIndex',@LookupStationInterfaceTypeIndex 'LookupStationInterfaceTypeIndex',@TankGuid 'TankGuid',@IssueByVolumeTransactionAliasGuid 'IssueByVolumeTransactionAliasGuid',@IssueByWeightTransactionAliasGuid 'IssueByWeightTransactionAliasGuid',@ReceiptByVolumeTransactionAliasGuid 'ReceiptByVolumeTransactionAliasGuid',@ReceiptByWeightTransactionAliasGuid 'ReceiptByWeightTransactionAliasGuid',@RecircTransactionAliasGuid 'RecircTransactionAliasGuid',@LogCommunications 'LogCommunications',@LogCommPath 'LogCommPath',@EnableScully 'EnableScully',@EnableEquipmentValidate 'EnableEquipmentValidate',@StationPromptTimeout 'StationPromptTimeout',@StationMessageTimeout 'StationMessageTimeout',@AssignedMeterGuid 'AssignedMeterGuid',@EnableDynamicRecipes 'EnableDynamicRecipes',@EthanolExcess 'EthanolExcess'
                ) AS remoteChanges ([ID],[SwingArmPosition],[VaporRecovery],[Enabled],[BOLPrinter],[PreloadPrinter],[BOLAgeInMinutes],[CardReader],[ThirtyFiveBitCardSupport],[NumberOfCopies],[NumberOfPreloadCopies],[InhibitLoadingByLoadID],[InhibitOperatingModePrompt],[SynchronizeReferenceDensity],[SignatureDevice],[SetDefaultPresetToZero],[ArmsServiced],[InhibitSettingRecipeNames],[SignatureDevicePort],[SignatureDeviceBaudRate],[MeterRecircCardNumber],[TouchKeyReader],[OffLoadByOffLoadID],[UseManualMeterData],[PromptForBOLNumber],[QueryForTrailers],[PromptForGravityCaptured],[PromptForTemperatureCaptured],[LastTransactionNumber],[LastTransactionNumberDateTime],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StationGuid],[SiteGuid],[LookupStationTypeIndex],[LookupStationInterfaceTypeIndex],[TankGuid],[IssueByVolumeTransactionAliasGuid],[IssueByWeightTransactionAliasGuid],[ReceiptByVolumeTransactionAliasGuid],[ReceiptByWeightTransactionAliasGuid],[RecircTransactionAliasGuid],[LogCommunications],[LogCommPath],[EnableScully],[EnableEquipmentValidate],[StationPromptTimeout],[StationMessageTimeout],[AssignedMeterGuid],[EnableDynamicRecipes],[EthanolExcess])
        ON (existingData.[StationGuid] = remoteChanges.[StationGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ID] = remoteChanges.[ID]
                       ,[SwingArmPosition] = remoteChanges.[SwingArmPosition]
                       ,[VaporRecovery] = remoteChanges.[VaporRecovery]
                       ,[Enabled] = remoteChanges.[Enabled]
                       ,[BOLPrinter] = remoteChanges.[BOLPrinter]
                       ,[PreloadPrinter] = remoteChanges.[PreloadPrinter]
                       ,[BOLAgeInMinutes] = remoteChanges.[BOLAgeInMinutes]
                       ,[CardReader] = remoteChanges.[CardReader]
                       ,[ThirtyFiveBitCardSupport] = remoteChanges.[ThirtyFiveBitCardSupport]
                       ,[NumberOfCopies] = remoteChanges.[NumberOfCopies]
                       ,[NumberOfPreloadCopies] = remoteChanges.[NumberOfPreloadCopies]
                       ,[InhibitLoadingByLoadID] = remoteChanges.[InhibitLoadingByLoadID]
                       ,[InhibitOperatingModePrompt] = remoteChanges.[InhibitOperatingModePrompt]
                       ,[SynchronizeReferenceDensity] = remoteChanges.[SynchronizeReferenceDensity]
                       ,[SignatureDevice] = remoteChanges.[SignatureDevice]
                       ,[SetDefaultPresetToZero] = remoteChanges.[SetDefaultPresetToZero]
                       ,[ArmsServiced] = remoteChanges.[ArmsServiced]
                       ,[InhibitSettingRecipeNames] = remoteChanges.[InhibitSettingRecipeNames]
                       ,[SignatureDevicePort] = remoteChanges.[SignatureDevicePort]
                       ,[SignatureDeviceBaudRate] = remoteChanges.[SignatureDeviceBaudRate]
                       ,[MeterRecircCardNumber] = remoteChanges.[MeterRecircCardNumber]
                       ,[TouchKeyReader] = remoteChanges.[TouchKeyReader]
                       ,[OffLoadByOffLoadID] = remoteChanges.[OffLoadByOffLoadID]
                       ,[UseManualMeterData] = remoteChanges.[UseManualMeterData]
                       ,[PromptForBOLNumber] = remoteChanges.[PromptForBOLNumber]
                       ,[QueryForTrailers] = remoteChanges.[QueryForTrailers]
                       ,[PromptForGravityCaptured] = remoteChanges.[PromptForGravityCaptured]
                       ,[PromptForTemperatureCaptured] = remoteChanges.[PromptForTemperatureCaptured]
                       ,[LastTransactionNumber] = remoteChanges.[LastTransactionNumber]
                       ,[LastTransactionNumberDateTime] = remoteChanges.[LastTransactionNumberDateTime]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[LookupStationTypeIndex] = remoteChanges.[LookupStationTypeIndex]
                       ,[LookupStationInterfaceTypeIndex] = remoteChanges.[LookupStationInterfaceTypeIndex]
                       ,[TankGuid] = remoteChanges.[TankGuid]
                       ,[IssueByVolumeTransactionAliasGuid] = remoteChanges.[IssueByVolumeTransactionAliasGuid]
                       ,[IssueByWeightTransactionAliasGuid] = remoteChanges.[IssueByWeightTransactionAliasGuid]
                       ,[ReceiptByVolumeTransactionAliasGuid] = remoteChanges.[ReceiptByVolumeTransactionAliasGuid]
                       ,[ReceiptByWeightTransactionAliasGuid] = remoteChanges.[ReceiptByWeightTransactionAliasGuid]
                       ,[RecircTransactionAliasGuid] = remoteChanges.[RecircTransactionAliasGuid]
                       ,[LogCommunications] = remoteChanges.[LogCommunications]
                       ,[LogCommPath] = remoteChanges.[LogCommPath]
                       ,[EnableScully] = remoteChanges.[EnableScully]
                       ,[EnableEquipmentValidate] = remoteChanges.[EnableEquipmentValidate]
                       ,[StationPromptTimeout] = remoteChanges.[StationPromptTimeout]
                       ,[StationMessageTimeout] = remoteChanges.[StationMessageTimeout]
                       ,[AssignedMeterGuid] = remoteChanges.[AssignedMeterGuid]
                       ,[EnableDynamicRecipes] = remoteChanges.[EnableDynamicRecipes]
                       ,[EthanolExcess] = remoteChanges.[EthanolExcess]

        WHEN NOT MATCHED THEN
            INSERT ([ID],[SwingArmPosition],[VaporRecovery],[Enabled],[BOLPrinter],[PreloadPrinter],[BOLAgeInMinutes],[CardReader],[ThirtyFiveBitCardSupport],[NumberOfCopies],[NumberOfPreloadCopies],[InhibitLoadingByLoadID],[InhibitOperatingModePrompt],[SynchronizeReferenceDensity],[SignatureDevice],[SetDefaultPresetToZero],[ArmsServiced],[InhibitSettingRecipeNames],[SignatureDevicePort],[SignatureDeviceBaudRate],[MeterRecircCardNumber],[TouchKeyReader],[OffLoadByOffLoadID],[UseManualMeterData],[PromptForBOLNumber],[QueryForTrailers],[PromptForGravityCaptured],[PromptForTemperatureCaptured],[LastTransactionNumber],[LastTransactionNumberDateTime],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StationGuid],[SiteGuid],[LookupStationTypeIndex],[LookupStationInterfaceTypeIndex],[TankGuid],[IssueByVolumeTransactionAliasGuid],[IssueByWeightTransactionAliasGuid],[ReceiptByVolumeTransactionAliasGuid],[ReceiptByWeightTransactionAliasGuid],[RecircTransactionAliasGuid],[LogCommunications],[LogCommPath],[EnableScully],[EnableEquipmentValidate],[StationPromptTimeout],[StationMessageTimeout],[AssignedMeterGuid],[EnableDynamicRecipes],[EthanolExcess])
                VALUES (@ID,@SwingArmPosition,@VaporRecovery,@Enabled,@BOLPrinter,@PreloadPrinter,@BOLAgeInMinutes,@CardReader,@ThirtyFiveBitCardSupport,@NumberOfCopies,@NumberOfPreloadCopies,@InhibitLoadingByLoadID,@InhibitOperatingModePrompt,@SynchronizeReferenceDensity,@SignatureDevice,@SetDefaultPresetToZero,@ArmsServiced,@InhibitSettingRecipeNames,@SignatureDevicePort,@SignatureDeviceBaudRate,@MeterRecircCardNumber,@TouchKeyReader,@OffLoadByOffLoadID,@UseManualMeterData,@PromptForBOLNumber,@QueryForTrailers,@PromptForGravityCaptured,@PromptForTemperatureCaptured,@LastTransactionNumber,@LastTransactionNumberDateTime,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@StationGuid,@SiteGuid,@LookupStationTypeIndex,@LookupStationInterfaceTypeIndex,@TankGuid,@IssueByVolumeTransactionAliasGuid,@IssueByWeightTransactionAliasGuid,@ReceiptByVolumeTransactionAliasGuid,@ReceiptByWeightTransactionAliasGuid,@RecircTransactionAliasGuid,@LogCommunications,@LogCommPath,@EnableScully,@EnableEquipmentValidate,@StationPromptTimeout,@StationMessageTimeout,@AssignedMeterGuid,@EnableDynamicRecipes,@EthanolExcess)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @StationGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @StationGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @StationGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
