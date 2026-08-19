-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExportResultDetails
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblExportResultDetails]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@RecordID nvarchar(64),
@Fail bit,
@TransVersion bigint,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@Error nvarchar(250),
@ExportResultDetailGuid uniqueidentifier,
@ExportResultGuid uniqueidentifier,
@InterfaceData01 nvarchar(100),
@InterfaceData02 nvarchar(100),
@InterfaceData03 nvarchar(100),
@InterfaceData04 nvarchar(100),
@InterfaceData05 nvarchar(100),
@InterfaceData06 nvarchar(100),
@InterfaceData07 nvarchar(100),
@InterfaceData08 nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblExportResultDetails varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblExportResultDetails] CT
                        WHERE CT.PK_ExportResultDetailGuid = @ExportResultDetailGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblExportResultDetails].[RecordID],[dbo].[tblExportResultDetails].[Fail],[dbo].[tblExportResultDetails].[TransVersion],[dbo].[tblExportResultDetails].[CreatedDate],[dbo].[tblExportResultDetails].[CreatedBy],[dbo].[tblExportResultDetails].[UpdatedDate],[dbo].[tblExportResultDetails].[UpdatedBy],[dbo].[tblExportResultDetails].[Error],[dbo].[tblExportResultDetails].[ExportResultDetailGuid],[dbo].[tblExportResultDetails].[ExportResultGuid],[dbo].[tblExportResultDetails].[InterfaceData01],[dbo].[tblExportResultDetails].[InterfaceData02],[dbo].[tblExportResultDetails].[InterfaceData03],[dbo].[tblExportResultDetails].[InterfaceData04],[dbo].[tblExportResultDetails].[InterfaceData05],[dbo].[tblExportResultDetails].[InterfaceData06],[dbo].[tblExportResultDetails].[InterfaceData07],[dbo].[tblExportResultDetails].[InterfaceData08]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblExportResultDetails]
                        INNER JOIN [track].[tblExportResultDetails] CT
                            ON CT.PK_ExportResultDetailGuid = [dbo].[tblExportResultDetails].[ExportResultDetailGuid] 
                    WHERE CT.PK_ExportResultDetailGuid = @ExportResultDetailGuid
            ) MERGE existingData
            USING (SELECT @RecordID,@Fail,@TransVersion,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@Error,@ExportResultDetailGuid,@ExportResultGuid,@InterfaceData01,@InterfaceData02,@InterfaceData03,@InterfaceData04,@InterfaceData05,@InterfaceData06,@InterfaceData07,@InterfaceData08
                    ) AS remoteChanges ([RecordID],[Fail],[TransVersion],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Error],[ExportResultDetailGuid],[ExportResultGuid],[InterfaceData01],[InterfaceData02],[InterfaceData03],[InterfaceData04],[InterfaceData05],[InterfaceData06],[InterfaceData07],[InterfaceData08])
            ON (existingData.[ExportResultDetailGuid] = remoteChanges.[ExportResultDetailGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [RecordID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RecordID'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[RecordID] ELSE remoteChanges.[RecordID] END
                       ,[Fail] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Fail'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[Fail] ELSE remoteChanges.[Fail] END
                       ,[TransVersion] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransVersion'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[TransVersion] ELSE remoteChanges.[TransVersion] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[Error] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Error'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[Error] ELSE remoteChanges.[Error] END
                       ,[ExportResultGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExportResultGuid'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[ExportResultGuid] ELSE remoteChanges.[ExportResultGuid] END
                       ,[InterfaceData01] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData01'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[InterfaceData01] ELSE remoteChanges.[InterfaceData01] END
                       ,[InterfaceData02] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData02'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[InterfaceData02] ELSE remoteChanges.[InterfaceData02] END
                       ,[InterfaceData03] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData03'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[InterfaceData03] ELSE remoteChanges.[InterfaceData03] END
                       ,[InterfaceData04] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData04'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[InterfaceData04] ELSE remoteChanges.[InterfaceData04] END
                       ,[InterfaceData05] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData05'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[InterfaceData05] ELSE remoteChanges.[InterfaceData05] END
                       ,[InterfaceData06] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData06'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[InterfaceData06] ELSE remoteChanges.[InterfaceData06] END
                       ,[InterfaceData07] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData07'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[InterfaceData07] ELSE remoteChanges.[InterfaceData07] END
                       ,[InterfaceData08] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData08'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN existingData.[InterfaceData08] ELSE remoteChanges.[InterfaceData08] END

            WHEN NOT MATCHED THEN
                INSERT ([RecordID],[Fail],[TransVersion],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Error],[ExportResultDetailGuid],[ExportResultGuid],[InterfaceData01],[InterfaceData02],[InterfaceData03],[InterfaceData04],[InterfaceData05],[InterfaceData06],[InterfaceData07],[InterfaceData08])
                    VALUES (@RecordID,@Fail,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransVersion'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN NULL ELSE @TransVersion END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Error'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN NULL ELSE @Error END),@ExportResultDetailGuid,@ExportResultGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData01'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN NULL ELSE @InterfaceData01 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData02'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN NULL ELSE @InterfaceData02 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData03'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN NULL ELSE @InterfaceData03 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData04'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN NULL ELSE @InterfaceData04 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData05'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN NULL ELSE @InterfaceData05 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData06'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN NULL ELSE @InterfaceData06 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData07'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN NULL ELSE @InterfaceData07 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InterfaceData08'), @sync_supported_columns_tblExportResultDetails)) WHEN 0 THEN NULL ELSE @InterfaceData08 END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExportResultDetailGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExportResultDetailGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExportResultDetailGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblExportResultDetails] WHERE ExportResultDetailGuid = @ExportResultDetailGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
