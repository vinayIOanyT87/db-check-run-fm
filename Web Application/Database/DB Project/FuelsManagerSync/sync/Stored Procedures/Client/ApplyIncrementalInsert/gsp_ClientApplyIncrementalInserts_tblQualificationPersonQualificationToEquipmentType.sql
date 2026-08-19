-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationPersonQualificationToEquipmentType
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblQualificationPersonQualificationToEquipmentType]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@QualificationPersonQualificationToEquipmentTypeGuid uniqueidentifier,
@QualificationGuid uniqueidentifier,
@EquipmentTypeGuid uniqueidentifier,
@Sequence int,
@Instructor nvarchar(50),
@DateCompleted datetimeoffset(7),
@DateDue datetimeoffset(7),
@ExpirationDate datetimeoffset(7),
@ID varchar(50),
@Rating nvarchar(20),
@HistoricalRecord bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@SiteGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [map].[tblQualificationPersonQualificationToEquipmentType] AS existingData
        USING (SELECT @QualificationPersonQualificationToEquipmentTypeGuid 'QualificationPersonQualificationToEquipmentTypeGuid',@QualificationGuid 'QualificationGuid',@EquipmentTypeGuid 'EquipmentTypeGuid',@Sequence 'Sequence',@Instructor 'Instructor',@DateCompleted 'DateCompleted',@DateDue 'DateDue',@ExpirationDate 'ExpirationDate',@ID 'ID',@Rating 'Rating',@HistoricalRecord 'HistoricalRecord',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@SiteGuid 'SiteGuid'
                ) AS remoteChanges ([QualificationPersonQualificationToEquipmentTypeGuid],[QualificationGuid],[EquipmentTypeGuid],[Sequence],[Instructor],[DateCompleted],[DateDue],[ExpirationDate],[ID],[Rating],[HistoricalRecord],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SiteGuid])
        ON (existingData.[QualificationPersonQualificationToEquipmentTypeGuid] = remoteChanges.[QualificationPersonQualificationToEquipmentTypeGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [QualificationGuid] = remoteChanges.[QualificationGuid]
                       ,[EquipmentTypeGuid] = remoteChanges.[EquipmentTypeGuid]
                       ,[Sequence] = remoteChanges.[Sequence]
                       ,[Instructor] = remoteChanges.[Instructor]
                       ,[DateCompleted] = remoteChanges.[DateCompleted]
                       ,[DateDue] = remoteChanges.[DateDue]
                       ,[ExpirationDate] = remoteChanges.[ExpirationDate]
                       ,[ID] = remoteChanges.[ID]
                       ,[Rating] = remoteChanges.[Rating]
                       ,[HistoricalRecord] = remoteChanges.[HistoricalRecord]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
				   ,[SiteGuid] = remoteChanges.[SiteGuid]

        WHEN NOT MATCHED THEN
            INSERT ([QualificationPersonQualificationToEquipmentTypeGuid],[QualificationGuid],[EquipmentTypeGuid],[Sequence],[Instructor],[DateCompleted],[DateDue],[ExpirationDate],[ID],[Rating],[HistoricalRecord],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SiteGuid])
                VALUES (@QualificationPersonQualificationToEquipmentTypeGuid,@QualificationGuid,@EquipmentTypeGuid,@Sequence,@Instructor,@DateCompleted,@DateDue,@ExpirationDate,@ID,@Rating,@HistoricalRecord,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@SiteGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @QualificationPersonQualificationToEquipmentTypeGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @QualificationPersonQualificationToEquipmentTypeGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @QualificationPersonQualificationToEquipmentTypeGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [map].[tblQualificationPersonQualificationToEquipmentType] WHERE QualificationPersonQualificationToEquipmentTypeGuid = @QualificationPersonQualificationToEquipmentTypeGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
