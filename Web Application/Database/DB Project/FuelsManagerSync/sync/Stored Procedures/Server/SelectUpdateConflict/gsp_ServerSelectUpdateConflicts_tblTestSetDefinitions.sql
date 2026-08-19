-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestSetDefinitions
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTestSetDefinitions]
@TestSetDefinitionGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTestSetDefinitions].[TestSetName],[dbo].[tblTestSetDefinitions].[DeleteFlag],[dbo].[tblTestSetDefinitions].[CreatedDate],[dbo].[tblTestSetDefinitions].[CreatedBy],[dbo].[tblTestSetDefinitions].[UpdatedDate],[dbo].[tblTestSetDefinitions].[UpdatedBy],[dbo].[tblTestSetDefinitions].[Flag01],[dbo].[tblTestSetDefinitions].[TestSetDefinitionGuid],[dbo].[tblTestSetDefinitions].[OwnerSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTestSetDefinitions]
            INNER JOIN [track].[tblTestSetDefinitions] CT
                ON CT.PK_TestSetDefinitionGuid = [dbo].[tblTestSetDefinitions].[TestSetDefinitionGuid]
        WHERE CT.PK_TestSetDefinitionGuid = @TestSetDefinitionGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
