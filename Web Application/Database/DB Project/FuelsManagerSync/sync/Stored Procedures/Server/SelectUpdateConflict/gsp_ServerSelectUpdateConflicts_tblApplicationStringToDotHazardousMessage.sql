-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToDotHazardousMessage
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblApplicationStringToDotHazardousMessage]
@ApplicationStringToDotHazardousMessageGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToDotHazardousMessage].[ApplicationStringToDotHazardousMessageGuid],[map].[tblApplicationStringToDotHazardousMessage].[ApplicationStringGuid],[map].[tblApplicationStringToDotHazardousMessage].[ProductGuid],[map].[tblApplicationStringToDotHazardousMessage].[Sequence],[map].[tblApplicationStringToDotHazardousMessage].[CreatedDate],[map].[tblApplicationStringToDotHazardousMessage].[CreatedBy],[map].[tblApplicationStringToDotHazardousMessage].[UpdatedDate],[map].[tblApplicationStringToDotHazardousMessage].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToDotHazardousMessage]
            INNER JOIN [track].[tblApplicationStringToDotHazardousMessage] CT
                ON CT.PK_ApplicationStringToDotHazardousMessageGuid = [map].[tblApplicationStringToDotHazardousMessage].[ApplicationStringToDotHazardousMessageGuid]
        WHERE CT.PK_ApplicationStringToDotHazardousMessageGuid = @ApplicationStringToDotHazardousMessageGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
