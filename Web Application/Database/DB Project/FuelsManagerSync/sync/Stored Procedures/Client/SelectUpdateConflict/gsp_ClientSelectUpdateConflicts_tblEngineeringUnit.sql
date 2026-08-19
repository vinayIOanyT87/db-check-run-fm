-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblEngineeringUnit
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEngineeringUnit]
@EngineeringUnitIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblEngineeringUnit].[EngineeringUnitIndex],[lookup].[tblEngineeringUnit].[EngineeringUnitCode],[lookup].[tblEngineeringUnit].[EngineeringUnitName],[lookup].[tblEngineeringUnit].[EngineeringUnitAbbreviation],[lookup].[tblEngineeringUnit].[EngineeringUnitGuid],[lookup].[tblEngineeringUnit].[CreatedDate],[lookup].[tblEngineeringUnit].[CreatedBy],[lookup].[tblEngineeringUnit].[UpdatedDate],[lookup].[tblEngineeringUnit].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblEngineeringUnit]
            INNER JOIN [track].[tblEngineeringUnit] CT
                ON CT.PK_EngineeringUnitIndex = [lookup].[tblEngineeringUnit].[EngineeringUnitIndex]
        WHERE CT.PK_EngineeringUnitIndex = @EngineeringUnitIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
