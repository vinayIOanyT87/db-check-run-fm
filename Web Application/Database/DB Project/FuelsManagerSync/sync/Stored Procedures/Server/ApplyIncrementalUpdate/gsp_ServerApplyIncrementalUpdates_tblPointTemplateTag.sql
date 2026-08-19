-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTemplateTag
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblPointTemplateTag]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(50),
@EngineeringUnitsType int,
@EngineeringUnitsIndex int,
@DecimalPlaces tinyint,
@ServerEngineeringUnitsIndex int,
@ValueType nvarchar(max),
@Value xml,
@Maximum float,
@Minimum float,
@PointTagInputOutputTypeIndex int,
@Input bit,
@AlarmStatus bit,
@ApplyPointTemplateEngineeringUnits bit,
@ApplyPointTemplateDecimalPlaces bit,
@ApplyPointTemplateMaximum bit,
@ApplyPointTemplateMinimum bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@PointTemplateTagGuid uniqueidentifier,
@PointTemplateGuid uniqueidentifier,
@WellKnownIdentityGuid uniqueidentifier,
@AlarmsEnabled bit,
@InhibitInputOutputTypeConfiguration bit,
@InhibitOverride bit,
@Module bit,
@Archived bit,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblPointTemplateTag varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblPointTemplateTag] CT
                        WHERE CT.PK_PointTemplateTagGuid = @PointTemplateTagGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblPointTemplateTag].[ID],[dbo].[tblPointTemplateTag].[EngineeringUnitsType],[dbo].[tblPointTemplateTag].[EngineeringUnitsIndex],[dbo].[tblPointTemplateTag].[DecimalPlaces],[dbo].[tblPointTemplateTag].[ServerEngineeringUnitsIndex],[dbo].[tblPointTemplateTag].[ValueType],[dbo].[tblPointTemplateTag].[Value],[dbo].[tblPointTemplateTag].[Maximum],[dbo].[tblPointTemplateTag].[Minimum],[dbo].[tblPointTemplateTag].[PointTagInputOutputTypeIndex],[dbo].[tblPointTemplateTag].[Input],[dbo].[tblPointTemplateTag].[AlarmStatus],[dbo].[tblPointTemplateTag].[ApplyPointTemplateEngineeringUnits],[dbo].[tblPointTemplateTag].[ApplyPointTemplateDecimalPlaces],[dbo].[tblPointTemplateTag].[ApplyPointTemplateMaximum],[dbo].[tblPointTemplateTag].[ApplyPointTemplateMinimum],[dbo].[tblPointTemplateTag].[CreatedDate],[dbo].[tblPointTemplateTag].[CreatedBy],[dbo].[tblPointTemplateTag].[UpdatedDate],[dbo].[tblPointTemplateTag].[UpdatedBy],[dbo].[tblPointTemplateTag].[PointTemplateTagGuid],[dbo].[tblPointTemplateTag].[PointTemplateGuid],[dbo].[tblPointTemplateTag].[WellKnownIdentityGuid],[dbo].[tblPointTemplateTag].[AlarmsEnabled],[dbo].[tblPointTemplateTag].[InhibitInputOutputTypeConfiguration],[dbo].[tblPointTemplateTag].[InhibitOverride],[dbo].[tblPointTemplateTag].[Module],[dbo].[tblPointTemplateTag].[Archived]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblPointTemplateTag]
                        INNER JOIN [track].[tblPointTemplateTag] CT
                            ON CT.PK_PointTemplateTagGuid = [dbo].[tblPointTemplateTag].[PointTemplateTagGuid] 
                    WHERE CT.PK_PointTemplateTagGuid = @PointTemplateTagGuid
            ) MERGE existingData
            USING (SELECT @ID,@EngineeringUnitsType,@EngineeringUnitsIndex,@DecimalPlaces,@ServerEngineeringUnitsIndex,@ValueType,@Value,@Maximum,@Minimum,@PointTagInputOutputTypeIndex,@Input,@AlarmStatus,@ApplyPointTemplateEngineeringUnits,@ApplyPointTemplateDecimalPlaces,@ApplyPointTemplateMaximum,@ApplyPointTemplateMinimum,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@PointTemplateTagGuid,@PointTemplateGuid,@WellKnownIdentityGuid,@AlarmsEnabled,@InhibitInputOutputTypeConfiguration,@InhibitOverride,@Module,@Archived
                    ) AS remoteChanges ([ID],[EngineeringUnitsType],[EngineeringUnitsIndex],[DecimalPlaces],[ServerEngineeringUnitsIndex],[ValueType],[Value],[Maximum],[Minimum],[PointTagInputOutputTypeIndex],[Input],[AlarmStatus],[ApplyPointTemplateEngineeringUnits],[ApplyPointTemplateDecimalPlaces],[ApplyPointTemplateMaximum],[ApplyPointTemplateMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplateTagGuid],[PointTemplateGuid],[WellKnownIdentityGuid],[AlarmsEnabled],[InhibitInputOutputTypeConfiguration],[InhibitOverride],[Module],[Archived])
            ON (existingData.[PointTemplateTagGuid] = remoteChanges.[PointTemplateTagGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[EngineeringUnitsType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EngineeringUnitsType'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[EngineeringUnitsType] ELSE remoteChanges.[EngineeringUnitsType] END
                       ,[EngineeringUnitsIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EngineeringUnitsIndex'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[EngineeringUnitsIndex] ELSE remoteChanges.[EngineeringUnitsIndex] END
                       ,[DecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DecimalPlaces'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[DecimalPlaces] ELSE remoteChanges.[DecimalPlaces] END
                       ,[ServerEngineeringUnitsIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ServerEngineeringUnitsIndex'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[ServerEngineeringUnitsIndex] ELSE remoteChanges.[ServerEngineeringUnitsIndex] END
                       ,[ValueType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ValueType'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[ValueType] ELSE remoteChanges.[ValueType] END
                       ,[Value] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Value'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[Value] ELSE remoteChanges.[Value] END
                       ,[Maximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Maximum'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[Maximum] ELSE remoteChanges.[Maximum] END
                       ,[Minimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Minimum'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[Minimum] ELSE remoteChanges.[Minimum] END
                       ,[PointTagInputOutputTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointTagInputOutputTypeIndex'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[PointTagInputOutputTypeIndex] ELSE remoteChanges.[PointTagInputOutputTypeIndex] END
                       ,[Input] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Input'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[Input] ELSE remoteChanges.[Input] END
                       ,[AlarmStatus] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmStatus'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[AlarmStatus] ELSE remoteChanges.[AlarmStatus] END
                       ,[ApplyPointTemplateEngineeringUnits] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyPointTemplateEngineeringUnits'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[ApplyPointTemplateEngineeringUnits] ELSE remoteChanges.[ApplyPointTemplateEngineeringUnits] END
                       ,[ApplyPointTemplateDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyPointTemplateDecimalPlaces'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[ApplyPointTemplateDecimalPlaces] ELSE remoteChanges.[ApplyPointTemplateDecimalPlaces] END
                       ,[ApplyPointTemplateMaximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyPointTemplateMaximum'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[ApplyPointTemplateMaximum] ELSE remoteChanges.[ApplyPointTemplateMaximum] END
                       ,[ApplyPointTemplateMinimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyPointTemplateMinimum'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[ApplyPointTemplateMinimum] ELSE remoteChanges.[ApplyPointTemplateMinimum] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[PointTemplateGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointTemplateGuid'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[PointTemplateGuid] ELSE remoteChanges.[PointTemplateGuid] END
                       ,[WellKnownIdentityGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WellKnownIdentityGuid'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[WellKnownIdentityGuid] ELSE remoteChanges.[WellKnownIdentityGuid] END
                       ,[AlarmsEnabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmsEnabled'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[AlarmsEnabled] ELSE remoteChanges.[AlarmsEnabled] END
                       ,[InhibitInputOutputTypeConfiguration] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitInputOutputTypeConfiguration'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[InhibitInputOutputTypeConfiguration] ELSE remoteChanges.[InhibitInputOutputTypeConfiguration] END
                       ,[InhibitOverride] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitOverride'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[InhibitOverride] ELSE remoteChanges.[InhibitOverride] END
                       ,[Module] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Module'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[Module] ELSE remoteChanges.[Module] END
                       ,[Archived] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Archived'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN existingData.[Archived] ELSE remoteChanges.[Archived] END

            WHEN NOT MATCHED THEN
                INSERT ([ID],[EngineeringUnitsType],[EngineeringUnitsIndex],[DecimalPlaces],[ServerEngineeringUnitsIndex],[ValueType],[Value],[Maximum],[Minimum],[PointTagInputOutputTypeIndex],[Input],[AlarmStatus],[ApplyPointTemplateEngineeringUnits],[ApplyPointTemplateDecimalPlaces],[ApplyPointTemplateMaximum],[ApplyPointTemplateMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplateTagGuid],[PointTemplateGuid],[WellKnownIdentityGuid],[AlarmsEnabled],[InhibitInputOutputTypeConfiguration],[InhibitOverride],[Module],[Archived])
                    VALUES (@ID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EngineeringUnitsType'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @EngineeringUnitsType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EngineeringUnitsIndex'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @EngineeringUnitsIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DecimalPlaces'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @DecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ServerEngineeringUnitsIndex'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @ServerEngineeringUnitsIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ValueType'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @ValueType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Value'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @Value END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Maximum'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @Maximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Minimum'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @Minimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointTagInputOutputTypeIndex'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @PointTagInputOutputTypeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Input'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @Input END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlarmStatus'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @AlarmStatus END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyPointTemplateEngineeringUnits'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @ApplyPointTemplateEngineeringUnits END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyPointTemplateDecimalPlaces'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @ApplyPointTemplateDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyPointTemplateMaximum'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @ApplyPointTemplateMaximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyPointTemplateMinimum'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @ApplyPointTemplateMinimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @UpdatedBy END),@PointTemplateTagGuid,@PointTemplateGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WellKnownIdentityGuid'), @sync_supported_columns_tblPointTemplateTag)) WHEN 0 THEN NULL ELSE @WellKnownIdentityGuid END),@AlarmsEnabled,@InhibitInputOutputTypeConfiguration,@InhibitOverride,@Module,@Archived)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateTagGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateTagGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PointTemplateTagGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblPointTemplateTag] WHERE PointTemplateTagGuid = @PointTemplateTagGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
