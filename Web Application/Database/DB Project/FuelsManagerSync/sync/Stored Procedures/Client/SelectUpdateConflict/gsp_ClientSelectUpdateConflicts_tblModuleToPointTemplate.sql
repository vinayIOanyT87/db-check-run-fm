-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblModuleToPointTemplate
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblModuleToPointTemplate]
@ModuleToPointTemplateGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblModuleToPointTemplate].[ID],[map].[tblModuleToPointTemplate].[Order],[map].[tblModuleToPointTemplate].[ModuleToPointTemplateData],[map].[tblModuleToPointTemplate].[CreatedDate],[map].[tblModuleToPointTemplate].[CreatedBy],[map].[tblModuleToPointTemplate].[UpdatedDate],[map].[tblModuleToPointTemplate].[UpdatedBy],[map].[tblModuleToPointTemplate].[ModuleToPointTemplateGuid],[map].[tblModuleToPointTemplate].[PointTemplateGuid],[map].[tblModuleToPointTemplate].[ModuleGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblModuleToPointTemplate]
            INNER JOIN [track].[tblModuleToPointTemplate] CT
                ON CT.PK_ModuleToPointTemplateGuid = [map].[tblModuleToPointTemplate].[ModuleToPointTemplateGuid]
        WHERE CT.PK_ModuleToPointTemplateGuid = @ModuleToPointTemplateGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
