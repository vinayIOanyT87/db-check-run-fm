-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTankQualityTagLog
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblTankQualityTagLog]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@TankID nvarchar(50),
@VesselType nvarchar(50),
@QualityTagName nvarchar(50),
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
@TankQualityTagLogGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupVesselTypeIndex int,
@QualityTagGuid uniqueidentifier,
@TankGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblTankQualityTagLog varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTankQualityTagLog] AS existingData
        USING (SELECT @TankID 'TankID',@VesselType 'VesselType',@QualityTagName 'QualityTagName',@TaggedDate 'TaggedDate',@TaggedBy 'TaggedBy',@Memo 'Memo',@RemovedDate 'RemovedDate',@RemovedBy 'RemovedBy',@DeleteFlag 'DeleteFlag',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@TagNumber 'TagNumber',@TankQualityTagLogGuid 'TankQualityTagLogGuid',@SiteGuid 'SiteGuid',@LookupVesselTypeIndex 'LookupVesselTypeIndex',@QualityTagGuid 'QualityTagGuid',@TankGuid 'TankGuid'
                ) AS remoteChanges ([TankID],[VesselType],[QualityTagName],[TaggedDate],[TaggedBy],[Memo],[RemovedDate],[RemovedBy],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TagNumber],[TankQualityTagLogGuid],[SiteGuid],[LookupVesselTypeIndex],[QualityTagGuid],[TankGuid])
        ON (existingData.[TankQualityTagLogGuid] = remoteChanges.[TankQualityTagLogGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [TankID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankID'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[TankID] ELSE remoteChanges.[TankID] END
                       ,[VesselType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VesselType'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[VesselType] ELSE remoteChanges.[VesselType] END
                       ,[QualityTagName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QualityTagName'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[QualityTagName] ELSE remoteChanges.[QualityTagName] END
                       ,[TaggedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TaggedDate'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[TaggedDate] ELSE remoteChanges.[TaggedDate] END
                       ,[TaggedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TaggedBy'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[TaggedBy] ELSE remoteChanges.[TaggedBy] END
                       ,[Memo] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Memo'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[Memo] ELSE remoteChanges.[Memo] END
                       ,[RemovedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RemovedDate'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[RemovedDate] ELSE remoteChanges.[RemovedDate] END
                       ,[RemovedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RemovedBy'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[RemovedBy] ELSE remoteChanges.[RemovedBy] END
                       ,[DeleteFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeleteFlag'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[DeleteFlag] ELSE remoteChanges.[DeleteFlag] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[TagNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TagNumber'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[TagNumber] ELSE remoteChanges.[TagNumber] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[LookupVesselTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupVesselTypeIndex'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[LookupVesselTypeIndex] ELSE remoteChanges.[LookupVesselTypeIndex] END
                       ,[QualityTagGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QualityTagGuid'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[QualityTagGuid] ELSE remoteChanges.[QualityTagGuid] END
                       ,[TankGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankGuid'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN existingData.[TankGuid] ELSE remoteChanges.[TankGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([TankID],[VesselType],[QualityTagName],[TaggedDate],[TaggedBy],[Memo],[RemovedDate],[RemovedBy],[DeleteFlag],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TagNumber],[TankQualityTagLogGuid],[SiteGuid],[LookupVesselTypeIndex],[QualityTagGuid],[TankGuid])
                VALUES (@TankID,@VesselType,@QualityTagName,@TaggedDate,@TaggedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Memo'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN NULL ELSE @Memo END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RemovedDate'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN NULL ELSE @RemovedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RemovedBy'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN NULL ELSE @RemovedBy END),@DeleteFlag,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TagNumber'), @sync_supported_columns_tblTankQualityTagLog)) WHEN 0 THEN NULL ELSE @TagNumber END),@TankQualityTagLogGuid,@SiteGuid,@LookupVesselTypeIndex,@QualityTagGuid,@TankGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TankQualityTagLogGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TankQualityTagLogGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TankQualityTagLogGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTankQualityTagLog] WHERE TankQualityTagLogGuid = @TankQualityTagLogGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

