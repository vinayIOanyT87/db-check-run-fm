-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPIDXProfiles
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPIDXProfiles]
@PIDXProfileGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPIDXProfiles].[Type],[dbo].[tblPIDXProfiles].[ID],[dbo].[tblPIDXProfiles].[IPAddress],[dbo].[tblPIDXProfiles].[Port],[dbo].[tblPIDXProfiles].[TerminalID],[dbo].[tblPIDXProfiles].[UserID],[dbo].[tblPIDXProfiles].[Password],[dbo].[tblPIDXProfiles].[Enabled],[dbo].[tblPIDXProfiles].[LoggingEnabled],[dbo].[tblPIDXProfiles].[LogFilePath],[dbo].[tblPIDXProfiles].[CreatedDate],[dbo].[tblPIDXProfiles].[CreatedBy],[dbo].[tblPIDXProfiles].[UpdatedDate],[dbo].[tblPIDXProfiles].[UpdatedBy],[dbo].[tblPIDXProfiles].[PIDXProfileGuid],[dbo].[tblPIDXProfiles].[SiteGuid],[dbo].[tblPIDXProfiles].[Version], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPIDXProfiles]
            INNER JOIN [track].[tblPIDXProfiles] CT
                ON CT.PK_PIDXProfileGuid = [dbo].[tblPIDXProfiles].[PIDXProfileGuid]
        WHERE CT.PK_PIDXProfileGuid = @PIDXProfileGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
