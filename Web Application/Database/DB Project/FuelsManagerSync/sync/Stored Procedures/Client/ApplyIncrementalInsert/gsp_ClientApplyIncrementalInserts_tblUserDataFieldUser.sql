-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldUser
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblUserDataFieldUser]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@UserDataFieldUserGuid uniqueidentifier,
@TransactionAliasGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@Number tinyint,
@DisplayOrder int,
@DisplayName nvarchar(30),
@LookupUserDataTypeIndex int,
@Required bit,
@UserGroupGuid uniqueidentifier,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@DispatchField bit,
@ClearOnNew bit,
@ReadOnly bit,
@Visibility int,
@DefaultValue nvarchar(120),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblUserDataFieldUser] AS existingData
        USING (SELECT @UserDataFieldUserGuid 'UserDataFieldUserGuid',@TransactionAliasGuid 'TransactionAliasGuid',@SiteGuid 'SiteGuid',@Number 'Number',@DisplayOrder 'DisplayOrder',@DisplayName 'DisplayName',@LookupUserDataTypeIndex 'LookupUserDataTypeIndex',@Required 'Required',@UserGroupGuid 'UserGroupGuid',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@DispatchField 'DispatchField',@ClearOnNew 'ClearOnNew',@ReadOnly 'ReadOnly',@Visibility 'Visibility',@DefaultValue 'DefaultValue'
                ) AS remoteChanges ([UserDataFieldUserGuid],[TransactionAliasGuid],[SiteGuid],[Number],[DisplayOrder],[DisplayName],[LookupUserDataTypeIndex],[Required],[UserGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValue])
        ON (existingData.[UserDataFieldUserGuid] = remoteChanges.[UserDataFieldUserGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [TransactionAliasGuid] = remoteChanges.[TransactionAliasGuid]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[Number] = remoteChanges.[Number]
                       ,[DisplayOrder] = remoteChanges.[DisplayOrder]
                       ,[DisplayName] = remoteChanges.[DisplayName]
                       ,[LookupUserDataTypeIndex] = remoteChanges.[LookupUserDataTypeIndex]
                       ,[Required] = remoteChanges.[Required]
                       ,[UserGroupGuid] = remoteChanges.[UserGroupGuid]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[DispatchField] = remoteChanges.[DispatchField]
                       ,[ClearOnNew] = remoteChanges.[ClearOnNew]
                       ,[ReadOnly] = remoteChanges.[ReadOnly]
                       ,[Visibility] = remoteChanges.[Visibility]
                       ,[DefaultValue] = remoteChanges.[DefaultValue]

        WHEN NOT MATCHED THEN
            INSERT ([UserDataFieldUserGuid],[TransactionAliasGuid],[SiteGuid],[Number],[DisplayOrder],[DisplayName],[LookupUserDataTypeIndex],[Required],[UserGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValue])
                VALUES (@UserDataFieldUserGuid,@TransactionAliasGuid,@SiteGuid,@Number,@DisplayOrder,@DisplayName,@LookupUserDataTypeIndex,@Required,@UserGroupGuid,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@DispatchField,@ClearOnNew,@ReadOnly,@Visibility,@DefaultValue)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @UserDataFieldUserGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @UserDataFieldUserGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @UserDataFieldUserGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblUserDataFieldUser] WHERE UserDataFieldUserGuid = @UserDataFieldUserGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
