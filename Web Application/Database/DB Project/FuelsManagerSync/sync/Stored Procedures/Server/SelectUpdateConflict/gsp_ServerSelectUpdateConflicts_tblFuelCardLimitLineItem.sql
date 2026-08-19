-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblFuelCardLimitLineItem
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblFuelCardLimitLineItem]
@FuelCardLimitLineItemGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblFuelCardLimitLineItem].[FuelCardLimitLineItemGuid],[dbo].[tblFuelCardLimitLineItem].[FuelCardLimitGuid],[dbo].[tblFuelCardLimitLineItem].[Limit],[dbo].[tblFuelCardLimitLineItem].[Period],[dbo].[tblFuelCardLimitLineItem].[ProductGuid],[dbo].[tblFuelCardLimitLineItem].[ProductGroupApplicationStringGuid],[dbo].[tblFuelCardLimitLineItem].[CreatedBy],[dbo].[tblFuelCardLimitLineItem].[CreatedDate],[dbo].[tblFuelCardLimitLineItem].[UpdatedBy],[dbo].[tblFuelCardLimitLineItem].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblFuelCardLimitLineItem]
            INNER JOIN [track].[tblFuelCardLimitLineItem] CT
                ON CT.PK_FuelCardLimitLineItemGuid = [dbo].[tblFuelCardLimitLineItem].[FuelCardLimitLineItemGuid]
        WHERE CT.PK_FuelCardLimitLineItemGuid = @FuelCardLimitLineItemGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
