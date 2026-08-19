-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPointAccessGroupToExposedPointSetting
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointAccessGroupToExposedPointSetting]
@PointAccessGroupToExposedSettingGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPointAccessGroupToExposedPointSetting].[PointAccessGroupToExposedSettingGuid],[map].[tblPointAccessGroupToExposedPointSetting].[PointAccessGroupGuid],[map].[tblPointAccessGroupToExposedPointSetting].[PointSettingGuid],[map].[tblPointAccessGroupToExposedPointSetting].[PropertyID],[map].[tblPointAccessGroupToExposedPointSetting].[View],[map].[tblPointAccessGroupToExposedPointSetting].[Modify],[map].[tblPointAccessGroupToExposedPointSetting].[CreatedDate],[map].[tblPointAccessGroupToExposedPointSetting].[CreatedBy],[map].[tblPointAccessGroupToExposedPointSetting].[UpdatedDate],[map].[tblPointAccessGroupToExposedPointSetting].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPointAccessGroupToExposedPointSetting]
            INNER JOIN [track].[tblPointAccessGroupToExposedPointSetting] CT
                ON CT.PK_PointAccessGroupToExposedSettingGuid = [map].[tblPointAccessGroupToExposedPointSetting].[PointAccessGroupToExposedSettingGuid]
        WHERE CT.PK_PointAccessGroupToExposedSettingGuid = @PointAccessGroupToExposedSettingGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
