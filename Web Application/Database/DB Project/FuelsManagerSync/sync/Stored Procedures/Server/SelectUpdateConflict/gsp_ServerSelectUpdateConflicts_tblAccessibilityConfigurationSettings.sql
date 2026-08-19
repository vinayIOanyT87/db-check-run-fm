-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAccessibilityConfigurationSettings
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAccessibilityConfigurationSettings]
@AccessibilityConfigurationSettingGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAccessibilityConfigurationSettings].[AccessibilityConfigurationSettingGuid],[dbo].[tblAccessibilityConfigurationSettings].[AccessibilityGuid],[dbo].[tblAccessibilityConfigurationSettings].[UserGuid],[dbo].[tblAccessibilityConfigurationSettings].[SettingValue],[dbo].[tblAccessibilityConfigurationSettings].[CreatedDate],[dbo].[tblAccessibilityConfigurationSettings].[CreatedBy],[dbo].[tblAccessibilityConfigurationSettings].[UpdatedDate],[dbo].[tblAccessibilityConfigurationSettings].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAccessibilityConfigurationSettings]
            INNER JOIN [track].[tblAccessibilityConfigurationSettings] CT
                ON CT.PK_AccessibilityConfigurationSettingGuid = [dbo].[tblAccessibilityConfigurationSettings].[AccessibilityConfigurationSettingGuid]
        WHERE CT.PK_AccessibilityConfigurationSettingGuid = @AccessibilityConfigurationSettingGuid
    ORDER BY CT.UpdatedRowVersion ASC
END