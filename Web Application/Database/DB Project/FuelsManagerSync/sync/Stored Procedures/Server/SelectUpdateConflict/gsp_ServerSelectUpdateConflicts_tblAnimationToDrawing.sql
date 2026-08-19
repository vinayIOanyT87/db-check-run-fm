-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblAnimationToDrawing
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAnimationToDrawing]
@AnimationToDrawingGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblAnimationToDrawing].[AnimationToDrawingGuid],[map].[tblAnimationToDrawing].[AnimationGuid],[map].[tblAnimationToDrawing].[DrawingGuid],[map].[tblAnimationToDrawing].[CreatedBy],[map].[tblAnimationToDrawing].[CreatedDate],[map].[tblAnimationToDrawing].[UpdatedBy],[map].[tblAnimationToDrawing].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblAnimationToDrawing]
            INNER JOIN [track].[tblAnimationToDrawing] CT
                ON CT.PK_AnimationToDrawingGuid = [map].[tblAnimationToDrawing].[AnimationToDrawingGuid]
        WHERE CT.PK_AnimationToDrawingGuid = @AnimationToDrawingGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
