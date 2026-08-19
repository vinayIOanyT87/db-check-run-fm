-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblScheduleCompanyAccess
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblScheduleCompanyAccess]
@ScheduleCompanyAccessGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblScheduleCompanyAccess].[ScheduleCompanyAccessGuid],[dbo].[tblScheduleCompanyAccess].[CompanyGuid],[dbo].[tblScheduleCompanyAccess].[LookupDayOfWeekIndex],[dbo].[tblScheduleCompanyAccess].[Enabled],[dbo].[tblScheduleCompanyAccess].[OpeningTime],[dbo].[tblScheduleCompanyAccess].[ClosingTime],[dbo].[tblScheduleCompanyAccess].[EndOfDayEnabled],[dbo].[tblScheduleCompanyAccess].[EndOfDayTime],[dbo].[tblScheduleCompanyAccess].[CreatedDate],[dbo].[tblScheduleCompanyAccess].[CreatedBy],[dbo].[tblScheduleCompanyAccess].[UpdatedDate],[dbo].[tblScheduleCompanyAccess].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblScheduleCompanyAccess]
            INNER JOIN [track].[tblScheduleCompanyAccess] CT
                ON CT.PK_ScheduleCompanyAccessGuid = [dbo].[tblScheduleCompanyAccess].[ScheduleCompanyAccessGuid]
        WHERE CT.PK_ScheduleCompanyAccessGuid = @ScheduleCompanyAccessGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
