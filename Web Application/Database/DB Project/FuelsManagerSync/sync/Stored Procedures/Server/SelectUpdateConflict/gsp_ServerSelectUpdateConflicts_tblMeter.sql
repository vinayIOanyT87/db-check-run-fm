-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMeter
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblMeter]
@MeterGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblMeter].[MeterGuid],[dbo].[tblMeter].[SiteGuid],[dbo].[tblMeter].[MeterID],[dbo].[tblMeter].[NumberOfDigits],[dbo].[tblMeter].[RotatesBackwardsFlag],[dbo].[tblMeter].[ReceiptMeterFlag],[dbo].[tblMeter].[MeterFactor],[dbo].[tblMeter].[FuelCompressionFactor],[dbo].[tblMeter].[CreatedDate],[dbo].[tblMeter].[CreatedBy],[dbo].[tblMeter].[UpdatedDate],[dbo].[tblMeter].[UpdatedBy],[dbo].[tblMeter].[DcuID],[dbo].[tblMeter].[DcuBatteryVoltage],[dbo].[tblMeter].[DcuBatteryCurrent],[dbo].[tblMeter].[DcuTemperature],[dbo].[tblMeter].[DcuResets],[dbo].[tblMeter].[DcuUpdateDate],[dbo].[tblMeter].[DcuConfigurationDate],[dbo].[tblMeter].[DcuFirmwareVersion],[dbo].[tblMeter].[DcuBluetoothAddress], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblMeter]
            INNER JOIN [track].[tblMeter] CT
                ON CT.PK_MeterGuid = [dbo].[tblMeter].[MeterGuid]
        WHERE CT.PK_MeterGuid = @MeterGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
