-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblDayOfWeek
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblDayOfWeek]
@DayOfWeekIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblDayOfWeek].[DayOfWeekIndex],[lookup].[tblDayOfWeek].[DayOfWeekCode],[lookup].[tblDayOfWeek].[DayOfWeekName],[lookup].[tblDayOfWeek].[DayOfWeekGuid],[lookup].[tblDayOfWeek].[CreatedDate],[lookup].[tblDayOfWeek].[CreatedBy],[lookup].[tblDayOfWeek].[UpdatedDate],[lookup].[tblDayOfWeek].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblDayOfWeek]
            INNER JOIN [track].[tblDayOfWeek] CT
                ON CT.PK_DayOfWeekIndex = [lookup].[tblDayOfWeek].[DayOfWeekIndex]
        WHERE CT.PK_DayOfWeekIndex = @DayOfWeekIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
