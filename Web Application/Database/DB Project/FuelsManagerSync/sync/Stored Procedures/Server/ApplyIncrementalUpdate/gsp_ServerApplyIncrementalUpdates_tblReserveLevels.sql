-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblReserveLevels
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblReserveLevels]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@MinimumLevel float,
@WarningLevel float,
@ReserveLevelGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblReserveLevels varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblReserveLevels] CT
                        WHERE CT.PK_ReserveLevelGuid = @ReserveLevelGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblReserveLevels].[MinimumLevel],[dbo].[tblReserveLevels].[WarningLevel],[dbo].[tblReserveLevels].[ReserveLevelGuid],[dbo].[tblReserveLevels].[SiteGuid],[dbo].[tblReserveLevels].[ProductGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblReserveLevels]
                        INNER JOIN [track].[tblReserveLevels] CT
                            ON CT.PK_ReserveLevelGuid = [dbo].[tblReserveLevels].[ReserveLevelGuid] 
                    WHERE CT.PK_ReserveLevelGuid = @ReserveLevelGuid
            ) MERGE existingData
            USING (SELECT @MinimumLevel,@WarningLevel,@ReserveLevelGuid,@SiteGuid,@ProductGuid
                    ) AS remoteChanges ([MinimumLevel],[WarningLevel],[ReserveLevelGuid],[SiteGuid],[ProductGuid])
            ON (existingData.[ReserveLevelGuid] = remoteChanges.[ReserveLevelGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [MinimumLevel] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MinimumLevel'), @sync_supported_columns_tblReserveLevels)) WHEN 0 THEN existingData.[MinimumLevel] ELSE remoteChanges.[MinimumLevel] END
                       ,[WarningLevel] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WarningLevel'), @sync_supported_columns_tblReserveLevels)) WHEN 0 THEN existingData.[WarningLevel] ELSE remoteChanges.[WarningLevel] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblReserveLevels)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblReserveLevels)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([MinimumLevel],[WarningLevel],[ReserveLevelGuid],[SiteGuid],[ProductGuid])
                    VALUES ((CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MinimumLevel'), @sync_supported_columns_tblReserveLevels)) WHEN 0 THEN NULL ELSE @MinimumLevel END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WarningLevel'), @sync_supported_columns_tblReserveLevels)) WHEN 0 THEN NULL ELSE @WarningLevel END),@ReserveLevelGuid,@SiteGuid,@ProductGuid)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ReserveLevelGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ReserveLevelGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ReserveLevelGuid)
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
