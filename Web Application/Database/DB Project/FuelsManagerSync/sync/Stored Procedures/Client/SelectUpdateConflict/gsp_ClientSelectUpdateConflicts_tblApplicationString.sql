-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblApplicationString
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblApplicationString]
@ApplicationStringGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblApplicationString].[ID],[dbo].[tblApplicationString].[CreatedDate],[dbo].[tblApplicationString].[CreatedBy],[dbo].[tblApplicationString].[UpdatedDate],[dbo].[tblApplicationString].[UpdatedBy],[dbo].[tblApplicationString].[StartDate],[dbo].[tblApplicationString].[EndDate],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid],[dbo].[tblApplicationString].[LookupApplicationStringTypeIndex], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblApplicationString]
            INNER JOIN [track].[tblApplicationString] CT
                ON CT.PK_ApplicationStringGuid = [dbo].[tblApplicationString].[ApplicationStringGuid]
        WHERE CT.PK_ApplicationStringGuid = @ApplicationStringGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
