-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityAdditiveProfileToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityAdditiveProfileToSite]
@AdditiveProfileToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityAdditiveProfileToSite].[AdditiveProfileToSiteGuid],[map].[tblEntityAdditiveProfileToSite].[AdditiveProfileGuid],[map].[tblEntityAdditiveProfileToSite].[SiteGuid],[map].[tblEntityAdditiveProfileToSite].[CreatedDate],[map].[tblEntityAdditiveProfileToSite].[CreatedBy],[map].[tblEntityAdditiveProfileToSite].[UpdatedDate],[map].[tblEntityAdditiveProfileToSite].[UpdatedBy],[map].[tblEntityAdditiveProfileToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityAdditiveProfileToSite]
            INNER JOIN [track].[tblEntityAdditiveProfileToSite] CT
                ON CT.PK_AdditiveProfileToSiteGuid = [map].[tblEntityAdditiveProfileToSite].[AdditiveProfileToSiteGuid]
        WHERE CT.PK_AdditiveProfileToSiteGuid = @AdditiveProfileToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
