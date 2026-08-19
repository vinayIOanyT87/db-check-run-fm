-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPointAccessGroupToTag
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPointAccessGroupToTag]
@PointAccessGroupToTagGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPointAccessGroupToTag].[PointAccessGroupToTagGuid],[map].[tblPointAccessGroupToTag].[PointAccessGroupGuid],[map].[tblPointAccessGroupToTag].[TagGuid],[map].[tblPointAccessGroupToTag].[View],[map].[tblPointAccessGroupToTag].[Modify],[map].[tblPointAccessGroupToTag].[ExceedRange],[map].[tblPointAccessGroupToTag].[Override],[map].[tblPointAccessGroupToTag].[CreatedDate],[map].[tblPointAccessGroupToTag].[CreatedBy],[map].[tblPointAccessGroupToTag].[UpdatedDate],[map].[tblPointAccessGroupToTag].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPointAccessGroupToTag]
            INNER JOIN [track].[tblPointAccessGroupToTag] CT
                ON CT.PK_PointAccessGroupToTagGuid = [map].[tblPointAccessGroupToTag].[PointAccessGroupToTagGuid]
        WHERE CT.PK_PointAccessGroupToTagGuid = @PointAccessGroupToTagGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
