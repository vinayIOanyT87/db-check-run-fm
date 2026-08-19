-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyEventObjectType
-- Description:	Select Delete Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectDeleteConflicts_tblGasboyEventObjectType]
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@GasboyEventObjectTypeIndex int
AS
BEGIN
    -- This command is used if the server provider cannot find
    -- a row in the base table.
    --
    SELECT CT.PK_GasboyEventObjectTypeIndex 'GasboyEventObjectTypeIndex', CT.DeletedContext, CT.DeletedRowVersion AS '_RowVersion'
        FROM [track].[tblGasboyEventObjectType] CT
        WHERE (CT.DeletedRowVersion > @sync_last_received_anchor)
                AND (CT.DeletedDate IS NOT NULL)
                AND (CT.PK_GasboyEventObjectTypeIndex = @GasboyEventObjectTypeIndex)
    ORDER BY _RowVersion ASC
END