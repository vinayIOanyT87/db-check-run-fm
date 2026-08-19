-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableLoadArmInputPermissive
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblProcessVariableLoadArmInputPermissive]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ProcessVariableLoadArmGuid uniqueidentifier,
@LookupProcessVariableTypeIndex int,
@InstanceNumber int,
@LoadArmGuid uniqueidentifier,
@OPCConnectionGuid uniqueidentifier,
@OPCItemID nvarchar(255),
@DataType int,
@ServerEngineeringUnitsIndex int,
@Quality smallint,
@SIValue varbinary(max),
@LookupSIValueVariantTypeIndex int,
@DateTimeStamp datetimeoffset(7),
@Maximum varbinary(max),
@LookupMaximumVariantTypeIndex int,
@Minimum varbinary(max),
@LookupMinimumVariantTypeIndex int,
@DataTypeEnabled bit,
@Input bit,
@InputEnabled bit,
@MessageApplicationStringGuid uniqueidentifier,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblProcessVariableLoadArmInputPermissive varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblProcessVariableLoadArmInputPermissive] CT
                        WHERE CT.PK_ProcessVariableLoadArmGuid = @ProcessVariableLoadArmGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblProcessVariableLoadArmInputPermissive].[ProcessVariableLoadArmGuid],[dbo].[tblProcessVariableLoadArmInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableLoadArmInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableLoadArmInputPermissive].[LoadArmGuid],[dbo].[tblProcessVariableLoadArmInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableLoadArmInputPermissive].[OPCItemID],[dbo].[tblProcessVariableLoadArmInputPermissive].[DataType],[dbo].[tblProcessVariableLoadArmInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableLoadArmInputPermissive].[Quality],[dbo].[tblProcessVariableLoadArmInputPermissive].[SIValue],[dbo].[tblProcessVariableLoadArmInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableLoadArmInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableLoadArmInputPermissive].[Maximum],[dbo].[tblProcessVariableLoadArmInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableLoadArmInputPermissive].[Minimum],[dbo].[tblProcessVariableLoadArmInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableLoadArmInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableLoadArmInputPermissive].[Input],[dbo].[tblProcessVariableLoadArmInputPermissive].[InputEnabled],[dbo].[tblProcessVariableLoadArmInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableLoadArmInputPermissive].[CreatedDate],[dbo].[tblProcessVariableLoadArmInputPermissive].[CreatedBy],[dbo].[tblProcessVariableLoadArmInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableLoadArmInputPermissive].[UpdatedBy]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblProcessVariableLoadArmInputPermissive]
                        INNER JOIN [track].[tblProcessVariableLoadArmInputPermissive] CT
                            ON CT.PK_ProcessVariableLoadArmGuid = [dbo].[tblProcessVariableLoadArmInputPermissive].[ProcessVariableLoadArmGuid] 
                    WHERE CT.PK_ProcessVariableLoadArmGuid = @ProcessVariableLoadArmGuid
            ) MERGE existingData
            USING (SELECT @ProcessVariableLoadArmGuid,@LookupProcessVariableTypeIndex,@InstanceNumber,@LoadArmGuid,@OPCConnectionGuid,@OPCItemID,@DataType,@ServerEngineeringUnitsIndex,@Quality,@SIValue,@LookupSIValueVariantTypeIndex,@DateTimeStamp,@Maximum,@LookupMaximumVariantTypeIndex,@Minimum,@LookupMinimumVariantTypeIndex,@DataTypeEnabled,@Input,@InputEnabled,@MessageApplicationStringGuid,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy
                    ) AS remoteChanges ([ProcessVariableLoadArmGuid],[LookupProcessVariableTypeIndex],[InstanceNumber],[LoadArmGuid],[OPCConnectionGuid],[OPCItemID],[DataType],[ServerEngineeringUnitsIndex],[Quality],[SIValue],[LookupSIValueVariantTypeIndex],[DateTimeStamp],[Maximum],[LookupMaximumVariantTypeIndex],[Minimum],[LookupMinimumVariantTypeIndex],[DataTypeEnabled],[Input],[InputEnabled],[MessageApplicationStringGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
            ON (existingData.[ProcessVariableLoadArmGuid] = remoteChanges.[ProcessVariableLoadArmGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [LookupProcessVariableTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupProcessVariableTypeIndex'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[LookupProcessVariableTypeIndex] ELSE remoteChanges.[LookupProcessVariableTypeIndex] END
                       ,[InstanceNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InstanceNumber'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[InstanceNumber] ELSE remoteChanges.[InstanceNumber] END
                       ,[LoadArmGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadArmGuid'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[LoadArmGuid] ELSE remoteChanges.[LoadArmGuid] END
                       ,[OPCConnectionGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OPCConnectionGuid'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[OPCConnectionGuid] ELSE remoteChanges.[OPCConnectionGuid] END
                       ,[OPCItemID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OPCItemID'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[OPCItemID] ELSE remoteChanges.[OPCItemID] END
                       ,[DataType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DataType'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[DataType] ELSE remoteChanges.[DataType] END
                       ,[ServerEngineeringUnitsIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ServerEngineeringUnitsIndex'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[ServerEngineeringUnitsIndex] ELSE remoteChanges.[ServerEngineeringUnitsIndex] END
                       ,[Quality] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Quality'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[Quality] ELSE remoteChanges.[Quality] END
                       ,[SIValue] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SIValue'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[SIValue] ELSE remoteChanges.[SIValue] END
                       ,[LookupSIValueVariantTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupSIValueVariantTypeIndex'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[LookupSIValueVariantTypeIndex] ELSE remoteChanges.[LookupSIValueVariantTypeIndex] END
                       ,[DateTimeStamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DateTimeStamp'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[DateTimeStamp] ELSE remoteChanges.[DateTimeStamp] END
                       ,[Maximum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Maximum'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[Maximum] ELSE remoteChanges.[Maximum] END
                       ,[LookupMaximumVariantTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupMaximumVariantTypeIndex'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[LookupMaximumVariantTypeIndex] ELSE remoteChanges.[LookupMaximumVariantTypeIndex] END
                       ,[Minimum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Minimum'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[Minimum] ELSE remoteChanges.[Minimum] END
                       ,[LookupMinimumVariantTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupMinimumVariantTypeIndex'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[LookupMinimumVariantTypeIndex] ELSE remoteChanges.[LookupMinimumVariantTypeIndex] END
                       ,[DataTypeEnabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DataTypeEnabled'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[DataTypeEnabled] ELSE remoteChanges.[DataTypeEnabled] END
                       ,[Input] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Input'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[Input] ELSE remoteChanges.[Input] END
                       ,[InputEnabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InputEnabled'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[InputEnabled] ELSE remoteChanges.[InputEnabled] END
                       ,[MessageApplicationStringGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MessageApplicationStringGuid'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[MessageApplicationStringGuid] ELSE remoteChanges.[MessageApplicationStringGuid] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

            WHEN NOT MATCHED THEN
                INSERT ([ProcessVariableLoadArmGuid],[LookupProcessVariableTypeIndex],[InstanceNumber],[LoadArmGuid],[OPCConnectionGuid],[OPCItemID],[DataType],[ServerEngineeringUnitsIndex],[Quality],[SIValue],[LookupSIValueVariantTypeIndex],[DateTimeStamp],[Maximum],[LookupMaximumVariantTypeIndex],[Minimum],[LookupMinimumVariantTypeIndex],[DataTypeEnabled],[Input],[InputEnabled],[MessageApplicationStringGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                    VALUES (@ProcessVariableLoadArmGuid,@LookupProcessVariableTypeIndex,@InstanceNumber,@LoadArmGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OPCConnectionGuid'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @OPCConnectionGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OPCItemID'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @OPCItemID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DataType'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @DataType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ServerEngineeringUnitsIndex'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @ServerEngineeringUnitsIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Quality'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @Quality END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SIValue'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @SIValue END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupSIValueVariantTypeIndex'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @LookupSIValueVariantTypeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DateTimeStamp'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @DateTimeStamp END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Maximum'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @Maximum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupMaximumVariantTypeIndex'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @LookupMaximumVariantTypeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Minimum'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @Minimum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupMinimumVariantTypeIndex'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @LookupMinimumVariantTypeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DataTypeEnabled'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @DataTypeEnabled END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Input'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @Input END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InputEnabled'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @InputEnabled END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MessageApplicationStringGuid'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @MessageApplicationStringGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblProcessVariableLoadArmInputPermissive)) WHEN 0 THEN NULL ELSE @UpdatedBy END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProcessVariableLoadArmGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProcessVariableLoadArmGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProcessVariableLoadArmGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblProcessVariableLoadArmInputPermissive] WHERE ProcessVariableLoadArmGuid = @ProcessVariableLoadArmGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
