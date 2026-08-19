-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblFuelCardLimit
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblFuelCardLimit]
@FuelCardLimitGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblFuelCardLimit].[FuelCardLimitGuid],[dbo].[tblFuelCardLimit].[ID],[dbo].[tblFuelCardLimit].[SiteGuid],[dbo].[tblFuelCardLimit].[CreatedBy],[dbo].[tblFuelCardLimit].[CreatedDate],[dbo].[tblFuelCardLimit].[UpdatedBy],[dbo].[tblFuelCardLimit].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblFuelCardLimit]
            INNER JOIN [track].[tblFuelCardLimit] CT
                ON CT.PK_FuelCardLimitGuid = [dbo].[tblFuelCardLimit].[FuelCardLimitGuid]
        WHERE CT.PK_FuelCardLimitGuid = @FuelCardLimitGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
