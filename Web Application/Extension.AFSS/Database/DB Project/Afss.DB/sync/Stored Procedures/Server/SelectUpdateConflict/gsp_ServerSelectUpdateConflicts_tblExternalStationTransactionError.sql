-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExternalStationTransactionError
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalStationTransactionError]
@ExternalStationTransactionErrorGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblExternalStationTransactionError].[ExternalStationTransactionErrorGuid],[dbo].[tblExternalStationTransactionError].[ExternalStationTransactionGuid],[dbo].[tblExternalStationTransactionError].[Error],[dbo].[tblExternalStationTransactionError].[CreatedBy],[dbo].[tblExternalStationTransactionError].[CreatedDate],[dbo].[tblExternalStationTransactionError].[UpdatedBy],[dbo].[tblExternalStationTransactionError].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExternalStationTransactionError]
            INNER JOIN [track].[tblExternalStationTransactionError] CT
                ON CT.PK_ExternalStationTransactionErrorGuid = [dbo].[tblExternalStationTransactionError].[ExternalStationTransactionErrorGuid]
        WHERE CT.PK_ExternalStationTransactionErrorGuid = @ExternalStationTransactionErrorGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
