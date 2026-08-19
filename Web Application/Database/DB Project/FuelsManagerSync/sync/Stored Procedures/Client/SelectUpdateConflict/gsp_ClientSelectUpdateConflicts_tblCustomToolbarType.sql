-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblCustomToolbarType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblCustomToolbarType]
@CustomToolbarTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblCustomToolbarType].[CustomToolbarTypeIndex],[lookup].[tblCustomToolbarType].[CustomToolbarTypeCode],[lookup].[tblCustomToolbarType].[CustomToolbarTypeName],[lookup].[tblCustomToolbarType].[CustomToolbarTypeGuid],[lookup].[tblCustomToolbarType].[CreatedDate],[lookup].[tblCustomToolbarType].[CreatedBy],[lookup].[tblCustomToolbarType].[UpdatedDate],[lookup].[tblCustomToolbarType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblCustomToolbarType]
            INNER JOIN [track].[tblCustomToolbarType] CT
                ON CT.PK_CustomToolbarTypeIndex = [lookup].[tblCustomToolbarType].[CustomToolbarTypeIndex]
        WHERE CT.PK_CustomToolbarTypeIndex = @CustomToolbarTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
