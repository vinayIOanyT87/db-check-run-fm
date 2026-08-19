-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExternalStation
-- Description:	Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblExternalStation]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ExternalStationGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ID nvarchar(50),
@LookupExternalStationTypeIndex int,
@BillingID nvarchar(50),
@DownloadTransactionsAutomatically bit,
@LookupExternalStationStatusIndex int,
@LastSuccessfulConnection datetimeoffset(7),
@LastConnectionAttempt datetimeoffset(7),
@LastTransactionID bigint,
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@LastDeviceCount int,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblExternalStation varchar(8000)
AS
BEGIN
    DECLARE @wasDeleted int
    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [dbo].[tblExternalStation]
                            INNER JOIN [track].[tblExternalStation] CT
                                ON CT.PK_ExternalStationGuid = [dbo].[tblExternalStation].[ExternalStationGuid] 
                        WHERE CT.PK_ExternalStationGuid = @ExternalStationGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblExternalStation].[ExternalStationGuid],[dbo].[tblExternalStation].[SiteGuid],[dbo].[tblExternalStation].[ID],[dbo].[tblExternalStation].[LookupExternalStationTypeIndex],[dbo].[tblExternalStation].[BillingID],[dbo].[tblExternalStation].[DownloadTransactionsAutomatically],[dbo].[tblExternalStation].[LookupExternalStationStatusIndex],[dbo].[tblExternalStation].[LastSuccessfulConnection],[dbo].[tblExternalStation].[LastConnectionAttempt],[dbo].[tblExternalStation].[LastTransactionID],[dbo].[tblExternalStation].[CreatedBy],[dbo].[tblExternalStation].[CreatedDate],[dbo].[tblExternalStation].[UpdatedBy],[dbo].[tblExternalStation].[UpdatedDate],[dbo].[tblExternalStation].[LastDeviceCount]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblExternalStation]
                        INNER JOIN [track].[tblExternalStation] CT
                            ON CT.PK_ExternalStationGuid = [dbo].[tblExternalStation].[ExternalStationGuid] 
                    WHERE CT.PK_ExternalStationGuid = @ExternalStationGuid
            ) MERGE existingData
            USING (SELECT @ExternalStationGuid,@SiteGuid,@ID,@LookupExternalStationTypeIndex,@BillingID,@DownloadTransactionsAutomatically,@LookupExternalStationStatusIndex,@LastSuccessfulConnection,@LastConnectionAttempt,@LastTransactionID,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@LastDeviceCount
                    ) AS remoteChanges ([ExternalStationGuid],[SiteGuid],[ID],[LookupExternalStationTypeIndex],[BillingID],[DownloadTransactionsAutomatically],[LookupExternalStationStatusIndex],[LastSuccessfulConnection],[LastConnectionAttempt],[LastTransactionID],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[LastDeviceCount])
            ON (existingData.[ExternalStationGuid] = remoteChanges.[ExternalStationGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[LookupExternalStationTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupExternalStationTypeIndex'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[LookupExternalStationTypeIndex] ELSE remoteChanges.[LookupExternalStationTypeIndex] END
                       ,[BillingID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BillingID'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[BillingID] ELSE remoteChanges.[BillingID] END
                       ,[DownloadTransactionsAutomatically] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DownloadTransactionsAutomatically'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[DownloadTransactionsAutomatically] ELSE remoteChanges.[DownloadTransactionsAutomatically] END
                       ,[LookupExternalStationStatusIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupExternalStationStatusIndex'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[LookupExternalStationStatusIndex] ELSE remoteChanges.[LookupExternalStationStatusIndex] END
                       ,[LastSuccessfulConnection] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastSuccessfulConnection'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[LastSuccessfulConnection] ELSE remoteChanges.[LastSuccessfulConnection] END
                       ,[LastConnectionAttempt] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastConnectionAttempt'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[LastConnectionAttempt] ELSE remoteChanges.[LastConnectionAttempt] END
                       ,[LastTransactionID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastTransactionID'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[LastTransactionID] ELSE remoteChanges.[LastTransactionID] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[LastDeviceCount] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastDeviceCount'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN existingData.[LastDeviceCount] ELSE remoteChanges.[LastDeviceCount] END

            WHEN NOT MATCHED THEN
                INSERT ([ExternalStationGuid],[SiteGuid],[ID],[LookupExternalStationTypeIndex],[BillingID],[DownloadTransactionsAutomatically],[LookupExternalStationStatusIndex],[LastSuccessfulConnection],[LastConnectionAttempt],[LastTransactionID],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[LastDeviceCount])
                    VALUES (@ExternalStationGuid,@SiteGuid,@ID,@LookupExternalStationTypeIndex,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BillingID'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN NULL ELSE @BillingID END),@DownloadTransactionsAutomatically,@LookupExternalStationStatusIndex,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastSuccessfulConnection'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN NULL ELSE @LastSuccessfulConnection END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastConnectionAttempt'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN NULL ELSE @LastConnectionAttempt END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastTransactionID'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN NULL ELSE @LastTransactionID END),@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastDeviceCount'), @sync_supported_columns_tblExternalStation)) WHEN 0 THEN NULL ELSE @LastDeviceCount END))
            ;
    END

    SET @sync_row_count = @@rowcount; 

    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExternalStationGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExternalStationGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExternalStationGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblExternalStation] WHERE ExternalStationGuid = @ExternalStationGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END
    
    DECLARE @minValidVersion BigInt 
    SET @minValidVersion = 0;	-- This is used to detect Change Tracking cleanup
					            -- If we support this, we should add a column to SynchronizationTable
								-- that records the MinValidVersion after change tracking information for
								-- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
