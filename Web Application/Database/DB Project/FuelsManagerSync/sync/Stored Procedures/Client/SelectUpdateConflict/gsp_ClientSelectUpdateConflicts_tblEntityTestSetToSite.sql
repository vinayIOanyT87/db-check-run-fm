-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityTestSetToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityTestSetToSite]
@TestSetToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityTestSetToSite].[TestSetToSiteGuid],[map].[tblEntityTestSetToSite].[TestSetDefinitionGuid],[map].[tblEntityTestSetToSite].[SiteGuid],[map].[tblEntityTestSetToSite].[CreatedDate],[map].[tblEntityTestSetToSite].[CreatedBy],[map].[tblEntityTestSetToSite].[UpdatedDate],[map].[tblEntityTestSetToSite].[UpdatedBy],[map].[tblEntityTestSetToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityTestSetToSite]
            INNER JOIN [track].[tblEntityTestSetToSite] CT
                ON CT.PK_TestSetToSiteGuid = [map].[tblEntityTestSetToSite].[TestSetToSiteGuid]
        WHERE CT.PK_TestSetToSiteGuid = @TestSetToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
