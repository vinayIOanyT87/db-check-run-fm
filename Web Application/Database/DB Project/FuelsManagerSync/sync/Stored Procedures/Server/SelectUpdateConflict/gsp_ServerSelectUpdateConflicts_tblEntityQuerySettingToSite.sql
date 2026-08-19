-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityQuerySettingToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityQuerySettingToSite]
@QuerySettingToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityQuerySettingToSite].[QuerySettingToSiteGuid],[map].[tblEntityQuerySettingToSite].[SiteGuid],[map].[tblEntityQuerySettingToSite].[MapToSiteGuid],[map].[tblEntityQuerySettingToSite].[CreatedDate],[map].[tblEntityQuerySettingToSite].[CreatedBy],[map].[tblEntityQuerySettingToSite].[UpdatedDate],[map].[tblEntityQuerySettingToSite].[UpdatedBy],[map].[tblEntityQuerySettingToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityQuerySettingToSite]
            INNER JOIN [track].[tblEntityQuerySettingToSite] CT
                ON CT.PK_QuerySettingToSiteGuid = [map].[tblEntityQuerySettingToSite].[QuerySettingToSiteGuid]
        WHERE CT.PK_QuerySettingToSiteGuid = @QuerySettingToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
