-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblTestSetStatus
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTestSetStatus]
@TestSetStatusIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblTestSetStatus].[TestSetStatusIndex],[lookup].[tblTestSetStatus].[TestSetStatusCode],[lookup].[tblTestSetStatus].[TestSetStatusName],[lookup].[tblTestSetStatus].[TestSetStatusGuid],[lookup].[tblTestSetStatus].[CreatedDate],[lookup].[tblTestSetStatus].[CreatedBy],[lookup].[tblTestSetStatus].[UpdatedDate],[lookup].[tblTestSetStatus].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblTestSetStatus]
            INNER JOIN [track].[tblTestSetStatus] CT
                ON CT.PK_TestSetStatusIndex = [lookup].[tblTestSetStatus].[TestSetStatusIndex]
        WHERE CT.PK_TestSetStatusIndex = @TestSetStatusIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
