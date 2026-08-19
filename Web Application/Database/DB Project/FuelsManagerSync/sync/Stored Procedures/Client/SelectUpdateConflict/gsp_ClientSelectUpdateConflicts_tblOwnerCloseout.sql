-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblOwnerCloseout
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblOwnerCloseout]
@OwnerCloseoutGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblOwnerCloseout].[Site],[dbo].[tblOwnerCloseout].[ManagerName],[dbo].[tblOwnerCloseout].[ProductName],CONVERT(CHAR(10), [dbo].[tblOwnerCloseout].[CloseoutDate], 111) AS [CloseoutDate],[dbo].[tblOwnerCloseout].[OwnerName],[dbo].[tblOwnerCloseout].[GrossBookInventory],[dbo].[tblOwnerCloseout].[NetBookInventory],[dbo].[tblOwnerCloseout].[CreatedDate],[dbo].[tblOwnerCloseout].[CreatedBy],[dbo].[tblOwnerCloseout].[UpdatedDate],[dbo].[tblOwnerCloseout].[UpdatedBy],[dbo].[tblOwnerCloseout].[GrossBookPrice],[dbo].[tblOwnerCloseout].[NetBookPrice],[dbo].[tblOwnerCloseout].[TransVersion],[dbo].[tblOwnerCloseout].[MassBookInventory],[dbo].[tblOwnerCloseout].[MassBookPrice],[dbo].[tblOwnerCloseout].[OwnerCloseoutGuid],[dbo].[tblOwnerCloseout].[SiteGuid],[dbo].[tblOwnerCloseout].[ManagerCompanyGuid],[dbo].[tblOwnerCloseout].[OwnerCompanyGuid],[dbo].[tblOwnerCloseout].[ProductGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblOwnerCloseout]
            INNER JOIN [track].[tblOwnerCloseout] CT
                ON CT.PK_OwnerCloseoutGuid = [dbo].[tblOwnerCloseout].[OwnerCloseoutGuid]
        WHERE CT.PK_OwnerCloseoutGuid = @OwnerCloseoutGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
