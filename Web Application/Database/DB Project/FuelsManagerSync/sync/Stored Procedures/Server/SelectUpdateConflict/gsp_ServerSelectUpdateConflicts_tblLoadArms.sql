-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblLoadArms
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblLoadArms]
@LoadArmGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblLoadArms].[LoadRackText],[dbo].[tblLoadArms].[Enabled],[dbo].[tblLoadArms].[SwingArm],[dbo].[tblLoadArms].[BayAArmNumber],[dbo].[tblLoadArms].[BayBArmNumber],[dbo].[tblLoadArms].[CreatedDate],[dbo].[tblLoadArms].[CreatedBy],[dbo].[tblLoadArms].[UpdatedDate],[dbo].[tblLoadArms].[UpdatedBy],[dbo].[tblLoadArms].[LoadArmGuid],[dbo].[tblLoadArms].[LookupPresetTypeIndex],[dbo].[tblLoadArms].[BayAStationGuid],[dbo].[tblLoadArms].[BayBStationGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblLoadArms]
            INNER JOIN [track].[tblLoadArms] CT
                ON CT.PK_LoadArmGuid = [dbo].[tblLoadArms].[LoadArmGuid]
        WHERE CT.PK_LoadArmGuid = @LoadArmGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
