-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblListViewFields
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblListViewFields]
@ListViewFieldGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblListViewFields].[ColumnOrder],[dbo].[tblListViewFields].[CreatedDate],[dbo].[tblListViewFields].[CreatedBy],[dbo].[tblListViewFields].[UpdatedDate],[dbo].[tblListViewFields].[UpdatedBy],[dbo].[tblListViewFields].[ListViewID],[dbo].[tblListViewFields].[ListViewFieldGuid],[dbo].[tblListViewFields].[LookupListViewFieldTypeIndex],[dbo].[tblListViewFields].[LookupStandardFieldTypeIndex],[dbo].[tblListViewFields].[ListViewGuid],[dbo].[tblListViewFields].[TransactionAliasGuid],[dbo].[tblListViewFields].[TransactionAliasFieldGuid],[dbo].[tblListViewFields].[UserDataFieldTransactionAliasGuid],[dbo].[tblListViewFields].[UserDataFieldTransactionAliasLineItemGuid],[dbo].[tblListViewFields].[LedgerAggregateColumnGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblListViewFields]
            INNER JOIN [track].[tblListViewFields] CT
                ON CT.PK_ListViewFieldGuid = [dbo].[tblListViewFields].[ListViewFieldGuid]
        WHERE CT.PK_ListViewFieldGuid = @ListViewFieldGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
