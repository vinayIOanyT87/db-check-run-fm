-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyDepartment
-- Description:	Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblGasboyDepartment]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@GasboyDepartmentGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@DepartmentCode bigint,
@DepartmentName nvarchar(50),
@GroupRuleName nvarchar(50),
@PriceListName nvarchar(50),
@LookupGasboyRecordStatusIndex int,
@UsePINCodeFlag bit,
@PINCode varbinary(256),
@AuthPINFrom tinyint,
@PromptForVehiclePlateFlag bit,
@LookupGasboyVehiclePlateCheckTypeIndex int,
@AlwaysPromptForAdditionalValidationFlag tinyint,
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@DepartmentID bigint,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    ;   MERGE [dbo].[tblGasboyDepartment] AS existingData
        USING (SELECT @GasboyDepartmentGuid 'GasboyDepartmentGuid',@SiteGuid 'SiteGuid',@DepartmentCode 'DepartmentCode',@DepartmentName 'DepartmentName',@GroupRuleName 'GroupRuleName',@PriceListName 'PriceListName',@LookupGasboyRecordStatusIndex 'LookupGasboyRecordStatusIndex',@UsePINCodeFlag 'UsePINCodeFlag',@PINCode 'PINCode',@AuthPINFrom 'AuthPINFrom',@PromptForVehiclePlateFlag 'PromptForVehiclePlateFlag',@LookupGasboyVehiclePlateCheckTypeIndex 'LookupGasboyVehiclePlateCheckTypeIndex',@AlwaysPromptForAdditionalValidationFlag 'AlwaysPromptForAdditionalValidationFlag',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate',@DepartmentID 'DepartmentID'
                ) AS remoteChanges ([GasboyDepartmentGuid],[SiteGuid],[DepartmentCode],[DepartmentName],[GroupRuleName],[PriceListName],[LookupGasboyRecordStatusIndex],[UsePINCodeFlag],[PINCode],[AuthPINFrom],[PromptForVehiclePlateFlag],[LookupGasboyVehiclePlateCheckTypeIndex],[AlwaysPromptForAdditionalValidationFlag],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[DepartmentID])
        ON (existingData.[GasboyDepartmentGuid] = remoteChanges.[GasboyDepartmentGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [SiteGuid] = remoteChanges.[SiteGuid]
                       ,[DepartmentCode] = remoteChanges.[DepartmentCode]
                       ,[DepartmentName] = remoteChanges.[DepartmentName]
                       ,[GroupRuleName] = remoteChanges.[GroupRuleName]
                       ,[PriceListName] = remoteChanges.[PriceListName]
                       ,[LookupGasboyRecordStatusIndex] = remoteChanges.[LookupGasboyRecordStatusIndex]
                       ,[UsePINCodeFlag] = remoteChanges.[UsePINCodeFlag]
                       ,[PINCode] = remoteChanges.[PINCode]
                       ,[AuthPINFrom] = remoteChanges.[AuthPINFrom]
                       ,[PromptForVehiclePlateFlag] = remoteChanges.[PromptForVehiclePlateFlag]
                       ,[LookupGasboyVehiclePlateCheckTypeIndex] = remoteChanges.[LookupGasboyVehiclePlateCheckTypeIndex]
                       ,[AlwaysPromptForAdditionalValidationFlag] = remoteChanges.[AlwaysPromptForAdditionalValidationFlag]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[DepartmentID] = remoteChanges.[DepartmentID]

        WHEN NOT MATCHED THEN
            INSERT ([GasboyDepartmentGuid],[SiteGuid],[DepartmentCode],[DepartmentName],[GroupRuleName],[PriceListName],[LookupGasboyRecordStatusIndex],[UsePINCodeFlag],[PINCode],[AuthPINFrom],[PromptForVehiclePlateFlag],[LookupGasboyVehiclePlateCheckTypeIndex],[AlwaysPromptForAdditionalValidationFlag],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[DepartmentID])
                VALUES (@GasboyDepartmentGuid,@SiteGuid,@DepartmentCode,@DepartmentName,@GroupRuleName,@PriceListName,@LookupGasboyRecordStatusIndex,@UsePINCodeFlag,@PINCode,@AuthPINFrom,@PromptForVehiclePlateFlag,@LookupGasboyVehiclePlateCheckTypeIndex,@AlwaysPromptForAdditionalValidationFlag,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@DepartmentID)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyDepartmentGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyDepartmentGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyDepartmentGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblGasboyDepartment] WHERE GasboyDepartmentGuid = @GasboyDepartmentGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
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
