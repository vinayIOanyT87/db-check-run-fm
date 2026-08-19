-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityAlarmPriorityToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityAlarmPriorityToSite]
@AlarmPriorityToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityAlarmPriorityToSite].[AlarmPriorityToSiteGuid],[map].[tblEntityAlarmPriorityToSite].[AlarmPriorityGuid],[map].[tblEntityAlarmPriorityToSite].[SiteGuid],[map].[tblEntityAlarmPriorityToSite].[CreatedDate],[map].[tblEntityAlarmPriorityToSite].[CreatedBy],[map].[tblEntityAlarmPriorityToSite].[UpdatedDate],[map].[tblEntityAlarmPriorityToSite].[UpdatedBy],[map].[tblEntityAlarmPriorityToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityAlarmPriorityToSite]
            INNER JOIN [track].[tblEntityAlarmPriorityToSite] CT
                ON CT.PK_AlarmPriorityToSiteGuid = [map].[tblEntityAlarmPriorityToSite].[AlarmPriorityToSiteGuid]
        WHERE CT.PK_AlarmPriorityToSiteGuid = @AlarmPriorityToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
