-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTanks
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTanks]
@TankGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTanks].[TankID],[dbo].[tblTanks].[CreatedDate],[dbo].[tblTanks].[CreatedBy],[dbo].[tblTanks].[UpdatedDate],[dbo].[tblTanks].[UpdatedBy],[dbo].[tblTanks].[TankGuid],[dbo].[tblTanks].[SiteGuid],[dbo].[tblTanks].[LookupVesselTypeIndex],[dbo].[tblTanks].[ManagerCompanyGuid],[dbo].[tblTanks].[ProductGuid],[dbo].[tblTanks].[HiddenDate],[dbo].[tblTanks].[AssetTrackingDeviceGuid],[dbo].[tblTanks].[LookupDeviceTankTypeIndex],[dbo].[tblTanks].[Latitude],[dbo].[tblTanks].[Longitude],[dbo].[tblTanks].[TankConfigurationNumber],[dbo].[tblTanks].[Zoom],[dbo].[tblTanks].[OwnerCompanyGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTanks]
            INNER JOIN [track].[tblTanks] CT
                ON CT.PK_TankGuid = [dbo].[tblTanks].[TankGuid]
        WHERE CT.PK_TankGuid = @TankGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
