-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMovementHistory
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblMovementHistory]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@MovementHistoryGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@Name nvarchar(100),
@Node nvarchar(100),
@InitiationCount bigint,
@RecordType int,
@TimeStamp datetime,
@ParentGuid uniqueidentifier,
@AutoStart bit,
@AutoStartTime datetimeoffset(7),
@AutoStop bit,
@AutoStopTime datetimeoffset(7),
@CloseoutDataModifiedBy nvarchar(50),
@CloseoutDensityProductInAir float,
@CloseoutDensityProductObserved float,
@CloseoutDensityProductObservedTime datetimeoffset(7),
@CloseoutDensityProductStandard float,
@CloseoutDensityProductStandardTime datetimeoffset(7),
@CloseoutDensityProductStandardInAir float,
@CloseoutLevelProduct float,
@CloseoutLevelProductTime datetimeoffset(7),
@CloseoutLevelWater float,
@CloseoutMassLiquid float,
@CloseoutPercentBsw float,
@CloseoutRoofMass float,
@CloseoutTankShellCorrection float,
@CloseoutTemperatureAmbient float,
@CloseoutTemperatureAmbientTime datetimeoffset(7),
@CloseoutTemperatureDensity float,
@CloseoutTemperatureProduct float,
@CloseoutTime datetimeoffset(7),
@CloseoutTransferGov float,
@CloseoutTransferNsv float,
@CloseoutTransferMassLiquid float,
@CloseoutTransferVolumeWater float,
@CloseoutVolumeBsw float,
@CloseoutVolumeCorrectionFactor float,
@CloseoutVolumeGrossObserved float,
@CloseoutVolumeGrossStandard float,
@CloseoutVolumeNetStandard float,
@CloseoutVolumeRoofCorrection float,
@CloseoutVolumeTotalObserved float,
@CloseoutVolumeWater float,
@Comment nvarchar(1000),
@Type nvarchar(20),
@OrderNumber nvarchar(100),
@PlannedStartTime datetimeoffset(7),
@Product nvarchar(100),
@ProductDescription nvarchar(1000),
@StartTime datetimeoffset(7),
@StopTime datetimeoffset(7),
@StartDensityProductObserved float,
@StartDensityProductObservedTime datetimeoffset(7),
@StartDensityProductObservedInAir float,
@StartDensityProductStandard float,
@StartDensityProductStandardTime datetimeoffset(7),
@StartUserID nvarchar(100),
@StartLevelProduct float,
@StartLevelProductTime datetimeoffset(7),
@StartLevelWater float,
@StartLevelWaterTime datetimeoffset(7),
@StartPercentBsw float,
@StartMassLiquid float,
@StartTankShellCorrection float,
@StartTemperatureAmbient float,
@StartTemperatureAmbientTime datetimeoffset(7),
@StartTemperatureProduct float,
@StartTemperatureProductTime datetimeoffset(7),
@StartTemperatureDensity float,
@StartTemperatureDensityTime datetimeoffset(7),
@StartVolume float,
@StartVolumeBsw float,
@StartVolumeCorrectionFactor float,
@StartVolumeGrossObserved float,
@StartVolumeGrossStandard float,
@StartVolumeNetStandard float,
@StartVolumeRoofCorrection float,
@StartVolumeTotalObserved float,
@StartVolumeWater float,
@UnitsLevelProductIndex int,
@UnitsTemperatureAmbientIndex int,
@UnitsTemperatureDensityIndex int,
@UnitsTemperatureProductIndex int,
@UnitsDensityProductObservedIndex int,
@UnitsDensityProductStandardIndex int,
@UnitsVolumeIndex int,
@UnitsMassIndex int,
@DecimalPlacesVolume int,
@DecimalPlacesLevel int,
@DecimalPlacesDensity int,
@DecimalPlacesTemperature int,
@UserData01 nvarchar(100),
@UserData02 nvarchar(100),
@UserData03 nvarchar(100),
@UserData04 nvarchar(100),
@UserData05 nvarchar(100),
@UserData06 nvarchar(100),
@UserData07 nvarchar(100),
@UserData08 nvarchar(100),
@UserData09 nvarchar(100),
@UserData10 nvarchar(100),
@TransferDeviation float,
@TransferPercentDeviation float,
@DecimalPlacesPercent int,
@TransferMode int,
@TransferStatus int,
@TransferTarget float,
@TransferTargetUnitsIndex int,
@TransferLevelTarget float,
@TransferVolumeTarget float,
@TransferTimeRemaining bigint,
@TransferDirection nvarchar(20),
@CommentDateTime datetime,
@CommentUserID nvarchar(50),
@Status int,
@VolumeWater float,
@LevelProduct float,
@StartDensityProductStandardInAir float,
@TransferredVolumeWater float,
@TransferredVolume float,
@MidnightRecord bit,
@PointGuid uniqueidentifier,
@RootParentGuid uniqueidentifier,
@RecordSeq int,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    ;   MERGE [dbo].[tblMovementHistory] AS existingData
        USING (SELECT @MovementHistoryGuid 'MovementHistoryGuid',@SiteGuid 'SiteGuid',@Name 'Name',@Node 'Node',@InitiationCount 'InitiationCount',@RecordType 'RecordType',@TimeStamp 'TimeStamp',@ParentGuid 'ParentGuid',@AutoStart 'AutoStart',@AutoStartTime 'AutoStartTime',@AutoStop 'AutoStop',@AutoStopTime 'AutoStopTime',@CloseoutDataModifiedBy 'CloseoutDataModifiedBy',@CloseoutDensityProductInAir 'CloseoutDensityProductInAir',@CloseoutDensityProductObserved 'CloseoutDensityProductObserved',@CloseoutDensityProductObservedTime 'CloseoutDensityProductObservedTime',@CloseoutDensityProductStandard 'CloseoutDensityProductStandard',@CloseoutDensityProductStandardTime 'CloseoutDensityProductStandardTime',@CloseoutDensityProductStandardInAir 'CloseoutDensityProductStandardInAir',@CloseoutLevelProduct 'CloseoutLevelProduct',@CloseoutLevelProductTime 'CloseoutLevelProductTime',@CloseoutLevelWater 'CloseoutLevelWater',@CloseoutMassLiquid 'CloseoutMassLiquid',@CloseoutPercentBsw 'CloseoutPercentBsw',@CloseoutRoofMass 'CloseoutRoofMass',@CloseoutTankShellCorrection 'CloseoutTankShellCorrection',@CloseoutTemperatureAmbient 'CloseoutTemperatureAmbient',@CloseoutTemperatureAmbientTime 'CloseoutTemperatureAmbientTime',@CloseoutTemperatureDensity 'CloseoutTemperatureDensity',@CloseoutTemperatureProduct 'CloseoutTemperatureProduct',@CloseoutTime 'CloseoutTime',@CloseoutTransferGov 'CloseoutTransferGov',@CloseoutTransferNsv 'CloseoutTransferNsv',@CloseoutTransferMassLiquid 'CloseoutTransferMassLiquid',@CloseoutTransferVolumeWater 'CloseoutTransferVolumeWater',@CloseoutVolumeBsw 'CloseoutVolumeBsw',@CloseoutVolumeCorrectionFactor 'CloseoutVolumeCorrectionFactor',@CloseoutVolumeGrossObserved 'CloseoutVolumeGrossObserved',@CloseoutVolumeGrossStandard 'CloseoutVolumeGrossStandard',@CloseoutVolumeNetStandard 'CloseoutVolumeNetStandard',@CloseoutVolumeRoofCorrection 'CloseoutVolumeRoofCorrection',@CloseoutVolumeTotalObserved 'CloseoutVolumeTotalObserved',@CloseoutVolumeWater 'CloseoutVolumeWater',@Comment 'Comment',@Type 'Type',@OrderNumber 'OrderNumber',@PlannedStartTime 'PlannedStartTime',@Product 'Product',@ProductDescription 'ProductDescription',@StartTime 'StartTime',@StopTime 'StopTime',@StartDensityProductObserved 'StartDensityProductObserved',@StartDensityProductObservedTime 'StartDensityProductObservedTime',@StartDensityProductObservedInAir 'StartDensityProductObservedInAir',@StartDensityProductStandard 'StartDensityProductStandard',@StartDensityProductStandardTime 'StartDensityProductStandardTime',@StartUserID 'StartUserID',@StartLevelProduct 'StartLevelProduct',@StartLevelProductTime 'StartLevelProductTime',@StartLevelWater 'StartLevelWater',@StartLevelWaterTime 'StartLevelWaterTime',@StartPercentBsw 'StartPercentBsw',@StartMassLiquid 'StartMassLiquid',@StartTankShellCorrection 'StartTankShellCorrection',@StartTemperatureAmbient 'StartTemperatureAmbient',@StartTemperatureAmbientTime 'StartTemperatureAmbientTime',@StartTemperatureProduct 'StartTemperatureProduct',@StartTemperatureProductTime 'StartTemperatureProductTime',@StartTemperatureDensity 'StartTemperatureDensity',@StartTemperatureDensityTime 'StartTemperatureDensityTime',@StartVolume 'StartVolume',@StartVolumeBsw 'StartVolumeBsw',@StartVolumeCorrectionFactor 'StartVolumeCorrectionFactor',@StartVolumeGrossObserved 'StartVolumeGrossObserved',@StartVolumeGrossStandard 'StartVolumeGrossStandard',@StartVolumeNetStandard 'StartVolumeNetStandard',@StartVolumeRoofCorrection 'StartVolumeRoofCorrection',@StartVolumeTotalObserved 'StartVolumeTotalObserved',@StartVolumeWater 'StartVolumeWater',@UnitsLevelProductIndex 'UnitsLevelProductIndex',@UnitsTemperatureAmbientIndex 'UnitsTemperatureAmbientIndex',@UnitsTemperatureDensityIndex 'UnitsTemperatureDensityIndex',@UnitsTemperatureProductIndex 'UnitsTemperatureProductIndex',@UnitsDensityProductObservedIndex 'UnitsDensityProductObservedIndex',@UnitsDensityProductStandardIndex 'UnitsDensityProductStandardIndex',@UnitsVolumeIndex 'UnitsVolumeIndex',@UnitsMassIndex 'UnitsMassIndex',@DecimalPlacesVolume 'DecimalPlacesVolume',@DecimalPlacesLevel 'DecimalPlacesLevel',@DecimalPlacesDensity 'DecimalPlacesDensity',@DecimalPlacesTemperature 'DecimalPlacesTemperature',@UserData01 'UserData01',@UserData02 'UserData02',@UserData03 'UserData03',@UserData04 'UserData04',@UserData05 'UserData05',@UserData06 'UserData06',@UserData07 'UserData07',@UserData08 'UserData08',@UserData09 'UserData09',@UserData10 'UserData10',@TransferDeviation 'TransferDeviation',@TransferPercentDeviation 'TransferPercentDeviation',@DecimalPlacesPercent 'DecimalPlacesPercent',@TransferMode 'TransferMode',@TransferStatus 'TransferStatus',@TransferTarget 'TransferTarget',@TransferTargetUnitsIndex 'TransferTargetUnitsIndex',@TransferLevelTarget 'TransferLevelTarget',@TransferVolumeTarget 'TransferVolumeTarget',@TransferTimeRemaining 'TransferTimeRemaining',@TransferDirection 'TransferDirection',@CommentDateTime 'CommentDateTime',@CommentUserID 'CommentUserID',@Status 'Status',@VolumeWater 'VolumeWater',@LevelProduct 'LevelProduct',@StartDensityProductStandardInAir 'StartDensityProductStandardInAir',@TransferredVolumeWater 'TransferredVolumeWater',@TransferredVolume 'TransferredVolume',@MidnightRecord 'MidnightRecord',@PointGuid 'PointGuid',@RootParentGuid 'RootParentGuid',@RecordSeq 'RecordSeq',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy'
                ) AS remoteChanges ([MovementHistoryGuid],[SiteGuid],[Name],[Node],[InitiationCount],[RecordType],[TimeStamp],[ParentGuid],[AutoStart],[AutoStartTime],[AutoStop],[AutoStopTime],[CloseoutDataModifiedBy],[CloseoutDensityProductInAir],[CloseoutDensityProductObserved],[CloseoutDensityProductObservedTime],[CloseoutDensityProductStandard],[CloseoutDensityProductStandardTime],[CloseoutDensityProductStandardInAir],[CloseoutLevelProduct],[CloseoutLevelProductTime],[CloseoutLevelWater],[CloseoutMassLiquid],[CloseoutPercentBsw],[CloseoutRoofMass],[CloseoutTankShellCorrection],[CloseoutTemperatureAmbient],[CloseoutTemperatureAmbientTime],[CloseoutTemperatureDensity],[CloseoutTemperatureProduct],[CloseoutTime],[CloseoutTransferGov],[CloseoutTransferNsv],[CloseoutTransferMassLiquid],[CloseoutTransferVolumeWater],[CloseoutVolumeBsw],[CloseoutVolumeCorrectionFactor],[CloseoutVolumeGrossObserved],[CloseoutVolumeGrossStandard],[CloseoutVolumeNetStandard],[CloseoutVolumeRoofCorrection],[CloseoutVolumeTotalObserved],[CloseoutVolumeWater],[Comment],[Type],[OrderNumber],[PlannedStartTime],[Product],[ProductDescription],[StartTime],[StopTime],[StartDensityProductObserved],[StartDensityProductObservedTime],[StartDensityProductObservedInAir],[StartDensityProductStandard],[StartDensityProductStandardTime],[StartUserID],[StartLevelProduct],[StartLevelProductTime],[StartLevelWater],[StartLevelWaterTime],[StartPercentBsw],[StartMassLiquid],[StartTankShellCorrection],[StartTemperatureAmbient],[StartTemperatureAmbientTime],[StartTemperatureProduct],[StartTemperatureProductTime],[StartTemperatureDensity],[StartTemperatureDensityTime],[StartVolume],[StartVolumeBsw],[StartVolumeCorrectionFactor],[StartVolumeGrossObserved],[StartVolumeGrossStandard],[StartVolumeNetStandard],[StartVolumeRoofCorrection],[StartVolumeTotalObserved],[StartVolumeWater],[UnitsLevelProductIndex],[UnitsTemperatureAmbientIndex],[UnitsTemperatureDensityIndex],[UnitsTemperatureProductIndex],[UnitsDensityProductObservedIndex],[UnitsDensityProductStandardIndex],[UnitsVolumeIndex],[UnitsMassIndex],[DecimalPlacesVolume],[DecimalPlacesLevel],[DecimalPlacesDensity],[DecimalPlacesTemperature],[UserData01],[UserData02],[UserData03],[UserData04],[UserData05],[UserData06],[UserData07],[UserData08],[UserData09],[UserData10],[TransferDeviation],[TransferPercentDeviation],[DecimalPlacesPercent],[TransferMode],[TransferStatus],[TransferTarget],[TransferTargetUnitsIndex],[TransferLevelTarget],[TransferVolumeTarget],[TransferTimeRemaining],[TransferDirection],[CommentDateTime],[CommentUserID],[Status],[VolumeWater],[LevelProduct],[StartDensityProductStandardInAir],[TransferredVolumeWater],[TransferredVolume],[MidnightRecord],[PointGuid],[RootParentGuid],[RecordSeq],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
        ON (existingData.[MovementHistoryGuid] = remoteChanges.[MovementHistoryGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [SiteGuid] = remoteChanges.[SiteGuid]
                       ,[Name] = remoteChanges.[Name]
                       ,[Node] = remoteChanges.[Node]
                       ,[InitiationCount] = remoteChanges.[InitiationCount]
                       ,[RecordType] = remoteChanges.[RecordType]
                       ,[TimeStamp] = remoteChanges.[TimeStamp]
                       ,[ParentGuid] = remoteChanges.[ParentGuid]
                       ,[AutoStart] = remoteChanges.[AutoStart]
                       ,[AutoStartTime] = remoteChanges.[AutoStartTime]
                       ,[AutoStop] = remoteChanges.[AutoStop]
                       ,[AutoStopTime] = remoteChanges.[AutoStopTime]
                       ,[CloseoutDataModifiedBy] = remoteChanges.[CloseoutDataModifiedBy]
                       ,[CloseoutDensityProductInAir] = remoteChanges.[CloseoutDensityProductInAir]
                       ,[CloseoutDensityProductObserved] = remoteChanges.[CloseoutDensityProductObserved]
                       ,[CloseoutDensityProductObservedTime] = remoteChanges.[CloseoutDensityProductObservedTime]
                       ,[CloseoutDensityProductStandard] = remoteChanges.[CloseoutDensityProductStandard]
                       ,[CloseoutDensityProductStandardTime] = remoteChanges.[CloseoutDensityProductStandardTime]
                       ,[CloseoutDensityProductStandardInAir] = remoteChanges.[CloseoutDensityProductStandardInAir]
                       ,[CloseoutLevelProduct] = remoteChanges.[CloseoutLevelProduct]
                       ,[CloseoutLevelProductTime] = remoteChanges.[CloseoutLevelProductTime]
                       ,[CloseoutLevelWater] = remoteChanges.[CloseoutLevelWater]
                       ,[CloseoutMassLiquid] = remoteChanges.[CloseoutMassLiquid]
                       ,[CloseoutPercentBsw] = remoteChanges.[CloseoutPercentBsw]
                       ,[CloseoutRoofMass] = remoteChanges.[CloseoutRoofMass]
                       ,[CloseoutTankShellCorrection] = remoteChanges.[CloseoutTankShellCorrection]
                       ,[CloseoutTemperatureAmbient] = remoteChanges.[CloseoutTemperatureAmbient]
                       ,[CloseoutTemperatureAmbientTime] = remoteChanges.[CloseoutTemperatureAmbientTime]
                       ,[CloseoutTemperatureDensity] = remoteChanges.[CloseoutTemperatureDensity]
                       ,[CloseoutTemperatureProduct] = remoteChanges.[CloseoutTemperatureProduct]
                       ,[CloseoutTime] = remoteChanges.[CloseoutTime]
                       ,[CloseoutTransferGov] = remoteChanges.[CloseoutTransferGov]
                       ,[CloseoutTransferNsv] = remoteChanges.[CloseoutTransferNsv]
                       ,[CloseoutTransferMassLiquid] = remoteChanges.[CloseoutTransferMassLiquid]
                       ,[CloseoutTransferVolumeWater] = remoteChanges.[CloseoutTransferVolumeWater]
                       ,[CloseoutVolumeBsw] = remoteChanges.[CloseoutVolumeBsw]
                       ,[CloseoutVolumeCorrectionFactor] = remoteChanges.[CloseoutVolumeCorrectionFactor]
                       ,[CloseoutVolumeGrossObserved] = remoteChanges.[CloseoutVolumeGrossObserved]
                       ,[CloseoutVolumeGrossStandard] = remoteChanges.[CloseoutVolumeGrossStandard]
                       ,[CloseoutVolumeNetStandard] = remoteChanges.[CloseoutVolumeNetStandard]
                       ,[CloseoutVolumeRoofCorrection] = remoteChanges.[CloseoutVolumeRoofCorrection]
                       ,[CloseoutVolumeTotalObserved] = remoteChanges.[CloseoutVolumeTotalObserved]
                       ,[CloseoutVolumeWater] = remoteChanges.[CloseoutVolumeWater]
                       ,[Comment] = remoteChanges.[Comment]
                       ,[Type] = remoteChanges.[Type]
                       ,[OrderNumber] = remoteChanges.[OrderNumber]
                       ,[PlannedStartTime] = remoteChanges.[PlannedStartTime]
                       ,[Product] = remoteChanges.[Product]
                       ,[ProductDescription] = remoteChanges.[ProductDescription]
                       ,[StartTime] = remoteChanges.[StartTime]
                       ,[StopTime] = remoteChanges.[StopTime]
                       ,[StartDensityProductObserved] = remoteChanges.[StartDensityProductObserved]
                       ,[StartDensityProductObservedTime] = remoteChanges.[StartDensityProductObservedTime]
                       ,[StartDensityProductObservedInAir] = remoteChanges.[StartDensityProductObservedInAir]
                       ,[StartDensityProductStandard] = remoteChanges.[StartDensityProductStandard]
                       ,[StartDensityProductStandardTime] = remoteChanges.[StartDensityProductStandardTime]
                       ,[StartUserID] = remoteChanges.[StartUserID]
                       ,[StartLevelProduct] = remoteChanges.[StartLevelProduct]
                       ,[StartLevelProductTime] = remoteChanges.[StartLevelProductTime]
                       ,[StartLevelWater] = remoteChanges.[StartLevelWater]
                       ,[StartLevelWaterTime] = remoteChanges.[StartLevelWaterTime]
                       ,[StartPercentBsw] = remoteChanges.[StartPercentBsw]
                       ,[StartMassLiquid] = remoteChanges.[StartMassLiquid]
                       ,[StartTankShellCorrection] = remoteChanges.[StartTankShellCorrection]
                       ,[StartTemperatureAmbient] = remoteChanges.[StartTemperatureAmbient]
                       ,[StartTemperatureAmbientTime] = remoteChanges.[StartTemperatureAmbientTime]
                       ,[StartTemperatureProduct] = remoteChanges.[StartTemperatureProduct]
                       ,[StartTemperatureProductTime] = remoteChanges.[StartTemperatureProductTime]
                       ,[StartTemperatureDensity] = remoteChanges.[StartTemperatureDensity]
                       ,[StartTemperatureDensityTime] = remoteChanges.[StartTemperatureDensityTime]
                       ,[StartVolume] = remoteChanges.[StartVolume]
                       ,[StartVolumeBsw] = remoteChanges.[StartVolumeBsw]
                       ,[StartVolumeCorrectionFactor] = remoteChanges.[StartVolumeCorrectionFactor]
                       ,[StartVolumeGrossObserved] = remoteChanges.[StartVolumeGrossObserved]
                       ,[StartVolumeGrossStandard] = remoteChanges.[StartVolumeGrossStandard]
                       ,[StartVolumeNetStandard] = remoteChanges.[StartVolumeNetStandard]
                       ,[StartVolumeRoofCorrection] = remoteChanges.[StartVolumeRoofCorrection]
                       ,[StartVolumeTotalObserved] = remoteChanges.[StartVolumeTotalObserved]
                       ,[StartVolumeWater] = remoteChanges.[StartVolumeWater]
                       ,[UnitsLevelProductIndex] = remoteChanges.[UnitsLevelProductIndex]
                       ,[UnitsTemperatureAmbientIndex] = remoteChanges.[UnitsTemperatureAmbientIndex]
                       ,[UnitsTemperatureDensityIndex] = remoteChanges.[UnitsTemperatureDensityIndex]
                       ,[UnitsTemperatureProductIndex] = remoteChanges.[UnitsTemperatureProductIndex]
                       ,[UnitsDensityProductObservedIndex] = remoteChanges.[UnitsDensityProductObservedIndex]
                       ,[UnitsDensityProductStandardIndex] = remoteChanges.[UnitsDensityProductStandardIndex]
                       ,[UnitsVolumeIndex] = remoteChanges.[UnitsVolumeIndex]
                       ,[UnitsMassIndex] = remoteChanges.[UnitsMassIndex]
                       ,[DecimalPlacesVolume] = remoteChanges.[DecimalPlacesVolume]
                       ,[DecimalPlacesLevel] = remoteChanges.[DecimalPlacesLevel]
                       ,[DecimalPlacesDensity] = remoteChanges.[DecimalPlacesDensity]
                       ,[DecimalPlacesTemperature] = remoteChanges.[DecimalPlacesTemperature]
                       ,[UserData01] = remoteChanges.[UserData01]
                       ,[UserData02] = remoteChanges.[UserData02]
                       ,[UserData03] = remoteChanges.[UserData03]
                       ,[UserData04] = remoteChanges.[UserData04]
                       ,[UserData05] = remoteChanges.[UserData05]
                       ,[UserData06] = remoteChanges.[UserData06]
                       ,[UserData07] = remoteChanges.[UserData07]
                       ,[UserData08] = remoteChanges.[UserData08]
                       ,[UserData09] = remoteChanges.[UserData09]
                       ,[UserData10] = remoteChanges.[UserData10]
                       ,[TransferDeviation] = remoteChanges.[TransferDeviation]
                       ,[TransferPercentDeviation] = remoteChanges.[TransferPercentDeviation]
                       ,[DecimalPlacesPercent] = remoteChanges.[DecimalPlacesPercent]
                       ,[TransferMode] = remoteChanges.[TransferMode]
                       ,[TransferStatus] = remoteChanges.[TransferStatus]
                       ,[TransferTarget] = remoteChanges.[TransferTarget]
                       ,[TransferTargetUnitsIndex] = remoteChanges.[TransferTargetUnitsIndex]
                       ,[TransferLevelTarget] = remoteChanges.[TransferLevelTarget]
                       ,[TransferVolumeTarget] = remoteChanges.[TransferVolumeTarget]
                       ,[TransferTimeRemaining] = remoteChanges.[TransferTimeRemaining]
                       ,[TransferDirection] = remoteChanges.[TransferDirection]
                       ,[CommentDateTime] = remoteChanges.[CommentDateTime]
                       ,[CommentUserID] = remoteChanges.[CommentUserID]
                       ,[Status] = remoteChanges.[Status]
                       ,[VolumeWater] = remoteChanges.[VolumeWater]
                       ,[LevelProduct] = remoteChanges.[LevelProduct]
                       ,[StartDensityProductStandardInAir] = remoteChanges.[StartDensityProductStandardInAir]
                       ,[TransferredVolumeWater] = remoteChanges.[TransferredVolumeWater]
                       ,[TransferredVolume] = remoteChanges.[TransferredVolume]
                       ,[MidnightRecord] = remoteChanges.[MidnightRecord]
                       ,[PointGuid] = remoteChanges.[PointGuid]
                       ,[RootParentGuid] = remoteChanges.[RootParentGuid]
                       ,[RecordSeq] = remoteChanges.[RecordSeq]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]

        WHEN NOT MATCHED THEN
            INSERT ([MovementHistoryGuid],[SiteGuid],[Name],[Node],[InitiationCount],[RecordType],[TimeStamp],[ParentGuid],[AutoStart],[AutoStartTime],[AutoStop],[AutoStopTime],[CloseoutDataModifiedBy],[CloseoutDensityProductInAir],[CloseoutDensityProductObserved],[CloseoutDensityProductObservedTime],[CloseoutDensityProductStandard],[CloseoutDensityProductStandardTime],[CloseoutDensityProductStandardInAir],[CloseoutLevelProduct],[CloseoutLevelProductTime],[CloseoutLevelWater],[CloseoutMassLiquid],[CloseoutPercentBsw],[CloseoutRoofMass],[CloseoutTankShellCorrection],[CloseoutTemperatureAmbient],[CloseoutTemperatureAmbientTime],[CloseoutTemperatureDensity],[CloseoutTemperatureProduct],[CloseoutTime],[CloseoutTransferGov],[CloseoutTransferNsv],[CloseoutTransferMassLiquid],[CloseoutTransferVolumeWater],[CloseoutVolumeBsw],[CloseoutVolumeCorrectionFactor],[CloseoutVolumeGrossObserved],[CloseoutVolumeGrossStandard],[CloseoutVolumeNetStandard],[CloseoutVolumeRoofCorrection],[CloseoutVolumeTotalObserved],[CloseoutVolumeWater],[Comment],[Type],[OrderNumber],[PlannedStartTime],[Product],[ProductDescription],[StartTime],[StopTime],[StartDensityProductObserved],[StartDensityProductObservedTime],[StartDensityProductObservedInAir],[StartDensityProductStandard],[StartDensityProductStandardTime],[StartUserID],[StartLevelProduct],[StartLevelProductTime],[StartLevelWater],[StartLevelWaterTime],[StartPercentBsw],[StartMassLiquid],[StartTankShellCorrection],[StartTemperatureAmbient],[StartTemperatureAmbientTime],[StartTemperatureProduct],[StartTemperatureProductTime],[StartTemperatureDensity],[StartTemperatureDensityTime],[StartVolume],[StartVolumeBsw],[StartVolumeCorrectionFactor],[StartVolumeGrossObserved],[StartVolumeGrossStandard],[StartVolumeNetStandard],[StartVolumeRoofCorrection],[StartVolumeTotalObserved],[StartVolumeWater],[UnitsLevelProductIndex],[UnitsTemperatureAmbientIndex],[UnitsTemperatureDensityIndex],[UnitsTemperatureProductIndex],[UnitsDensityProductObservedIndex],[UnitsDensityProductStandardIndex],[UnitsVolumeIndex],[UnitsMassIndex],[DecimalPlacesVolume],[DecimalPlacesLevel],[DecimalPlacesDensity],[DecimalPlacesTemperature],[UserData01],[UserData02],[UserData03],[UserData04],[UserData05],[UserData06],[UserData07],[UserData08],[UserData09],[UserData10],[TransferDeviation],[TransferPercentDeviation],[DecimalPlacesPercent],[TransferMode],[TransferStatus],[TransferTarget],[TransferTargetUnitsIndex],[TransferLevelTarget],[TransferVolumeTarget],[TransferTimeRemaining],[TransferDirection],[CommentDateTime],[CommentUserID],[Status],[VolumeWater],[LevelProduct],[StartDensityProductStandardInAir],[TransferredVolumeWater],[TransferredVolume],[MidnightRecord],[PointGuid],[RootParentGuid],[RecordSeq],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                VALUES (@MovementHistoryGuid,@SiteGuid,@Name,@Node,@InitiationCount,@RecordType,@TimeStamp,@ParentGuid,@AutoStart,@AutoStartTime,@AutoStop,@AutoStopTime,@CloseoutDataModifiedBy,@CloseoutDensityProductInAir,@CloseoutDensityProductObserved,@CloseoutDensityProductObservedTime,@CloseoutDensityProductStandard,@CloseoutDensityProductStandardTime,@CloseoutDensityProductStandardInAir,@CloseoutLevelProduct,@CloseoutLevelProductTime,@CloseoutLevelWater,@CloseoutMassLiquid,@CloseoutPercentBsw,@CloseoutRoofMass,@CloseoutTankShellCorrection,@CloseoutTemperatureAmbient,@CloseoutTemperatureAmbientTime,@CloseoutTemperatureDensity,@CloseoutTemperatureProduct,@CloseoutTime,@CloseoutTransferGov,@CloseoutTransferNsv,@CloseoutTransferMassLiquid,@CloseoutTransferVolumeWater,@CloseoutVolumeBsw,@CloseoutVolumeCorrectionFactor,@CloseoutVolumeGrossObserved,@CloseoutVolumeGrossStandard,@CloseoutVolumeNetStandard,@CloseoutVolumeRoofCorrection,@CloseoutVolumeTotalObserved,@CloseoutVolumeWater,@Comment,@Type,@OrderNumber,@PlannedStartTime,@Product,@ProductDescription,@StartTime,@StopTime,@StartDensityProductObserved,@StartDensityProductObservedTime,@StartDensityProductObservedInAir,@StartDensityProductStandard,@StartDensityProductStandardTime,@StartUserID,@StartLevelProduct,@StartLevelProductTime,@StartLevelWater,@StartLevelWaterTime,@StartPercentBsw,@StartMassLiquid,@StartTankShellCorrection,@StartTemperatureAmbient,@StartTemperatureAmbientTime,@StartTemperatureProduct,@StartTemperatureProductTime,@StartTemperatureDensity,@StartTemperatureDensityTime,@StartVolume,@StartVolumeBsw,@StartVolumeCorrectionFactor,@StartVolumeGrossObserved,@StartVolumeGrossStandard,@StartVolumeNetStandard,@StartVolumeRoofCorrection,@StartVolumeTotalObserved,@StartVolumeWater,@UnitsLevelProductIndex,@UnitsTemperatureAmbientIndex,@UnitsTemperatureDensityIndex,@UnitsTemperatureProductIndex,@UnitsDensityProductObservedIndex,@UnitsDensityProductStandardIndex,@UnitsVolumeIndex,@UnitsMassIndex,@DecimalPlacesVolume,@DecimalPlacesLevel,@DecimalPlacesDensity,@DecimalPlacesTemperature,@UserData01,@UserData02,@UserData03,@UserData04,@UserData05,@UserData06,@UserData07,@UserData08,@UserData09,@UserData10,@TransferDeviation,@TransferPercentDeviation,@DecimalPlacesPercent,@TransferMode,@TransferStatus,@TransferTarget,@TransferTargetUnitsIndex,@TransferLevelTarget,@TransferVolumeTarget,@TransferTimeRemaining,@TransferDirection,@CommentDateTime,@CommentUserID,@Status,@VolumeWater,@LevelProduct,@StartDensityProductStandardInAir,@TransferredVolumeWater,@TransferredVolume,@MidnightRecord,@PointGuid,@RootParentGuid,@RecordSeq,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MovementHistoryGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MovementHistoryGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MovementHistoryGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblMovementHistory] WHERE MovementHistoryGuid = @MovementHistoryGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
