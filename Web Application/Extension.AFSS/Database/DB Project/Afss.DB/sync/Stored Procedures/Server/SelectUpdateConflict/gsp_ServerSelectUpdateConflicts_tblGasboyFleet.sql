-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyFleet
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGasboyFleet]
@GasboyFleetGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblGasboyFleet].[GasboyFleetGuid],[dbo].[tblGasboyFleet].[SiteGuid],[dbo].[tblGasboyFleet].[FleetID],[dbo].[tblGasboyFleet].[FleetCode],[dbo].[tblGasboyFleet].[FleetName],[dbo].[tblGasboyFleet].[GroupRuleName],[dbo].[tblGasboyFleet].[PriceListName],[dbo].[tblGasboyFleet].[LookupGasboyRecordStatusIndex],[dbo].[tblGasboyFleet].[UsePINCodeFlag],[dbo].[tblGasboyFleet].[PINCode],[dbo].[tblGasboyFleet].[AuthPINFrom],[dbo].[tblGasboyFleet].[PromptForVehiclePlateFlag],[dbo].[tblGasboyFleet].[LookupGasboyVehiclePlateCheckTypeIndex],[dbo].[tblGasboyFleet].[AlwaysPromptForAdditionalValidationFlag],[dbo].[tblGasboyFleet].[CreatedBy],[dbo].[tblGasboyFleet].[CreatedDate],[dbo].[tblGasboyFleet].[UpdatedBy],[dbo].[tblGasboyFleet].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblGasboyFleet]
            INNER JOIN [track].[tblGasboyFleet] CT
                ON CT.PK_GasboyFleetGuid = [dbo].[tblGasboyFleet].[GasboyFleetGuid]
        WHERE CT.PK_GasboyFleetGuid = @GasboyFleetGuid
    ORDER BY CT.UpdatedRowVersion ASC
END