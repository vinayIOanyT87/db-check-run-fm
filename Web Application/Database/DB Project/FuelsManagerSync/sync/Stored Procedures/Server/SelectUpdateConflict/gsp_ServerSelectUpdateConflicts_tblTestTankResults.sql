-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestTankResults
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTestTankResults]
@TestTankResultGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTestTankResults].[TestName],[dbo].[tblTestTankResults].[Measurement],[dbo].[tblTestTankResults].[TestDate],[dbo].[tblTestTankResults].[DeleteFlag],[dbo].[tblTestTankResults].[CreatedDate],[dbo].[tblTestTankResults].[CreatedBy],[dbo].[tblTestTankResults].[UpdatedDate],[dbo].[tblTestTankResults].[UpdatedBy],[dbo].[tblTestTankResults].[PerformedBy],[dbo].[tblTestTankResults].[Supervisor],[dbo].[tblTestTankResults].[Flag01],[dbo].[tblTestTankResults].[Flag02],[dbo].[tblTestTankResults].[TestCode],[dbo].[tblTestTankResults].[TestTankResultGuid],[dbo].[tblTestTankResults].[LookupTestSetStatusIndex],[dbo].[tblTestTankResults].[TestSetTankResultGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTestTankResults]
            INNER JOIN [track].[tblTestTankResults] CT
                ON CT.PK_TestTankResultGuid = [dbo].[tblTestTankResults].[TestTankResultGuid]
        WHERE CT.PK_TestTankResultGuid = @TestTankResultGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
