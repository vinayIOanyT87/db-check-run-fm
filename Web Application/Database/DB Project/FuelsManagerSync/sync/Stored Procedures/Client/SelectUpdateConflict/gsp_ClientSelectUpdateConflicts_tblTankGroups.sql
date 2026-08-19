-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTankGroups
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTankGroups]
@TankGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTankGroups].[ID],[dbo].[tblTankGroups].[CreatedDate],[dbo].[tblTankGroups].[CreatedBy],[dbo].[tblTankGroups].[UpdatedDate],[dbo].[tblTankGroups].[UpdatedBy],[dbo].[tblTankGroups].[TankGroupGuid],[dbo].[tblTankGroups].[SiteGuid],[dbo].[tblTankGroups].[ProductGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTankGroups]
            INNER JOIN [track].[tblTankGroups] CT
                ON CT.PK_TankGroupGuid = [dbo].[tblTankGroups].[TankGroupGuid]
        WHERE CT.PK_TankGroupGuid = @TankGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
