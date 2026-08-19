-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationPersonTrainingToEquipmentType
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblQualificationPersonTrainingToEquipmentType]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@QualificationPersonTrainingToEquipmentTypeGuid uniqueidentifier,
@QualificationGuid uniqueidentifier,
@EquipmentTypeGuid uniqueidentifier,
@Sequence int,
@Instructor nvarchar(50),
@DateCompleted datetimeoffset(7),
@DateDue datetimeoffset(7),
@ExpirationDate datetimeoffset(7),
@ID varchar(50),
@Rating nvarchar(20),
@HistoricalRecord bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@SiteGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblQualificationPersonTrainingToEquipmentType varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [map].[tblQualificationPersonTrainingToEquipmentType] AS existingData
        USING (SELECT @QualificationPersonTrainingToEquipmentTypeGuid 'QualificationPersonTrainingToEquipmentTypeGuid',@QualificationGuid 'QualificationGuid',@EquipmentTypeGuid 'EquipmentTypeGuid',@Sequence 'Sequence',@Instructor 'Instructor',@DateCompleted 'DateCompleted',@DateDue 'DateDue',@ExpirationDate 'ExpirationDate',@ID 'ID',@Rating 'Rating',@HistoricalRecord 'HistoricalRecord',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@SiteGuid 'SiteGuid'
                ) AS remoteChanges ([QualificationPersonTrainingToEquipmentTypeGuid],[QualificationGuid],[EquipmentTypeGuid],[Sequence],[Instructor],[DateCompleted],[DateDue],[ExpirationDate],[ID],[Rating],[HistoricalRecord],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SiteGuid])
        ON (existingData.[QualificationPersonTrainingToEquipmentTypeGuid] = remoteChanges.[QualificationPersonTrainingToEquipmentTypeGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [QualificationGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QualificationGuid'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[QualificationGuid] ELSE remoteChanges.[QualificationGuid] END
                       ,[EquipmentTypeGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentTypeGuid'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[EquipmentTypeGuid] ELSE remoteChanges.[EquipmentTypeGuid] END
                       ,[Sequence] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Sequence'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[Sequence] ELSE remoteChanges.[Sequence] END
                       ,[Instructor] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Instructor'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[Instructor] ELSE remoteChanges.[Instructor] END
                       ,[DateCompleted] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DateCompleted'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[DateCompleted] ELSE remoteChanges.[DateCompleted] END
                       ,[DateDue] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DateDue'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[DateDue] ELSE remoteChanges.[DateDue] END
                       ,[ExpirationDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExpirationDate'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[ExpirationDate] ELSE remoteChanges.[ExpirationDate] END
                       ,[ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[Rating] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Rating'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[Rating] ELSE remoteChanges.[Rating] END
                       ,[HistoricalRecord] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HistoricalRecord'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[HistoricalRecord] ELSE remoteChanges.[HistoricalRecord] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
				   ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([QualificationPersonTrainingToEquipmentTypeGuid],[QualificationGuid],[EquipmentTypeGuid],[Sequence],[Instructor],[DateCompleted],[DateDue],[ExpirationDate],[ID],[Rating],[HistoricalRecord],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SiteGuid])
                VALUES (@QualificationPersonTrainingToEquipmentTypeGuid,@QualificationGuid,@EquipmentTypeGuid,@Sequence,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Instructor'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @Instructor END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DateCompleted'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @DateCompleted END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DateDue'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @DateDue END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExpirationDate'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @ExpirationDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @ID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Rating'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @Rating END),@HistoricalRecord,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @UpdatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblQualificationPersonTrainingToEquipmentType)) WHEN 0 THEN NULL ELSE @SiteGuid END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @QualificationPersonTrainingToEquipmentTypeGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @QualificationPersonTrainingToEquipmentTypeGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @QualificationPersonTrainingToEquipmentTypeGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [map].[tblQualificationPersonTrainingToEquipmentType] WHERE QualificationPersonTrainingToEquipmentTypeGuid = @QualificationPersonTrainingToEquipmentTypeGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

