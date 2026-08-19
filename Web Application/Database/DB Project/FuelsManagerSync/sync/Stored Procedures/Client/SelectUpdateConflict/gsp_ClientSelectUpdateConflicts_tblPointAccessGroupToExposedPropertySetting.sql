-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPointAccessGroupToExposedPropertySetting
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPointAccessGroupToExposedPropertySetting]
@PointAccessGroupToExposedSettingGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPointAccessGroupToExposedPropertySetting].[PointAccessGroupToExposedSettingGuid],[map].[tblPointAccessGroupToExposedPropertySetting].[PointAccessGroupGuid],[map].[tblPointAccessGroupToExposedPropertySetting].[PointSettingGuid],[map].[tblPointAccessGroupToExposedPropertySetting].[PropertyID],[map].[tblPointAccessGroupToExposedPropertySetting].[View],[map].[tblPointAccessGroupToExposedPropertySetting].[Modify],[map].[tblPointAccessGroupToExposedPropertySetting].[CreatedDate],[map].[tblPointAccessGroupToExposedPropertySetting].[CreatedBy],[map].[tblPointAccessGroupToExposedPropertySetting].[UpdatedDate],[map].[tblPointAccessGroupToExposedPropertySetting].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPointAccessGroupToExposedPropertySetting]
            INNER JOIN [track].[tblPointAccessGroupToExposedPropertySetting] CT
                ON CT.PK_PointAccessGroupToExposedSettingGuid = [map].[tblPointAccessGroupToExposedPropertySetting].[PointAccessGroupToExposedSettingGuid]
        WHERE CT.PK_PointAccessGroupToExposedSettingGuid = @PointAccessGroupToExposedSettingGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
