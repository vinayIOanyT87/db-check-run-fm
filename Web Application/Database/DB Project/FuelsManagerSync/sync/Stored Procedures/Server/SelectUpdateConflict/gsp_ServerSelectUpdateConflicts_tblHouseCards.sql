-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblHouseCards
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblHouseCards]
@HouseCardGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblHouseCards].[ID],[dbo].[tblHouseCards].[Number],[dbo].[tblHouseCards].[CreatedDate],[dbo].[tblHouseCards].[CreatedBy],[dbo].[tblHouseCards].[UpdatedDate],[dbo].[tblHouseCards].[UpdatedBy],[dbo].[tblHouseCards].[HouseCardGuid],[dbo].[tblHouseCards].[SiteGuid],[dbo].[tblHouseCards].[DriverPersonnelGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblHouseCards]
            INNER JOIN [track].[tblHouseCards] CT
                ON CT.PK_HouseCardGuid = [dbo].[tblHouseCards].[HouseCardGuid]
        WHERE CT.PK_HouseCardGuid = @HouseCardGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
