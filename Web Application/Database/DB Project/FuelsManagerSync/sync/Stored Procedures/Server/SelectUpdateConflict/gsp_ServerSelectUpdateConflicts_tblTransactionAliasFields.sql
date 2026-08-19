-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionAliasFields
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTransactionAliasFields]
@TransactionAliasFieldGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTransactionAliasFields].[AliasID],[dbo].[tblTransactionAliasFields].[DbName],[dbo].[tblTransactionAliasFields].[DisplayOrder],[dbo].[tblTransactionAliasFields].[DisplayName],[dbo].[tblTransactionAliasFields].[CreatedDate],[dbo].[tblTransactionAliasFields].[CreatedBy],[dbo].[tblTransactionAliasFields].[UpdatedDate],[dbo].[tblTransactionAliasFields].[UpdatedBy],[dbo].[tblTransactionAliasFields].[Required],[dbo].[tblTransactionAliasFields].[Virtual],[dbo].[tblTransactionAliasFields].[TransactionAliasFieldGuid],[dbo].[tblTransactionAliasFields].[LookupTransactionFieldTypeIndex],[dbo].[tblTransactionAliasFields].[TransactionAliasGuid],[dbo].[tblTransactionAliasFields].[UserGroupGuid],[dbo].[tblTransactionAliasFields].[DispatchField],[dbo].[tblTransactionAliasFields].[ClearOnNew],[dbo].[tblTransactionAliasFields].[ReadOnly],[dbo].[tblTransactionAliasFields].[Visibility],[dbo].[tblTransactionAliasFields].[DefaultValueType],[dbo].[tblTransactionAliasFields].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTransactionAliasFields]
            INNER JOIN [track].[tblTransactionAliasFields] CT
                ON CT.PK_TransactionAliasFieldGuid = [dbo].[tblTransactionAliasFields].[TransactionAliasFieldGuid]
        WHERE CT.PK_TransactionAliasFieldGuid = @TransactionAliasFieldGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
