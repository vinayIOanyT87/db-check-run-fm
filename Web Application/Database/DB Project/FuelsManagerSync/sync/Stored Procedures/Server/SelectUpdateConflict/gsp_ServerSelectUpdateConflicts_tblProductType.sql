-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblProductType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProductType]
@ProductTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblProductType].[ProductTypeIndex],[lookup].[tblProductType].[ProductTypeCode],[lookup].[tblProductType].[ProductTypeName],[lookup].[tblProductType].[ProductTypeGuid],[lookup].[tblProductType].[CreatedDate],[lookup].[tblProductType].[CreatedBy],[lookup].[tblProductType].[UpdatedDate],[lookup].[tblProductType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblProductType]
            INNER JOIN [track].[tblProductType] CT
                ON CT.PK_ProductTypeIndex = [lookup].[tblProductType].[ProductTypeIndex]
        WHERE CT.PK_ProductTypeIndex = @ProductTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
