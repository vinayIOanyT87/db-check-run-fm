-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblAirplaneTankLocation
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAirplaneTankLocation]
@TankLocationIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblAirplaneTankLocation].[TankLocationIndex],[lookup].[tblAirplaneTankLocation].[TankLocationCode],[lookup].[tblAirplaneTankLocation].[TankLocationName],[lookup].[tblAirplaneTankLocation].[TankLocationGuid],[lookup].[tblAirplaneTankLocation].[CreatedDate],[lookup].[tblAirplaneTankLocation].[CreatedBy],[lookup].[tblAirplaneTankLocation].[UpdatedDate],[lookup].[tblAirplaneTankLocation].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblAirplaneTankLocation]
            INNER JOIN [track].[tblAirplaneTankLocation] CT
                ON CT.PK_TankLocationIndex = [lookup].[tblAirplaneTankLocation].[TankLocationIndex]
        WHERE CT.PK_TankLocationIndex = @TankLocationIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
