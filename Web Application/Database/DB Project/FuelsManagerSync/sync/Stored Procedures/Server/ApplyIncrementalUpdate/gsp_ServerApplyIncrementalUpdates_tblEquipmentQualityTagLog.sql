-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEquipmentQualityTagLog
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblEquipmentQualityTagLog]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@QualityTagName nvarchar(50),
@EquipmentID nvarchar(50),
@EquipmentType nvarchar(50),
@TaggedDate datetimeoffset(7),
@TaggedBy nvarchar(50),
@Memo nvarchar(1000),
@RemovedDate datetimeoffset(7),
@RemovedBy nvarchar(255),
@DeleteFlag bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@TagNumber int,
@EquipmentQualityTagLogGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@EquipmentGuid uniqueidentifier,
@QualityTagGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblEquipmentQualityTagLog varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblEquipmentQualityTagLog] CT
                        WHERE CT.PK_EquipmentQualityTagLogGuid = @EquipmentQualityTagLogGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblEquipmentQualityTagLog].[QualityTagName],[dbo].[tblEquipmentQualityTagLog].[EquipmentID],[dbo].[tblEquipmentQualityTagLog].[EquipmentType],[dbo].[tblEquipmentQualityTagLog].[TaggedDate],[dbo].[tblEquipmentQualityTagLog].[TaggedBy],[dbo].[tblEquipmentQualityTagLog].[Memo],[dbo].[tblEquipmentQualityTagLog].[RemovedDate],[dbo].[tblEquipmentQualityTagLog].[RemovedBy],[dbo].[tblEquipmentQualityTagLog].[DeleteFlag],[dbo].[tblEquipmentQualityTagLog].[CreatedDate],[dbo].[tblEquipmentQualityTagLog].[CreatedBy],[dbo].[tblEquipmentQualityTagLog].[UpdatedDate],[dbo].[tblEquipmentQualityTagLog].[UpdatedBy],[dbo].[tblEquipmentQualityTagLog].[TagNumber],[dbo].[tblEquipmentQualityTagLog].[EquipmentQualityTagLogGuid],[dbo].[tblEquipmentQualityTagLog].[SiteGuid],[dbo].[tblEquipmentQualityTagLog].[EquipmentGuid],[dbo].[tblEquipmentQualityTagLog].[QualityTagGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblEquipmentQualityTagLog]
                        INNER JOIN [track].[tblEquipmentQualityTagLog] CT
                            ON CT.PK_EquipmentQualityTagLogGuid = [dbo].[tblEquipmentQualityTagLog].[EquipmentQualityTagLogGuid] 
                    WHERE CT.PK_EquipmentQualityTagLogGuid = @EquipmentQualityTagLogGuid
            ) MERGE existingData
            USING (SELECT @QualityTagName,@EquipmentID,@EquipmentType,@TaggedDate,@TaggedBy,@Memo,@RemovedDate,@RemovedBy,@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@TagNumber,@EquipmentQualityTagLogGuid,@SiteGuid,@EquipmentGuid,@QualityTagGuid
                    ) AS remoteChanges ([QualityTagName],[EquipmentID],[EquipmentType],[TaggedDate],[TaggedBy],[Memo],[RemovedDate],[RemovedBy],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TagNumber],[EquipmentQualityTagLogGuid],[SiteGuid],[EquipmentGuid],[QualityTagGuid])
            ON (existingData.[EquipmentQualityTagLogGuid] = remoteChanges.[EquipmentQualityTagLogGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [QualityTagName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QualityTagName'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[QualityTagName] ELSE remoteChanges.[QualityTagName] END
                       ,[EquipmentID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentID'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[EquipmentID] ELSE remoteChanges.[EquipmentID] END
                       ,[EquipmentType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentType'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[EquipmentType] ELSE remoteChanges.[EquipmentType] END
                       ,[TaggedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TaggedDate'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[TaggedDate] ELSE remoteChanges.[TaggedDate] END
                       ,[TaggedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TaggedBy'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[TaggedBy] ELSE remoteChanges.[TaggedBy] END
                       ,[Memo] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Memo'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[Memo] ELSE remoteChanges.[Memo] END
                       ,[RemovedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RemovedDate'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[RemovedDate] ELSE remoteChanges.[RemovedDate] END
                       ,[RemovedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RemovedBy'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[RemovedBy] ELSE remoteChanges.[RemovedBy] END
                       ,[DeleteFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeleteFlag'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[DeleteFlag] ELSE remoteChanges.[DeleteFlag] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[TagNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TagNumber'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[TagNumber] ELSE remoteChanges.[TagNumber] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[EquipmentGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentGuid'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[EquipmentGuid] ELSE remoteChanges.[EquipmentGuid] END
                       ,[QualityTagGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QualityTagGuid'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN existingData.[QualityTagGuid] ELSE remoteChanges.[QualityTagGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([QualityTagName],[EquipmentID],[EquipmentType],[TaggedDate],[TaggedBy],[Memo],[RemovedDate],[RemovedBy],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TagNumber],[EquipmentQualityTagLogGuid],[SiteGuid],[EquipmentGuid],[QualityTagGuid])
                    VALUES (@QualityTagName,@EquipmentID,@EquipmentType,@TaggedDate,@TaggedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Memo'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN NULL ELSE @Memo END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RemovedDate'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN NULL ELSE @RemovedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RemovedBy'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN NULL ELSE @RemovedBy END),@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TagNumber'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN NULL ELSE @TagNumber END),@EquipmentQualityTagLogGuid,@SiteGuid,@EquipmentGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QualityTagGuid'), @sync_supported_columns_tblEquipmentQualityTagLog)) WHEN 0 THEN NULL ELSE @QualityTagGuid END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EquipmentQualityTagLogGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EquipmentQualityTagLogGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EquipmentQualityTagLogGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblEquipmentQualityTagLog] WHERE EquipmentQualityTagLogGuid = @EquipmentQualityTagLogGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
