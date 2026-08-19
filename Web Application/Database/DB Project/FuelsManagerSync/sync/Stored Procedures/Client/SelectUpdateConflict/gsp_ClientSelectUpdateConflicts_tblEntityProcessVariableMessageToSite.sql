-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityProcessVariableMessageToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityProcessVariableMessageToSite]
@ProcessVariableMessageToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityProcessVariableMessageToSite].[ProcessVariableMessageToSiteGuid],[map].[tblEntityProcessVariableMessageToSite].[ApplicationStringGuid],[map].[tblEntityProcessVariableMessageToSite].[SiteGuid],[map].[tblEntityProcessVariableMessageToSite].[CreatedDate],[map].[tblEntityProcessVariableMessageToSite].[CreatedBy],[map].[tblEntityProcessVariableMessageToSite].[UpdatedDate],[map].[tblEntityProcessVariableMessageToSite].[UpdatedBy],[map].[tblEntityProcessVariableMessageToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityProcessVariableMessageToSite]
            INNER JOIN [track].[tblEntityProcessVariableMessageToSite] CT
                ON CT.PK_ProcessVariableMessageToSiteGuid = [map].[tblEntityProcessVariableMessageToSite].[ProcessVariableMessageToSiteGuid]
        WHERE CT.PK_ProcessVariableMessageToSiteGuid = @ProcessVariableMessageToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
