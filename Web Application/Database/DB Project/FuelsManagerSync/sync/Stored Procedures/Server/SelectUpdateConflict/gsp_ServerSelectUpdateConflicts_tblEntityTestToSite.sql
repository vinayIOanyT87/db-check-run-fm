-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityTestToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityTestToSite]
@TestToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityTestToSite].[TestToSiteGuid],[map].[tblEntityTestToSite].[TestDefinitionGuid],[map].[tblEntityTestToSite].[SiteGuid],[map].[tblEntityTestToSite].[CreatedDate],[map].[tblEntityTestToSite].[CreatedBy],[map].[tblEntityTestToSite].[UpdatedDate],[map].[tblEntityTestToSite].[UpdatedBy],[map].[tblEntityTestToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityTestToSite]
            INNER JOIN [track].[tblEntityTestToSite] CT
                ON CT.PK_TestToSiteGuid = [map].[tblEntityTestToSite].[TestToSiteGuid]
        WHERE CT.PK_TestToSiteGuid = @TestToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
