-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAppointmentTank
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblAppointmentTank]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@AppointmentTankGuid uniqueidentifier,
@TankGuid uniqueidentifier,
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
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblAppointmentTank] AS existingData
        USING (SELECT @AppointmentTankGuid 'AppointmentTankGuid',@TankGuid 'TankGuid',@TestSetDefinitionGuid 'TestSetDefinitionGuid',@SiteGuid 'SiteGuid',@AssetText 'AssetText',@AppointmentCategory 'AppointmentCategory',@AppointmentIsSingle 'AppointmentIsSingle',@ScheduleOnWeekends 'ScheduleOnWeekends',@ScheduleOnHolidays 'ScheduleOnHolidays',@StartDate 'StartDate',@Duration 'Duration',@AppointmentPeriod 'AppointmentPeriod',@AppointmentPeriodText 'AppointmentPeriodText',@Description 'Description',@AppointmentTimeInterval 'AppointmentTimeInterval',@AppointmentDayOfTheWeekText 'AppointmentDayOfTheWeekText',@AppointmentDayOfTheWeek 'AppointmentDayOfTheWeek',@AppointmentReoccuranceInterval 'AppointmentReoccuranceInterval',@AppointmentOption2Selected 'AppointmentOption2Selected',@AppointmentTimeOptionSelectionText 'AppointmentTimeOptionSelectionText',@AppointmentTimeOptionSelection 'AppointmentTimeOptionSelection',@AppointmentMonthSelectionText 'AppointmentMonthSelectionText',@AppointmentMonthSelection 'AppointmentMonthSelection',@AppointmentDayOfTheMonth 'AppointmentDayOfTheMonth',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy'
                ) AS remoteChanges ([AppointmentTankGuid],[TankGuid],[TestSetDefinitionGuid],[SiteGuid],[AssetText],[AppointmentCategory],[AppointmentIsSingle],[ScheduleOnWeekends],[ScheduleOnHolidays],[StartDate],[Duration],[AppointmentPeriod],[AppointmentPeriodText],[Description],[AppointmentTimeInterval],[AppointmentDayOfTheWeekText],[AppointmentDayOfTheWeek],[AppointmentReoccuranceInterval],[AppointmentOption2Selected],[AppointmentTimeOptionSelectionText],[AppointmentTimeOptionSelection],[AppointmentMonthSelectionText],[AppointmentMonthSelection],[AppointmentDayOfTheMonth],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
        ON (existingData.[AppointmentTankGuid] = remoteChanges.[AppointmentTankGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [TankGuid] = remoteChanges.[TankGuid]
                       ,[TestSetDefinitionGuid] = remoteChanges.[TestSetDefinitionGuid]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[AssetText] = remoteChanges.[AssetText]
                       ,[AppointmentCategory] = remoteChanges.[AppointmentCategory]
                       ,[AppointmentIsSingle] = remoteChanges.[AppointmentIsSingle]
                       ,[ScheduleOnWeekends] = remoteChanges.[ScheduleOnWeekends]
                       ,[ScheduleOnHolidays] = remoteChanges.[ScheduleOnHolidays]
                       ,[StartDate] = remoteChanges.[StartDate]
                       ,[Duration] = remoteChanges.[Duration]
                       ,[AppointmentPeriod] = remoteChanges.[AppointmentPeriod]
                       ,[AppointmentPeriodText] = remoteChanges.[AppointmentPeriodText]
                       ,[Description] = remoteChanges.[Description]
                       ,[AppointmentTimeInterval] = remoteChanges.[AppointmentTimeInterval]
                       ,[AppointmentDayOfTheWeekText] = remoteChanges.[AppointmentDayOfTheWeekText]
                       ,[AppointmentDayOfTheWeek] = remoteChanges.[AppointmentDayOfTheWeek]
                       ,[AppointmentReoccuranceInterval] = remoteChanges.[AppointmentReoccuranceInterval]
                       ,[AppointmentOption2Selected] = remoteChanges.[AppointmentOption2Selected]
                       ,[AppointmentTimeOptionSelectionText] = remoteChanges.[AppointmentTimeOptionSelectionText]
                       ,[AppointmentTimeOptionSelection] = remoteChanges.[AppointmentTimeOptionSelection]
                       ,[AppointmentMonthSelectionText] = remoteChanges.[AppointmentMonthSelectionText]
                       ,[AppointmentMonthSelection] = remoteChanges.[AppointmentMonthSelection]
                       ,[AppointmentDayOfTheMonth] = remoteChanges.[AppointmentDayOfTheMonth]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]

        WHEN NOT MATCHED THEN
            INSERT ([AppointmentTankGuid],[TankGuid],[TestSetDefinitionGuid],[SiteGuid],[AssetText],[AppointmentCategory],[AppointmentIsSingle],[ScheduleOnWeekends],[ScheduleOnHolidays],[StartDate],[Duration],[AppointmentPeriod],[AppointmentPeriodText],[Description],[AppointmentTimeInterval],[AppointmentDayOfTheWeekText],[AppointmentDayOfTheWeek],[AppointmentReoccuranceInterval],[AppointmentOption2Selected],[AppointmentTimeOptionSelectionText],[AppointmentTimeOptionSelection],[AppointmentMonthSelectionText],[AppointmentMonthSelection],[AppointmentDayOfTheMonth],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                VALUES (@AppointmentTankGuid,@TankGuid,@TestSetDefinitionGuid,@SiteGuid,@AssetText,@AppointmentCategory,@AppointmentIsSingle,@ScheduleOnWeekends,@ScheduleOnHolidays,@StartDate,@Duration,@AppointmentPeriod,@AppointmentPeriodText,@Description,@AppointmentTimeInterval,@AppointmentDayOfTheWeekText,@AppointmentDayOfTheWeek,@AppointmentReoccuranceInterval,@AppointmentOption2Selected,@AppointmentTimeOptionSelectionText,@AppointmentTimeOptionSelection,@AppointmentMonthSelectionText,@AppointmentMonthSelection,@AppointmentDayOfTheMonth,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AppointmentTankGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AppointmentTankGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AppointmentTankGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblAppointmentTank] WHERE AppointmentTankGuid = @AppointmentTankGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
