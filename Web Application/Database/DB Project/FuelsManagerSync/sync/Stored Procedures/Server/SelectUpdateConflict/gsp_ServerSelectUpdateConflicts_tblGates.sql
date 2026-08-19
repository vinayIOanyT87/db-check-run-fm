-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGates
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGates]
@GateGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblGates].[ID],[dbo].[tblGates].[Description],[dbo].[tblGates].[ConcourseID],[dbo].[tblGates].[CreatedDate],[dbo].[tblGates].[CreatedBy],[dbo].[tblGates].[UpdatedDate],[dbo].[tblGates].[UpdatedBy],[dbo].[tblGates].[GateGuid],[dbo].[tblGates].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblGates]
            INNER JOIN [track].[tblGates] CT
                ON CT.PK_GateGuid = [dbo].[tblGates].[GateGuid]
        WHERE CT.PK_GateGuid = @GateGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
