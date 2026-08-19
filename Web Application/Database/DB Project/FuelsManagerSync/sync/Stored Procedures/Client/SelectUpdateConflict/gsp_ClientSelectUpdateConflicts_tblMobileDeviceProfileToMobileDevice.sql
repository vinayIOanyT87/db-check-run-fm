-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblMobileDeviceProfileToMobileDevice
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMobileDeviceProfileToMobileDevice]
@MobileDeviceProfileToMobileDeviceGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblMobileDeviceProfileToMobileDevice].[MobileDeviceProfileToMobileDeviceGuid],[map].[tblMobileDeviceProfileToMobileDevice].[MobileDeviceProfileGuid],[map].[tblMobileDeviceProfileToMobileDevice].[AssignedToMobileDeviceGuid],[map].[tblMobileDeviceProfileToMobileDevice].[CreatedDate],[map].[tblMobileDeviceProfileToMobileDevice].[CreatedBy],[map].[tblMobileDeviceProfileToMobileDevice].[UpdatedDate],[map].[tblMobileDeviceProfileToMobileDevice].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblMobileDeviceProfileToMobileDevice]
            INNER JOIN [track].[tblMobileDeviceProfileToMobileDevice] CT
                ON CT.PK_MobileDeviceProfileToMobileDeviceGuid = [map].[tblMobileDeviceProfileToMobileDevice].[MobileDeviceProfileToMobileDeviceGuid]
        WHERE CT.PK_MobileDeviceProfileToMobileDeviceGuid = @MobileDeviceProfileToMobileDeviceGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
