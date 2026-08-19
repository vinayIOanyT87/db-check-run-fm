-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAirplaneTank
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAirplaneTank]
@TankGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAirplaneTank].[Alias],[dbo].[tblAirplaneTank].[AirlineTankId],[dbo].[tblAirplaneTank].[Description],[dbo].[tblAirplaneTank].[Capacity],[dbo].[tblAirplaneTank].[Position],[dbo].[tblAirplaneTank].[Location],[dbo].[tblAirplaneTank].[DisplayOrder],[dbo].[tblAirplaneTank].[TankGuid],[dbo].[tblAirplaneTank].[EquipmentTypeGuid],[dbo].[tblAirplaneTank].[CreatedDate],[dbo].[tblAirplaneTank].[CreatedBy],[dbo].[tblAirplaneTank].[UpdatedDate],[dbo].[tblAirplaneTank].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAirplaneTank]
            INNER JOIN [track].[tblAirplaneTank] CT
                ON CT.PK_TankGuid = [dbo].[tblAirplaneTank].[TankGuid]
        WHERE CT.PK_TankGuid = @TankGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
