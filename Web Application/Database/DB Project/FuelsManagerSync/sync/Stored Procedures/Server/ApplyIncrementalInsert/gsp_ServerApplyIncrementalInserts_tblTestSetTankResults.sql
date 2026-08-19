-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestSetTankResults
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblTestSetTankResults]
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblTestSetTankResults varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTestSetTankResults] AS existingData
        USING (SELECT @ResultTimeStamp 'ResultTimeStamp',@TestSetName 'TestSetName',@Inspector 'Inspector',@Supervisor 'Supervisor',@TankID 'TankID',@SampleNumber 'SampleNumber',@SampleSize 'SampleSize',@IsRetest 'IsRetest',@PreviousSampleNumber 'PreviousSampleNumber',@DocumentNumber 'DocumentNumber',@Memo 'Memo',@GallonsRepresented 'GallonsRepresented',@Override 'Override',@DeleteFlag 'DeleteFlag',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@Flag01 'Flag01',@Flag02 'Flag02',@UserData01 'UserData01',@UserData02 'UserData02',@TestSetTankResultGuid 'TestSetTankResultGuid',@SiteGuid 'SiteGuid',@LookupTestSetStatusIndex 'LookupTestSetStatusIndex',@TankGuid 'TankGuid'
                ) AS remoteChanges ([ResultTimeStamp],[TestSetName],[Inspector],[Supervisor],[TankID],[SampleNumber],[SampleSize],[IsRetest],[PreviousSampleNumber],[DocumentNumber],[Memo],[GallonsRepresented],[Override],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Flag01],[Flag02],[UserData01],[UserData02],[TestSetTankResultGuid],[SiteGuid],[LookupTestSetStatusIndex],[TankGuid])
        ON (existingData.[TestSetTankResultGuid] = remoteChanges.[TestSetTankResultGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ResultTimeStamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ResultTimeStamp'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[ResultTimeStamp] ELSE remoteChanges.[ResultTimeStamp] END
                       ,[TestSetName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestSetName'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[TestSetName] ELSE remoteChanges.[TestSetName] END
                       ,[Inspector] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Inspector'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[Inspector] ELSE remoteChanges.[Inspector] END
                       ,[Supervisor] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Supervisor'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[Supervisor] ELSE remoteChanges.[Supervisor] END
                       ,[TankID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankID'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[TankID] ELSE remoteChanges.[TankID] END
                       ,[SampleNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SampleNumber'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[SampleNumber] ELSE remoteChanges.[SampleNumber] END
                       ,[SampleSize] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SampleSize'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[SampleSize] ELSE remoteChanges.[SampleSize] END
                       ,[IsRetest] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IsRetest'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[IsRetest] ELSE remoteChanges.[IsRetest] END
                       ,[PreviousSampleNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PreviousSampleNumber'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[PreviousSampleNumber] ELSE remoteChanges.[PreviousSampleNumber] END
                       ,[DocumentNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DocumentNumber'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[DocumentNumber] ELSE remoteChanges.[DocumentNumber] END
                       ,[Memo] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Memo'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[Memo] ELSE remoteChanges.[Memo] END
                       ,[GallonsRepresented] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GallonsRepresented'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[GallonsRepresented] ELSE remoteChanges.[GallonsRepresented] END
                       ,[Override] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Override'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[Override] ELSE remoteChanges.[Override] END
                       ,[DeleteFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeleteFlag'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[DeleteFlag] ELSE remoteChanges.[DeleteFlag] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[Flag01] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag01'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[Flag01] ELSE remoteChanges.[Flag01] END
                       ,[Flag02] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag02'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[Flag02] ELSE remoteChanges.[Flag02] END
                       ,[UserData01] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData01'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[UserData01] ELSE remoteChanges.[UserData01] END
                       ,[UserData02] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData02'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[UserData02] ELSE remoteChanges.[UserData02] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[LookupTestSetStatusIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupTestSetStatusIndex'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[LookupTestSetStatusIndex] ELSE remoteChanges.[LookupTestSetStatusIndex] END
                       ,[TankGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankGuid'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN existingData.[TankGuid] ELSE remoteChanges.[TankGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([ResultTimeStamp],[TestSetName],[Inspector],[Supervisor],[TankID],[SampleNumber],[SampleSize],[IsRetest],[PreviousSampleNumber],[DocumentNumber],[Memo],[GallonsRepresented],[Override],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Flag01],[Flag02],[UserData01],[UserData02],[TestSetTankResultGuid],[SiteGuid],[LookupTestSetStatusIndex],[TankGuid])
                VALUES (@ResultTimeStamp,@TestSetName,@Inspector,@Supervisor,@TankID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SampleNumber'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN NULL ELSE @SampleNumber END),@SampleSize,@IsRetest,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PreviousSampleNumber'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN NULL ELSE @PreviousSampleNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DocumentNumber'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN NULL ELSE @DocumentNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Memo'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN NULL ELSE @Memo END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GallonsRepresented'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN NULL ELSE @GallonsRepresented END),@Override,@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag01'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN NULL ELSE @Flag01 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag02'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN NULL ELSE @Flag02 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData01'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN NULL ELSE @UserData01 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData02'), @sync_supported_columns_tblTestSetTankResults)) WHEN 0 THEN NULL ELSE @UserData02 END),@TestSetTankResultGuid,@SiteGuid,@LookupTestSetStatusIndex,@TankGuid)
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
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

