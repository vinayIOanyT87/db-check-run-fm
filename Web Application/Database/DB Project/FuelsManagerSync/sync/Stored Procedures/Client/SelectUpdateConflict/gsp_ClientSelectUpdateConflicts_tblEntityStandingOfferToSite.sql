-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityStandingOfferToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityStandingOfferToSite]
@StandingOfferToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityStandingOfferToSite].[StandingOfferToSiteGuid],[map].[tblEntityStandingOfferToSite].[StandingOfferGuid],[map].[tblEntityStandingOfferToSite].[SiteGuid],[map].[tblEntityStandingOfferToSite].[CreatedDate],[map].[tblEntityStandingOfferToSite].[CreatedBy],[map].[tblEntityStandingOfferToSite].[UpdatedDate],[map].[tblEntityStandingOfferToSite].[UpdatedBy],[map].[tblEntityStandingOfferToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityStandingOfferToSite]
            INNER JOIN [track].[tblEntityStandingOfferToSite] CT
                ON CT.PK_StandingOfferToSiteGuid = [map].[tblEntityStandingOfferToSite].[StandingOfferToSiteGuid]
        WHERE CT.PK_StandingOfferToSiteGuid = @StandingOfferToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
