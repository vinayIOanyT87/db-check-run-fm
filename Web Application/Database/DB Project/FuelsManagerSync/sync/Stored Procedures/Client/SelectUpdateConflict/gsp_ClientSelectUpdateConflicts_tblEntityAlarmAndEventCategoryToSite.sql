-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityAlarmAndEventCategoryToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityAlarmAndEventCategoryToSite]
@AlarmAndEventCategoryToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityAlarmAndEventCategoryToSite].[AlarmAndEventCategoryToSiteGuid],[map].[tblEntityAlarmAndEventCategoryToSite].[ApplicationStringGuid],[map].[tblEntityAlarmAndEventCategoryToSite].[SiteGuid],[map].[tblEntityAlarmAndEventCategoryToSite].[CreatedDate],[map].[tblEntityAlarmAndEventCategoryToSite].[CreatedBy],[map].[tblEntityAlarmAndEventCategoryToSite].[UpdatedDate],[map].[tblEntityAlarmAndEventCategoryToSite].[UpdatedBy],[map].[tblEntityAlarmAndEventCategoryToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityAlarmAndEventCategoryToSite]
            INNER JOIN [track].[tblEntityAlarmAndEventCategoryToSite] CT
                ON CT.PK_AlarmAndEventCategoryToSiteGuid = [map].[tblEntityAlarmAndEventCategoryToSite].[AlarmAndEventCategoryToSiteGuid]
        WHERE CT.PK_AlarmAndEventCategoryToSiteGuid = @AlarmAndEventCategoryToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
