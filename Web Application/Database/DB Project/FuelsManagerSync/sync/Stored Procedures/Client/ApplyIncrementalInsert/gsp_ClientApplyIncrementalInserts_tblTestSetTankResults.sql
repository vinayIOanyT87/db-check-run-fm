-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestSetTankResults
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblTestSetTankResults]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ResultTimeStamp datetimeoffset(7),
@TestSetName nvarchar(80),
@Inspector nvarchar(100),
@Supervisor nvarchar(100),
@TankID nvarchar(50),
@SampleNumber int,
@SampleSize float,
@IsRetest bit,
@PreviousSampleNumber int,
@DocumentNumber nvarchar(50),
@Memo nvarchar(1000),
@GallonsRepresented float,
@Override bit,
@DeleteFlag bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@Flag01 bit,
@Flag02 bit,
@UserData01 nvarchar(60),
@UserData02 nvarchar(60),
@TestSetTankResultGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupTestSetStatusIndex int,
@TankGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTestSetTankResults] AS existingData
        USING (SELECT @ResultTimeStamp 'ResultTimeStamp',@TestSetName 'TestSetName',@Inspector 'Inspector',@Supervisor 'Supervisor',@TankID 'TankID',@SampleNumber 'SampleNumber',@SampleSize 'SampleSize',@IsRetest 'IsRetest',@PreviousSampleNumber 'PreviousSampleNumber',@DocumentNumber 'DocumentNumber',@Memo 'Memo',@GallonsRepresented 'GallonsRepresented',@Override 'Override',@DeleteFlag 'DeleteFlag',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@Flag01 'Flag01',@Flag02 'Flag02',@UserData01 'UserData01',@UserData02 'UserData02',@TestSetTankResultGuid 'TestSetTankResultGuid',@SiteGuid 'SiteGuid',@LookupTestSetStatusIndex 'LookupTestSetStatusIndex',@TankGuid 'TankGuid'
                ) AS remoteChanges ([ResultTimeStamp],[TestSetName],[Inspector],[Supervisor],[TankID],[SampleNumber],[SampleSize],[IsRetest],[PreviousSampleNumber],[DocumentNumber],[Memo],[GallonsRepresented],[Override],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Flag01],[Flag02],[UserData01],[UserData02],[TestSetTankResultGuid],[SiteGuid],[LookupTestSetStatusIndex],[TankGuid])
        ON (existingData.[TestSetTankResultGuid] = remoteChanges.[TestSetTankResultGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ResultTimeStamp] = remoteChanges.[ResultTimeStamp]
                       ,[TestSetName] = remoteChanges.[TestSetName]
                       ,[Inspector] = remoteChanges.[Inspector]
                       ,[Supervisor] = remoteChanges.[Supervisor]
                       ,[TankID] = remoteChanges.[TankID]
                       ,[SampleNumber] = remoteChanges.[SampleNumber]
                       ,[SampleSize] = remoteChanges.[SampleSize]
                       ,[IsRetest] = remoteChanges.[IsRetest]
                       ,[PreviousSampleNumber] = remoteChanges.[PreviousSampleNumber]
                       ,[DocumentNumber] = remoteChanges.[DocumentNumber]
                       ,[Memo] = remoteChanges.[Memo]
                       ,[GallonsRepresented] = remoteChanges.[GallonsRepresented]
                       ,[Override] = remoteChanges.[Override]
                       ,[DeleteFlag] = remoteChanges.[DeleteFlag]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[Flag01] = remoteChanges.[Flag01]
                       ,[Flag02] = remoteChanges.[Flag02]
                       ,[UserData01] = remoteChanges.[UserData01]
                       ,[UserData02] = remoteChanges.[UserData02]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[LookupTestSetStatusIndex] = remoteChanges.[LookupTestSetStatusIndex]
                       ,[TankGuid] = remoteChanges.[TankGuid]

        WHEN NOT MATCHED THEN
            INSERT ([ResultTimeStamp],[TestSetName],[Inspector],[Supervisor],[TankID],[SampleNumber],[SampleSize],[IsRetest],[PreviousSampleNumber],[DocumentNumber],[Memo],[GallonsRepresented],[Override],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Flag01],[Flag02],[UserData01],[UserData02],[TestSetTankResultGuid],[SiteGuid],[LookupTestSetStatusIndex],[TankGuid])
                VALUES (@ResultTimeStamp,@TestSetName,@Inspector,@Supervisor,@TankID,@SampleNumber,@SampleSize,@IsRetest,@PreviousSampleNumber,@DocumentNumber,@Memo,@GallonsRepresented,@Override,@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@Flag01,@Flag02,@UserData01,@UserData02,@TestSetTankResultGuid,@SiteGuid,@LookupTestSetStatusIndex,@TankGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestSetTankResultGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestSetTankResultGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestSetTankResultGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTestSetTankResults] WHERE TestSetTankResultGuid = @TestSetTankResultGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
