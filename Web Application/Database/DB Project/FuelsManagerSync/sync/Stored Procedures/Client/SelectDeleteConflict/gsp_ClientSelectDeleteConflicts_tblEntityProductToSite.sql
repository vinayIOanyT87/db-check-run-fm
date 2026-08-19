-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityProductToSite
-- Description: Select Delete Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectDeleteConflicts_tblEntityProductToSite]
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ProductToSiteGuid uniqueidentifier
AS
BEGIN
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    
    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    -- This command is used if the server provider cannot find
    -- a row in the base table.
    --
    SELECT CT.PK_ProductToSiteGuid 'ProductToSiteGuid', CT.DeletedContext, CT.DeletedRowVersion AS '_RowVersion'
        FROM [track].[tblEntityProductToSite] CT
        WHERE (CT.DeletedRowVersion > @sync_last_received_anchor_varbinary)
                AND (CT.DeletedDate IS NOT NULL)
                AND (CT.PK_ProductToSiteGuid = @ProductToSiteGuid)
        ORDER BY _RowVersion ASC
END
