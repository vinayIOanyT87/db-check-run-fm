-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityGasboyDeviceToSite
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityGasboyDeviceToSite]
@GasboyDeviceToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityGasboyDeviceToSite].[GasboyDeviceToSiteGuid],[map].[tblEntityGasboyDeviceToSite].[OwnerSiteGuid],[map].[tblEntityGasboyDeviceToSite].[MapToSiteGuid],[map].[tblEntityGasboyDeviceToSite].[AssignedFromSiteGuid],[map].[tblEntityGasboyDeviceToSite].[CreatedBy],[map].[tblEntityGasboyDeviceToSite].[CreatedDate],[map].[tblEntityGasboyDeviceToSite].[UpdatedBy],[map].[tblEntityGasboyDeviceToSite].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityGasboyDeviceToSite]
            INNER JOIN [track].[tblEntityGasboyDeviceToSite] CT
                ON CT.PK_GasboyDeviceToSiteGuid = [map].[tblEntityGasboyDeviceToSite].[GasboyDeviceToSiteGuid]
        WHERE CT.PK_GasboyDeviceToSiteGuid = @GasboyDeviceToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
