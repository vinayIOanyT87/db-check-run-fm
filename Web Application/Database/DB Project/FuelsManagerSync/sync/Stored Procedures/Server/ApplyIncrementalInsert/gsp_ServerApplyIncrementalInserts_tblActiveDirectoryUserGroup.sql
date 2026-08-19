-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblActiveDirectoryUserGroup
-- Description:	Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblActiveDirectoryUserGroup]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ActiveDirectoryUserGroupGuid uniqueidentifier,
@Name nvarchar(50),
@Ssid nvarchar(32),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblActiveDirectoryUserGroup varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    ;   MERGE [dbo].[tblActiveDirectoryUserGroup] AS existingData
        USING (SELECT @ActiveDirectoryUserGroupGuid 'ActiveDirectoryUserGroupGuid',@Name 'Name',@Ssid 'Ssid',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate'
                ) AS remoteChanges ([ActiveDirectoryUserGroupGuid],[Name],[Ssid],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
        ON (existingData.[ActiveDirectoryUserGroupGuid] = remoteChanges.[ActiveDirectoryUserGroupGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [Name] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Name'), @sync_supported_columns_tblActiveDirectoryUserGroup)) WHEN 0 THEN existingData.[Name] ELSE remoteChanges.[Name] END
                       ,[Ssid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Ssid'), @sync_supported_columns_tblActiveDirectoryUserGroup)) WHEN 0 THEN existingData.[Ssid] ELSE remoteChanges.[Ssid] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblActiveDirectoryUserGroup)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblActiveDirectoryUserGroup)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblActiveDirectoryUserGroup)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblActiveDirectoryUserGroup)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END

        WHEN NOT MATCHED THEN
            INSERT ([ActiveDirectoryUserGroupGuid],[Name],[Ssid],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                VALUES (@ActiveDirectoryUserGroupGuid,@Name,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Ssid'), @sync_supported_columns_tblActiveDirectoryUserGroup)) WHEN 0 THEN NULL ELSE @Ssid END),@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ActiveDirectoryUserGroupGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ActiveDirectoryUserGroupGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ActiveDirectoryUserGroupGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblActiveDirectoryUserGroup] WHERE ActiveDirectoryUserGroupGuid = @ActiveDirectoryUserGroupGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;	-- This is used to detect Change Tracking cleanup
					            -- If we support this, we should add a column to SynchronizationTable
								-- that records the MinValidVersion after change tracking information for
								-- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
