-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmTest
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblAlarmTest]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
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
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblAlarmTest] AS existingData
        USING (SELECT @AlarmTestGuid 'AlarmTestGuid',@AlarmGuid 'AlarmGuid',@ID 'ID',@LimitTagGuid 'LimitTagGuid',@TagField 'TagField',@AlarmPriorityGuid 'AlarmPriorityGuid',@NormalUnacknowledgedAlarmPriorityGuid 'NormalUnacknowledgedAlarmPriorityGuid',@TestType 'TestType',@BitMask 'BitMask',@Enabled 'Enabled',@Order 'Order',@AlarmState 'AlarmState',@Holdoff 'Holdoff',@AlarmText 'AlarmText',@HelpFile 'HelpFile',@DrawingGuid 'DrawingGuid',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@BitwiseOperator 'BitwiseOperator',@TimedHoldOffInSeconds 'TimedHoldOffInSeconds',@AlarmTestTemplateGuid 'AlarmTestTemplateGuid'
                ) AS remoteChanges ([AlarmTestGuid],[AlarmGuid],[ID],[LimitTagGuid],[TagField],[AlarmPriorityGuid],[NormalUnacknowledgedAlarmPriorityGuid],[TestType],[BitMask],[Enabled],[Order],[AlarmState],[Holdoff],[AlarmText],[HelpFile],[DrawingGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[BitwiseOperator],[TimedHoldOffInSeconds],[AlarmTestTemplateGuid])
        ON (existingData.[AlarmTestGuid] = remoteChanges.[AlarmTestGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [AlarmGuid] = remoteChanges.[AlarmGuid]
                       ,[ID] = remoteChanges.[ID]
                       ,[LimitTagGuid] = remoteChanges.[LimitTagGuid]
                       ,[TagField] = remoteChanges.[TagField]
                       ,[AlarmPriorityGuid] = remoteChanges.[AlarmPriorityGuid]
                       ,[NormalUnacknowledgedAlarmPriorityGuid] = remoteChanges.[NormalUnacknowledgedAlarmPriorityGuid]
                       ,[TestType] = remoteChanges.[TestType]
                       ,[BitMask] = remoteChanges.[BitMask]
                       ,[Enabled] = remoteChanges.[Enabled]
                       ,[Order] = remoteChanges.[Order]
                       ,[AlarmState] = remoteChanges.[AlarmState]
                       ,[Holdoff] = remoteChanges.[Holdoff]
                       ,[AlarmText] = remoteChanges.[AlarmText]
                       ,[HelpFile] = remoteChanges.[HelpFile]
                       ,[DrawingGuid] = remoteChanges.[DrawingGuid]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[BitwiseOperator] = remoteChanges.[BitwiseOperator]
                       ,[TimedHoldOffInSeconds] = remoteChanges.[TimedHoldOffInSeconds]
                       ,[AlarmTestTemplateGuid] = remoteChanges.[AlarmTestTemplateGuid]

        WHEN NOT MATCHED THEN
            INSERT ([AlarmTestGuid],[AlarmGuid],[ID],[LimitTagGuid],[TagField],[AlarmPriorityGuid],[NormalUnacknowledgedAlarmPriorityGuid],[TestType],[BitMask],[Enabled],[Order],[AlarmState],[Holdoff],[AlarmText],[HelpFile],[DrawingGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[BitwiseOperator],[TimedHoldOffInSeconds],[AlarmTestTemplateGuid])
                VALUES (@AlarmTestGuid,@AlarmGuid,@ID,@LimitTagGuid,@TagField,@AlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,@TestType,@BitMask,@Enabled,@Order,@AlarmState,@Holdoff,@AlarmText,@HelpFile,@DrawingGuid,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@BitwiseOperator,@TimedHoldOffInSeconds,@AlarmTestTemplateGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
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
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
