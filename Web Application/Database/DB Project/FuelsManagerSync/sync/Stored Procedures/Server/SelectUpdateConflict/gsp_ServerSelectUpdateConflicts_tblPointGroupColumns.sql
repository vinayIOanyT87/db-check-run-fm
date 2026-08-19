-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointGroupColumns
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointGroupColumns]
@PointGroupColumnsGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointGroupColumns].[PointGroupColumnsGuid],[dbo].[tblPointGroupColumns].[PointGroupGuid],[dbo].[tblPointGroupColumns].[ColumnsDefinition],[dbo].[tblPointGroupColumns].[FontSize],[dbo].[tblPointGroupColumns].[OwnerUserGuid],[dbo].[tblPointGroupColumns].[SiteGuid],[dbo].[tblPointGroupColumns].[CreatedDate],[dbo].[tblPointGroupColumns].[CreatedBy],[dbo].[tblPointGroupColumns].[UpdatedDate],[dbo].[tblPointGroupColumns].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointGroupColumns]
            INNER JOIN [track].[tblPointGroupColumns] CT
                ON CT.PK_PointGroupColumnsGuid = [dbo].[tblPointGroupColumns].[PointGroupColumnsGuid]
        WHERE CT.PK_PointGroupColumnsGuid = @PointGroupColumnsGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
