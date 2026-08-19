-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityProductGroupToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityProductGroupToSite]
@ProductGroupToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityProductGroupToSite].[ProductGroupToSiteGuid],[map].[tblEntityProductGroupToSite].[ApplicationStringGuid],[map].[tblEntityProductGroupToSite].[SiteGuid],[map].[tblEntityProductGroupToSite].[CreatedDate],[map].[tblEntityProductGroupToSite].[CreatedBy],[map].[tblEntityProductGroupToSite].[UpdatedDate],[map].[tblEntityProductGroupToSite].[UpdatedBy],[map].[tblEntityProductGroupToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityProductGroupToSite]
            INNER JOIN [track].[tblEntityProductGroupToSite] CT
                ON CT.PK_ProductGroupToSiteGuid = [map].[tblEntityProductGroupToSite].[ProductGroupToSiteGuid]
        WHERE CT.PK_ProductGroupToSiteGuid = @ProductGroupToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
