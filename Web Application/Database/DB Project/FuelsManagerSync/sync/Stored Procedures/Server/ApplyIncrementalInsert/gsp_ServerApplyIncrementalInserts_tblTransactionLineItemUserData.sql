-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionLineItemUserData
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblTransactionLineItemUserData]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@UserData1 nvarchar(60),
@UserData2 nvarchar(60),
@UserData3 nvarchar(60),
@UserData4 nvarchar(60),
@UserData5 nvarchar(60),
@UserData6 nvarchar(60),
@UserData7 nvarchar(60),
@UserData8 nvarchar(60),
@UserData9 nvarchar(60),
@UserData10 nvarchar(60),
@UserData11 nvarchar(60),
@UserData12 nvarchar(60),
@UserData13 nvarchar(60),
@UserData14 nvarchar(60),
@UserData15 nvarchar(60),
@UserData16 nvarchar(60),
@UserData17 nvarchar(60),
@UserData18 nvarchar(60),
@UserData19 nvarchar(60),
@UserData20 nvarchar(60),
@UserData21 nvarchar(60),
@UserData22 nvarchar(60),
@UserData23 nvarchar(60),
@UserData24 nvarchar(60),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@TransactionLineItemUserDataGuid uniqueidentifier,
@TransactionLineItemGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblTransactionLineItemUserData varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTransactionLineItemUserData] AS existingData
        USING (SELECT @UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@UserData9 'UserData9',@UserData10 'UserData10',@UserData11 'UserData11',@UserData12 'UserData12',@UserData13 'UserData13',@UserData14 'UserData14',@UserData15 'UserData15',@UserData16 'UserData16',@UserData17 'UserData17',@UserData18 'UserData18',@UserData19 'UserData19',@UserData20 'UserData20',@UserData21 'UserData21',@UserData22 'UserData22',@UserData23 'UserData23',@UserData24 'UserData24',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate',@TransactionLineItemUserDataGuid 'TransactionLineItemUserDataGuid',@TransactionLineItemGuid 'TransactionLineItemGuid'
                ) AS remoteChanges ([UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[TransactionLineItemUserDataGuid],[TransactionLineItemGuid])
        ON (existingData.[TransactionLineItemUserDataGuid] = remoteChanges.[TransactionLineItemUserDataGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [UserData1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData1] ELSE remoteChanges.[UserData1] END
                       ,[UserData2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData2] ELSE remoteChanges.[UserData2] END
                       ,[UserData3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData3] ELSE remoteChanges.[UserData3] END
                       ,[UserData4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData4] ELSE remoteChanges.[UserData4] END
                       ,[UserData5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData5] ELSE remoteChanges.[UserData5] END
                       ,[UserData6] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData6] ELSE remoteChanges.[UserData6] END
                       ,[UserData7] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData7] ELSE remoteChanges.[UserData7] END
                       ,[UserData8] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData8] ELSE remoteChanges.[UserData8] END
                       ,[UserData9] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData9'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData9] ELSE remoteChanges.[UserData9] END
                       ,[UserData10] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData10'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData10] ELSE remoteChanges.[UserData10] END
                       ,[UserData11] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData11'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData11] ELSE remoteChanges.[UserData11] END
                       ,[UserData12] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData12'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData12] ELSE remoteChanges.[UserData12] END
                       ,[UserData13] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData13'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData13] ELSE remoteChanges.[UserData13] END
                       ,[UserData14] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData14'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData14] ELSE remoteChanges.[UserData14] END
                       ,[UserData15] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData15'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData15] ELSE remoteChanges.[UserData15] END
                       ,[UserData16] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData16'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData16] ELSE remoteChanges.[UserData16] END
                       ,[UserData17] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData17'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData17] ELSE remoteChanges.[UserData17] END
                       ,[UserData18] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData18'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData18] ELSE remoteChanges.[UserData18] END
                       ,[UserData19] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData19'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData19] ELSE remoteChanges.[UserData19] END
                       ,[UserData20] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData20'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData20] ELSE remoteChanges.[UserData20] END
                       ,[UserData21] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData21'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData21] ELSE remoteChanges.[UserData21] END
                       ,[UserData22] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData22'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData22] ELSE remoteChanges.[UserData22] END
                       ,[UserData23] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData23'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData23] ELSE remoteChanges.[UserData23] END
                       ,[UserData24] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData24'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UserData24] ELSE remoteChanges.[UserData24] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[TransactionLineItemGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionLineItemGuid'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN existingData.[TransactionLineItemGuid] ELSE remoteChanges.[TransactionLineItemGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[TransactionLineItemUserDataGuid],[TransactionLineItemGuid])
                VALUES ((CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData6 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData7 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData8 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData9'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData9 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData10'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData10 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData11'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData11 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData12'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData12 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData13'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData13 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData14'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData14 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData15'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData15 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData16'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData16 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData17'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData17 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData18'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData18 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData19'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData19 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData20'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData20 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData21'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData21 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData22'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData22 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData23'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData23 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData24'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UserData24 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UpdatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTransactionLineItemUserData)) WHEN 0 THEN NULL ELSE @UpdatedDate END),@TransactionLineItemUserDataGuid,@TransactionLineItemGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionLineItemUserDataGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionLineItemUserDataGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionLineItemUserDataGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTransactionLineItemUserData] WHERE TransactionLineItemUserDataGuid = @TransactionLineItemUserDataGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

