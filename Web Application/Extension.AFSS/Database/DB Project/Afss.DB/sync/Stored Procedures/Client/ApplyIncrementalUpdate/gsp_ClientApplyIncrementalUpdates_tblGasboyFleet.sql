-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyFleet
-- Description:	Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblGasboyFleet]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@GasboyFleetGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@FleetCode bigint,
@FleetName nvarchar(50),
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
@FleetID bigint,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @wasDeleted int
    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [dbo].[tblGasboyFleet]
                            INNER JOIN [track].[tblGasboyFleet] CT
                                ON CT.PK_GasboyFleetGuid = [dbo].[tblGasboyFleet].[GasboyFleetGuid] 
                        WHERE CT.PK_GasboyFleetGuid = @GasboyFleetGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblGasboyFleet].[GasboyFleetGuid],[dbo].[tblGasboyFleet].[SiteGuid],[dbo].[tblGasboyFleet].[FleetCode],[dbo].[tblGasboyFleet].[FleetName],[dbo].[tblGasboyFleet].[GroupRuleName],[dbo].[tblGasboyFleet].[PriceListName],[dbo].[tblGasboyFleet].[LookupGasboyRecordStatusIndex],[dbo].[tblGasboyFleet].[UsePINCodeFlag],[dbo].[tblGasboyFleet].[PINCode],[dbo].[tblGasboyFleet].[AuthPINFrom],[dbo].[tblGasboyFleet].[PromptForVehiclePlateFlag],[dbo].[tblGasboyFleet].[LookupGasboyVehiclePlateCheckTypeIndex],[dbo].[tblGasboyFleet].[AlwaysPromptForAdditionalValidationFlag],[dbo].[tblGasboyFleet].[CreatedBy],[dbo].[tblGasboyFleet].[CreatedDate],[dbo].[tblGasboyFleet].[UpdatedBy],[dbo].[tblGasboyFleet].[UpdatedDate],[dbo].[tblGasboyFleet].[FleetID]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblGasboyFleet]
                        INNER JOIN [track].[tblGasboyFleet] CT
                            ON CT.PK_GasboyFleetGuid = [dbo].[tblGasboyFleet].[GasboyFleetGuid] 
                    WHERE CT.PK_GasboyFleetGuid = @GasboyFleetGuid
            ) MERGE existingData
            USING (SELECT @GasboyFleetGuid,@SiteGuid,@FleetCode,@FleetName,@GroupRuleName,@PriceListName,@LookupGasboyRecordStatusIndex,@UsePINCodeFlag,@PINCode,@AuthPINFrom,@PromptForVehiclePlateFlag,@LookupGasboyVehiclePlateCheckTypeIndex,@AlwaysPromptForAdditionalValidationFlag,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@FleetID
                    ) AS remoteChanges ([GasboyFleetGuid],[SiteGuid],[FleetCode],[FleetName],[GroupRuleName],[PriceListName],[LookupGasboyRecordStatusIndex],[UsePINCodeFlag],[PINCode],[AuthPINFrom],[PromptForVehiclePlateFlag],[LookupGasboyVehiclePlateCheckTypeIndex],[AlwaysPromptForAdditionalValidationFlag],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[FleetID])
            ON (existingData.[GasboyFleetGuid] = remoteChanges.[GasboyFleetGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [SiteGuid] = remoteChanges.[SiteGuid]
                       ,[FleetCode] = remoteChanges.[FleetCode]
                       ,[FleetName] = remoteChanges.[FleetName]
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
                       ,[FleetID] = remoteChanges.[FleetID]

            WHEN NOT MATCHED THEN
                INSERT ([GasboyFleetGuid],[SiteGuid],[FleetCode],[FleetName],[GroupRuleName],[PriceListName],[LookupGasboyRecordStatusIndex],[UsePINCodeFlag],[PINCode],[AuthPINFrom],[PromptForVehiclePlateFlag],[LookupGasboyVehiclePlateCheckTypeIndex],[AlwaysPromptForAdditionalValidationFlag],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[FleetID])
                    VALUES (@GasboyFleetGuid,@SiteGuid,@FleetCode,@FleetName,@GroupRuleName,@PriceListName,@LookupGasboyRecordStatusIndex,@UsePINCodeFlag,@PINCode,@AuthPINFrom,@PromptForVehiclePlateFlag,@LookupGasboyVehiclePlateCheckTypeIndex,@AlwaysPromptForAdditionalValidationFlag,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@FleetID)
            ;
    END

    SET @sync_row_count = @@rowcount; 
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyFleetGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyFleetGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyFleetGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblGasboyFleet] WHERE GasboyFleetGuid = @GasboyFleetGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
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
