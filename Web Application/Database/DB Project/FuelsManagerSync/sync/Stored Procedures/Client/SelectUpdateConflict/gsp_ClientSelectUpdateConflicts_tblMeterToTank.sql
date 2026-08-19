-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblMeterToTank
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMeterToTank]
@MeterToTankGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblMeterToTank].[MeterToTankGuid],[map].[tblMeterToTank].[MeterGuid],[map].[tblMeterToTank].[TankGuid],[map].[tblMeterToTank].[CreatedDate],[map].[tblMeterToTank].[CreatedBy],[map].[tblMeterToTank].[UpdatedDate],[map].[tblMeterToTank].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblMeterToTank]
            INNER JOIN [track].[tblMeterToTank] CT
                ON CT.PK_MeterToTankGuid = [map].[tblMeterToTank].[MeterToTankGuid]
        WHERE CT.PK_MeterToTankGuid = @MeterToTankGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
