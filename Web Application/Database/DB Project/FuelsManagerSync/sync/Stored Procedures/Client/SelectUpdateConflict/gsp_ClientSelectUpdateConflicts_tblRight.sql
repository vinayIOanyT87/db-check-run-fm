-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblRight
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblRight]
@RightIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblRight].[RightIndex],[lookup].[tblRight].[RightCode],[lookup].[tblRight].[RightName],[lookup].[tblRight].[RightGuid],[lookup].[tblRight].[CreatedDate],[lookup].[tblRight].[CreatedBy],[lookup].[tblRight].[UpdatedDate],[lookup].[tblRight].[UpdatedBy],[lookup].[tblRight].[RightDescription], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblRight]
            INNER JOIN [track].[tblRight] CT
                ON CT.PK_RightIndex = [lookup].[tblRight].[RightIndex]
        WHERE CT.PK_RightIndex = @RightIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
