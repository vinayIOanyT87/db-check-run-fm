-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityIATACodeToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityIATACodeToSite]
@IATACodeToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityIATACodeToSite].[IATACodeToSiteGuid],[map].[tblEntityIATACodeToSite].[IATAGuid],[map].[tblEntityIATACodeToSite].[SiteGuid],[map].[tblEntityIATACodeToSite].[CreatedDate],[map].[tblEntityIATACodeToSite].[CreatedBy],[map].[tblEntityIATACodeToSite].[UpdatedDate],[map].[tblEntityIATACodeToSite].[UpdatedBy],[map].[tblEntityIATACodeToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityIATACodeToSite]
            INNER JOIN [track].[tblEntityIATACodeToSite] CT
                ON CT.PK_IATACodeToSiteGuid = [map].[tblEntityIATACodeToSite].[IATACodeToSiteGuid]
        WHERE CT.PK_IATACodeToSiteGuid = @IATACodeToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
