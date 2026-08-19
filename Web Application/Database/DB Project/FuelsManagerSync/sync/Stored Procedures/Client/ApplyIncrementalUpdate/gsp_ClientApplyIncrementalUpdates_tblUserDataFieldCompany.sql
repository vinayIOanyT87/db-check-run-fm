-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldCompany
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblUserDataFieldCompany]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@UserDataFieldCompanyGuid uniqueidentifier,
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
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblUserDataFieldCompany] CT
                        WHERE CT.PK_UserDataFieldCompanyGuid = @UserDataFieldCompanyGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblUserDataFieldCompany].[UserDataFieldCompanyGuid],[dbo].[tblUserDataFieldCompany].[TransactionAliasGuid],[dbo].[tblUserDataFieldCompany].[SiteGuid],[dbo].[tblUserDataFieldCompany].[Number],[dbo].[tblUserDataFieldCompany].[DisplayOrder],[dbo].[tblUserDataFieldCompany].[DisplayName],[dbo].[tblUserDataFieldCompany].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldCompany].[Required],[dbo].[tblUserDataFieldCompany].[UserGroupGuid],[dbo].[tblUserDataFieldCompany].[CreatedDate],[dbo].[tblUserDataFieldCompany].[CreatedBy],[dbo].[tblUserDataFieldCompany].[UpdatedDate],[dbo].[tblUserDataFieldCompany].[UpdatedBy],[dbo].[tblUserDataFieldCompany].[DispatchField],[dbo].[tblUserDataFieldCompany].[ClearOnNew],[dbo].[tblUserDataFieldCompany].[ReadOnly],[dbo].[tblUserDataFieldCompany].[Visibility],[dbo].[tblUserDataFieldCompany].[DefaultValue]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblUserDataFieldCompany]
                        INNER JOIN [track].[tblUserDataFieldCompany] CT
                            ON CT.PK_UserDataFieldCompanyGuid = [dbo].[tblUserDataFieldCompany].[UserDataFieldCompanyGuid] 
                    WHERE CT.PK_UserDataFieldCompanyGuid = @UserDataFieldCompanyGuid
            ) MERGE existingData
            USING (SELECT @UserDataFieldCompanyGuid,@TransactionAliasGuid,@SiteGuid,@Number,@DisplayOrder,@DisplayName,@LookupUserDataTypeIndex,@Required,@UserGroupGuid,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@DispatchField,@ClearOnNew,@ReadOnly,@Visibility,@DefaultValue
                    ) AS remoteChanges ([UserDataFieldCompanyGuid],[TransactionAliasGuid],[SiteGuid],[Number],[DisplayOrder],[DisplayName],[LookupUserDataTypeIndex],[Required],[UserGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValue])
            ON (existingData.[UserDataFieldCompanyGuid] = remoteChanges.[UserDataFieldCompanyGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
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
                INSERT ([UserDataFieldCompanyGuid],[TransactionAliasGuid],[SiteGuid],[Number],[DisplayOrder],[DisplayName],[LookupUserDataTypeIndex],[Required],[UserGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DispatchField],[ClearOnNew],[ReadOnly],[Visibility],[DefaultValue])
                    VALUES (@UserDataFieldCompanyGuid,@TransactionAliasGuid,@SiteGuid,@Number,@DisplayOrder,@DisplayName,@LookupUserDataTypeIndex,@Required,@UserGroupGuid,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@DispatchField,@ClearOnNew,@ReadOnly,@Visibility,@DefaultValue)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @UserDataFieldCompanyGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @UserDataFieldCompanyGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @UserDataFieldCompanyGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblUserDataFieldCompany] WHERE UserDataFieldCompanyGuid = @UserDataFieldCompanyGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
