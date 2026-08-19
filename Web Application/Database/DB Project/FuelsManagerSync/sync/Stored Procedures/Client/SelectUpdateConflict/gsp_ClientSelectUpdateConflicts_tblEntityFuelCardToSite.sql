-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityFuelCardToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityFuelCardToSite]
@FuelCardToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityFuelCardToSite].[FuelCardToSiteGuid],[map].[tblEntityFuelCardToSite].[FuelCardGuid],[map].[tblEntityFuelCardToSite].[SiteGuid],[map].[tblEntityFuelCardToSite].[CreatedDate],[map].[tblEntityFuelCardToSite].[CreatedBy],[map].[tblEntityFuelCardToSite].[UpdatedDate],[map].[tblEntityFuelCardToSite].[UpdatedBy],[map].[tblEntityFuelCardToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityFuelCardToSite]
            INNER JOIN [track].[tblEntityFuelCardToSite] CT
                ON CT.PK_FuelCardToSiteGuid = [map].[tblEntityFuelCardToSite].[FuelCardToSiteGuid]
        WHERE CT.PK_FuelCardToSiteGuid = @FuelCardToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
