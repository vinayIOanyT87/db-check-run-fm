-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblVariantType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblVariantType]
@VariantTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblVariantType].[VariantTypeIndex],[lookup].[tblVariantType].[CodeType],[lookup].[tblVariantType].[DatabaseType],[lookup].[tblVariantType].[VariantTypeGuid],[lookup].[tblVariantType].[CreatedDate],[lookup].[tblVariantType].[CreatedBy],[lookup].[tblVariantType].[UpdatedDate],[lookup].[tblVariantType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblVariantType]
            INNER JOIN [track].[tblVariantType] CT
                ON CT.PK_VariantTypeIndex = [lookup].[tblVariantType].[VariantTypeIndex]
        WHERE CT.PK_VariantTypeIndex = @VariantTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
