-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblCloseoutInventory
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblCloseoutInventory]
@CloseoutInventoryGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblCloseoutInventory].[Site],CONVERT(CHAR(10), [dbo].[tblCloseoutInventory].[CloseoutDate], 111) AS [CloseoutDate],[dbo].[tblCloseoutInventory].[ProductName],[dbo].[tblCloseoutInventory].[ManagerName],[dbo].[tblCloseoutInventory].[GrossBookInventory],[dbo].[tblCloseoutInventory].[NetBookInventory],[dbo].[tblCloseoutInventory].[GrossPhysicalInventory],[dbo].[tblCloseoutInventory].[NetPhysicalInventory],[dbo].[tblCloseoutInventory].[GrossVariance],[dbo].[tblCloseoutInventory].[NetVariance],[dbo].[tblCloseoutInventory].[CreatedDate],[dbo].[tblCloseoutInventory].[CreatedBy],[dbo].[tblCloseoutInventory].[UpdatedDate],[dbo].[tblCloseoutInventory].[UpdatedBy],[dbo].[tblCloseoutInventory].[GrossBookPrice],[dbo].[tblCloseoutInventory].[NetBookPrice],[dbo].[tblCloseoutInventory].[GrossPhysicalPrice],[dbo].[tblCloseoutInventory].[NetPhysicalPrice],[dbo].[tblCloseoutInventory].[TransVersion],[dbo].[tblCloseoutInventory].[MassBookInventory],[dbo].[tblCloseoutInventory].[MassPhysicalInventory],[dbo].[tblCloseoutInventory].[MassVariance],[dbo].[tblCloseoutInventory].[MassBookPrice],[dbo].[tblCloseoutInventory].[MassPhysicalPrice],[dbo].[tblCloseoutInventory].[CloseoutInventoryGuid],[dbo].[tblCloseoutInventory].[SiteGuid],[dbo].[tblCloseoutInventory].[ManagerCompanyGuid],[dbo].[tblCloseoutInventory].[ProductGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblCloseoutInventory]
            INNER JOIN [track].[tblCloseoutInventory] CT
                ON CT.PK_CloseoutInventoryGuid = [dbo].[tblCloseoutInventory].[CloseoutInventoryGuid]
        WHERE CT.PK_CloseoutInventoryGuid = @CloseoutInventoryGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
