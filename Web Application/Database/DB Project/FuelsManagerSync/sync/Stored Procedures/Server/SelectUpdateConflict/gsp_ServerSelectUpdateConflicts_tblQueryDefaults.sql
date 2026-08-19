-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblQueryDefaults
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblQueryDefaults]
@QueryDefaultGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblQueryDefaults].[Header],[dbo].[tblQueryDefaults].[Footer],[dbo].[tblQueryDefaults].[CreatedDate],[dbo].[tblQueryDefaults].[CreatedBy],[dbo].[tblQueryDefaults].[UpdatedDate],[dbo].[tblQueryDefaults].[UpdatedBy],[dbo].[tblQueryDefaults].[QueryDefaultGuid],[dbo].[tblQueryDefaults].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblQueryDefaults]
            INNER JOIN [track].[tblQueryDefaults] CT
                ON CT.PK_QueryDefaultGuid = [dbo].[tblQueryDefaults].[QueryDefaultGuid]
        WHERE CT.PK_QueryDefaultGuid = @QueryDefaultGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
