-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblReserveLevels
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblReserveLevels]
@ReserveLevelGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblReserveLevels].[MinimumLevel],[dbo].[tblReserveLevels].[WarningLevel],[dbo].[tblReserveLevels].[ReserveLevelGuid],[dbo].[tblReserveLevels].[SiteGuid],[dbo].[tblReserveLevels].[ProductGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblReserveLevels]
            INNER JOIN [track].[tblReserveLevels] CT
                ON CT.PK_ReserveLevelGuid = [dbo].[tblReserveLevels].[ReserveLevelGuid]
        WHERE CT.PK_ReserveLevelGuid = @ReserveLevelGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
