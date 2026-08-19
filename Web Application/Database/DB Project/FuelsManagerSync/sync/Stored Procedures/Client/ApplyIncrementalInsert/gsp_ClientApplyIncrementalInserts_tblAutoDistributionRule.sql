-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAutoDistributionRule
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblAutoDistributionRule]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@AutoDistributionRuleGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@RuleID nvarchar(50),
@RuleDescription nvarchar(255),
@RuleEnabled bit,
@DefaultEOM bit,
@TransactionAliasGuid uniqueidentifier,
@DefaultReasonCodeGuid uniqueidentifier,
@DefaultNotes nvarchar(1000),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblAutoDistributionRule] AS existingData
        USING (SELECT @AutoDistributionRuleGuid 'AutoDistributionRuleGuid',@SiteGuid 'SiteGuid',@RuleID 'RuleID',@RuleDescription 'RuleDescription',@RuleEnabled 'RuleEnabled',@DefaultEOM 'DefaultEOM',@TransactionAliasGuid 'TransactionAliasGuid',@DefaultReasonCodeGuid 'DefaultReasonCodeGuid',@DefaultNotes 'DefaultNotes',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy'
                ) AS remoteChanges ([AutoDistributionRuleGuid],[SiteGuid],[RuleID],[RuleDescription],[RuleEnabled],[DefaultEOM],[TransactionAliasGuid],[DefaultReasonCodeGuid],[DefaultNotes],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
        ON (existingData.[AutoDistributionRuleGuid] = remoteChanges.[AutoDistributionRuleGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [SiteGuid] = remoteChanges.[SiteGuid]
                       ,[RuleID] = remoteChanges.[RuleID]
                       ,[RuleDescription] = remoteChanges.[RuleDescription]
                       ,[RuleEnabled] = remoteChanges.[RuleEnabled]
                       ,[DefaultEOM] = remoteChanges.[DefaultEOM]
                       ,[TransactionAliasGuid] = remoteChanges.[TransactionAliasGuid]
                       ,[DefaultReasonCodeGuid] = remoteChanges.[DefaultReasonCodeGuid]
                       ,[DefaultNotes] = remoteChanges.[DefaultNotes]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]

        WHEN NOT MATCHED THEN
            INSERT ([AutoDistributionRuleGuid],[SiteGuid],[RuleID],[RuleDescription],[RuleEnabled],[DefaultEOM],[TransactionAliasGuid],[DefaultReasonCodeGuid],[DefaultNotes],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                VALUES (@AutoDistributionRuleGuid,@SiteGuid,@RuleID,@RuleDescription,@RuleEnabled,@DefaultEOM,@TransactionAliasGuid,@DefaultReasonCodeGuid,@DefaultNotes,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AutoDistributionRuleGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AutoDistributionRuleGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AutoDistributionRuleGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblAutoDistributionRule] WHERE AutoDistributionRuleGuid = @AutoDistributionRuleGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
