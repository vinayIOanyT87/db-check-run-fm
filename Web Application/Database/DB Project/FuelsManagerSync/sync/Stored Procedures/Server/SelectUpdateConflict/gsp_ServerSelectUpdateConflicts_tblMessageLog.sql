-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMessageLog
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblMessageLog]
@MessageLogGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblMessageLog].[CreatedDate],[dbo].[tblMessageLog].[CreatedBy],[dbo].[tblMessageLog].[MessageLogGuid],[dbo].[tblMessageLog].[CompanyGuid],[dbo].[tblMessageLog].[MessageGuid],[dbo].[tblMessageLog].[PersonnelGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblMessageLog]
            INNER JOIN [track].[tblMessageLog] CT
                ON CT.PK_MessageLogGuid = [dbo].[tblMessageLog].[MessageLogGuid]
        WHERE CT.PK_MessageLogGuid = @MessageLogGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
