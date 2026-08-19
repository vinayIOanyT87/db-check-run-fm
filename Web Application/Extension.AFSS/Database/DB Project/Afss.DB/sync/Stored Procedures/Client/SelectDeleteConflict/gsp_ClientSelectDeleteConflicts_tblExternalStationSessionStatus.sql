-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExternalStationSessionStatus
-- Description:	Select Delete Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectDeleteConflicts_tblExternalStationSessionStatus]
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ExternalStationSessionStatusIndex int
AS
BEGIN
    -- This command is used if the server provider cannot find
    -- a row in the base table.
    --
    SELECT CT.PK_ExternalStationSessionStatusIndex 'ExternalStationSessionStatusIndex', CT.DeletedContext, CT.DeletedRowVersion AS '_RowVersion'
        FROM [track].[tblExternalStationSessionStatus] CT
        WHERE (CT.DeletedRowVersion > @sync_last_received_anchor)
                AND (CT.DeletedDate IS NOT NULL)
                AND (CT.PK_ExternalStationSessionStatusIndex = @ExternalStationSessionStatusIndex)
    ORDER BY _RowVersion ASC
END