-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblCustomToolbarCommandType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCustomToolbarCommandType]
@CustomToolbarCommandTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblCustomToolbarCommandType].[CustomToolbarCommandTypeIndex],[lookup].[tblCustomToolbarCommandType].[CustomToolbarCommandTypeCode],[lookup].[tblCustomToolbarCommandType].[CustomToolbarCommandTypeName],[lookup].[tblCustomToolbarCommandType].[LookupCustomToolbarTypeIndex],[lookup].[tblCustomToolbarCommandType].[CustomToolbarCommandTypeGuid],[lookup].[tblCustomToolbarCommandType].[CreatedDate],[lookup].[tblCustomToolbarCommandType].[CreatedBy],[lookup].[tblCustomToolbarCommandType].[UpdatedDate],[lookup].[tblCustomToolbarCommandType].[UpdatedBy],[lookup].[tblCustomToolbarCommandType].[Default],[lookup].[tblCustomToolbarCommandType].[DefaultOrder],[lookup].[tblCustomToolbarCommandType].[ImageSource], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblCustomToolbarCommandType]
            INNER JOIN [track].[tblCustomToolbarCommandType] CT
                ON CT.PK_CustomToolbarCommandTypeIndex = [lookup].[tblCustomToolbarCommandType].[CustomToolbarCommandTypeIndex]
        WHERE CT.PK_CustomToolbarCommandTypeIndex = @CustomToolbarCommandTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
