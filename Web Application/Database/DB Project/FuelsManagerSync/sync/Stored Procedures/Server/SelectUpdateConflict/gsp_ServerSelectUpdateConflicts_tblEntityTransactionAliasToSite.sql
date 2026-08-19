-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityTransactionAliasToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityTransactionAliasToSite]
@TransactionAliasToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityTransactionAliasToSite].[TransactionAliasToSiteGuid],[map].[tblEntityTransactionAliasToSite].[TransactionAliasGuid],[map].[tblEntityTransactionAliasToSite].[SiteGuid],[map].[tblEntityTransactionAliasToSite].[CreatedDate],[map].[tblEntityTransactionAliasToSite].[CreatedBy],[map].[tblEntityTransactionAliasToSite].[UpdatedDate],[map].[tblEntityTransactionAliasToSite].[UpdatedBy],[map].[tblEntityTransactionAliasToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityTransactionAliasToSite]
            INNER JOIN [track].[tblEntityTransactionAliasToSite] CT
                ON CT.PK_TransactionAliasToSiteGuid = [map].[tblEntityTransactionAliasToSite].[TransactionAliasToSiteGuid]
        WHERE CT.PK_TransactionAliasToSiteGuid = @TransactionAliasToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
