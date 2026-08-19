-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityPersonnelTrainingToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityPersonnelTrainingToSite]
@PersonnelTrainingToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityPersonnelTrainingToSite].[PersonnelTrainingToSiteGuid],[map].[tblEntityPersonnelTrainingToSite].[QualificationGuid],[map].[tblEntityPersonnelTrainingToSite].[SiteGuid],[map].[tblEntityPersonnelTrainingToSite].[CreatedDate],[map].[tblEntityPersonnelTrainingToSite].[CreatedBy],[map].[tblEntityPersonnelTrainingToSite].[UpdatedDate],[map].[tblEntityPersonnelTrainingToSite].[UpdatedBy],[map].[tblEntityPersonnelTrainingToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityPersonnelTrainingToSite]
            INNER JOIN [track].[tblEntityPersonnelTrainingToSite] CT
                ON CT.PK_PersonnelTrainingToSiteGuid = [map].[tblEntityPersonnelTrainingToSite].[PersonnelTrainingToSiteGuid]
        WHERE CT.PK_PersonnelTrainingToSiteGuid = @PersonnelTrainingToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
