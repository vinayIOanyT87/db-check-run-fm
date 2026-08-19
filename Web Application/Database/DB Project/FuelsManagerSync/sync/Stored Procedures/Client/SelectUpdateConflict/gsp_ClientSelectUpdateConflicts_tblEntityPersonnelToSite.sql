-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityPersonnelToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityPersonnelToSite]
@PersonnelToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityPersonnelToSite].[PersonnelToSiteGuid],[map].[tblEntityPersonnelToSite].[PersonnelGuid],[map].[tblEntityPersonnelToSite].[SiteGuid],[map].[tblEntityPersonnelToSite].[CreatedDate],[map].[tblEntityPersonnelToSite].[CreatedBy],[map].[tblEntityPersonnelToSite].[UpdatedDate],[map].[tblEntityPersonnelToSite].[UpdatedBy],[map].[tblEntityPersonnelToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityPersonnelToSite]
            INNER JOIN [track].[tblEntityPersonnelToSite] CT
                ON CT.PK_PersonnelToSiteGuid = [map].[tblEntityPersonnelToSite].[PersonnelToSiteGuid]
        WHERE CT.PK_PersonnelToSiteGuid = @PersonnelToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
