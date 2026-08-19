-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAppointmentPersonnel
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblAppointmentPersonnel]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@AppointmentPersonnelGuid uniqueidentifier,
@PersonnelGuid uniqueidentifier,
@TestSetDefinitionGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@AssetText nvarchar(100),
@AppointmentCategory nvarchar(50),
@AppointmentIsSingle bit,
@ScheduleOnWeekends bit,
@ScheduleOnHolidays bit,
@StartDate datetimeoffset(7),
@Duration int,
@AppointmentPeriod int,
@AppointmentPeriodText nvarchar(50),
@Description nvarchar(50),
@AppointmentTimeInterval int,
@AppointmentDayOfTheWeekText nvarchar(20),
@AppointmentDayOfTheWeek int,
@AppointmentReoccuranceInterval int,
@AppointmentOption2Selected bit,
@AppointmentTimeOptionSelectionText nvarchar(20),
@AppointmentTimeOptionSelection int,
@AppointmentMonthSelectionText nvarchar(20),
@AppointmentMonthSelection int,
@AppointmentDayOfTheMonth int,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblAppointmentPersonnel varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblAppointmentPersonnel] AS existingData
        USING (SELECT @AppointmentPersonnelGuid 'AppointmentPersonnelGuid',@PersonnelGuid 'PersonnelGuid',@TestSetDefinitionGuid 'TestSetDefinitionGuid',@SiteGuid 'SiteGuid',@AssetText 'AssetText',@AppointmentCategory 'AppointmentCategory',@AppointmentIsSingle 'AppointmentIsSingle',@ScheduleOnWeekends 'ScheduleOnWeekends',@ScheduleOnHolidays 'ScheduleOnHolidays',@StartDate 'StartDate',@Duration 'Duration',@AppointmentPeriod 'AppointmentPeriod',@AppointmentPeriodText 'AppointmentPeriodText',@Description 'Description',@AppointmentTimeInterval 'AppointmentTimeInterval',@AppointmentDayOfTheWeekText 'AppointmentDayOfTheWeekText',@AppointmentDayOfTheWeek 'AppointmentDayOfTheWeek',@AppointmentReoccuranceInterval 'AppointmentReoccuranceInterval',@AppointmentOption2Selected 'AppointmentOption2Selected',@AppointmentTimeOptionSelectionText 'AppointmentTimeOptionSelectionText',@AppointmentTimeOptionSelection 'AppointmentTimeOptionSelection',@AppointmentMonthSelectionText 'AppointmentMonthSelectionText',@AppointmentMonthSelection 'AppointmentMonthSelection',@AppointmentDayOfTheMonth 'AppointmentDayOfTheMonth',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy'
                ) AS remoteChanges ([AppointmentPersonnelGuid],[PersonnelGuid],[TestSetDefinitionGuid],[SiteGuid],[AssetText],[AppointmentCategory],[AppointmentIsSingle],[ScheduleOnWeekends],[ScheduleOnHolidays],[StartDate],[Duration],[AppointmentPeriod],[AppointmentPeriodText],[Description],[AppointmentTimeInterval],[AppointmentDayOfTheWeekText],[AppointmentDayOfTheWeek],[AppointmentReoccuranceInterval],[AppointmentOption2Selected],[AppointmentTimeOptionSelectionText],[AppointmentTimeOptionSelection],[AppointmentMonthSelectionText],[AppointmentMonthSelection],[AppointmentDayOfTheMonth],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
        ON (existingData.[AppointmentPersonnelGuid] = remoteChanges.[AppointmentPersonnelGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [PersonnelGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PersonnelGuid'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[PersonnelGuid] ELSE remoteChanges.[PersonnelGuid] END
                       ,[TestSetDefinitionGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestSetDefinitionGuid'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[TestSetDefinitionGuid] ELSE remoteChanges.[TestSetDefinitionGuid] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[AssetText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssetText'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AssetText] ELSE remoteChanges.[AssetText] END
                       ,[AppointmentCategory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentCategory'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentCategory] ELSE remoteChanges.[AppointmentCategory] END
                       ,[AppointmentIsSingle] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentIsSingle'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentIsSingle] ELSE remoteChanges.[AppointmentIsSingle] END
                       ,[ScheduleOnWeekends] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ScheduleOnWeekends'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[ScheduleOnWeekends] ELSE remoteChanges.[ScheduleOnWeekends] END
                       ,[ScheduleOnHolidays] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ScheduleOnHolidays'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[ScheduleOnHolidays] ELSE remoteChanges.[ScheduleOnHolidays] END
                       ,[StartDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StartDate'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[StartDate] ELSE remoteChanges.[StartDate] END
                       ,[Duration] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Duration'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[Duration] ELSE remoteChanges.[Duration] END
                       ,[AppointmentPeriod] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentPeriod'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentPeriod] ELSE remoteChanges.[AppointmentPeriod] END
                       ,[AppointmentPeriodText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentPeriodText'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentPeriodText] ELSE remoteChanges.[AppointmentPeriodText] END
                       ,[Description] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[Description] ELSE remoteChanges.[Description] END
                       ,[AppointmentTimeInterval] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentTimeInterval'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentTimeInterval] ELSE remoteChanges.[AppointmentTimeInterval] END
                       ,[AppointmentDayOfTheWeekText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentDayOfTheWeekText'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentDayOfTheWeekText] ELSE remoteChanges.[AppointmentDayOfTheWeekText] END
                       ,[AppointmentDayOfTheWeek] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentDayOfTheWeek'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentDayOfTheWeek] ELSE remoteChanges.[AppointmentDayOfTheWeek] END
                       ,[AppointmentReoccuranceInterval] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentReoccuranceInterval'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentReoccuranceInterval] ELSE remoteChanges.[AppointmentReoccuranceInterval] END
                       ,[AppointmentOption2Selected] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentOption2Selected'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentOption2Selected] ELSE remoteChanges.[AppointmentOption2Selected] END
                       ,[AppointmentTimeOptionSelectionText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentTimeOptionSelectionText'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentTimeOptionSelectionText] ELSE remoteChanges.[AppointmentTimeOptionSelectionText] END
                       ,[AppointmentTimeOptionSelection] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentTimeOptionSelection'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentTimeOptionSelection] ELSE remoteChanges.[AppointmentTimeOptionSelection] END
                       ,[AppointmentMonthSelectionText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentMonthSelectionText'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentMonthSelectionText] ELSE remoteChanges.[AppointmentMonthSelectionText] END
                       ,[AppointmentMonthSelection] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentMonthSelection'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentMonthSelection] ELSE remoteChanges.[AppointmentMonthSelection] END
                       ,[AppointmentDayOfTheMonth] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentDayOfTheMonth'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[AppointmentDayOfTheMonth] ELSE remoteChanges.[AppointmentDayOfTheMonth] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

        WHEN NOT MATCHED THEN
            INSERT ([AppointmentPersonnelGuid],[PersonnelGuid],[TestSetDefinitionGuid],[SiteGuid],[AssetText],[AppointmentCategory],[AppointmentIsSingle],[ScheduleOnWeekends],[ScheduleOnHolidays],[StartDate],[Duration],[AppointmentPeriod],[AppointmentPeriodText],[Description],[AppointmentTimeInterval],[AppointmentDayOfTheWeekText],[AppointmentDayOfTheWeek],[AppointmentReoccuranceInterval],[AppointmentOption2Selected],[AppointmentTimeOptionSelectionText],[AppointmentTimeOptionSelection],[AppointmentMonthSelectionText],[AppointmentMonthSelection],[AppointmentDayOfTheMonth],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                VALUES (@AppointmentPersonnelGuid,@PersonnelGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestSetDefinitionGuid'), @sync_supported_columns_tblAppointmentPersonnel)) WHEN 0 THEN NULL ELSE @TestSetDefinitionGuid END),@SiteGuid,@AssetText,@AppointmentCategory,@AppointmentIsSingle,@ScheduleOnWeekends,@ScheduleOnHolidays,@StartDate,@Duration,@AppointmentPeriod,@AppointmentPeriodText,@Description,@AppointmentTimeInterval,@AppointmentDayOfTheWeekText,@AppointmentDayOfTheWeek,@AppointmentReoccuranceInterval,@AppointmentOption2Selected,@AppointmentTimeOptionSelectionText,@AppointmentTimeOptionSelection,@AppointmentMonthSelectionText,@AppointmentMonthSelection,@AppointmentDayOfTheMonth,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AppointmentPersonnelGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AppointmentPersonnelGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AppointmentPersonnelGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblAppointmentPersonnel] WHERE AppointmentPersonnelGuid = @AppointmentPersonnelGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

