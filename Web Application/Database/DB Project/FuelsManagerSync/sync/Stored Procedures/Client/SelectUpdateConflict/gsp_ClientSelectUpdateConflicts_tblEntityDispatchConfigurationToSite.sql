-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityDispatchConfigurationToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityDispatchConfigurationToSite]
@DispatchConfigurationToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityDispatchConfigurationToSite].[DispatchConfigurationToSiteGuid],[map].[tblEntityDispatchConfigurationToSite].[DispatchConfigurationGuid],[map].[tblEntityDispatchConfigurationToSite].[SiteGuid],[map].[tblEntityDispatchConfigurationToSite].[CreatedDate],[map].[tblEntityDispatchConfigurationToSite].[CreatedBy],[map].[tblEntityDispatchConfigurationToSite].[UpdatedDate],[map].[tblEntityDispatchConfigurationToSite].[UpdatedBy],[map].[tblEntityDispatchConfigurationToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityDispatchConfigurationToSite]
            INNER JOIN [track].[tblEntityDispatchConfigurationToSite] CT
                ON CT.PK_DispatchConfigurationToSiteGuid = [map].[tblEntityDispatchConfigurationToSite].[DispatchConfigurationToSiteGuid]
        WHERE CT.PK_DispatchConfigurationToSiteGuid = @DispatchConfigurationToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
