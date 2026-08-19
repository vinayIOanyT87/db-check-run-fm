-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAppointmentTank
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblAppointmentTank]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblAppointmentTank varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblAppointmentTank] CT
                        WHERE CT.PK_AppointmentTankGuid = @AppointmentTankGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblAppointmentTank].[AppointmentTankGuid],[dbo].[tblAppointmentTank].[TankGuid],[dbo].[tblAppointmentTank].[TestSetDefinitionGuid],[dbo].[tblAppointmentTank].[SiteGuid],[dbo].[tblAppointmentTank].[AssetText],[dbo].[tblAppointmentTank].[AppointmentCategory],[dbo].[tblAppointmentTank].[AppointmentIsSingle],[dbo].[tblAppointmentTank].[ScheduleOnWeekends],[dbo].[tblAppointmentTank].[ScheduleOnHolidays],[dbo].[tblAppointmentTank].[StartDate],[dbo].[tblAppointmentTank].[Duration],[dbo].[tblAppointmentTank].[AppointmentPeriod],[dbo].[tblAppointmentTank].[AppointmentPeriodText],[dbo].[tblAppointmentTank].[Description],[dbo].[tblAppointmentTank].[AppointmentTimeInterval],[dbo].[tblAppointmentTank].[AppointmentDayOfTheWeekText],[dbo].[tblAppointmentTank].[AppointmentDayOfTheWeek],[dbo].[tblAppointmentTank].[AppointmentReoccuranceInterval],[dbo].[tblAppointmentTank].[AppointmentOption2Selected],[dbo].[tblAppointmentTank].[AppointmentTimeOptionSelectionText],[dbo].[tblAppointmentTank].[AppointmentTimeOptionSelection],[dbo].[tblAppointmentTank].[AppointmentMonthSelectionText],[dbo].[tblAppointmentTank].[AppointmentMonthSelection],[dbo].[tblAppointmentTank].[AppointmentDayOfTheMonth],[dbo].[tblAppointmentTank].[CreatedDate],[dbo].[tblAppointmentTank].[CreatedBy],[dbo].[tblAppointmentTank].[UpdatedDate],[dbo].[tblAppointmentTank].[UpdatedBy]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblAppointmentTank]
                        INNER JOIN [track].[tblAppointmentTank] CT
                            ON CT.PK_AppointmentTankGuid = [dbo].[tblAppointmentTank].[AppointmentTankGuid] 
                    WHERE CT.PK_AppointmentTankGuid = @AppointmentTankGuid
            ) MERGE existingData
            USING (SELECT @AppointmentTankGuid,@TankGuid,@TestSetDefinitionGuid,@SiteGuid,@AssetText,@AppointmentCategory,@AppointmentIsSingle,@ScheduleOnWeekends,@ScheduleOnHolidays,@StartDate,@Duration,@AppointmentPeriod,@AppointmentPeriodText,@Description,@AppointmentTimeInterval,@AppointmentDayOfTheWeekText,@AppointmentDayOfTheWeek,@AppointmentReoccuranceInterval,@AppointmentOption2Selected,@AppointmentTimeOptionSelectionText,@AppointmentTimeOptionSelection,@AppointmentMonthSelectionText,@AppointmentMonthSelection,@AppointmentDayOfTheMonth,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy
                    ) AS remoteChanges ([AppointmentTankGuid],[TankGuid],[TestSetDefinitionGuid],[SiteGuid],[AssetText],[AppointmentCategory],[AppointmentIsSingle],[ScheduleOnWeekends],[ScheduleOnHolidays],[StartDate],[Duration],[AppointmentPeriod],[AppointmentPeriodText],[Description],[AppointmentTimeInterval],[AppointmentDayOfTheWeekText],[AppointmentDayOfTheWeek],[AppointmentReoccuranceInterval],[AppointmentOption2Selected],[AppointmentTimeOptionSelectionText],[AppointmentTimeOptionSelection],[AppointmentMonthSelectionText],[AppointmentMonthSelection],[AppointmentDayOfTheMonth],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
            ON (existingData.[AppointmentTankGuid] = remoteChanges.[AppointmentTankGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [TankGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankGuid'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[TankGuid] ELSE remoteChanges.[TankGuid] END
                       ,[TestSetDefinitionGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestSetDefinitionGuid'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[TestSetDefinitionGuid] ELSE remoteChanges.[TestSetDefinitionGuid] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[AssetText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssetText'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AssetText] ELSE remoteChanges.[AssetText] END
                       ,[AppointmentCategory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentCategory'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentCategory] ELSE remoteChanges.[AppointmentCategory] END
                       ,[AppointmentIsSingle] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentIsSingle'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentIsSingle] ELSE remoteChanges.[AppointmentIsSingle] END
                       ,[ScheduleOnWeekends] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ScheduleOnWeekends'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[ScheduleOnWeekends] ELSE remoteChanges.[ScheduleOnWeekends] END
                       ,[ScheduleOnHolidays] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ScheduleOnHolidays'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[ScheduleOnHolidays] ELSE remoteChanges.[ScheduleOnHolidays] END
                       ,[StartDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StartDate'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[StartDate] ELSE remoteChanges.[StartDate] END
                       ,[Duration] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Duration'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[Duration] ELSE remoteChanges.[Duration] END
                       ,[AppointmentPeriod] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentPeriod'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentPeriod] ELSE remoteChanges.[AppointmentPeriod] END
                       ,[AppointmentPeriodText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentPeriodText'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentPeriodText] ELSE remoteChanges.[AppointmentPeriodText] END
                       ,[Description] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[Description] ELSE remoteChanges.[Description] END
                       ,[AppointmentTimeInterval] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentTimeInterval'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentTimeInterval] ELSE remoteChanges.[AppointmentTimeInterval] END
                       ,[AppointmentDayOfTheWeekText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentDayOfTheWeekText'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentDayOfTheWeekText] ELSE remoteChanges.[AppointmentDayOfTheWeekText] END
                       ,[AppointmentDayOfTheWeek] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentDayOfTheWeek'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentDayOfTheWeek] ELSE remoteChanges.[AppointmentDayOfTheWeek] END
                       ,[AppointmentReoccuranceInterval] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentReoccuranceInterval'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentReoccuranceInterval] ELSE remoteChanges.[AppointmentReoccuranceInterval] END
                       ,[AppointmentOption2Selected] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentOption2Selected'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentOption2Selected] ELSE remoteChanges.[AppointmentOption2Selected] END
                       ,[AppointmentTimeOptionSelectionText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentTimeOptionSelectionText'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentTimeOptionSelectionText] ELSE remoteChanges.[AppointmentTimeOptionSelectionText] END
                       ,[AppointmentTimeOptionSelection] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentTimeOptionSelection'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentTimeOptionSelection] ELSE remoteChanges.[AppointmentTimeOptionSelection] END
                       ,[AppointmentMonthSelectionText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentMonthSelectionText'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentMonthSelectionText] ELSE remoteChanges.[AppointmentMonthSelectionText] END
                       ,[AppointmentMonthSelection] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentMonthSelection'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentMonthSelection] ELSE remoteChanges.[AppointmentMonthSelection] END
                       ,[AppointmentDayOfTheMonth] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppointmentDayOfTheMonth'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[AppointmentDayOfTheMonth] ELSE remoteChanges.[AppointmentDayOfTheMonth] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

            WHEN NOT MATCHED THEN
                INSERT ([AppointmentTankGuid],[TankGuid],[TestSetDefinitionGuid],[SiteGuid],[AssetText],[AppointmentCategory],[AppointmentIsSingle],[ScheduleOnWeekends],[ScheduleOnHolidays],[StartDate],[Duration],[AppointmentPeriod],[AppointmentPeriodText],[Description],[AppointmentTimeInterval],[AppointmentDayOfTheWeekText],[AppointmentDayOfTheWeek],[AppointmentReoccuranceInterval],[AppointmentOption2Selected],[AppointmentTimeOptionSelectionText],[AppointmentTimeOptionSelection],[AppointmentMonthSelectionText],[AppointmentMonthSelection],[AppointmentDayOfTheMonth],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                    VALUES (@AppointmentTankGuid,@TankGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestSetDefinitionGuid'), @sync_supported_columns_tblAppointmentTank)) WHEN 0 THEN NULL ELSE @TestSetDefinitionGuid END),@SiteGuid,@AssetText,@AppointmentCategory,@AppointmentIsSingle,@ScheduleOnWeekends,@ScheduleOnHolidays,@StartDate,@Duration,@AppointmentPeriod,@AppointmentPeriodText,@Description,@AppointmentTimeInterval,@AppointmentDayOfTheWeekText,@AppointmentDayOfTheWeek,@AppointmentReoccuranceInterval,@AppointmentOption2Selected,@AppointmentTimeOptionSelectionText,@AppointmentTimeOptionSelection,@AppointmentMonthSelectionText,@AppointmentMonthSelection,@AppointmentDayOfTheMonth,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
            ;
         SET @sync_row_count = @@rowcount;
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END

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
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
