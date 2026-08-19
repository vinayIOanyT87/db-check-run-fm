-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblControllersLog
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblControllersLog]
@ControllersLogGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblControllersLog].[EventTime],[dbo].[tblControllersLog].[Controller],[dbo].[tblControllersLog].[Memo],[dbo].[tblControllersLog].[Deleted],[dbo].[tblControllersLog].[CreatedDate],[dbo].[tblControllersLog].[CreatedBy],[dbo].[tblControllersLog].[UpdatedDate],[dbo].[tblControllersLog].[UpdatedBy],[dbo].[tblControllersLog].[ControllersLogGuid],[dbo].[tblControllersLog].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblControllersLog]
            INNER JOIN [track].[tblControllersLog] CT
                ON CT.PK_ControllersLogGuid = [dbo].[tblControllersLog].[ControllersLogGuid]
        WHERE CT.PK_ControllersLogGuid = @ControllersLogGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
