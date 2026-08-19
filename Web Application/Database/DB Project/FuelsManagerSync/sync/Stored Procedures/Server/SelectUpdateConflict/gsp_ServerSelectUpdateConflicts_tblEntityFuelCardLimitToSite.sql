-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityFuelCardLimitToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityFuelCardLimitToSite]
@FuelCardLimitToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityFuelCardLimitToSite].[FuelCardLimitToSiteGuid],[map].[tblEntityFuelCardLimitToSite].[FuelCardLimitGuid],[map].[tblEntityFuelCardLimitToSite].[SiteGuid],[map].[tblEntityFuelCardLimitToSite].[AssignedFromSiteGuid],[map].[tblEntityFuelCardLimitToSite].[CreatedBy],[map].[tblEntityFuelCardLimitToSite].[CreatedDate],[map].[tblEntityFuelCardLimitToSite].[UpdatedBy],[map].[tblEntityFuelCardLimitToSite].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityFuelCardLimitToSite]
            INNER JOIN [track].[tblEntityFuelCardLimitToSite] CT
                ON CT.PK_FuelCardLimitToSiteGuid = [map].[tblEntityFuelCardLimitToSite].[FuelCardLimitToSiteGuid]
        WHERE CT.PK_FuelCardLimitToSiteGuid = @FuelCardLimitToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
