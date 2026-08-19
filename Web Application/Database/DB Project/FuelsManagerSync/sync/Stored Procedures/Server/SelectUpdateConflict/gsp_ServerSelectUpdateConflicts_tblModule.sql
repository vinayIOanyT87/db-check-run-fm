-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblModule
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblModule]
@ModuleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblModule].[ID],[dbo].[tblModule].[Description],[dbo].[tblModule].[Standard],[dbo].[tblModule].[ModuleCalculation],[dbo].[tblModule].[ModuleTypeName],[dbo].[tblModule].[ModuleData],[dbo].[tblModule].[ModuleScript],[dbo].[tblModule].[CreatedDate],[dbo].[tblModule].[CreatedBy],[dbo].[tblModule].[UpdatedDate],[dbo].[tblModule].[UpdatedBy],[dbo].[tblModule].[ModuleGuid],[dbo].[tblModule].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblModule]
            INNER JOIN [track].[tblModule] CT
                ON CT.PK_ModuleGuid = [dbo].[tblModule].[ModuleGuid]
        WHERE CT.PK_ModuleGuid = @ModuleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
