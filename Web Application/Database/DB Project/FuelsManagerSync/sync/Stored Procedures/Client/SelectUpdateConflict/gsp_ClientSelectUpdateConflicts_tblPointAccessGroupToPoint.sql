-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPointAccessGroupToPoint
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPointAccessGroupToPoint]
@PointAccessGroupToPointGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPointAccessGroupToPoint].[PointAccessGroupToPointGuid],[map].[tblPointAccessGroupToPoint].[PointAccessGroupGuid],[map].[tblPointAccessGroupToPoint].[PointGuid],[map].[tblPointAccessGroupToPoint].[CreatedDate],[map].[tblPointAccessGroupToPoint].[CreatedBy],[map].[tblPointAccessGroupToPoint].[UpdatedDate],[map].[tblPointAccessGroupToPoint].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPointAccessGroupToPoint]
            INNER JOIN [track].[tblPointAccessGroupToPoint] CT
                ON CT.PK_PointAccessGroupToPointGuid = [map].[tblPointAccessGroupToPoint].[PointAccessGroupToPointGuid]
        WHERE CT.PK_PointAccessGroupToPointGuid = @PointAccessGroupToPointGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
