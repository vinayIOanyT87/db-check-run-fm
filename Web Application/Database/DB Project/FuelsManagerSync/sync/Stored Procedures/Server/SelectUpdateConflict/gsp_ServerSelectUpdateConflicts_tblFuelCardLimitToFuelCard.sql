-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblFuelCardLimitToFuelCard
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblFuelCardLimitToFuelCard]
@FuelCardLimitToFuelCardGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblFuelCardLimitToFuelCard].[FuelCardLimitToFuelCardGuid],[map].[tblFuelCardLimitToFuelCard].[FuelCardLimitGuid],[map].[tblFuelCardLimitToFuelCard].[FuelCardGuid],[map].[tblFuelCardLimitToFuelCard].[CreatedBy],[map].[tblFuelCardLimitToFuelCard].[CreatedDate],[map].[tblFuelCardLimitToFuelCard].[UpdatedBy],[map].[tblFuelCardLimitToFuelCard].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblFuelCardLimitToFuelCard]
            INNER JOIN [track].[tblFuelCardLimitToFuelCard] CT
                ON CT.PK_FuelCardLimitToFuelCardGuid = [map].[tblFuelCardLimitToFuelCard].[FuelCardLimitToFuelCardGuid]
        WHERE CT.PK_FuelCardLimitToFuelCardGuid = @FuelCardLimitToFuelCardGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
