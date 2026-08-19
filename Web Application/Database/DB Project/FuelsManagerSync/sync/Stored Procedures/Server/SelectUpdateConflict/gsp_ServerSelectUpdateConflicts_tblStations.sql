-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblStations
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblStations]
@StationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblStations].[ID],[dbo].[tblStations].[SwingArmPosition],[dbo].[tblStations].[VaporRecovery],[dbo].[tblStations].[Enabled],[dbo].[tblStations].[BOLPrinter],[dbo].[tblStations].[PreloadPrinter],[dbo].[tblStations].[BOLAgeInMinutes],[dbo].[tblStations].[CardReader],[dbo].[tblStations].[ThirtyFiveBitCardSupport],[dbo].[tblStations].[NumberOfCopies],[dbo].[tblStations].[NumberOfPreloadCopies],[dbo].[tblStations].[InhibitLoadingByLoadID],[dbo].[tblStations].[InhibitOperatingModePrompt],[dbo].[tblStations].[SynchronizeReferenceDensity],[dbo].[tblStations].[SignatureDevice],[dbo].[tblStations].[SetDefaultPresetToZero],[dbo].[tblStations].[ArmsServiced],[dbo].[tblStations].[InhibitSettingRecipeNames],[dbo].[tblStations].[SignatureDevicePort],[dbo].[tblStations].[SignatureDeviceBaudRate],[dbo].[tblStations].[MeterRecircCardNumber],[dbo].[tblStations].[TouchKeyReader],[dbo].[tblStations].[OffLoadByOffLoadID],[dbo].[tblStations].[UseManualMeterData],[dbo].[tblStations].[PromptForBOLNumber],[dbo].[tblStations].[QueryForTrailers],[dbo].[tblStations].[PromptForGravityCaptured],[dbo].[tblStations].[PromptForTemperatureCaptured],[dbo].[tblStations].[LastTransactionNumber],[dbo].[tblStations].[LastTransactionNumberDateTime],[dbo].[tblStations].[CreatedDate],[dbo].[tblStations].[CreatedBy],[dbo].[tblStations].[UpdatedDate],[dbo].[tblStations].[UpdatedBy],[dbo].[tblStations].[StationGuid],[dbo].[tblStations].[SiteGuid],[dbo].[tblStations].[LookupStationTypeIndex],[dbo].[tblStations].[LookupStationInterfaceTypeIndex],[dbo].[tblStations].[TankGuid],[dbo].[tblStations].[IssueByVolumeTransactionAliasGuid],[dbo].[tblStations].[IssueByWeightTransactionAliasGuid],[dbo].[tblStations].[ReceiptByVolumeTransactionAliasGuid],[dbo].[tblStations].[ReceiptByWeightTransactionAliasGuid],[dbo].[tblStations].[RecircTransactionAliasGuid],[dbo].[tblStations].[LogCommunications],[dbo].[tblStations].[LogCommPath],[dbo].[tblStations].[EnableScully],[dbo].[tblStations].[EnableEquipmentValidate],[dbo].[tblStations].[StationPromptTimeout],[dbo].[tblStations].[StationMessageTimeout],[dbo].[tblStations].[AssignedMeterGuid],[dbo].[tblStations].[EnableDynamicRecipes],[dbo].[tblStations].[EthanolExcess], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblStations]
            INNER JOIN [track].[tblStations] CT
                ON CT.PK_StationGuid = [dbo].[tblStations].[StationGuid]
        WHERE CT.PK_StationGuid = @StationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
