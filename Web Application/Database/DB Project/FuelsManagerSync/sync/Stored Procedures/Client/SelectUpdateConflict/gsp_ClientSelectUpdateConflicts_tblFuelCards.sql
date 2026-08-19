-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblFuelCards
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblFuelCards]
@FuelCardGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblFuelCards].[ID],[dbo].[tblFuelCards].[Provider],[dbo].[tblFuelCards].[ActivationStatus],[dbo].[tblFuelCards].[InactivityPeriod],[dbo].[tblFuelCards].[Notes],[dbo].[tblFuelCards].[StatusModifiedDate],[dbo].[tblFuelCards].[StatusModifiedBy],[dbo].[tblFuelCards].[UserData1],[dbo].[tblFuelCards].[UserData2],[dbo].[tblFuelCards].[UserData3],[dbo].[tblFuelCards].[UserData4],[dbo].[tblFuelCards].[UserData5],[dbo].[tblFuelCards].[UserData6],[dbo].[tblFuelCards].[UserData7],[dbo].[tblFuelCards].[UserData8],[dbo].[tblFuelCards].[CreatedDate],[dbo].[tblFuelCards].[CreatedBy],[dbo].[tblFuelCards].[UpdatedDate],[dbo].[tblFuelCards].[UpdatedBy],[dbo].[tblFuelCards].[FuelCardGuid],[dbo].[tblFuelCards].[SiteGuid],[dbo].[tblFuelCards].[BillToCompanyGuid],[dbo].[tblFuelCards].[ManagerCompanyGuid],[dbo].[tblFuelCards].[OwnerCompanyGuid],[dbo].[tblFuelCards].[ShipperCompanyGuid],[dbo].[tblFuelCards].[ShipToCompanyGuid],[dbo].[tblFuelCards].[ExpirationDate],[dbo].[tblFuelCards].[TransientCardFlag],[dbo].[tblFuelCards].[PIN],[dbo].[tblFuelCards].[ProviderID],[dbo].[tblFuelCards].[FuelCardTypeApplicationStringGuid],[dbo].[tblFuelCards].[HiddenDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblFuelCards]
            INNER JOIN [track].[tblFuelCards] CT
                ON CT.PK_FuelCardGuid = [dbo].[tblFuelCards].[FuelCardGuid]
        WHERE CT.PK_FuelCardGuid = @FuelCardGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
