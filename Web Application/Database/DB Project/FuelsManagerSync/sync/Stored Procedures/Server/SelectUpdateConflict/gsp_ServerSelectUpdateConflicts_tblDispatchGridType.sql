-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblDispatchGridType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblDispatchGridType]
@DispatchGridTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblDispatchGridType].[DispatchGridTypeIndex],[lookup].[tblDispatchGridType].[DispatchGridTypeCode],[lookup].[tblDispatchGridType].[DispatchGridTypeName],[lookup].[tblDispatchGridType].[DispatchGridTypeGuid],[lookup].[tblDispatchGridType].[CreatedDate],[lookup].[tblDispatchGridType].[CreatedBy],[lookup].[tblDispatchGridType].[UpdatedDate],[lookup].[tblDispatchGridType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblDispatchGridType]
            INNER JOIN [track].[tblDispatchGridType] CT
                ON CT.PK_DispatchGridTypeIndex = [lookup].[tblDispatchGridType].[DispatchGridTypeIndex]
        WHERE CT.PK_DispatchGridTypeIndex = @DispatchGridTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
