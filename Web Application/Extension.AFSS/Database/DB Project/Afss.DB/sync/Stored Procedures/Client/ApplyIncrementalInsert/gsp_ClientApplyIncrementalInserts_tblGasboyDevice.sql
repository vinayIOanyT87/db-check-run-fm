-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyDevice
-- Description:	Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblGasboyDevice]
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
@sync_table_name nvarchar(512)
AS
BEGIN
    ;   MERGE [dbo].[tblGasboyDevice] AS existingData
        USING (SELECT @GasboyDeviceGuid 'GasboyDeviceGuid',@SiteGuid 'SiteGuid',@GasboyDepartmentGuid 'GasboyDepartmentGuid',@DeviceCode 'DeviceCode',@DeviceName 'DeviceName',@CardNumber 'CardNumber',@GroupRuleName 'GroupRuleName',@LookupGasboyDeviceTypeIndex 'LookupGasboyDeviceTypeIndex',@LookupGasboyRecordStatusIndex 'LookupGasboyRecordStatusIndex',@LookupGasboyHardwareTypeIndex 'LookupGasboyHardwareTypeIndex',@LookupGasboyAuthTypeIndex 'LookupGasboyAuthTypeIndex',@LookupGasboyEmployeeTypeIndex 'LookupGasboyEmployeeTypeIndex',@LookupGasboyTwoStageDriverValidationTypeIndex 'LookupGasboyTwoStageDriverValidationTypeIndex',@UsePINCodeFlag 'UsePINCodeFlag',@PINCode 'PINCode',@AuthPINFrom 'AuthPINFrom',@VehiclePlate 'VehiclePlate',@PromptForVehiclePlateFlag 'PromptForVehiclePlateFlag',@LookupGasboyVehiclePlateCheckTypeIndex 'LookupGasboyVehiclePlateCheckTypeIndex',@AlwaysPromptForAdditionalValidationFlag 'AlwaysPromptForAdditionalValidationFlag',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate',@DeviceID 'DeviceID'
                ) AS remoteChanges ([GasboyDeviceGuid],[SiteGuid],[GasboyDepartmentGuid],[DeviceCode],[DeviceName],[CardNumber],[GroupRuleName],[LookupGasboyDeviceTypeIndex],[LookupGasboyRecordStatusIndex],[LookupGasboyHardwareTypeIndex],[LookupGasboyAuthTypeIndex],[LookupGasboyEmployeeTypeIndex],[LookupGasboyTwoStageDriverValidationTypeIndex],[UsePINCodeFlag],[PINCode],[AuthPINFrom],[VehiclePlate],[PromptForVehiclePlateFlag],[LookupGasboyVehiclePlateCheckTypeIndex],[AlwaysPromptForAdditionalValidationFlag],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[DeviceID])
        ON (existingData.[GasboyDeviceGuid] = remoteChanges.[GasboyDeviceGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [SiteGuid] = remoteChanges.[SiteGuid]
                       ,[GasboyDepartmentGuid] = remoteChanges.[GasboyDepartmentGuid]
                       ,[DeviceCode] = remoteChanges.[DeviceCode]
                       ,[DeviceName] = remoteChanges.[DeviceName]
                       ,[CardNumber] = remoteChanges.[CardNumber]
                       ,[GroupRuleName] = remoteChanges.[GroupRuleName]
                       ,[LookupGasboyDeviceTypeIndex] = remoteChanges.[LookupGasboyDeviceTypeIndex]
                       ,[LookupGasboyRecordStatusIndex] = remoteChanges.[LookupGasboyRecordStatusIndex]
                       ,[LookupGasboyHardwareTypeIndex] = remoteChanges.[LookupGasboyHardwareTypeIndex]
                       ,[LookupGasboyAuthTypeIndex] = remoteChanges.[LookupGasboyAuthTypeIndex]
                       ,[LookupGasboyEmployeeTypeIndex] = remoteChanges.[LookupGasboyEmployeeTypeIndex]
                       ,[LookupGasboyTwoStageDriverValidationTypeIndex] = remoteChanges.[LookupGasboyTwoStageDriverValidationTypeIndex]
                       ,[UsePINCodeFlag] = remoteChanges.[UsePINCodeFlag]
                       ,[PINCode] = remoteChanges.[PINCode]
                       ,[AuthPINFrom] = remoteChanges.[AuthPINFrom]
                       ,[VehiclePlate] = remoteChanges.[VehiclePlate]
                       ,[PromptForVehiclePlateFlag] = remoteChanges.[PromptForVehiclePlateFlag]
                       ,[LookupGasboyVehiclePlateCheckTypeIndex] = remoteChanges.[LookupGasboyVehiclePlateCheckTypeIndex]
                       ,[AlwaysPromptForAdditionalValidationFlag] = remoteChanges.[AlwaysPromptForAdditionalValidationFlag]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[DeviceID] = remoteChanges.[DeviceID]

        WHEN NOT MATCHED THEN
            INSERT ([GasboyDeviceGuid],[SiteGuid],[GasboyDepartmentGuid],[DeviceCode],[DeviceName],[CardNumber],[GroupRuleName],[LookupGasboyDeviceTypeIndex],[LookupGasboyRecordStatusIndex],[LookupGasboyHardwareTypeIndex],[LookupGasboyAuthTypeIndex],[LookupGasboyEmployeeTypeIndex],[LookupGasboyTwoStageDriverValidationTypeIndex],[UsePINCodeFlag],[PINCode],[AuthPINFrom],[VehiclePlate],[PromptForVehiclePlateFlag],[LookupGasboyVehiclePlateCheckTypeIndex],[AlwaysPromptForAdditionalValidationFlag],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[DeviceID])
                VALUES (@GasboyDeviceGuid,@SiteGuid,@GasboyDepartmentGuid,@DeviceCode,@DeviceName,@CardNumber,@GroupRuleName,@LookupGasboyDeviceTypeIndex,@LookupGasboyRecordStatusIndex,@LookupGasboyHardwareTypeIndex,@LookupGasboyAuthTypeIndex,@LookupGasboyEmployeeTypeIndex,@LookupGasboyTwoStageDriverValidationTypeIndex,@UsePINCodeFlag,@PINCode,@AuthPINFrom,@VehiclePlate,@PromptForVehiclePlateFlag,@LookupGasboyVehiclePlateCheckTypeIndex,@AlwaysPromptForAdditionalValidationFlag,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@DeviceID)
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
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
