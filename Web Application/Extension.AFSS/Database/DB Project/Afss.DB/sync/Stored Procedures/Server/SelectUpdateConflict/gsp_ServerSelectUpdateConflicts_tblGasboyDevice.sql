-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyDevice
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGasboyDevice]
@GasboyDeviceGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblGasboyDevice].[GasboyDeviceGuid],[dbo].[tblGasboyDevice].[SiteGuid],[dbo].[tblGasboyDevice].[GasboyDepartmentGuid],[dbo].[tblGasboyDevice].[DeviceID],[dbo].[tblGasboyDevice].[DeviceCode],[dbo].[tblGasboyDevice].[DeviceName],[dbo].[tblGasboyDevice].[CardNumber],[dbo].[tblGasboyDevice].[GroupRuleName],[dbo].[tblGasboyDevice].[LookupGasboyDeviceTypeIndex],[dbo].[tblGasboyDevice].[LookupGasboyRecordStatusIndex],[dbo].[tblGasboyDevice].[LookupGasboyHardwareTypeIndex],[dbo].[tblGasboyDevice].[LookupGasboyAuthTypeIndex],[dbo].[tblGasboyDevice].[LookupGasboyEmployeeTypeIndex],[dbo].[tblGasboyDevice].[LookupGasboyTwoStageDriverValidationTypeIndex],[dbo].[tblGasboyDevice].[UsePINCodeFlag],[dbo].[tblGasboyDevice].[PINCode],[dbo].[tblGasboyDevice].[AuthPINFrom],[dbo].[tblGasboyDevice].[VehiclePlate],[dbo].[tblGasboyDevice].[PromptForVehiclePlateFlag],[dbo].[tblGasboyDevice].[LookupGasboyVehiclePlateCheckTypeIndex],[dbo].[tblGasboyDevice].[AlwaysPromptForAdditionalValidationFlag],[dbo].[tblGasboyDevice].[CreatedBy],[dbo].[tblGasboyDevice].[CreatedDate],[dbo].[tblGasboyDevice].[UpdatedBy],[dbo].[tblGasboyDevice].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblGasboyDevice]
            INNER JOIN [track].[tblGasboyDevice] CT
                ON CT.PK_GasboyDeviceGuid = [dbo].[tblGasboyDevice].[GasboyDeviceGuid]
        WHERE CT.PK_GasboyDeviceGuid = @GasboyDeviceGuid
    ORDER BY CT.UpdatedRowVersion ASC
END