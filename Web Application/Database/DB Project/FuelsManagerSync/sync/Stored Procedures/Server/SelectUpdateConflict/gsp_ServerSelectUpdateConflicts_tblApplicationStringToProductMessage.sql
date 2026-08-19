-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToProductMessage
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblApplicationStringToProductMessage]
@ApplicationStringToProductMessageGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToProductMessage].[ApplicationStringToProductMessageGuid],[map].[tblApplicationStringToProductMessage].[ApplicationStringGuid],[map].[tblApplicationStringToProductMessage].[ProductGuid],[map].[tblApplicationStringToProductMessage].[Sequence],[map].[tblApplicationStringToProductMessage].[CreatedDate],[map].[tblApplicationStringToProductMessage].[CreatedBy],[map].[tblApplicationStringToProductMessage].[UpdatedDate],[map].[tblApplicationStringToProductMessage].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToProductMessage]
            INNER JOIN [track].[tblApplicationStringToProductMessage] CT
                ON CT.PK_ApplicationStringToProductMessageGuid = [map].[tblApplicationStringToProductMessage].[ApplicationStringToProductMessageGuid]
        WHERE CT.PK_ApplicationStringToProductMessageGuid = @ApplicationStringToProductMessageGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
