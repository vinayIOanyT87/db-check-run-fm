-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityProductMessageToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityProductMessageToSite]
@ProductMessageToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityProductMessageToSite].[ProductMessageToSiteGuid],[map].[tblEntityProductMessageToSite].[ApplicationStringGuid],[map].[tblEntityProductMessageToSite].[SiteGuid],[map].[tblEntityProductMessageToSite].[CreatedDate],[map].[tblEntityProductMessageToSite].[CreatedBy],[map].[tblEntityProductMessageToSite].[UpdatedDate],[map].[tblEntityProductMessageToSite].[UpdatedBy],[map].[tblEntityProductMessageToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityProductMessageToSite]
            INNER JOIN [track].[tblEntityProductMessageToSite] CT
                ON CT.PK_ProductMessageToSiteGuid = [map].[tblEntityProductMessageToSite].[ProductMessageToSiteGuid]
        WHERE CT.PK_ProductMessageToSiteGuid = @ProductMessageToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
