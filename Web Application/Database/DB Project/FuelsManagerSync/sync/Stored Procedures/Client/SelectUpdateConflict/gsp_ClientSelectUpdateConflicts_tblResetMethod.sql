-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblResetMethod
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblResetMethod]
@ResetMethodIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblResetMethod].[ResetMethodIndex],[lookup].[tblResetMethod].[ResetMethodCode],[lookup].[tblResetMethod].[ResetMethodName],[lookup].[tblResetMethod].[ResetMethodGuid],[lookup].[tblResetMethod].[CreatedDate],[lookup].[tblResetMethod].[CreatedBy],[lookup].[tblResetMethod].[UpdatedDate],[lookup].[tblResetMethod].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblResetMethod]
            INNER JOIN [track].[tblResetMethod] CT
                ON CT.PK_ResetMethodIndex = [lookup].[tblResetMethod].[ResetMethodIndex]
        WHERE CT.PK_ResetMethodIndex = @ResetMethodIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
