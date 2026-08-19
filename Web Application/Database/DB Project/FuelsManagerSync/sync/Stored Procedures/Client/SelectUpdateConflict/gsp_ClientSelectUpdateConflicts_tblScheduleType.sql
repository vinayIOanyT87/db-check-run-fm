-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblScheduleType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblScheduleType]
@ScheduleTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblScheduleType].[ScheduleTypeIndex],[lookup].[tblScheduleType].[ScheduleTypeCode],[lookup].[tblScheduleType].[ScheduleTypeName],[lookup].[tblScheduleType].[ScheduleTypeGuid],[lookup].[tblScheduleType].[CreatedDate],[lookup].[tblScheduleType].[CreatedBy],[lookup].[tblScheduleType].[UpdatedDate],[lookup].[tblScheduleType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblScheduleType]
            INNER JOIN [track].[tblScheduleType] CT
                ON CT.PK_ScheduleTypeIndex = [lookup].[tblScheduleType].[ScheduleTypeIndex]
        WHERE CT.PK_ScheduleTypeIndex = @ScheduleTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
