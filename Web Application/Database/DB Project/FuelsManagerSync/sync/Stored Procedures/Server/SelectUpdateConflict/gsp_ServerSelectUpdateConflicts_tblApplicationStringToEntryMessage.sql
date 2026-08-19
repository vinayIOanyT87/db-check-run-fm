-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToEntryMessage
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblApplicationStringToEntryMessage]
@ApplicationStringToEntryMessageGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToEntryMessage].[ApplicationStringToEntryMessageGuid],[map].[tblApplicationStringToEntryMessage].[ApplicationStringGuid],[map].[tblApplicationStringToEntryMessage].[ProductGroupApplicationStringGuid],[map].[tblApplicationStringToEntryMessage].[Sequence],[map].[tblApplicationStringToEntryMessage].[CreatedDate],[map].[tblApplicationStringToEntryMessage].[CreatedBy],[map].[tblApplicationStringToEntryMessage].[UpdatedDate],[map].[tblApplicationStringToEntryMessage].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToEntryMessage]
            INNER JOIN [track].[tblApplicationStringToEntryMessage] CT
                ON CT.PK_ApplicationStringToEntryMessageGuid = [map].[tblApplicationStringToEntryMessage].[ApplicationStringToEntryMessageGuid]
        WHERE CT.PK_ApplicationStringToEntryMessageGuid = @ApplicationStringToEntryMessageGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
