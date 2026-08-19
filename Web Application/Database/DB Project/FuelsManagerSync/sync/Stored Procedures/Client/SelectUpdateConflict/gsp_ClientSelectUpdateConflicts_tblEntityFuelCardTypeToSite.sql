-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityFuelCardTypeToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityFuelCardTypeToSite]
@FuelCardTypeToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityFuelCardTypeToSite].[FuelCardTypeToSiteGuid],[map].[tblEntityFuelCardTypeToSite].[ApplicationStringGuid],[map].[tblEntityFuelCardTypeToSite].[SiteGuid],[map].[tblEntityFuelCardTypeToSite].[CreatedDate],[map].[tblEntityFuelCardTypeToSite].[CreatedBy],[map].[tblEntityFuelCardTypeToSite].[UpdatedDate],[map].[tblEntityFuelCardTypeToSite].[UpdatedBy],[map].[tblEntityFuelCardTypeToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityFuelCardTypeToSite]
            INNER JOIN [track].[tblEntityFuelCardTypeToSite] CT
                ON CT.PK_FuelCardTypeToSiteGuid = [map].[tblEntityFuelCardTypeToSite].[FuelCardTypeToSiteGuid]
        WHERE CT.PK_FuelCardTypeToSiteGuid = @FuelCardTypeToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
