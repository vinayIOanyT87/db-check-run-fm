-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityAlarmAndEventToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityAlarmAndEventToSite]
@AlarmAndEventToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityAlarmAndEventToSite].[AlarmAndEventToSiteGuid],[map].[tblEntityAlarmAndEventToSite].[OwnerSiteGuid],[map].[tblEntityAlarmAndEventToSite].[MapToSiteGuid],[map].[tblEntityAlarmAndEventToSite].[CreatedDate],[map].[tblEntityAlarmAndEventToSite].[CreatedBy],[map].[tblEntityAlarmAndEventToSite].[UpdatedDate],[map].[tblEntityAlarmAndEventToSite].[UpdatedBy],[map].[tblEntityAlarmAndEventToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityAlarmAndEventToSite]
            INNER JOIN [track].[tblEntityAlarmAndEventToSite] CT
                ON CT.PK_AlarmAndEventToSiteGuid = [map].[tblEntityAlarmAndEventToSite].[AlarmAndEventToSiteGuid]
        WHERE CT.PK_AlarmAndEventToSiteGuid = @AlarmAndEventToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
