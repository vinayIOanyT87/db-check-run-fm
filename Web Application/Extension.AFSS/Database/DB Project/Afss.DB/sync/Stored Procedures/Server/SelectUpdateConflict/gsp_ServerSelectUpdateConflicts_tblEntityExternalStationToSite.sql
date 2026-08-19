-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityExternalStationToSite
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityExternalStationToSite]
@ExternalStationToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityExternalStationToSite].[ExternalStationToSiteGuid],[map].[tblEntityExternalStationToSite].[ExternalStationGuid],[map].[tblEntityExternalStationToSite].[SiteGuid],[map].[tblEntityExternalStationToSite].[AssignedFromSiteGuid],[map].[tblEntityExternalStationToSite].[CreatedBy],[map].[tblEntityExternalStationToSite].[CreatedDate],[map].[tblEntityExternalStationToSite].[UpdatedBy],[map].[tblEntityExternalStationToSite].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityExternalStationToSite]
            INNER JOIN [track].[tblEntityExternalStationToSite] CT
                ON CT.PK_ExternalStationToSiteGuid = [map].[tblEntityExternalStationToSite].[ExternalStationToSiteGuid]
        WHERE CT.PK_ExternalStationToSiteGuid = @ExternalStationToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END