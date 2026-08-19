-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEmailTemplate
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEmailTemplate]
@EmailTemplateGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblEmailTemplate].[EmailTemplateGuid],[dbo].[tblEmailTemplate].[Subject],[dbo].[tblEmailTemplate].[Body],[dbo].[tblEmailTemplate].[CreatedDate],[dbo].[tblEmailTemplate].[CreatedBy],[dbo].[tblEmailTemplate].[UpdatedDate],[dbo].[tblEmailTemplate].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblEmailTemplate]
            INNER JOIN [track].[tblEmailTemplate] CT
                ON CT.PK_EmailTemplateGuid = [dbo].[tblEmailTemplate].[EmailTemplateGuid]
        WHERE CT.PK_EmailTemplateGuid = @EmailTemplateGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
