-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmTest
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblAlarmTest]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@AlarmTestGuid uniqueidentifier,
@AlarmGuid uniqueidentifier,
@ID nvarchar(256),
@LimitTagGuid uniqueidentifier,
@TagField int,
@AlarmPriorityGuid uniqueidentifier,
@NormalUnacknowledgedAlarmPriorityGuid uniqueidentifier,
@TestType int,
@BitMask bigint,
@Enabled bit,
@Order int,
@AlarmState nvarchar(100),
@Holdoff float,
@AlarmText nvarchar(256),
@HelpFile nvarchar(max),
@DrawingGuid uniqueidentifier,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@BitwiseOperator int,
@TimedHoldOffInSeconds int,
@AlarmTestTemplateGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblAlarmTest varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblAlarmTest] CT
                        WHERE CT.PK_AlarmTestGuid = @AlarmTestGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblAlarmTest].[AlarmTestGuid],[dbo].[tblAlarmTest].[AlarmGuid],[dbo].[tblAlarmTest].[ID],[dbo].[tblAlarmTest].[LimitTagGuid],[dbo].[tblAlarmTest].[TagField],[dbo].[tblAlarmTest].[AlarmPriorityGuid],[dbo].[tblAlarmTest].[NormalUnacknowledgedAlarmPriorityGuid],[dbo].[tblAlarmTest].[TestType],[dbo].[tblAlarmTest].[BitMask],[dbo].[tblAlarmTest].[Enabled],[dbo].[tblAlarmTest].[Order],[dbo].[tblAlarmTest].[AlarmState],[dbo].[tblAlarmTest].[Holdoff],[dbo].[tblAlarmTest].[AlarmText],[dbo].[tblAlarmTest].[HelpFile],[dbo].[tblAlarmTest].[DrawingGuid],[dbo].[tblAlarmTest].[CreatedDate],[dbo].[tblAlarmTest].[CreatedBy],[dbo].[tblAlarmTest].[UpdatedDate],[dbo].[tblAlarmTest].[UpdatedBy],[dbo].[tblAlarmTest].[BitwiseOperator],[dbo].[tblAlarmTest].[TimedHoldOffInSeconds],[dbo].[tblAlarmTest].[AlarmTestTemplateGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblAlarmTest]
                        INNER JOIN [track].[tblAlarmTest] CT
                            ON CT.PK_AlarmTestGuid = [dbo].[tblAlarmTest].[AlarmTestGuid] 
                    WHERE CT.PK_AlarmTestGuid = @AlarmTestGuid
            ) MERGE existingData
            USING (SELECT @AlarmTestGuid,@AlarmGuid,@ID,@LimitTagGuid,@TagField,@AlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,@TestType,@BitMask,@Enabled,@Order,@AlarmState,@Holdoff,@AlarmText,@HelpFile,@DrawingGuid,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@BitwiseOperator,@TimedHoldOffInSeconds,@AlarmTestTemplateGuid
                    ) AS remoteChanges ([AlarmTestGuid],[AlarmGuid],[ID],[LimitTagGuid],[TagField],[AlarmPriorityGuid],[NormalUnacknowledgedAlarmPriorityGuid],[TestType],[BitMask],[Enabled],[Order],[AlarmState],[Holdoff],[AlarmText],[HelpFile],[DrawingGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[BitwiseOperator],[TimedHoldOffInSeconds],[AlarmTestTemplateGuid])
            ON (existingData.[AlarmTestGuid] = remoteChanges.[AlarmTestGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [AlarmGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmGuid'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[AlarmGuid] ELSE remoteChanges.[AlarmGuid] END
                       ,[ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[LimitTagGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LimitTagGuid'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[LimitTagGuid] ELSE remoteChanges.[LimitTagGuid] END
                       ,[TagField] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TagField'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[TagField] ELSE remoteChanges.[TagField] END
                       ,[AlarmPriorityGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmPriorityGuid'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[AlarmPriorityGuid] ELSE remoteChanges.[AlarmPriorityGuid] END
                       ,[NormalUnacknowledgedAlarmPriorityGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NormalUnacknowledgedAlarmPriorityGuid'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[NormalUnacknowledgedAlarmPriorityGuid] ELSE remoteChanges.[NormalUnacknowledgedAlarmPriorityGuid] END
                       ,[TestType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestType'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[TestType] ELSE remoteChanges.[TestType] END
                       ,[BitMask] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BitMask'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[BitMask] ELSE remoteChanges.[BitMask] END
                       ,[Enabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Enabled'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[Enabled] ELSE remoteChanges.[Enabled] END
                       ,[Order] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Order'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[Order] ELSE remoteChanges.[Order] END
                       ,[AlarmState] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmState'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[AlarmState] ELSE remoteChanges.[AlarmState] END
                       ,[Holdoff] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Holdoff'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[Holdoff] ELSE remoteChanges.[Holdoff] END
                       ,[AlarmText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmText'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[AlarmText] ELSE remoteChanges.[AlarmText] END
                       ,[HelpFile] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HelpFile'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[HelpFile] ELSE remoteChanges.[HelpFile] END
                       ,[DrawingGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DrawingGuid'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[DrawingGuid] ELSE remoteChanges.[DrawingGuid] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[BitwiseOperator] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BitwiseOperator'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[BitwiseOperator] ELSE remoteChanges.[BitwiseOperator] END
                       ,[TimedHoldOffInSeconds] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TimedHoldOffInSeconds'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[TimedHoldOffInSeconds] ELSE remoteChanges.[TimedHoldOffInSeconds] END
                       ,[AlarmTestTemplateGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmTestTemplateGuid'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN existingData.[AlarmTestTemplateGuid] ELSE remoteChanges.[AlarmTestTemplateGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([AlarmTestGuid],[AlarmGuid],[ID],[LimitTagGuid],[TagField],[AlarmPriorityGuid],[NormalUnacknowledgedAlarmPriorityGuid],[TestType],[BitMask],[Enabled],[Order],[AlarmState],[Holdoff],[AlarmText],[HelpFile],[DrawingGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[BitwiseOperator],[TimedHoldOffInSeconds],[AlarmTestTemplateGuid])
                    VALUES (@AlarmTestGuid,@AlarmGuid,@ID,@LimitTagGuid,@TagField,@AlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,@TestType,@BitMask,@Enabled,@Order,@AlarmState,@Holdoff,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmText'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN NULL ELSE @AlarmText END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HelpFile'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN NULL ELSE @HelpFile END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DrawingGuid'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN NULL ELSE @DrawingGuid END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@BitwiseOperator,@TimedHoldOffInSeconds,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmTestTemplateGuid'), @sync_supported_columns_tblAlarmTest)) WHEN 0 THEN NULL ELSE @AlarmTestTemplateGuid END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmTestGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmTestGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmTestGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblAlarmTest] WHERE AlarmTestGuid = @AlarmTestGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
