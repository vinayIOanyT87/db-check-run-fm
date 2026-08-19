-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblStations
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblStations]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblStations varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblStations] CT
                        WHERE CT.PK_StationGuid = @StationGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblStations].[ID],[dbo].[tblStations].[SwingArmPosition],[dbo].[tblStations].[VaporRecovery],[dbo].[tblStations].[Enabled],[dbo].[tblStations].[BOLPrinter],[dbo].[tblStations].[PreloadPrinter],[dbo].[tblStations].[BOLAgeInMinutes],[dbo].[tblStations].[CardReader],[dbo].[tblStations].[ThirtyFiveBitCardSupport],[dbo].[tblStations].[NumberOfCopies],[dbo].[tblStations].[NumberOfPreloadCopies],[dbo].[tblStations].[InhibitLoadingByLoadID],[dbo].[tblStations].[InhibitOperatingModePrompt],[dbo].[tblStations].[SynchronizeReferenceDensity],[dbo].[tblStations].[SignatureDevice],[dbo].[tblStations].[SetDefaultPresetToZero],[dbo].[tblStations].[ArmsServiced],[dbo].[tblStations].[InhibitSettingRecipeNames],[dbo].[tblStations].[SignatureDevicePort],[dbo].[tblStations].[SignatureDeviceBaudRate],[dbo].[tblStations].[MeterRecircCardNumber],[dbo].[tblStations].[TouchKeyReader],[dbo].[tblStations].[OffLoadByOffLoadID],[dbo].[tblStations].[UseManualMeterData],[dbo].[tblStations].[PromptForBOLNumber],[dbo].[tblStations].[QueryForTrailers],[dbo].[tblStations].[PromptForGravityCaptured],[dbo].[tblStations].[PromptForTemperatureCaptured],[dbo].[tblStations].[LastTransactionNumber],[dbo].[tblStations].[LastTransactionNumberDateTime],[dbo].[tblStations].[CreatedDate],[dbo].[tblStations].[CreatedBy],[dbo].[tblStations].[UpdatedDate],[dbo].[tblStations].[UpdatedBy],[dbo].[tblStations].[StationGuid],[dbo].[tblStations].[SiteGuid],[dbo].[tblStations].[LookupStationTypeIndex],[dbo].[tblStations].[LookupStationInterfaceTypeIndex],[dbo].[tblStations].[TankGuid],[dbo].[tblStations].[IssueByVolumeTransactionAliasGuid],[dbo].[tblStations].[IssueByWeightTransactionAliasGuid],[dbo].[tblStations].[ReceiptByVolumeTransactionAliasGuid],[dbo].[tblStations].[ReceiptByWeightTransactionAliasGuid],[dbo].[tblStations].[RecircTransactionAliasGuid],[dbo].[tblStations].[LogCommunications],[dbo].[tblStations].[LogCommPath],[dbo].[tblStations].[EnableScully],[dbo].[tblStations].[EnableEquipmentValidate],[dbo].[tblStations].[StationPromptTimeout],[dbo].[tblStations].[StationMessageTimeout],[dbo].[tblStations].[AssignedMeterGuid],[dbo].[tblStations].[EnableDynamicRecipes],[dbo].[tblStations].[EthanolExcess]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblStations]
                        INNER JOIN [track].[tblStations] CT
                            ON CT.PK_StationGuid = [dbo].[tblStations].[StationGuid] 
                    WHERE CT.PK_StationGuid = @StationGuid
            ) MERGE existingData
            USING (SELECT @ID,@SwingArmPosition,@VaporRecovery,@Enabled,@BOLPrinter,@PreloadPrinter,@BOLAgeInMinutes,@CardReader,@ThirtyFiveBitCardSupport,@NumberOfCopies,@NumberOfPreloadCopies,@InhibitLoadingByLoadID,@InhibitOperatingModePrompt,@SynchronizeReferenceDensity,@SignatureDevice,@SetDefaultPresetToZero,@ArmsServiced,@InhibitSettingRecipeNames,@SignatureDevicePort,@SignatureDeviceBaudRate,@MeterRecircCardNumber,@TouchKeyReader,@OffLoadByOffLoadID,@UseManualMeterData,@PromptForBOLNumber,@QueryForTrailers,@PromptForGravityCaptured,@PromptForTemperatureCaptured,@LastTransactionNumber,@LastTransactionNumberDateTime,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@StationGuid,@SiteGuid,@LookupStationTypeIndex,@LookupStationInterfaceTypeIndex,@TankGuid,@IssueByVolumeTransactionAliasGuid,@IssueByWeightTransactionAliasGuid,@ReceiptByVolumeTransactionAliasGuid,@ReceiptByWeightTransactionAliasGuid,@RecircTransactionAliasGuid,@LogCommunications,@LogCommPath,@EnableScully,@EnableEquipmentValidate,@StationPromptTimeout,@StationMessageTimeout,@AssignedMeterGuid,@EnableDynamicRecipes,@EthanolExcess
                    ) AS remoteChanges ([ID],[SwingArmPosition],[VaporRecovery],[Enabled],[BOLPrinter],[PreloadPrinter],[BOLAgeInMinutes],[CardReader],[ThirtyFiveBitCardSupport],[NumberOfCopies],[NumberOfPreloadCopies],[InhibitLoadingByLoadID],[InhibitOperatingModePrompt],[SynchronizeReferenceDensity],[SignatureDevice],[SetDefaultPresetToZero],[ArmsServiced],[InhibitSettingRecipeNames],[SignatureDevicePort],[SignatureDeviceBaudRate],[MeterRecircCardNumber],[TouchKeyReader],[OffLoadByOffLoadID],[UseManualMeterData],[PromptForBOLNumber],[QueryForTrailers],[PromptForGravityCaptured],[PromptForTemperatureCaptured],[LastTransactionNumber],[LastTransactionNumberDateTime],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StationGuid],[SiteGuid],[LookupStationTypeIndex],[LookupStationInterfaceTypeIndex],[TankGuid],[IssueByVolumeTransactionAliasGuid],[IssueByWeightTransactionAliasGuid],[ReceiptByVolumeTransactionAliasGuid],[ReceiptByWeightTransactionAliasGuid],[RecircTransactionAliasGuid],[LogCommunications],[LogCommPath],[EnableScully],[EnableEquipmentValidate],[StationPromptTimeout],[StationMessageTimeout],[AssignedMeterGuid],[EnableDynamicRecipes],[EthanolExcess])
            ON (existingData.[StationGuid] = remoteChanges.[StationGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[SwingArmPosition] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SwingArmPosition'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[SwingArmPosition] ELSE remoteChanges.[SwingArmPosition] END
                       ,[VaporRecovery] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VaporRecovery'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[VaporRecovery] ELSE remoteChanges.[VaporRecovery] END
                       ,[Enabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Enabled'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[Enabled] ELSE remoteChanges.[Enabled] END
                       ,[BOLPrinter] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BOLPrinter'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[BOLPrinter] ELSE remoteChanges.[BOLPrinter] END
                       ,[PreloadPrinter] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PreloadPrinter'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[PreloadPrinter] ELSE remoteChanges.[PreloadPrinter] END
                       ,[BOLAgeInMinutes] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BOLAgeInMinutes'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[BOLAgeInMinutes] ELSE remoteChanges.[BOLAgeInMinutes] END
                       ,[CardReader] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CardReader'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[CardReader] ELSE remoteChanges.[CardReader] END
                       ,[ThirtyFiveBitCardSupport] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ThirtyFiveBitCardSupport'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[ThirtyFiveBitCardSupport] ELSE remoteChanges.[ThirtyFiveBitCardSupport] END
                       ,[NumberOfCopies] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NumberOfCopies'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[NumberOfCopies] ELSE remoteChanges.[NumberOfCopies] END
                       ,[NumberOfPreloadCopies] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NumberOfPreloadCopies'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[NumberOfPreloadCopies] ELSE remoteChanges.[NumberOfPreloadCopies] END
                       ,[InhibitLoadingByLoadID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitLoadingByLoadID'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[InhibitLoadingByLoadID] ELSE remoteChanges.[InhibitLoadingByLoadID] END
                       ,[InhibitOperatingModePrompt] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitOperatingModePrompt'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[InhibitOperatingModePrompt] ELSE remoteChanges.[InhibitOperatingModePrompt] END
                       ,[SynchronizeReferenceDensity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SynchronizeReferenceDensity'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[SynchronizeReferenceDensity] ELSE remoteChanges.[SynchronizeReferenceDensity] END
                       ,[SignatureDevice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SignatureDevice'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[SignatureDevice] ELSE remoteChanges.[SignatureDevice] END
                       ,[SetDefaultPresetToZero] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SetDefaultPresetToZero'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[SetDefaultPresetToZero] ELSE remoteChanges.[SetDefaultPresetToZero] END
                       ,[ArmsServiced] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ArmsServiced'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[ArmsServiced] ELSE remoteChanges.[ArmsServiced] END
                       ,[InhibitSettingRecipeNames] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitSettingRecipeNames'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[InhibitSettingRecipeNames] ELSE remoteChanges.[InhibitSettingRecipeNames] END
                       ,[SignatureDevicePort] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SignatureDevicePort'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[SignatureDevicePort] ELSE remoteChanges.[SignatureDevicePort] END
                       ,[SignatureDeviceBaudRate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SignatureDeviceBaudRate'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[SignatureDeviceBaudRate] ELSE remoteChanges.[SignatureDeviceBaudRate] END
                       ,[MeterRecircCardNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterRecircCardNumber'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[MeterRecircCardNumber] ELSE remoteChanges.[MeterRecircCardNumber] END
                       ,[TouchKeyReader] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TouchKeyReader'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[TouchKeyReader] ELSE remoteChanges.[TouchKeyReader] END
                       ,[OffLoadByOffLoadID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OffLoadByOffLoadID'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[OffLoadByOffLoadID] ELSE remoteChanges.[OffLoadByOffLoadID] END
                       ,[UseManualMeterData] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseManualMeterData'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[UseManualMeterData] ELSE remoteChanges.[UseManualMeterData] END
                       ,[PromptForBOLNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PromptForBOLNumber'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[PromptForBOLNumber] ELSE remoteChanges.[PromptForBOLNumber] END
                       ,[QueryForTrailers] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QueryForTrailers'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[QueryForTrailers] ELSE remoteChanges.[QueryForTrailers] END
                       ,[PromptForGravityCaptured] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PromptForGravityCaptured'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[PromptForGravityCaptured] ELSE remoteChanges.[PromptForGravityCaptured] END
                       ,[PromptForTemperatureCaptured] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PromptForTemperatureCaptured'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[PromptForTemperatureCaptured] ELSE remoteChanges.[PromptForTemperatureCaptured] END
                       ,[LastTransactionNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastTransactionNumber'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[LastTransactionNumber] ELSE remoteChanges.[LastTransactionNumber] END
                       ,[LastTransactionNumberDateTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastTransactionNumberDateTime'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[LastTransactionNumberDateTime] ELSE remoteChanges.[LastTransactionNumberDateTime] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[LookupStationTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupStationTypeIndex'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[LookupStationTypeIndex] ELSE remoteChanges.[LookupStationTypeIndex] END
                       ,[LookupStationInterfaceTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupStationInterfaceTypeIndex'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[LookupStationInterfaceTypeIndex] ELSE remoteChanges.[LookupStationInterfaceTypeIndex] END
                       ,[TankGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[TankGuid] ELSE remoteChanges.[TankGuid] END
                       ,[IssueByVolumeTransactionAliasGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IssueByVolumeTransactionAliasGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[IssueByVolumeTransactionAliasGuid] ELSE remoteChanges.[IssueByVolumeTransactionAliasGuid] END
                       ,[IssueByWeightTransactionAliasGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IssueByWeightTransactionAliasGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[IssueByWeightTransactionAliasGuid] ELSE remoteChanges.[IssueByWeightTransactionAliasGuid] END
                       ,[ReceiptByVolumeTransactionAliasGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReceiptByVolumeTransactionAliasGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[ReceiptByVolumeTransactionAliasGuid] ELSE remoteChanges.[ReceiptByVolumeTransactionAliasGuid] END
                       ,[ReceiptByWeightTransactionAliasGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReceiptByWeightTransactionAliasGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[ReceiptByWeightTransactionAliasGuid] ELSE remoteChanges.[ReceiptByWeightTransactionAliasGuid] END
                       ,[RecircTransactionAliasGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RecircTransactionAliasGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[RecircTransactionAliasGuid] ELSE remoteChanges.[RecircTransactionAliasGuid] END
                       ,[LogCommunications] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LogCommunications'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[LogCommunications] ELSE remoteChanges.[LogCommunications] END
                       ,[LogCommPath] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LogCommPath'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[LogCommPath] ELSE remoteChanges.[LogCommPath] END
                       ,[EnableScully] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableScully'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[EnableScully] ELSE remoteChanges.[EnableScully] END
                       ,[EnableEquipmentValidate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableEquipmentValidate'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[EnableEquipmentValidate] ELSE remoteChanges.[EnableEquipmentValidate] END
                       ,[StationPromptTimeout] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StationPromptTimeout'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[StationPromptTimeout] ELSE remoteChanges.[StationPromptTimeout] END
                       ,[StationMessageTimeout] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StationMessageTimeout'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[StationMessageTimeout] ELSE remoteChanges.[StationMessageTimeout] END
                       ,[AssignedMeterGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedMeterGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[AssignedMeterGuid] ELSE remoteChanges.[AssignedMeterGuid] END
                       ,[EnableDynamicRecipes] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableDynamicRecipes'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[EnableDynamicRecipes] ELSE remoteChanges.[EnableDynamicRecipes] END
                       ,[EthanolExcess] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EthanolExcess'), @sync_supported_columns_tblStations)) WHEN 0 THEN existingData.[EthanolExcess] ELSE remoteChanges.[EthanolExcess] END

            WHEN NOT MATCHED THEN
                INSERT ([ID],[SwingArmPosition],[VaporRecovery],[Enabled],[BOLPrinter],[PreloadPrinter],[BOLAgeInMinutes],[CardReader],[ThirtyFiveBitCardSupport],[NumberOfCopies],[NumberOfPreloadCopies],[InhibitLoadingByLoadID],[InhibitOperatingModePrompt],[SynchronizeReferenceDensity],[SignatureDevice],[SetDefaultPresetToZero],[ArmsServiced],[InhibitSettingRecipeNames],[SignatureDevicePort],[SignatureDeviceBaudRate],[MeterRecircCardNumber],[TouchKeyReader],[OffLoadByOffLoadID],[UseManualMeterData],[PromptForBOLNumber],[QueryForTrailers],[PromptForGravityCaptured],[PromptForTemperatureCaptured],[LastTransactionNumber],[LastTransactionNumberDateTime],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[StationGuid],[SiteGuid],[LookupStationTypeIndex],[LookupStationInterfaceTypeIndex],[TankGuid],[IssueByVolumeTransactionAliasGuid],[IssueByWeightTransactionAliasGuid],[ReceiptByVolumeTransactionAliasGuid],[ReceiptByWeightTransactionAliasGuid],[RecircTransactionAliasGuid],[LogCommunications],[LogCommPath],[EnableScully],[EnableEquipmentValidate],[StationPromptTimeout],[StationMessageTimeout],[AssignedMeterGuid],[EnableDynamicRecipes],[EthanolExcess])
                    VALUES (@ID,@SwingArmPosition,@VaporRecovery,@Enabled,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BOLPrinter'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @BOLPrinter END),@PreloadPrinter,@BOLAgeInMinutes,@CardReader,@ThirtyFiveBitCardSupport,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NumberOfCopies'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @NumberOfCopies END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NumberOfPreloadCopies'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @NumberOfPreloadCopies END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitLoadingByLoadID'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @InhibitLoadingByLoadID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitOperatingModePrompt'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @InhibitOperatingModePrompt END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SynchronizeReferenceDensity'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @SynchronizeReferenceDensity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SignatureDevice'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @SignatureDevice END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SetDefaultPresetToZero'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @SetDefaultPresetToZero END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ArmsServiced'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @ArmsServiced END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitSettingRecipeNames'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @InhibitSettingRecipeNames END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SignatureDevicePort'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @SignatureDevicePort END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SignatureDeviceBaudRate'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @SignatureDeviceBaudRate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterRecircCardNumber'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @MeterRecircCardNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TouchKeyReader'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @TouchKeyReader END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OffLoadByOffLoadID'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @OffLoadByOffLoadID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseManualMeterData'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @UseManualMeterData END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PromptForBOLNumber'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @PromptForBOLNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QueryForTrailers'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @QueryForTrailers END),@PromptForGravityCaptured,@PromptForTemperatureCaptured,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastTransactionNumber'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @LastTransactionNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastTransactionNumberDateTime'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @LastTransactionNumberDateTime END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@StationGuid,@SiteGuid,@LookupStationTypeIndex,@LookupStationInterfaceTypeIndex,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @TankGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IssueByVolumeTransactionAliasGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @IssueByVolumeTransactionAliasGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IssueByWeightTransactionAliasGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @IssueByWeightTransactionAliasGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReceiptByVolumeTransactionAliasGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @ReceiptByVolumeTransactionAliasGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReceiptByWeightTransactionAliasGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @ReceiptByWeightTransactionAliasGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RecircTransactionAliasGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @RecircTransactionAliasGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LogCommunications'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @LogCommunications END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LogCommPath'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @LogCommPath END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableScully'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @EnableScully END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableEquipmentValidate'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @EnableEquipmentValidate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StationPromptTimeout'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @StationPromptTimeout END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StationMessageTimeout'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @StationMessageTimeout END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedMeterGuid'), @sync_supported_columns_tblStations)) WHEN 0 THEN NULL ELSE @AssignedMeterGuid END),@EnableDynamicRecipes,@EthanolExcess)
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
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
