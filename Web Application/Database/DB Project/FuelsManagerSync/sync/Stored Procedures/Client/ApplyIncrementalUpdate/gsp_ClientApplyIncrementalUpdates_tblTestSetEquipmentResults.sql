-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestSetEquipmentResults
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblTestSetEquipmentResults]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ResultTimeStamp datetimeoffset(7),
@TestSetName nvarchar(80),
@Inspector nvarchar(100),
@Supervisor nvarchar(100),
@EquipmentID nvarchar(50),
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
@TestSetEquipmentResultGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupTestSetStatusIndex int,
@EquipmentGuid uniqueidentifier,
@Flag01 bit,
@Flag02 bit,
@UserData01 nvarchar(60),
@UserData02 nvarchar(60),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblTestSetEquipmentResults] CT
                        WHERE CT.PK_TestSetEquipmentResultGuid = @TestSetEquipmentResultGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblTestSetEquipmentResults].[ResultTimeStamp],[dbo].[tblTestSetEquipmentResults].[TestSetName],[dbo].[tblTestSetEquipmentResults].[Inspector],[dbo].[tblTestSetEquipmentResults].[Supervisor],[dbo].[tblTestSetEquipmentResults].[EquipmentID],[dbo].[tblTestSetEquipmentResults].[SampleNumber],[dbo].[tblTestSetEquipmentResults].[SampleSize],[dbo].[tblTestSetEquipmentResults].[IsRetest],[dbo].[tblTestSetEquipmentResults].[PreviousSampleNumber],[dbo].[tblTestSetEquipmentResults].[DocumentNumber],[dbo].[tblTestSetEquipmentResults].[Memo],[dbo].[tblTestSetEquipmentResults].[GallonsRepresented],[dbo].[tblTestSetEquipmentResults].[Override],[dbo].[tblTestSetEquipmentResults].[DeleteFlag],[dbo].[tblTestSetEquipmentResults].[CreatedDate],[dbo].[tblTestSetEquipmentResults].[CreatedBy],[dbo].[tblTestSetEquipmentResults].[UpdatedDate],[dbo].[tblTestSetEquipmentResults].[UpdatedBy],[dbo].[tblTestSetEquipmentResults].[TestSetEquipmentResultGuid],[dbo].[tblTestSetEquipmentResults].[SiteGuid],[dbo].[tblTestSetEquipmentResults].[LookupTestSetStatusIndex],[dbo].[tblTestSetEquipmentResults].[EquipmentGuid],[dbo].[tblTestSetEquipmentResults].[Flag01],[dbo].[tblTestSetEquipmentResults].[Flag02],[dbo].[tblTestSetEquipmentResults].[UserData01],[dbo].[tblTestSetEquipmentResults].[UserData02]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblTestSetEquipmentResults]
                        INNER JOIN [track].[tblTestSetEquipmentResults] CT
                            ON CT.PK_TestSetEquipmentResultGuid = [dbo].[tblTestSetEquipmentResults].[TestSetEquipmentResultGuid] 
                    WHERE CT.PK_TestSetEquipmentResultGuid = @TestSetEquipmentResultGuid
            ) MERGE existingData
            USING (SELECT @ResultTimeStamp,@TestSetName,@Inspector,@Supervisor,@EquipmentID,@SampleNumber,@SampleSize,@IsRetest,@PreviousSampleNumber,@DocumentNumber,@Memo,@GallonsRepresented,@Override,@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@TestSetEquipmentResultGuid,@SiteGuid,@LookupTestSetStatusIndex,@EquipmentGuid,@Flag01,@Flag02,@UserData01,@UserData02
                    ) AS remoteChanges ([ResultTimeStamp],[TestSetName],[Inspector],[Supervisor],[EquipmentID],[SampleNumber],[SampleSize],[IsRetest],[PreviousSampleNumber],[DocumentNumber],[Memo],[GallonsRepresented],[Override],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TestSetEquipmentResultGuid],[SiteGuid],[LookupTestSetStatusIndex],[EquipmentGuid],[Flag01],[Flag02],[UserData01],[UserData02])
            ON (existingData.[TestSetEquipmentResultGuid] = remoteChanges.[TestSetEquipmentResultGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ResultTimeStamp] = remoteChanges.[ResultTimeStamp]
                       ,[TestSetName] = remoteChanges.[TestSetName]
                       ,[Inspector] = remoteChanges.[Inspector]
                       ,[Supervisor] = remoteChanges.[Supervisor]
                       ,[EquipmentID] = remoteChanges.[EquipmentID]
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
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[LookupTestSetStatusIndex] = remoteChanges.[LookupTestSetStatusIndex]
                       ,[EquipmentGuid] = remoteChanges.[EquipmentGuid]
                       ,[Flag01] = remoteChanges.[Flag01]
                       ,[Flag02] = remoteChanges.[Flag02]
                       ,[UserData01] = remoteChanges.[UserData01]
                       ,[UserData02] = remoteChanges.[UserData02]

            WHEN NOT MATCHED THEN
                INSERT ([ResultTimeStamp],[TestSetName],[Inspector],[Supervisor],[EquipmentID],[SampleNumber],[SampleSize],[IsRetest],[PreviousSampleNumber],[DocumentNumber],[Memo],[GallonsRepresented],[Override],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TestSetEquipmentResultGuid],[SiteGuid],[LookupTestSetStatusIndex],[EquipmentGuid],[Flag01],[Flag02],[UserData01],[UserData02])
                    VALUES (@ResultTimeStamp,@TestSetName,@Inspector,@Supervisor,@EquipmentID,@SampleNumber,@SampleSize,@IsRetest,@PreviousSampleNumber,@DocumentNumber,@Memo,@GallonsRepresented,@Override,@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@TestSetEquipmentResultGuid,@SiteGuid,@LookupTestSetStatusIndex,@EquipmentGuid,@Flag01,@Flag02,@UserData01,@UserData02)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestSetEquipmentResultGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestSetEquipmentResultGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TestSetEquipmentResultGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTestSetEquipmentResults] WHERE TestSetEquipmentResultGuid = @TestSetEquipmentResultGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
