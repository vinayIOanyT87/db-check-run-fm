-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldFuelCard
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataFieldFuelCard]
@UserDataFieldFuelCardGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid],[dbo].[tblUserDataFieldFuelCard].[TransactionAliasGuid],[dbo].[tblUserDataFieldFuelCard].[SiteGuid],[dbo].[tblUserDataFieldFuelCard].[Number],[dbo].[tblUserDataFieldFuelCard].[DisplayOrder],[dbo].[tblUserDataFieldFuelCard].[DisplayName],[dbo].[tblUserDataFieldFuelCard].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldFuelCard].[Required],[dbo].[tblUserDataFieldFuelCard].[UserGroupGuid],[dbo].[tblUserDataFieldFuelCard].[CreatedDate],[dbo].[tblUserDataFieldFuelCard].[CreatedBy],[dbo].[tblUserDataFieldFuelCard].[UpdatedDate],[dbo].[tblUserDataFieldFuelCard].[UpdatedBy],[dbo].[tblUserDataFieldFuelCard].[DispatchField],[dbo].[tblUserDataFieldFuelCard].[ClearOnNew],[dbo].[tblUserDataFieldFuelCard].[ReadOnly],[dbo].[tblUserDataFieldFuelCard].[Visibility],[dbo].[tblUserDataFieldFuelCard].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataFieldFuelCard]
            INNER JOIN [track].[tblUserDataFieldFuelCard] CT
                ON CT.PK_UserDataFieldFuelCardGuid = [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid]
        WHERE CT.PK_UserDataFieldFuelCardGuid = @UserDataFieldFuelCardGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
