-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblFillMethod
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblFillMethod]
@FillMethodIndex tinyint
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblFillMethod].[FillMethodIndex],[lookup].[tblFillMethod].[FillMethodCode],[lookup].[tblFillMethod].[FillMethodName],[lookup].[tblFillMethod].[FillMethodGuid],[lookup].[tblFillMethod].[CreatedDate],[lookup].[tblFillMethod].[CreatedBy],[lookup].[tblFillMethod].[UpdatedDate],[lookup].[tblFillMethod].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblFillMethod]
            INNER JOIN [track].[tblFillMethod] CT
                ON CT.PK_FillMethodIndex = [lookup].[tblFillMethod].[FillMethodIndex]
        WHERE CT.PK_FillMethodIndex = @FillMethodIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
