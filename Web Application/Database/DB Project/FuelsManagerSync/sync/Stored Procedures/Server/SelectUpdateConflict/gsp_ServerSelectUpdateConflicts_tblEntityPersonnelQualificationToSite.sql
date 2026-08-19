-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityPersonnelQualificationToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityPersonnelQualificationToSite]
@PersonnelQualificationToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityPersonnelQualificationToSite].[PersonnelQualificationToSiteGuid],[map].[tblEntityPersonnelQualificationToSite].[QualificationGuid],[map].[tblEntityPersonnelQualificationToSite].[SiteGuid],[map].[tblEntityPersonnelQualificationToSite].[CreatedDate],[map].[tblEntityPersonnelQualificationToSite].[CreatedBy],[map].[tblEntityPersonnelQualificationToSite].[UpdatedDate],[map].[tblEntityPersonnelQualificationToSite].[UpdatedBy],[map].[tblEntityPersonnelQualificationToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityPersonnelQualificationToSite]
            INNER JOIN [track].[tblEntityPersonnelQualificationToSite] CT
                ON CT.PK_PersonnelQualificationToSiteGuid = [map].[tblEntityPersonnelQualificationToSite].[PersonnelQualificationToSiteGuid]
        WHERE CT.PK_PersonnelQualificationToSiteGuid = @PersonnelQualificationToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
