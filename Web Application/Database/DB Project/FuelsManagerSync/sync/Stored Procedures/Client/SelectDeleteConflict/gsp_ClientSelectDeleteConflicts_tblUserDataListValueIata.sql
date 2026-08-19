-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueIata
-- Description:	Select Delete Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectDeleteConflicts_tblUserDataListValueIata]
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@UserDataListValueIataGuid uniqueidentifier
AS
BEGIN
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    -- This command is used if the server provider cannot find
    -- a row in the base table.
    --
    SELECT CT.PK_UserDataListValueIataGuid 'UserDataListValueIataGuid', CT.DeletedContext, CT.DeletedRowVersion AS '_RowVersion'
        FROM [track].[tblUserDataListValueIata] CT
        WHERE (CT.DeletedRowVersion > @sync_last_received_anchor_varbinary)
                AND (CT.DeletedDate IS NOT NULL)
                AND (CT.PK_UserDataListValueIataGuid = @UserDataListValueIataGuid)
    ORDER BY _RowVersion ASC
END
