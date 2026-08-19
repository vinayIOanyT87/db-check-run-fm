-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityProductToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityProductToSite]
@ProductToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityProductToSite].[ProductToSiteGuid],[map].[tblEntityProductToSite].[ProductGuid],[map].[tblEntityProductToSite].[SiteGuid],[map].[tblEntityProductToSite].[CreatedDate],[map].[tblEntityProductToSite].[CreatedBy],[map].[tblEntityProductToSite].[UpdatedDate],[map].[tblEntityProductToSite].[UpdatedBy],[map].[tblEntityProductToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityProductToSite]
            INNER JOIN [track].[tblEntityProductToSite] CT
                ON CT.PK_ProductToSiteGuid = [map].[tblEntityProductToSite].[ProductToSiteGuid]
        WHERE CT.PK_ProductToSiteGuid = @ProductToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
