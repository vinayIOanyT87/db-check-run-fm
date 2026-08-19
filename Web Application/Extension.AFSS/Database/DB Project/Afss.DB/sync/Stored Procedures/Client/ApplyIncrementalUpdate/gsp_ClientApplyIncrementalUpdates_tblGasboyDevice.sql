-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyDevice
-- Description:	Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblGasboyDevice]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
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
    DECLARE @wasDeleted int
    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [dbo].[tblGasboyDevice]
                            INNER JOIN [track].[tblGasboyDevice] CT
                                ON CT.PK_GasboyDeviceGuid = [dbo].[tblGasboyDevice].[GasboyDeviceGuid] 
                        WHERE CT.PK_GasboyDeviceGuid = @GasboyDeviceGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblGasboyDevice].[GasboyDeviceGuid],[dbo].[tblGasboyDevice].[SiteGuid],[dbo].[tblGasboyDevice].[GasboyDepartmentGuid],[dbo].[tblGasboyDevice].[DeviceCode],[dbo].[tblGasboyDevice].[DeviceName],[dbo].[tblGasboyDevice].[CardNumber],[dbo].[tblGasboyDevice].[GroupRuleName],[dbo].[tblGasboyDevice].[LookupGasboyDeviceTypeIndex],[dbo].[tblGasboyDevice].[LookupGasboyRecordStatusIndex],[dbo].[tblGasboyDevice].[LookupGasboyHardwareTypeIndex],[dbo].[tblGasboyDevice].[LookupGasboyAuthTypeIndex],[dbo].[tblGasboyDevice].[LookupGasboyEmployeeTypeIndex],[dbo].[tblGasboyDevice].[LookupGasboyTwoStageDriverValidationTypeIndex],[dbo].[tblGasboyDevice].[UsePINCodeFlag],[dbo].[tblGasboyDevice].[PINCode],[dbo].[tblGasboyDevice].[AuthPINFrom],[dbo].[tblGasboyDevice].[VehiclePlate],[dbo].[tblGasboyDevice].[PromptForVehiclePlateFlag],[dbo].[tblGasboyDevice].[LookupGasboyVehiclePlateCheckTypeIndex],[dbo].[tblGasboyDevice].[AlwaysPromptForAdditionalValidationFlag],[dbo].[tblGasboyDevice].[CreatedBy],[dbo].[tblGasboyDevice].[CreatedDate],[dbo].[tblGasboyDevice].[UpdatedBy],[dbo].[tblGasboyDevice].[UpdatedDate],[dbo].[tblGasboyDevice].[DeviceID]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblGasboyDevice]
                        INNER JOIN [track].[tblGasboyDevice] CT
                            ON CT.PK_GasboyDeviceGuid = [dbo].[tblGasboyDevice].[GasboyDeviceGuid] 
                    WHERE CT.PK_GasboyDeviceGuid = @GasboyDeviceGuid
            ) MERGE existingData
            USING (SELECT @GasboyDeviceGuid,@SiteGuid,@GasboyDepartmentGuid,@DeviceCode,@DeviceName,@CardNumber,@GroupRuleName,@LookupGasboyDeviceTypeIndex,@LookupGasboyRecordStatusIndex,@LookupGasboyHardwareTypeIndex,@LookupGasboyAuthTypeIndex,@LookupGasboyEmployeeTypeIndex,@LookupGasboyTwoStageDriverValidationTypeIndex,@UsePINCodeFlag,@PINCode,@AuthPINFrom,@VehiclePlate,@PromptForVehiclePlateFlag,@LookupGasboyVehiclePlateCheckTypeIndex,@AlwaysPromptForAdditionalValidationFlag,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@DeviceID
                    ) AS remoteChanges ([GasboyDeviceGuid],[SiteGuid],[GasboyDepartmentGuid],[DeviceCode],[DeviceName],[CardNumber],[GroupRuleName],[LookupGasboyDeviceTypeIndex],[LookupGasboyRecordStatusIndex],[LookupGasboyHardwareTypeIndex],[LookupGasboyAuthTypeIndex],[LookupGasboyEmployeeTypeIndex],[LookupGasboyTwoStageDriverValidationTypeIndex],[UsePINCodeFlag],[PINCode],[AuthPINFrom],[VehiclePlate],[PromptForVehiclePlateFlag],[LookupGasboyVehiclePlateCheckTypeIndex],[AlwaysPromptForAdditionalValidationFlag],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[DeviceID])
            ON (existingData.[GasboyDeviceGuid] = remoteChanges.[GasboyDeviceGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
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
    END

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
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
