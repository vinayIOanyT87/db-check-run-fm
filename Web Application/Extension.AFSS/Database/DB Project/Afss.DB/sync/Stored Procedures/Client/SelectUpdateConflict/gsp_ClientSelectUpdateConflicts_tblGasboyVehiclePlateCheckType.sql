-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyVehiclePlateCheckType
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGasboyVehiclePlateCheckType]
@GasboyVehiclePlateCheckTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblGasboyVehiclePlateCheckType].[GasboyVehiclePlateCheckTypeIndex],[lookup].[tblGasboyVehiclePlateCheckType].[GasboyVehiclePlateCheckTypeCode],[lookup].[tblGasboyVehiclePlateCheckType].[GasboyVehiclePlateCheckTypeName],[lookup].[tblGasboyVehiclePlateCheckType].[GasboyVehiclePlateCheckTypeGuid],[lookup].[tblGasboyVehiclePlateCheckType].[CreatedBy],[lookup].[tblGasboyVehiclePlateCheckType].[CreatedDate],[lookup].[tblGasboyVehiclePlateCheckType].[UpdatedBy],[lookup].[tblGasboyVehiclePlateCheckType].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblGasboyVehiclePlateCheckType]
            INNER JOIN [track].[tblGasboyVehiclePlateCheckType] CT
                ON CT.PK_GasboyVehiclePlateCheckTypeIndex = [lookup].[tblGasboyVehiclePlateCheckType].[GasboyVehiclePlateCheckTypeIndex]
        WHERE CT.PK_GasboyVehiclePlateCheckTypeIndex = @GasboyVehiclePlateCheckTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END