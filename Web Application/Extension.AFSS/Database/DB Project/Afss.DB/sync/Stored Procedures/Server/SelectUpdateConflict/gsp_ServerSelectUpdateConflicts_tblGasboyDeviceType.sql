-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyDeviceType
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGasboyDeviceType]
@GasboyDeviceTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblGasboyDeviceType].[GasboyDeviceTypeIndex],[lookup].[tblGasboyDeviceType].[GasboyDeviceTypeCode],[lookup].[tblGasboyDeviceType].[GasboyDeviceTypeName],[lookup].[tblGasboyDeviceType].[GasboyDeviceTypeGuid],[lookup].[tblGasboyDeviceType].[CreatedBy],[lookup].[tblGasboyDeviceType].[CreatedDate],[lookup].[tblGasboyDeviceType].[UpdatedBy],[lookup].[tblGasboyDeviceType].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblGasboyDeviceType]
            INNER JOIN [track].[tblGasboyDeviceType] CT
                ON CT.PK_GasboyDeviceTypeIndex = [lookup].[tblGasboyDeviceType].[GasboyDeviceTypeIndex]
        WHERE CT.PK_GasboyDeviceTypeIndex = @GasboyDeviceTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END