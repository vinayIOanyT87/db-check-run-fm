-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : erv.tblGlobalSpecificChangesQueue
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGlobalSpecificChangesQueue]
@GSQueueGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [erv].[tblGlobalSpecificChangesQueue].[GSQueueGuid],[erv].[tblGlobalSpecificChangesQueue].[EntityTypeId],[erv].[tblGlobalSpecificChangesQueue].[EntityGuid],[erv].[tblGlobalSpecificChangesQueue].[MasterRecordGuid],[erv].[tblGlobalSpecificChangesQueue].[SiteGuid],[erv].[tblGlobalSpecificChangesQueue].[BatchProcessingMarker],[erv].[tblGlobalSpecificChangesQueue].[CreatedDate],[erv].[tblGlobalSpecificChangesQueue].[CreatedBy],[erv].[tblGlobalSpecificChangesQueue].[UpdatedDate],[erv].[tblGlobalSpecificChangesQueue].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [erv].[tblGlobalSpecificChangesQueue]
            INNER JOIN [track].[tblGlobalSpecificChangesQueue] CT
                ON CT.PK_GSQueueGuid = [erv].[tblGlobalSpecificChangesQueue].[GSQueueGuid]
        WHERE CT.PK_GSQueueGuid = @GSQueueGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
