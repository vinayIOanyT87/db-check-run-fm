-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : erv.tblEntitySegmentTemplate
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblEntitySegmentTemplate]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@EntitySegmentTemplateGuid uniqueidentifier,
@AppTableName nvarchar(100),
@EntityIndexFieldName nvarchar(100),
@EntityTypeId nvarchar(100),
@EntityTypeDisplayName nvarchar(100),
@FilterFieldName nvarchar(100),
@FilterDisplayName nvarchar(100),
@FilterValuesStoredProc nvarchar(100),
@FieldLevelConfigSegment bit,
@LocationBasedConstraintSegment bit,
@SystemSegment bit,
@EntityAssignmentTableName nvarchar(100),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblEntitySegmentTemplate varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblEntitySegmentTemplate] CT
                        WHERE CT.PK_EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [erv].[tblEntitySegmentTemplate].[EntitySegmentTemplateGuid],[erv].[tblEntitySegmentTemplate].[AppTableName],[erv].[tblEntitySegmentTemplate].[EntityIndexFieldName],[erv].[tblEntitySegmentTemplate].[EntityTypeId],[erv].[tblEntitySegmentTemplate].[EntityTypeDisplayName],[erv].[tblEntitySegmentTemplate].[FilterFieldName],[erv].[tblEntitySegmentTemplate].[FilterDisplayName],[erv].[tblEntitySegmentTemplate].[FilterValuesStoredProc],[erv].[tblEntitySegmentTemplate].[FieldLevelConfigSegment],[erv].[tblEntitySegmentTemplate].[LocationBasedConstraintSegment],[erv].[tblEntitySegmentTemplate].[SystemSegment],[erv].[tblEntitySegmentTemplate].[EntityAssignmentTableName],[erv].[tblEntitySegmentTemplate].[CreatedDate],[erv].[tblEntitySegmentTemplate].[CreatedBy],[erv].[tblEntitySegmentTemplate].[UpdatedDate],[erv].[tblEntitySegmentTemplate].[UpdatedBy]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [erv].[tblEntitySegmentTemplate]
                        INNER JOIN [track].[tblEntitySegmentTemplate] CT
                            ON CT.PK_EntitySegmentTemplateGuid = [erv].[tblEntitySegmentTemplate].[EntitySegmentTemplateGuid] 
                    WHERE CT.PK_EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid
            ) MERGE existingData
            USING (SELECT @EntitySegmentTemplateGuid,@AppTableName,@EntityIndexFieldName,@EntityTypeId,@EntityTypeDisplayName,@FilterFieldName,@FilterDisplayName,@FilterValuesStoredProc,@FieldLevelConfigSegment,@LocationBasedConstraintSegment,@SystemSegment,@EntityAssignmentTableName,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy
                    ) AS remoteChanges ([EntitySegmentTemplateGuid],[AppTableName],[EntityIndexFieldName],[EntityTypeId],[EntityTypeDisplayName],[FilterFieldName],[FilterDisplayName],[FilterValuesStoredProc],[FieldLevelConfigSegment],[LocationBasedConstraintSegment],[SystemSegment],[EntityAssignmentTableName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
            ON (existingData.[EntitySegmentTemplateGuid] = remoteChanges.[EntitySegmentTemplateGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [AppTableName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AppTableName'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[AppTableName] ELSE remoteChanges.[AppTableName] END
                       ,[EntityIndexFieldName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EntityIndexFieldName'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[EntityIndexFieldName] ELSE remoteChanges.[EntityIndexFieldName] END
                       ,[EntityTypeId] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EntityTypeId'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[EntityTypeId] ELSE remoteChanges.[EntityTypeId] END
                       ,[EntityTypeDisplayName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EntityTypeDisplayName'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[EntityTypeDisplayName] ELSE remoteChanges.[EntityTypeDisplayName] END
                       ,[FilterFieldName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FilterFieldName'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[FilterFieldName] ELSE remoteChanges.[FilterFieldName] END
                       ,[FilterDisplayName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FilterDisplayName'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[FilterDisplayName] ELSE remoteChanges.[FilterDisplayName] END
                       ,[FilterValuesStoredProc] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FilterValuesStoredProc'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[FilterValuesStoredProc] ELSE remoteChanges.[FilterValuesStoredProc] END
                       ,[FieldLevelConfigSegment] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FieldLevelConfigSegment'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[FieldLevelConfigSegment] ELSE remoteChanges.[FieldLevelConfigSegment] END
                       ,[LocationBasedConstraintSegment] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LocationBasedConstraintSegment'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[LocationBasedConstraintSegment] ELSE remoteChanges.[LocationBasedConstraintSegment] END
                       ,[SystemSegment] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SystemSegment'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[SystemSegment] ELSE remoteChanges.[SystemSegment] END
                       ,[EntityAssignmentTableName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EntityAssignmentTableName'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[EntityAssignmentTableName] ELSE remoteChanges.[EntityAssignmentTableName] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

            WHEN NOT MATCHED THEN
                INSERT ([EntitySegmentTemplateGuid],[AppTableName],[EntityIndexFieldName],[EntityTypeId],[EntityTypeDisplayName],[FilterFieldName],[FilterDisplayName],[FilterValuesStoredProc],[FieldLevelConfigSegment],[LocationBasedConstraintSegment],[SystemSegment],[EntityAssignmentTableName],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                    VALUES (@EntitySegmentTemplateGuid,@AppTableName,@EntityIndexFieldName,@EntityTypeId,@EntityTypeDisplayName,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FilterFieldName'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN NULL ELSE @FilterFieldName END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FilterDisplayName'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN NULL ELSE @FilterDisplayName END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FilterValuesStoredProc'), @sync_supported_columns_tblEntitySegmentTemplate)) WHEN 0 THEN NULL ELSE @FilterValuesStoredProc END),@FieldLevelConfigSegment,@LocationBasedConstraintSegment,@SystemSegment,@EntityAssignmentTableName,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EntitySegmentTemplateGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EntitySegmentTemplateGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EntitySegmentTemplateGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [erv].[tblEntitySegmentTemplate] WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
