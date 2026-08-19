-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMessageLog
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblMessageLog]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@MessageLogGuid uniqueidentifier,
@CompanyGuid uniqueidentifier,
@MessageGuid uniqueidentifier,
@PersonnelGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblMessageLog varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblMessageLog] CT
                        WHERE CT.PK_MessageLogGuid = @MessageLogGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblMessageLog].[CreatedDate],[dbo].[tblMessageLog].[CreatedBy],[dbo].[tblMessageLog].[MessageLogGuid],[dbo].[tblMessageLog].[CompanyGuid],[dbo].[tblMessageLog].[MessageGuid],[dbo].[tblMessageLog].[PersonnelGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblMessageLog]
                        INNER JOIN [track].[tblMessageLog] CT
                            ON CT.PK_MessageLogGuid = [dbo].[tblMessageLog].[MessageLogGuid] 
                    WHERE CT.PK_MessageLogGuid = @MessageLogGuid
            ) MERGE existingData
            USING (SELECT @CreatedDate,@CreatedBy,@MessageLogGuid,@CompanyGuid,@MessageGuid,@PersonnelGuid
                    ) AS remoteChanges ([CreatedDate],[CreatedBy],[MessageLogGuid],[CompanyGuid],[MessageGuid],[PersonnelGuid])
            ON (existingData.[MessageLogGuid] = remoteChanges.[MessageLogGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblMessageLog)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblMessageLog)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyGuid'), @sync_supported_columns_tblMessageLog)) WHEN 0 THEN existingData.[CompanyGuid] ELSE remoteChanges.[CompanyGuid] END
                       ,[MessageGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MessageGuid'), @sync_supported_columns_tblMessageLog)) WHEN 0 THEN existingData.[MessageGuid] ELSE remoteChanges.[MessageGuid] END
                       ,[PersonnelGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PersonnelGuid'), @sync_supported_columns_tblMessageLog)) WHEN 0 THEN existingData.[PersonnelGuid] ELSE remoteChanges.[PersonnelGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([CreatedDate],[CreatedBy],[MessageLogGuid],[CompanyGuid],[MessageGuid],[PersonnelGuid])
                    VALUES (@CreatedDate,@CreatedBy,@MessageLogGuid,@CompanyGuid,@MessageGuid,@PersonnelGuid)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MessageLogGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MessageLogGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MessageLogGuid)
        END
        SET NOCOUNT OFF
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
