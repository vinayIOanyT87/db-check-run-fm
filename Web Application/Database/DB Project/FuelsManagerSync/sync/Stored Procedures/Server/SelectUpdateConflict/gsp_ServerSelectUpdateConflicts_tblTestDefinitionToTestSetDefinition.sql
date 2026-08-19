-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblTestDefinitionToTestSetDefinition
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTestDefinitionToTestSetDefinition]
@TestDefinitionToTestSetDefinitionGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblTestDefinitionToTestSetDefinition].[TestDefinitionToTestSetDefinitionGuid],[map].[tblTestDefinitionToTestSetDefinition].[TestDefinitionGuid],[map].[tblTestDefinitionToTestSetDefinition].[TestSetDefinitionGuid],[map].[tblTestDefinitionToTestSetDefinition].[DeleteFlag],[map].[tblTestDefinitionToTestSetDefinition].[CreatedDate],[map].[tblTestDefinitionToTestSetDefinition].[CreatedBy],[map].[tblTestDefinitionToTestSetDefinition].[UpdatedDate],[map].[tblTestDefinitionToTestSetDefinition].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblTestDefinitionToTestSetDefinition]
            INNER JOIN [track].[tblTestDefinitionToTestSetDefinition] CT
                ON CT.PK_TestDefinitionToTestSetDefinitionGuid = [map].[tblTestDefinitionToTestSetDefinition].[TestDefinitionToTestSetDefinitionGuid]
        WHERE CT.PK_TestDefinitionToTestSetDefinitionGuid = @TestDefinitionToTestSetDefinitionGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
