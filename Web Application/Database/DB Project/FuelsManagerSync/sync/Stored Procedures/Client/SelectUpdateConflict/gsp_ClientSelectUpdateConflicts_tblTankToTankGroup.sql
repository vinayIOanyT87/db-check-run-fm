-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblTankToTankGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTankToTankGroup]
@TankToTankGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblTankToTankGroup].[TankToTankGroupGuid],[map].[tblTankToTankGroup].[TankGuid],[map].[tblTankToTankGroup].[AssignedToTankGroupGuid],[map].[tblTankToTankGroup].[CreatedDate],[map].[tblTankToTankGroup].[CreatedBy],[map].[tblTankToTankGroup].[UpdatedDate],[map].[tblTankToTankGroup].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblTankToTankGroup]
            INNER JOIN [track].[tblTankToTankGroup] CT
                ON CT.PK_TankToTankGroupGuid = [map].[tblTankToTankGroup].[TankToTankGroupGuid]
        WHERE CT.PK_TankToTankGroupGuid = @TankToTankGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
