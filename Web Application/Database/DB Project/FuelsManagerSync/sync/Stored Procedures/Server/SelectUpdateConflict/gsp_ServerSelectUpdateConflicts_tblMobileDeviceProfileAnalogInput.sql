-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMobileDeviceProfileAnalogInput
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblMobileDeviceProfileAnalogInput]
@MobileDeviceProfileAnalogInputGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblMobileDeviceProfileAnalogInput].[MobileDeviceProfileAnalogInputGuid],[dbo].[tblMobileDeviceProfileAnalogInput].[MobileDeviceProfileGuid],[dbo].[tblMobileDeviceProfileAnalogInput].[LowLimit],[dbo].[tblMobileDeviceProfileAnalogInput].[HighLimit],[dbo].[tblMobileDeviceProfileAnalogInput].[ParameterA],[dbo].[tblMobileDeviceProfileAnalogInput].[ParameterB],[dbo].[tblMobileDeviceProfileAnalogInput].[ParameterC],[dbo].[tblMobileDeviceProfileAnalogInput].[AnalogFormula],[dbo].[tblMobileDeviceProfileAnalogInput].[CreatedDate],[dbo].[tblMobileDeviceProfileAnalogInput].[CreatedBy],[dbo].[tblMobileDeviceProfileAnalogInput].[UpdatedDate],[dbo].[tblMobileDeviceProfileAnalogInput].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblMobileDeviceProfileAnalogInput]
            INNER JOIN [track].[tblMobileDeviceProfileAnalogInput] CT
                ON CT.PK_MobileDeviceProfileAnalogInputGuid = [dbo].[tblMobileDeviceProfileAnalogInput].[MobileDeviceProfileAnalogInputGuid]
        WHERE CT.PK_MobileDeviceProfileAnalogInputGuid = @MobileDeviceProfileAnalogInputGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
