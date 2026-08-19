-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToExitMessage
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblApplicationStringToExitMessage]
@ApplicationStringToExitMessageGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToExitMessage].[ApplicationStringToExitMessageGuid],[map].[tblApplicationStringToExitMessage].[ApplicationStringGuid],[map].[tblApplicationStringToExitMessage].[ProductGroupApplicationStringGuid],[map].[tblApplicationStringToExitMessage].[Sequence],[map].[tblApplicationStringToExitMessage].[CreatedDate],[map].[tblApplicationStringToExitMessage].[CreatedBy],[map].[tblApplicationStringToExitMessage].[UpdatedDate],[map].[tblApplicationStringToExitMessage].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToExitMessage]
            INNER JOIN [track].[tblApplicationStringToExitMessage] CT
                ON CT.PK_ApplicationStringToExitMessageGuid = [map].[tblApplicationStringToExitMessage].[ApplicationStringToExitMessageGuid]
        WHERE CT.PK_ApplicationStringToExitMessageGuid = @ApplicationStringToExitMessageGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
