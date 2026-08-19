-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMobileDevice
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMobileDevice]
@MobileDeviceGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblMobileDevice].[MobileDeviceGuid],[dbo].[tblMobileDevice].[SiteGuid],[dbo].[tblMobileDevice].[MobileDeviceId],[dbo].[tblMobileDevice].[Description],[dbo].[tblMobileDevice].[MobileDeviceType],[dbo].[tblMobileDevice].[CreatedDate],[dbo].[tblMobileDevice].[CreatedBy],[dbo].[tblMobileDevice].[UpdatedDate],[dbo].[tblMobileDevice].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblMobileDevice]
            INNER JOIN [track].[tblMobileDevice] CT
                ON CT.PK_MobileDeviceGuid = [dbo].[tblMobileDevice].[MobileDeviceGuid]
        WHERE CT.PK_MobileDeviceGuid = @MobileDeviceGuid
    ORDER BY CT.UpdatedRowVersion ASC
END