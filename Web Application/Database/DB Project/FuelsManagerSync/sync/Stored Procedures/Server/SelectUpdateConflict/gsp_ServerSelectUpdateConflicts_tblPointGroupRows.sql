-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointGroupRows
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointGroupRows]
@PointGroupRowsGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointGroupRows].[PointGroupRowsGuid],[dbo].[tblPointGroupRows].[PointGroupGuid],[dbo].[tblPointGroupRows].[RowsDefinition],[dbo].[tblPointGroupRows].[OwnerUserGuid],[dbo].[tblPointGroupRows].[SiteGuid],[dbo].[tblPointGroupRows].[CreatedDate],[dbo].[tblPointGroupRows].[CreatedBy],[dbo].[tblPointGroupRows].[UpdatedDate],[dbo].[tblPointGroupRows].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointGroupRows]
            INNER JOIN [track].[tblPointGroupRows] CT
                ON CT.PK_PointGroupRowsGuid = [dbo].[tblPointGroupRows].[PointGroupRowsGuid]
        WHERE CT.PK_PointGroupRowsGuid = @PointGroupRowsGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
