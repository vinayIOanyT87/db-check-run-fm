-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblConfigurationSetting
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblConfigurationSetting]
@ConfigurationSettingGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblConfigurationSetting].[ConfigurationSettingGuid],[dbo].[tblConfigurationSetting].[KeyType],[dbo].[tblConfigurationSetting].[SettingKey],[dbo].[tblConfigurationSetting].[SettingValue],[dbo].[tblConfigurationSetting].[CreatedDate],[dbo].[tblConfigurationSetting].[CreatedBy],[dbo].[tblConfigurationSetting].[UpdatedDate],[dbo].[tblConfigurationSetting].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblConfigurationSetting]
            INNER JOIN [track].[tblConfigurationSetting] CT
                ON CT.PK_ConfigurationSettingGuid = [dbo].[tblConfigurationSetting].[ConfigurationSettingGuid]
        WHERE CT.PK_ConfigurationSettingGuid = @ConfigurationSettingGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
