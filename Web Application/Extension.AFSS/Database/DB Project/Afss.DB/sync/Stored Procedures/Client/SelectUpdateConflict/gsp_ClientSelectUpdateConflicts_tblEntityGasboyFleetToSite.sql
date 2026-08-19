-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityGasboyFleetToSite
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityGasboyFleetToSite]
@GasboyFleetToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityGasboyFleetToSite].[GasboyFleetToSiteGuid],[map].[tblEntityGasboyFleetToSite].[GasboyFleetGuid],[map].[tblEntityGasboyFleetToSite].[SiteGuid],[map].[tblEntityGasboyFleetToSite].[AssignedFromSiteGuid],[map].[tblEntityGasboyFleetToSite].[CreatedBy],[map].[tblEntityGasboyFleetToSite].[CreatedDate],[map].[tblEntityGasboyFleetToSite].[UpdatedBy],[map].[tblEntityGasboyFleetToSite].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityGasboyFleetToSite]
            INNER JOIN [track].[tblEntityGasboyFleetToSite] CT
                ON CT.PK_GasboyFleetToSiteGuid = [map].[tblEntityGasboyFleetToSite].[GasboyFleetToSiteGuid]
        WHERE CT.PK_GasboyFleetToSiteGuid = @GasboyFleetToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END