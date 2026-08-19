-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyHardwareType
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGasboyHardwareType]
@GasboyHardwareTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblGasboyHardwareType].[GasboyHardwareTypeIndex],[lookup].[tblGasboyHardwareType].[GasboyHardwareTypeCode],[lookup].[tblGasboyHardwareType].[GasboyHardwareTypeName],[lookup].[tblGasboyHardwareType].[GasboyHardwareTypeGuid],[lookup].[tblGasboyHardwareType].[CreatedBy],[lookup].[tblGasboyHardwareType].[CreatedDate],[lookup].[tblGasboyHardwareType].[UpdatedBy],[lookup].[tblGasboyHardwareType].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblGasboyHardwareType]
            INNER JOIN [track].[tblGasboyHardwareType] CT
                ON CT.PK_GasboyHardwareTypeIndex = [lookup].[tblGasboyHardwareType].[GasboyHardwareTypeIndex]
        WHERE CT.PK_GasboyHardwareTypeIndex = @GasboyHardwareTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END