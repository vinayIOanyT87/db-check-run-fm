-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyDevice
-- Description:	Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblGasboyDevice]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@GasboyDeviceGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@GasboyDepartmentGuid uniqueidentifier,
@DeviceCode bigint,
@DeviceName nvarchar(50),
@CardNumber nvarchar(50),
@GroupRuleName nvarchar(50),
@LookupGasboyDeviceTypeIndex int,
@LookupGasboyRecordStatusIndex int,
@LookupGasboyHardwareTypeIndex int,
@LookupGasboyAuthTypeIndex int,
@LookupGasboyEmployeeTypeIndex int,
@LookupGasboyTwoStageDriverValidationTypeIndex int,
@UsePINCodeFlag bit,
@PINCode varbinary(256),
@AuthPINFrom tinyint,
@VehiclePlate nvarchar(50),
@PromptForVehiclePlateFlag bit,
@LookupGasboyVehiclePlateCheckTypeIndex int,
@AlwaysPromptForAdditionalValidationFlag tinyint,
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@DeviceID bigint,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblGasboyDevice varchar(8000)
AS
BEGIN
    ;   MERGE [dbo].[tblGasboyDevice] AS existingData
        USING (SELECT @GasboyDeviceGuid 'GasboyDeviceGuid',@SiteGuid 'SiteGuid',@GasboyDepartmentGuid 'GasboyDepartmentGuid',@DeviceCode 'DeviceCode',@DeviceName 'DeviceName',@CardNumber 'CardNumber',@GroupRuleName 'GroupRuleName',@LookupGasboyDeviceTypeIndex 'LookupGasboyDeviceTypeIndex',@LookupGasboyRecordStatusIndex 'LookupGasboyRecordStatusIndex',@LookupGasboyHardwareTypeIndex 'LookupGasboyHardwareTypeIndex',@LookupGasboyAuthTypeIndex 'LookupGasboyAuthTypeIndex',@LookupGasboyEmployeeTypeIndex 'LookupGasboyEmployeeTypeIndex',@LookupGasboyTwoStageDriverValidationTypeIndex 'LookupGasboyTwoStageDriverValidationTypeIndex',@UsePINCodeFlag 'UsePINCodeFlag',@PINCode 'PINCode',@AuthPINFrom 'AuthPINFrom',@VehiclePlate 'VehiclePlate',@PromptForVehiclePlateFlag 'PromptForVehiclePlateFlag',@LookupGasboyVehiclePlateCheckTypeIndex 'LookupGasboyVehiclePlateCheckTypeIndex',@AlwaysPromptForAdditionalValidationFlag 'AlwaysPromptForAdditionalValidationFlag',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate',@DeviceID 'DeviceID'
                ) AS remoteChanges ([GasboyDeviceGuid],[SiteGuid],[GasboyDepartmentGuid],[DeviceCode],[DeviceName],[CardNumber],[GroupRuleName],[LookupGasboyDeviceTypeIndex],[LookupGasboyRecordStatusIndex],[LookupGasboyHardwareTypeIndex],[LookupGasboyAuthTypeIndex],[LookupGasboyEmployeeTypeIndex],[LookupGasboyTwoStageDriverValidationTypeIndex],[UsePINCodeFlag],[PINCode],[AuthPINFrom],[VehiclePlate],[PromptForVehiclePlateFlag],[LookupGasboyVehiclePlateCheckTypeIndex],[AlwaysPromptForAdditionalValidationFlag],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[DeviceID])
        ON (existingData.[GasboyDeviceGuid] = remoteChanges.[GasboyDeviceGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[GasboyDepartmentGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GasboyDepartmentGuid'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[GasboyDepartmentGuid] ELSE remoteChanges.[GasboyDepartmentGuid] END
                       ,[DeviceCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeviceCode'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[DeviceCode] ELSE remoteChanges.[DeviceCode] END
                       ,[DeviceName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeviceName'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[DeviceName] ELSE remoteChanges.[DeviceName] END
                       ,[CardNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CardNumber'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[CardNumber] ELSE remoteChanges.[CardNumber] END
                       ,[GroupRuleName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GroupRuleName'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[GroupRuleName] ELSE remoteChanges.[GroupRuleName] END
                       ,[LookupGasboyDeviceTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyDeviceTypeIndex'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[LookupGasboyDeviceTypeIndex] ELSE remoteChanges.[LookupGasboyDeviceTypeIndex] END
                       ,[LookupGasboyRecordStatusIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyRecordStatusIndex'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[LookupGasboyRecordStatusIndex] ELSE remoteChanges.[LookupGasboyRecordStatusIndex] END
                       ,[LookupGasboyHardwareTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyHardwareTypeIndex'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[LookupGasboyHardwareTypeIndex] ELSE remoteChanges.[LookupGasboyHardwareTypeIndex] END
                       ,[LookupGasboyAuthTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyAuthTypeIndex'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[LookupGasboyAuthTypeIndex] ELSE remoteChanges.[LookupGasboyAuthTypeIndex] END
                       ,[LookupGasboyEmployeeTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyEmployeeTypeIndex'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[LookupGasboyEmployeeTypeIndex] ELSE remoteChanges.[LookupGasboyEmployeeTypeIndex] END
                       ,[LookupGasboyTwoStageDriverValidationTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyTwoStageDriverValidationTypeIndex'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[LookupGasboyTwoStageDriverValidationTypeIndex] ELSE remoteChanges.[LookupGasboyTwoStageDriverValidationTypeIndex] END
                       ,[UsePINCodeFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UsePINCodeFlag'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[UsePINCodeFlag] ELSE remoteChanges.[UsePINCodeFlag] END
                       ,[PINCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PINCode'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[PINCode] ELSE remoteChanges.[PINCode] END
                       ,[AuthPINFrom] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AuthPINFrom'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[AuthPINFrom] ELSE remoteChanges.[AuthPINFrom] END
                       ,[VehiclePlate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VehiclePlate'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[VehiclePlate] ELSE remoteChanges.[VehiclePlate] END
                       ,[PromptForVehiclePlateFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PromptForVehiclePlateFlag'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[PromptForVehiclePlateFlag] ELSE remoteChanges.[PromptForVehiclePlateFlag] END
                       ,[LookupGasboyVehiclePlateCheckTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupGasboyVehiclePlateCheckTypeIndex'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[LookupGasboyVehiclePlateCheckTypeIndex] ELSE remoteChanges.[LookupGasboyVehiclePlateCheckTypeIndex] END
                       ,[AlwaysPromptForAdditionalValidationFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AlwaysPromptForAdditionalValidationFlag'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[AlwaysPromptForAdditionalValidationFlag] ELSE remoteChanges.[AlwaysPromptForAdditionalValidationFlag] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[DeviceID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeviceID'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN existingData.[DeviceID] ELSE remoteChanges.[DeviceID] END

        WHEN NOT MATCHED THEN
            INSERT ([GasboyDeviceGuid],[SiteGuid],[GasboyDepartmentGuid],[DeviceCode],[DeviceName],[CardNumber],[GroupRuleName],[LookupGasboyDeviceTypeIndex],[LookupGasboyRecordStatusIndex],[LookupGasboyHardwareTypeIndex],[LookupGasboyAuthTypeIndex],[LookupGasboyEmployeeTypeIndex],[LookupGasboyTwoStageDriverValidationTypeIndex],[UsePINCodeFlag],[PINCode],[AuthPINFrom],[VehiclePlate],[PromptForVehiclePlateFlag],[LookupGasboyVehiclePlateCheckTypeIndex],[AlwaysPromptForAdditionalValidationFlag],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[DeviceID])
                VALUES (@GasboyDeviceGuid,@SiteGuid,@GasboyDepartmentGuid,@DeviceCode,@DeviceName,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CardNumber'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN NULL ELSE @CardNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GroupRuleName'), @sync_supported_columns_tblGasboyDevice)) WHEN 0 THEN NULL ELSE @GroupRuleName END),@LookupGasboyDeviceTypeIndex,@LookupGasboyRecordStatusIndex,@LookupGasboyHardwareTypeIndex,@LookupGasboyAuthTypeIndex,@LookupGasboyEmployeeTypeIndex,@LookupGasboyTwoStageDriverValidationTypeIndex,@UsePINCodeFlag,@PINCode,@AuthPINFrom,@VehiclePlate,@PromptForVehiclePlateFlag,@LookupGasboyVehiclePlateCheckTypeIndex,@AlwaysPromptForAdditionalValidationFlag,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@DeviceID)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyDeviceGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyDeviceGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyDeviceGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblGasboyDevice] WHERE GasboyDeviceGuid = @GasboyDeviceGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    DECLARE @minValidVersion BigInt
    SET @minValidVersion = 0;	-- This is used to detect Change Tracking cleanup
					            -- If we support this, we should add a column to SynchronizationTable
								-- that records the MinValidVersion after change tracking information for
								-- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
