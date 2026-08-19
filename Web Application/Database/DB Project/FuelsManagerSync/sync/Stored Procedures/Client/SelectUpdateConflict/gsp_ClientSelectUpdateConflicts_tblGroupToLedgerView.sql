-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblGroupToLedgerView
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGroupToLedgerView]
@GroupToLedgerViewGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblGroupToLedgerView].[GroupToLedgerViewGuid],[map].[tblGroupToLedgerView].[GroupGuid],[map].[tblGroupToLedgerView].[ListViewGuid],[map].[tblGroupToLedgerView].[CreatedDate],[map].[tblGroupToLedgerView].[CreatedBy],[map].[tblGroupToLedgerView].[UpdatedDate],[map].[tblGroupToLedgerView].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblGroupToLedgerView]
            INNER JOIN [track].[tblGroupToLedgerView] CT
                ON CT.PK_GroupToLedgerViewGuid = [map].[tblGroupToLedgerView].[GroupToLedgerViewGuid]
        WHERE CT.PK_GroupToLedgerViewGuid = @GroupToLedgerViewGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
