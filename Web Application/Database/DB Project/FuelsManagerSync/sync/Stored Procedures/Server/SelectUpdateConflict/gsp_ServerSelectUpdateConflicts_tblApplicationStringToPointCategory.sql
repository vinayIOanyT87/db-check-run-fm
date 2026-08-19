-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToPointCategory
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblApplicationStringToPointCategory]
@ApplicationStringToPointCategoryGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToPointCategory].[ApplicationStringToPointCategoryGuid],[map].[tblApplicationStringToPointCategory].[ApplicationStringGuid],[map].[tblApplicationStringToPointCategory].[PointGuid],[map].[tblApplicationStringToPointCategory].[Sequence],[map].[tblApplicationStringToPointCategory].[CreatedDate],[map].[tblApplicationStringToPointCategory].[CreatedBy],[map].[tblApplicationStringToPointCategory].[UpdatedDate],[map].[tblApplicationStringToPointCategory].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToPointCategory]
            INNER JOIN [track].[tblApplicationStringToPointCategory] CT
                ON CT.PK_ApplicationStringToPointCategoryGuid = [map].[tblApplicationStringToPointCategory].[ApplicationStringToPointCategoryGuid]
        WHERE CT.PK_ApplicationStringToPointCategoryGuid = @ApplicationStringToPointCategoryGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
