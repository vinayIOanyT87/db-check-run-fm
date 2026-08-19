-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAuditLog
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAuditLog]
@AuditLogGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAuditLog].[SessionID],[dbo].[tblAuditLog].[ActionID],[dbo].[tblAuditLog].[TypeID],[dbo].[tblAuditLog].[ID],[dbo].[tblAuditLog].[PropertyID],[dbo].[tblAuditLog].[NewValue],[dbo].[tblAuditLog].[OldValue],[dbo].[tblAuditLog].[CreatedDate],[dbo].[tblAuditLog].[CreatedBy],[dbo].[tblAuditLog].[ParentTypeID],[dbo].[tblAuditLog].[AuditLogGuid],[dbo].[tblAuditLog].[SiteGuid],[dbo].[tblAuditLog].[AuditedDate],[dbo].[tblAuditLog].[SourceNode],[dbo].[tblAuditLog].[AuditContext], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAuditLog]
            INNER JOIN [track].[tblAuditLog] CT
                ON CT.PK_AuditLogGuid = [dbo].[tblAuditLog].[AuditLogGuid]
        WHERE CT.PK_AuditLogGuid = @AuditLogGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
