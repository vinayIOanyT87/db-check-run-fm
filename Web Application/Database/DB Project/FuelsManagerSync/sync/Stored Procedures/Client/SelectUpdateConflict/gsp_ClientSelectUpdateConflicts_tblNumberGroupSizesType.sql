-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblNumberGroupSizesType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblNumberGroupSizesType]
@NumberGroupSizesTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblNumberGroupSizesType].[NumberGroupSizesTypeIndex],[lookup].[tblNumberGroupSizesType].[NumberGroupSizesTypeCode],[lookup].[tblNumberGroupSizesType].[NumberGroupSizesTypeName],[lookup].[tblNumberGroupSizesType].[NumberGroupSizesTypeGuid],[lookup].[tblNumberGroupSizesType].[CreatedDate],[lookup].[tblNumberGroupSizesType].[CreatedBy],[lookup].[tblNumberGroupSizesType].[UpdatedDate],[lookup].[tblNumberGroupSizesType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblNumberGroupSizesType]
            INNER JOIN [track].[tblNumberGroupSizesType] CT
                ON CT.PK_NumberGroupSizesTypeIndex = [lookup].[tblNumberGroupSizesType].[NumberGroupSizesTypeIndex]
        WHERE CT.PK_NumberGroupSizesTypeIndex = @NumberGroupSizesTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
