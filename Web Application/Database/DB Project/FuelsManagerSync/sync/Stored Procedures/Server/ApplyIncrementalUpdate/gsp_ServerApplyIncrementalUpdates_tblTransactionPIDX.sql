-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionPIDX
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblTransactionPIDX]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@AuthorizationNumber nvarchar(8),
@SentFlag bit,
@DateSent datetimeoffset(7),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@BrokenBlend bit,
@TransactionPIDXGuid uniqueidentifier,
@PIDXProfileGuid uniqueidentifier,
@TransactionGuid uniqueidentifier,
@CompanyPersonnelToShipToBillToGuid uniqueidentifier,
@BOLVersion int,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblTransactionPIDX varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblTransactionPIDX] CT
                        WHERE CT.PK_TransactionPIDXGuid = @TransactionPIDXGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblTransactionPIDX].[AuthorizationNumber],[dbo].[tblTransactionPIDX].[SentFlag],[dbo].[tblTransactionPIDX].[DateSent],[dbo].[tblTransactionPIDX].[CreatedBy],[dbo].[tblTransactionPIDX].[CreatedDate],[dbo].[tblTransactionPIDX].[UpdatedBy],[dbo].[tblTransactionPIDX].[UpdatedDate],[dbo].[tblTransactionPIDX].[BrokenBlend],[dbo].[tblTransactionPIDX].[TransactionPIDXGuid],[dbo].[tblTransactionPIDX].[PIDXProfileGuid],[dbo].[tblTransactionPIDX].[TransactionGuid],[dbo].[tblTransactionPIDX].[CompanyPersonnelToShipToBillToGuid],[dbo].[tblTransactionPIDX].[BOLVersion]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblTransactionPIDX]
                        INNER JOIN [track].[tblTransactionPIDX] CT
                            ON CT.PK_TransactionPIDXGuid = [dbo].[tblTransactionPIDX].[TransactionPIDXGuid] 
                    WHERE CT.PK_TransactionPIDXGuid = @TransactionPIDXGuid
            ) MERGE existingData
            USING (SELECT @AuthorizationNumber,@SentFlag,@DateSent,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@BrokenBlend,@TransactionPIDXGuid,@PIDXProfileGuid,@TransactionGuid,@CompanyPersonnelToShipToBillToGuid,@BOLVersion
                    ) AS remoteChanges ([AuthorizationNumber],[SentFlag],[DateSent],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[BrokenBlend],[TransactionPIDXGuid],[PIDXProfileGuid],[TransactionGuid],[CompanyPersonnelToShipToBillToGuid],[BOLVersion])
            ON (existingData.[TransactionPIDXGuid] = remoteChanges.[TransactionPIDXGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [AuthorizationNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AuthorizationNumber'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[AuthorizationNumber] ELSE remoteChanges.[AuthorizationNumber] END
                       ,[SentFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SentFlag'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[SentFlag] ELSE remoteChanges.[SentFlag] END
                       ,[DateSent] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DateSent'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[DateSent] ELSE remoteChanges.[DateSent] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[BrokenBlend] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BrokenBlend'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[BrokenBlend] ELSE remoteChanges.[BrokenBlend] END
                       ,[PIDXProfileGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PIDXProfileGuid'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[PIDXProfileGuid] ELSE remoteChanges.[PIDXProfileGuid] END
                       ,[TransactionGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionGuid'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[TransactionGuid] ELSE remoteChanges.[TransactionGuid] END
                       ,[CompanyPersonnelToShipToBillToGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyPersonnelToShipToBillToGuid'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[CompanyPersonnelToShipToBillToGuid] ELSE remoteChanges.[CompanyPersonnelToShipToBillToGuid] END
                       ,[BOLVersion] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BOLVersion'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN existingData.[BOLVersion] ELSE remoteChanges.[BOLVersion] END

            WHEN NOT MATCHED THEN
                INSERT ([AuthorizationNumber],[SentFlag],[DateSent],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[BrokenBlend],[TransactionPIDXGuid],[PIDXProfileGuid],[TransactionGuid],[CompanyPersonnelToShipToBillToGuid],[BOLVersion])
                    VALUES ((CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AuthorizationNumber'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN NULL ELSE @AuthorizationNumber END),@SentFlag,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DateSent'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN NULL ELSE @DateSent END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN NULL ELSE @UpdatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BrokenBlend'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN NULL ELSE @BrokenBlend END),@TransactionPIDXGuid,@PIDXProfileGuid,@TransactionGuid,@CompanyPersonnelToShipToBillToGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BOLVersion'), @sync_supported_columns_tblTransactionPIDX)) WHEN 0 THEN NULL ELSE @BOLVersion END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionPIDXGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionPIDXGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionPIDXGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTransactionPIDX] WHERE TransactionPIDXGuid = @TransactionPIDXGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
