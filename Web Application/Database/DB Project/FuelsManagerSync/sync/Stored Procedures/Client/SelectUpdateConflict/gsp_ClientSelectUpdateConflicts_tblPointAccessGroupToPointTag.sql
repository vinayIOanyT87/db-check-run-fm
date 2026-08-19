-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPointAccessGroupToPointTag
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPointAccessGroupToPointTag]
@PointAccessGroupToPointTagGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPointAccessGroupToPointTag].[PointAccessGroupToPointTagGuid],[map].[tblPointAccessGroupToPointTag].[PointAccessGroupGuid],[map].[tblPointAccessGroupToPointTag].[TagGuid],[map].[tblPointAccessGroupToPointTag].[View],[map].[tblPointAccessGroupToPointTag].[Modify],[map].[tblPointAccessGroupToPointTag].[ExceedRange],[map].[tblPointAccessGroupToPointTag].[Override],[map].[tblPointAccessGroupToPointTag].[CreatedDate],[map].[tblPointAccessGroupToPointTag].[CreatedBy],[map].[tblPointAccessGroupToPointTag].[UpdatedDate],[map].[tblPointAccessGroupToPointTag].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPointAccessGroupToPointTag]
            INNER JOIN [track].[tblPointAccessGroupToPointTag] CT
                ON CT.PK_PointAccessGroupToPointTagGuid = [map].[tblPointAccessGroupToPointTag].[PointAccessGroupToPointTagGuid]
        WHERE CT.PK_PointAccessGroupToPointTagGuid = @PointAccessGroupToPointTagGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
