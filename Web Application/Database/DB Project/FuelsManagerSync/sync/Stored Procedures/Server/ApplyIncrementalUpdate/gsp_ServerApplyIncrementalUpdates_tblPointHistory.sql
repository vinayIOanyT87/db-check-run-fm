-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointHistory
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblPointHistory]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@PointHistoryGuid uniqueidentifier,
@UserGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@StartDate datetimeoffset(7),
@IntervalQuantity int,
@IntervalType int,
@RangeQuantity int,
@RangeType int,
@ColumnsDefinition nvarchar(max),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblPointHistory varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblPointHistory] CT
                        WHERE CT.PK_PointHistoryGuid = @PointHistoryGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblPointHistory].[PointHistoryGuid],[dbo].[tblPointHistory].[UserGuid],[dbo].[tblPointHistory].[SiteGuid],[dbo].[tblPointHistory].[StartDate],[dbo].[tblPointHistory].[IntervalQuantity],[dbo].[tblPointHistory].[IntervalType],[dbo].[tblPointHistory].[RangeQuantity],[dbo].[tblPointHistory].[RangeType],[dbo].[tblPointHistory].[ColumnsDefinition],[dbo].[tblPointHistory].[CreatedDate],[dbo].[tblPointHistory].[CreatedBy],[dbo].[tblPointHistory].[UpdatedDate],[dbo].[tblPointHistory].[UpdatedBy]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblPointHistory]
                        INNER JOIN [track].[tblPointHistory] CT
                            ON CT.PK_PointHistoryGuid = [dbo].[tblPointHistory].[PointHistoryGuid] 
                    WHERE CT.PK_PointHistoryGuid = @PointHistoryGuid
            ) MERGE existingData
            USING (SELECT @PointHistoryGuid,@UserGuid,@SiteGuid,@StartDate,@IntervalQuantity,@IntervalType,@RangeQuantity,@RangeType,@ColumnsDefinition,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy
                    ) AS remoteChanges ([PointHistoryGuid],[UserGuid],[SiteGuid],[StartDate],[IntervalQuantity],[IntervalType],[RangeQuantity],[RangeType],[ColumnsDefinition],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
            ON (existingData.[PointHistoryGuid] = remoteChanges.[PointHistoryGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [UserGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserGuid'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[UserGuid] ELSE remoteChanges.[UserGuid] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[StartDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StartDate'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[StartDate] ELSE remoteChanges.[StartDate] END
                       ,[IntervalQuantity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IntervalQuantity'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[IntervalQuantity] ELSE remoteChanges.[IntervalQuantity] END
                       ,[IntervalType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IntervalType'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[IntervalType] ELSE remoteChanges.[IntervalType] END
                       ,[RangeQuantity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RangeQuantity'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[RangeQuantity] ELSE remoteChanges.[RangeQuantity] END
                       ,[RangeType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RangeType'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[RangeType] ELSE remoteChanges.[RangeType] END
                       ,[ColumnsDefinition] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ColumnsDefinition'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[ColumnsDefinition] ELSE remoteChanges.[ColumnsDefinition] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblPointHistory)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

            WHEN NOT MATCHED THEN
                INSERT ([PointHistoryGuid],[UserGuid],[SiteGuid],[StartDate],[IntervalQuantity],[IntervalType],[RangeQuantity],[RangeType],[ColumnsDefinition],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                    VALUES (@PointHistoryGuid,@UserGuid,@SiteGuid,@StartDate,@IntervalQuantity,@IntervalType,@RangeQuantity,@RangeType,@ColumnsDefinition,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointHistoryGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointHistoryGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointHistoryGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblPointHistory] WHERE PointHistoryGuid = @PointHistoryGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
