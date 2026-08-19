-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : erv.tblEntityRecordVersioningFieldConfig
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblEntityRecordVersioningFieldConfig]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@FieldConfigGuid uniqueidentifier,
@EntitySegmentTemplateGuid uniqueidentifier,
@SiteGroupGuid uniqueidentifier,
@TargetField nvarchar(100),
@IsExternalAttribute bit,
@InternalFieldName nvarchar(100),
@FilterValueGuid uniqueidentifier,
@FilterValueName nvarchar(100),
@InheritedControlMode nvarchar(20),
@ForwardControlMode nvarchar(20),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblEntityRecordVersioningFieldConfig varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblEntityRecordVersioningFieldConfig] CT
                        WHERE CT.PK_FieldConfigGuid = @FieldConfigGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [erv].[tblEntityRecordVersioningFieldConfig].[FieldConfigGuid],[erv].[tblEntityRecordVersioningFieldConfig].[EntitySegmentTemplateGuid],[erv].[tblEntityRecordVersioningFieldConfig].[SiteGroupGuid],[erv].[tblEntityRecordVersioningFieldConfig].[TargetField],[erv].[tblEntityRecordVersioningFieldConfig].[IsExternalAttribute],[erv].[tblEntityRecordVersioningFieldConfig].[InternalFieldName],[erv].[tblEntityRecordVersioningFieldConfig].[FilterValueGuid],[erv].[tblEntityRecordVersioningFieldConfig].[FilterValueName],[erv].[tblEntityRecordVersioningFieldConfig].[InheritedControlMode],[erv].[tblEntityRecordVersioningFieldConfig].[ForwardControlMode],[erv].[tblEntityRecordVersioningFieldConfig].[CreatedDate],[erv].[tblEntityRecordVersioningFieldConfig].[CreatedBy],[erv].[tblEntityRecordVersioningFieldConfig].[UpdatedDate],[erv].[tblEntityRecordVersioningFieldConfig].[UpdatedBy]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [erv].[tblEntityRecordVersioningFieldConfig]
                        INNER JOIN [track].[tblEntityRecordVersioningFieldConfig] CT
                            ON CT.PK_FieldConfigGuid = [erv].[tblEntityRecordVersioningFieldConfig].[FieldConfigGuid] 
                    WHERE CT.PK_FieldConfigGuid = @FieldConfigGuid
            ) MERGE existingData
            USING (SELECT @FieldConfigGuid,@EntitySegmentTemplateGuid,@SiteGroupGuid,@TargetField,@IsExternalAttribute,@InternalFieldName,@FilterValueGuid,@FilterValueName,@InheritedControlMode,@ForwardControlMode,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy
                    ) AS remoteChanges ([FieldConfigGuid],[EntitySegmentTemplateGuid],[SiteGroupGuid],[TargetField],[IsExternalAttribute],[InternalFieldName],[FilterValueGuid],[FilterValueName],[InheritedControlMode],[ForwardControlMode],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
            ON (existingData.[FieldConfigGuid] = remoteChanges.[FieldConfigGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [EntitySegmentTemplateGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EntitySegmentTemplateGuid'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[EntitySegmentTemplateGuid] ELSE remoteChanges.[EntitySegmentTemplateGuid] END
                       ,[SiteGroupGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGroupGuid'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[SiteGroupGuid] ELSE remoteChanges.[SiteGroupGuid] END
                       ,[TargetField] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TargetField'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[TargetField] ELSE remoteChanges.[TargetField] END
                       ,[IsExternalAttribute] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IsExternalAttribute'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[IsExternalAttribute] ELSE remoteChanges.[IsExternalAttribute] END
                       ,[InternalFieldName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InternalFieldName'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[InternalFieldName] ELSE remoteChanges.[InternalFieldName] END
                       ,[FilterValueGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FilterValueGuid'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[FilterValueGuid] ELSE remoteChanges.[FilterValueGuid] END
                       ,[FilterValueName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FilterValueName'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[FilterValueName] ELSE remoteChanges.[FilterValueName] END
                       ,[InheritedControlMode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InheritedControlMode'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[InheritedControlMode] ELSE remoteChanges.[InheritedControlMode] END
                       ,[ForwardControlMode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ForwardControlMode'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[ForwardControlMode] ELSE remoteChanges.[ForwardControlMode] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

            WHEN NOT MATCHED THEN
                INSERT ([FieldConfigGuid],[EntitySegmentTemplateGuid],[SiteGroupGuid],[TargetField],[IsExternalAttribute],[InternalFieldName],[FilterValueGuid],[FilterValueName],[InheritedControlMode],[ForwardControlMode],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                    VALUES (@FieldConfigGuid,@EntitySegmentTemplateGuid,@SiteGroupGuid,@TargetField,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IsExternalAttribute'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN NULL ELSE @IsExternalAttribute END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InternalFieldName'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN NULL ELSE @InternalFieldName END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FilterValueGuid'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN NULL ELSE @FilterValueGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FilterValueName'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN NULL ELSE @FilterValueName END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InheritedControlMode'), @sync_supported_columns_tblEntityRecordVersioningFieldConfig)) WHEN 0 THEN NULL ELSE @InheritedControlMode END),@ForwardControlMode,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @FieldConfigGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @FieldConfigGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @FieldConfigGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [erv].[tblEntityRecordVersioningFieldConfig] WHERE FieldConfigGuid = @FieldConfigGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
