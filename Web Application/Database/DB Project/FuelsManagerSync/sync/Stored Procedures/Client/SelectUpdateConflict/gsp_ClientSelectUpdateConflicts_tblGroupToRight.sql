-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblGroupToRight
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGroupToRight]
@GroupToRightGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblGroupToRight].[GroupToRightGuid],[map].[tblGroupToRight].[GroupGuid],[map].[tblGroupToRight].[LookupRightIndex],[map].[tblGroupToRight].[CreatedDate],[map].[tblGroupToRight].[CreatedBy],[map].[tblGroupToRight].[UpdatedDate],[map].[tblGroupToRight].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblGroupToRight]
            INNER JOIN [track].[tblGroupToRight] CT
                ON CT.PK_GroupToRightGuid = [map].[tblGroupToRight].[GroupToRightGuid]
        WHERE CT.PK_GroupToRightGuid = @GroupToRightGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
