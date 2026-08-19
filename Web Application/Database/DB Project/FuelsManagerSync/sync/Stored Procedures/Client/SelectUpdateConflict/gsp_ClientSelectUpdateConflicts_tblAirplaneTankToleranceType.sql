-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblAirplaneTankToleranceType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAirplaneTankToleranceType]
@TankToleranceTypeIndex smallint
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblAirplaneTankToleranceType].[TankToleranceTypeIndex],[lookup].[tblAirplaneTankToleranceType].[TankToleranceTypeCode],[lookup].[tblAirplaneTankToleranceType].[TankToleranceTypeName],[lookup].[tblAirplaneTankToleranceType].[TankToleranceTypeGuid],[lookup].[tblAirplaneTankToleranceType].[CreatedDate],[lookup].[tblAirplaneTankToleranceType].[CreatedBy],[lookup].[tblAirplaneTankToleranceType].[UpdatedDate],[lookup].[tblAirplaneTankToleranceType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblAirplaneTankToleranceType]
            INNER JOIN [track].[tblAirplaneTankToleranceType] CT
                ON CT.PK_TankToleranceTypeIndex = [lookup].[tblAirplaneTankToleranceType].[TankToleranceTypeIndex]
        WHERE CT.PK_TankToleranceTypeIndex = @TankToleranceTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
