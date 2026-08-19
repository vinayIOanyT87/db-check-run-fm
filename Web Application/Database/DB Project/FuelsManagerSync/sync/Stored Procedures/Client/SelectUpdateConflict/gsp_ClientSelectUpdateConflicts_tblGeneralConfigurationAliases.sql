-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGeneralConfigurationAliases
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGeneralConfigurationAliases]
@GeneralConfigurationAliasGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblGeneralConfigurationAliases].[AliasID],[dbo].[tblGeneralConfigurationAliases].[CreatedBy],[dbo].[tblGeneralConfigurationAliases].[CreatedDate],[dbo].[tblGeneralConfigurationAliases].[UpdatedBy],[dbo].[tblGeneralConfigurationAliases].[UpdatedDate],[dbo].[tblGeneralConfigurationAliases].[GeneralConfigurationAliasGuid],[dbo].[tblGeneralConfigurationAliases].[GeneralConfigurationGuid],[dbo].[tblGeneralConfigurationAliases].[TransactionAliasGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblGeneralConfigurationAliases]
            INNER JOIN [track].[tblGeneralConfigurationAliases] CT
                ON CT.PK_GeneralConfigurationAliasGuid = [dbo].[tblGeneralConfigurationAliases].[GeneralConfigurationAliasGuid]
        WHERE CT.PK_GeneralConfigurationAliasGuid = @GeneralConfigurationAliasGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
