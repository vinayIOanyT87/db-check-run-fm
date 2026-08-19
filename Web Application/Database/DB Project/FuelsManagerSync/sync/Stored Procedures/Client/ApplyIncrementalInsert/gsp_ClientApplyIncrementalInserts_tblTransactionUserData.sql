-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionUserData
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblTransactionUserData]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@UserData1 nvarchar(255),
@UserData2 nvarchar(255),
@UserData3 nvarchar(255),
@UserData4 nvarchar(255),
@UserData5 nvarchar(255),
@UserData6 nvarchar(255),
@UserData7 nvarchar(255),
@UserData8 nvarchar(255),
@UserData9 nvarchar(255),
@UserData10 nvarchar(255),
@UserData11 nvarchar(255),
@UserData12 nvarchar(255),
@UserData13 nvarchar(255),
@UserData14 nvarchar(255),
@UserData15 nvarchar(255),
@UserData16 nvarchar(255),
@UserData17 nvarchar(255),
@UserData18 nvarchar(255),
@UserData19 nvarchar(255),
@UserData20 nvarchar(255),
@UserData21 nvarchar(255),
@UserData22 nvarchar(255),
@UserData23 nvarchar(255),
@UserData24 nvarchar(255),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@TransactionUserDataGuid uniqueidentifier,
@TransactionGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTransactionUserData] AS existingData
        USING (SELECT @UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@UserData9 'UserData9',@UserData10 'UserData10',@UserData11 'UserData11',@UserData12 'UserData12',@UserData13 'UserData13',@UserData14 'UserData14',@UserData15 'UserData15',@UserData16 'UserData16',@UserData17 'UserData17',@UserData18 'UserData18',@UserData19 'UserData19',@UserData20 'UserData20',@UserData21 'UserData21',@UserData22 'UserData22',@UserData23 'UserData23',@UserData24 'UserData24',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate',@TransactionUserDataGuid 'TransactionUserDataGuid',@TransactionGuid 'TransactionGuid'
                ) AS remoteChanges ([UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[TransactionUserDataGuid],[TransactionGuid])
        ON (existingData.[TransactionUserDataGuid] = remoteChanges.[TransactionUserDataGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [UserData1] = remoteChanges.[UserData1]
                       ,[UserData2] = remoteChanges.[UserData2]
                       ,[UserData3] = remoteChanges.[UserData3]
                       ,[UserData4] = remoteChanges.[UserData4]
                       ,[UserData5] = remoteChanges.[UserData5]
                       ,[UserData6] = remoteChanges.[UserData6]
                       ,[UserData7] = remoteChanges.[UserData7]
                       ,[UserData8] = remoteChanges.[UserData8]
                       ,[UserData9] = remoteChanges.[UserData9]
                       ,[UserData10] = remoteChanges.[UserData10]
                       ,[UserData11] = remoteChanges.[UserData11]
                       ,[UserData12] = remoteChanges.[UserData12]
                       ,[UserData13] = remoteChanges.[UserData13]
                       ,[UserData14] = remoteChanges.[UserData14]
                       ,[UserData15] = remoteChanges.[UserData15]
                       ,[UserData16] = remoteChanges.[UserData16]
                       ,[UserData17] = remoteChanges.[UserData17]
                       ,[UserData18] = remoteChanges.[UserData18]
                       ,[UserData19] = remoteChanges.[UserData19]
                       ,[UserData20] = remoteChanges.[UserData20]
                       ,[UserData21] = remoteChanges.[UserData21]
                       ,[UserData22] = remoteChanges.[UserData22]
                       ,[UserData23] = remoteChanges.[UserData23]
                       ,[UserData24] = remoteChanges.[UserData24]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[TransactionGuid] = remoteChanges.[TransactionGuid]

        WHEN NOT MATCHED THEN
            INSERT ([UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[TransactionUserDataGuid],[TransactionGuid])
                VALUES (@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@UserData9,@UserData10,@UserData11,@UserData12,@UserData13,@UserData14,@UserData15,@UserData16,@UserData17,@UserData18,@UserData19,@UserData20,@UserData21,@UserData22,@UserData23,@UserData24,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@TransactionUserDataGuid,@TransactionGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionUserDataGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionUserDataGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionUserDataGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTransactionUserData] WHERE TransactionUserDataGuid = @TransactionUserDataGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
