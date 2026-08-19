-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionAliasFields
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblTransactionAliasFields]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@AliasID int,
@DbName nvarchar(50),
@DisplayOrder int,
@DisplayName nvarchar(50),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@Required bit,
@Virtual bit,
@TransactionAliasFieldGuid uniqueidentifier,
@LookupTransactionFieldTypeIndex int,
@TransactionAliasGuid uniqueidentifier,
@UserGroupGuid uniqueidentifier,
@DispatchField bit,
@ClearOnNew bit,
@ReadOnly bit,
@Visibility int,
@DefaultValueType nvarchar(max),
@DefaultValue xml,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblTransactionAliasFields] CT
                        WHERE CT.PK_TransactionAliasFieldGuid = @TransactionAliasFieldGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblTransactionAliasFields].[AliasID],[dbo].[tblTransactionAliasFields].[DbName],[dbo].[tblTransactionAliasFields].[DisplayOrder],[dbo].[tblTransactionAliasFields].[DisplayName],[dbo].[tblTransactionAliasFields].[CreatedDate],[dbo].[tblTransactionAliasFields].[CreatedBy],[dbo].[tblTransactionAliasFields].[UpdatedDate],[dbo].[tblTransactionAliasFields].[UpdatedBy],[dbo].[tblTransactionAliasFields].[Required],[dbo].[tblTransactionAliasFields].[Virtual],[dbo].[tblTransactionAliasFields].[TransactionAliasFieldGuid],[dbo].[tblTransactionAliasFields].[LookupTransactionFieldTypeIndex],[dbo].[tblTransactionAliasFields].[TransactionAliasGuid],[dbo].[tblTransactionAliasFields].[UserGroupGuid],[dbo].[tblTransactionAliasFields].[DispatchField],[dbo].[tblTransactionAliasFields].[ClearOnNew],[dbo].[tblTransactionAliasFields].[ReadOnly],[dbo].[tblTransactionAliasFields].[Visibility],[dbo].[tblTransactionAliasFields].[DefaultValueType],[dbo].[tblTransactionAliasFields].[DefaultValue]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblTransactionAliasFields]
                        INNER JOIN [track].[tblTransactionAliasFields] CT
                            ON CT.PK_TransactionAliasFieldGuid = [dbo].[tblTransactionAliasFields].[TransactionAliasFieldGuid] 
                    WHERE CT.PK_TransactionAliasFieldGuid = @TransactionAliasFieldGuid
            ) MERGE existingData
            USING (SELECT @AliasID,@DbName,@DisplayOrder,@DisplayName,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@Required,@Virtual,@TransactionAliasFieldGuid,@LookupTransactionFieldTypeIndex,@TransactionAliasGuid,@UserGroupGuid,@DispatchField,@ClearOnNew,@ReadOnly,@Visibility,@DefaultValueType,@DefaultValue
                    ) AS remoteChanges ([AliasID],[DbName],[DisplayOrder],[DisplayName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Required],[Virtual],[TransactionAliasFieldGuid],[LookupTransactionFieldTypeIndex],[TransactionAliasGuid],[UserGroupGuid],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValueType],[DefaultValue])
            ON (existingData.[TransactionAliasFieldGuid] = remoteChanges.[TransactionAliasFieldGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [AliasID] = remoteChanges.[AliasID]
                       ,[DbName] = remoteChanges.[DbName]
                       ,[DisplayOrder] = remoteChanges.[DisplayOrder]
                       ,[DisplayName] = remoteChanges.[DisplayName]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[Required] = remoteChanges.[Required]
                       ,[Virtual] = remoteChanges.[Virtual]
                       ,[LookupTransactionFieldTypeIndex] = remoteChanges.[LookupTransactionFieldTypeIndex]
                       ,[TransactionAliasGuid] = remoteChanges.[TransactionAliasGuid]
                       ,[UserGroupGuid] = remoteChanges.[UserGroupGuid]
                       ,[DispatchField] = remoteChanges.[DispatchField]
                       ,[ClearOnNew] = remoteChanges.[ClearOnNew]
                       ,[ReadOnly] = remoteChanges.[ReadOnly]
                       ,[Visibility] = remoteChanges.[Visibility]
                       ,[DefaultValueType] = remoteChanges.[DefaultValueType]
                       ,[DefaultValue] = remoteChanges.[DefaultValue]

            WHEN NOT MATCHED THEN
                INSERT ([AliasID],[DbName],[DisplayOrder],[DisplayName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Required],[Virtual],[TransactionAliasFieldGuid],[LookupTransactionFieldTypeIndex],[TransactionAliasGuid],[UserGroupGuid],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValueType],[DefaultValue])
                    VALUES (@AliasID,@DbName,@DisplayOrder,@DisplayName,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@Required,@Virtual,@TransactionAliasFieldGuid,@LookupTransactionFieldTypeIndex,@TransactionAliasGuid,@UserGroupGuid,@DispatchField,@ClearOnNew,@ReadOnly,@Visibility,@DefaultValueType,@DefaultValue)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionAliasFieldGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionAliasFieldGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionAliasFieldGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTransactionAliasFields] WHERE TransactionAliasFieldGuid = @TransactionAliasFieldGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
