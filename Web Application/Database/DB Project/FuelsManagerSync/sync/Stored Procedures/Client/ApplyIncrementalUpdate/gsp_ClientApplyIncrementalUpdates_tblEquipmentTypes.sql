-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEquipmentTypes
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblEquipmentTypes]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@EqTypeName nvarchar(50),
@EqTypeDescription nvarchar(50),
@Capacity float,
@SafeFill float,
@Make nvarchar(20),
@Model nvarchar(32),
@Year smallint,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@DeleteFlag bit,
@IssPt nvarchar(20),
@MultiCompartment bit,
@EquipmentTypeGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupEquipmentTypeIndex int,
@ProductGuid uniqueidentifier,
@CustomerDesignator nvarchar(128),
@ServiceTime float,
@VolumeUnits int,
@VolumeDecimalPlaces smallint,
@MassUnits int,
@MassDecimalPlaces smallint,
@WingToWingToleranceType smallint,
@WingToWingToleranceValue float,
@TankToTankToleranceType smallint,
@TankToTankToleranceValue float,
@FuelServiceToleranceType smallint,
@FuelServiceToleranceValue float,
@FuelServiceToleranceMaxType smallint,
@FuelServiceToleranceMaxValue float,
@AllowFuelingByWeight bit,
@LookupCompanyRoleIndex int,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblEquipmentTypes] CT
                        WHERE CT.PK_EquipmentTypeGuid = @EquipmentTypeGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblEquipmentTypes].[EqTypeName],[dbo].[tblEquipmentTypes].[EqTypeDescription],[dbo].[tblEquipmentTypes].[Capacity],[dbo].[tblEquipmentTypes].[SafeFill],[dbo].[tblEquipmentTypes].[Make],[dbo].[tblEquipmentTypes].[Model],[dbo].[tblEquipmentTypes].[Year],[dbo].[tblEquipmentTypes].[CreatedDate],[dbo].[tblEquipmentTypes].[CreatedBy],[dbo].[tblEquipmentTypes].[UpdatedDate],[dbo].[tblEquipmentTypes].[UpdatedBy],[dbo].[tblEquipmentTypes].[DeleteFlag],[dbo].[tblEquipmentTypes].[IssPt],[dbo].[tblEquipmentTypes].[MultiCompartment],[dbo].[tblEquipmentTypes].[EquipmentTypeGuid],[dbo].[tblEquipmentTypes].[SiteGuid],[dbo].[tblEquipmentTypes].[LookupEquipmentTypeIndex],[dbo].[tblEquipmentTypes].[ProductGuid],[dbo].[tblEquipmentTypes].[CustomerDesignator],[dbo].[tblEquipmentTypes].[ServiceTime],[dbo].[tblEquipmentTypes].[VolumeUnits],[dbo].[tblEquipmentTypes].[VolumeDecimalPlaces],[dbo].[tblEquipmentTypes].[MassUnits],[dbo].[tblEquipmentTypes].[MassDecimalPlaces],[dbo].[tblEquipmentTypes].[WingToWingToleranceType],[dbo].[tblEquipmentTypes].[WingToWingToleranceValue],[dbo].[tblEquipmentTypes].[TankToTankToleranceType],[dbo].[tblEquipmentTypes].[TankToTankToleranceValue],[dbo].[tblEquipmentTypes].[FuelServiceToleranceType],[dbo].[tblEquipmentTypes].[FuelServiceToleranceValue],[dbo].[tblEquipmentTypes].[FuelServiceToleranceMaxType],[dbo].[tblEquipmentTypes].[FuelServiceToleranceMaxValue],[dbo].[tblEquipmentTypes].[AllowFuelingByWeight],[dbo].[tblEquipmentTypes].[LookupCompanyRoleIndex]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblEquipmentTypes]
                        INNER JOIN [track].[tblEquipmentTypes] CT
                            ON CT.PK_EquipmentTypeGuid = [dbo].[tblEquipmentTypes].[EquipmentTypeGuid] 
                    WHERE CT.PK_EquipmentTypeGuid = @EquipmentTypeGuid
            ) MERGE existingData
            USING (SELECT @EqTypeName,@EqTypeDescription,@Capacity,@SafeFill,@Make,@Model,@Year,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@DeleteFlag,@IssPt,@MultiCompartment,@EquipmentTypeGuid,@SiteGuid,@LookupEquipmentTypeIndex,@ProductGuid,@CustomerDesignator,@ServiceTime,@VolumeUnits,@VolumeDecimalPlaces,@MassUnits,@MassDecimalPlaces,@WingToWingToleranceType,@WingToWingToleranceValue,@TankToTankToleranceType,@TankToTankToleranceValue,@FuelServiceToleranceType,@FuelServiceToleranceValue,@FuelServiceToleranceMaxType,@FuelServiceToleranceMaxValue,@AllowFuelingByWeight,@LookupCompanyRoleIndex
                    ) AS remoteChanges ([EqTypeName],[EqTypeDescription],[Capacity],[SafeFill],[Make],[Model],[Year],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DeleteFlag],[IssPt],[MultiCompartment],[EquipmentTypeGuid],[SiteGuid],[LookupEquipmentTypeIndex],[ProductGuid],[CustomerDesignator],[ServiceTime],[VolumeUnits],[VolumeDecimalPlaces],[MassUnits],[MassDecimalPlaces],[WingToWingToleranceType],[WingToWingToleranceValue],[TankToTankToleranceType],[TankToTankToleranceValue],[FuelServiceToleranceType],[FuelServiceToleranceValue],[FuelServiceToleranceMaxType],[FuelServiceToleranceMaxValue],[AllowFuelingByWeight],[LookupCompanyRoleIndex])
            ON (existingData.[EquipmentTypeGuid] = remoteChanges.[EquipmentTypeGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [EqTypeName] = remoteChanges.[EqTypeName]
                       ,[EqTypeDescription] = remoteChanges.[EqTypeDescription]
                       ,[Capacity] = remoteChanges.[Capacity]
                       ,[SafeFill] = remoteChanges.[SafeFill]
                       ,[Make] = remoteChanges.[Make]
                       ,[Model] = remoteChanges.[Model]
                       ,[Year] = remoteChanges.[Year]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[DeleteFlag] = remoteChanges.[DeleteFlag]
                       ,[IssPt] = remoteChanges.[IssPt]
                       ,[MultiCompartment] = remoteChanges.[MultiCompartment]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[LookupEquipmentTypeIndex] = remoteChanges.[LookupEquipmentTypeIndex]
                       ,[ProductGuid] = remoteChanges.[ProductGuid]
                       ,[CustomerDesignator] = remoteChanges.[CustomerDesignator]
                       ,[ServiceTime] = remoteChanges.[ServiceTime]
                       ,[VolumeUnits] = remoteChanges.[VolumeUnits]
                       ,[VolumeDecimalPlaces] = remoteChanges.[VolumeDecimalPlaces]
                       ,[MassUnits] = remoteChanges.[MassUnits]
                       ,[MassDecimalPlaces] = remoteChanges.[MassDecimalPlaces]
                       ,[WingToWingToleranceType] = remoteChanges.[WingToWingToleranceType]
                       ,[WingToWingToleranceValue] = remoteChanges.[WingToWingToleranceValue]
                       ,[TankToTankToleranceType] = remoteChanges.[TankToTankToleranceType]
                       ,[TankToTankToleranceValue] = remoteChanges.[TankToTankToleranceValue]
                       ,[FuelServiceToleranceType] = remoteChanges.[FuelServiceToleranceType]
                       ,[FuelServiceToleranceValue] = remoteChanges.[FuelServiceToleranceValue]
                       ,[FuelServiceToleranceMaxType] = remoteChanges.[FuelServiceToleranceMaxType]
                       ,[FuelServiceToleranceMaxValue] = remoteChanges.[FuelServiceToleranceMaxValue]
                       ,[AllowFuelingByWeight] = remoteChanges.[AllowFuelingByWeight]
                       ,[LookupCompanyRoleIndex] = remoteChanges.[LookupCompanyRoleIndex]

            WHEN NOT MATCHED THEN
                INSERT ([EqTypeName],[EqTypeDescription],[Capacity],[SafeFill],[Make],[Model],[Year],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DeleteFlag],[IssPt],[MultiCompartment],[EquipmentTypeGuid],[SiteGuid],[LookupEquipmentTypeIndex],[ProductGuid],[CustomerDesignator],[ServiceTime],[VolumeUnits],[VolumeDecimalPlaces],[MassUnits],[MassDecimalPlaces],[WingToWingToleranceType],[WingToWingToleranceValue],[TankToTankToleranceType],[TankToTankToleranceValue],[FuelServiceToleranceType],[FuelServiceToleranceValue],[FuelServiceToleranceMaxType],[FuelServiceToleranceMaxValue],[AllowFuelingByWeight],[LookupCompanyRoleIndex])
                    VALUES (@EqTypeName,@EqTypeDescription,@Capacity,@SafeFill,@Make,@Model,@Year,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@DeleteFlag,@IssPt,@MultiCompartment,@EquipmentTypeGuid,@SiteGuid,@LookupEquipmentTypeIndex,@ProductGuid,@CustomerDesignator,@ServiceTime,@VolumeUnits,@VolumeDecimalPlaces,@MassUnits,@MassDecimalPlaces,@WingToWingToleranceType,@WingToWingToleranceValue,@TankToTankToleranceType,@TankToTankToleranceValue,@FuelServiceToleranceType,@FuelServiceToleranceValue,@FuelServiceToleranceMaxType,@FuelServiceToleranceMaxValue,@AllowFuelingByWeight,@LookupCompanyRoleIndex)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EquipmentTypeGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EquipmentTypeGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EquipmentTypeGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblEquipmentTypes] WHERE EquipmentTypeGuid = @EquipmentTypeGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
