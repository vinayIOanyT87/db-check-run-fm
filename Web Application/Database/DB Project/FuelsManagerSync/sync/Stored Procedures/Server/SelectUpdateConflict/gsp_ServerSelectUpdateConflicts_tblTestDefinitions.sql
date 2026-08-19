-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestDefinitions
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTestDefinitions]
@TestDefinitionGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTestDefinitions].[TestName],[dbo].[tblTestDefinitions].[MeasurementUnit],[dbo].[tblTestDefinitions].[ValidationRule],[dbo].[tblTestDefinitions].[SampleSize],[dbo].[tblTestDefinitions].[TestCode],[dbo].[tblTestDefinitions].[TestMethod],[dbo].[tblTestDefinitions].[ProductID],[dbo].[tblTestDefinitions].[DeleteFlag],[dbo].[tblTestDefinitions].[CreatedDate],[dbo].[tblTestDefinitions].[CreatedBy],[dbo].[tblTestDefinitions].[UpdatedDate],[dbo].[tblTestDefinitions].[UpdatedBy],[dbo].[tblTestDefinitions].[TestDefinitionGuid],[dbo].[tblTestDefinitions].[OwnerSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTestDefinitions]
            INNER JOIN [track].[tblTestDefinitions] CT
                ON CT.PK_TestDefinitionGuid = [dbo].[tblTestDefinitions].[TestDefinitionGuid]
        WHERE CT.PK_TestDefinitionGuid = @TestDefinitionGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
