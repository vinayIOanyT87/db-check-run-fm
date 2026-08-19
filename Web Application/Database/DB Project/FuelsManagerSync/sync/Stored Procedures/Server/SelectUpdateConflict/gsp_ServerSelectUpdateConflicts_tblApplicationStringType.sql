-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblApplicationStringType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblApplicationStringType]
@ApplicationStringTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblApplicationStringType].[ApplicationStringTypeIndex],[lookup].[tblApplicationStringType].[ApplicationStringTypeCode],[lookup].[tblApplicationStringType].[ApplicationStringTypeName],[lookup].[tblApplicationStringType].[ApplicationStringTypeGuid],[lookup].[tblApplicationStringType].[CreatedDate],[lookup].[tblApplicationStringType].[CreatedBy],[lookup].[tblApplicationStringType].[UpdatedDate],[lookup].[tblApplicationStringType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblApplicationStringType]
            INNER JOIN [track].[tblApplicationStringType] CT
                ON CT.PK_ApplicationStringTypeIndex = [lookup].[tblApplicationStringType].[ApplicationStringTypeIndex]
        WHERE CT.PK_ApplicationStringTypeIndex = @ApplicationStringTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
