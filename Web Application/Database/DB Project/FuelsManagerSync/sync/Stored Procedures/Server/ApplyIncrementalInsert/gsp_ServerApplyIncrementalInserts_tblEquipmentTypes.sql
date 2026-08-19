-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEquipmentTypes
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblEquipmentTypes]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblEquipmentTypes varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblEquipmentTypes] AS existingData
        USING (SELECT @EqTypeName 'EqTypeName',@EqTypeDescription 'EqTypeDescription',@Capacity 'Capacity',@SafeFill 'SafeFill',@Make 'Make',@Model 'Model',@Year 'Year',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@DeleteFlag 'DeleteFlag',@IssPt 'IssPt',@MultiCompartment 'MultiCompartment',@EquipmentTypeGuid 'EquipmentTypeGuid',@SiteGuid 'SiteGuid',@LookupEquipmentTypeIndex 'LookupEquipmentTypeIndex',@ProductGuid 'ProductGuid',@CustomerDesignator 'CustomerDesignator',@ServiceTime 'ServiceTime',@VolumeUnits 'VolumeUnits',@VolumeDecimalPlaces 'VolumeDecimalPlaces',@MassUnits 'MassUnits',@MassDecimalPlaces 'MassDecimalPlaces',@WingToWingToleranceType 'WingToWingToleranceType',@WingToWingToleranceValue 'WingToWingToleranceValue',@TankToTankToleranceType 'TankToTankToleranceType',@TankToTankToleranceValue 'TankToTankToleranceValue',@FuelServiceToleranceType 'FuelServiceToleranceType',@FuelServiceToleranceValue 'FuelServiceToleranceValue',@FuelServiceToleranceMaxType 'FuelServiceToleranceMaxType',@FuelServiceToleranceMaxValue 'FuelServiceToleranceMaxValue',@AllowFuelingByWeight 'AllowFuelingByWeight',@LookupCompanyRoleIndex 'LookupCompanyRoleIndex'
                ) AS remoteChanges ([EqTypeName],[EqTypeDescription],[Capacity],[SafeFill],[Make],[Model],[Year],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DeleteFlag],[IssPt],[MultiCompartment],[EquipmentTypeGuid],[SiteGuid],[LookupEquipmentTypeIndex],[ProductGuid],[CustomerDesignator],[ServiceTime],[VolumeUnits],[VolumeDecimalPlaces],[MassUnits],[MassDecimalPlaces],[WingToWingToleranceType],[WingToWingToleranceValue],[TankToTankToleranceType],[TankToTankToleranceValue],[FuelServiceToleranceType],[FuelServiceToleranceValue],[FuelServiceToleranceMaxType],[FuelServiceToleranceMaxValue],[AllowFuelingByWeight],[LookupCompanyRoleIndex])
        ON (existingData.[EquipmentTypeGuid] = remoteChanges.[EquipmentTypeGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [EqTypeName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EqTypeName'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[EqTypeName] ELSE remoteChanges.[EqTypeName] END
                       ,[EqTypeDescription] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EqTypeDescription'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[EqTypeDescription] ELSE remoteChanges.[EqTypeDescription] END
                       ,[Capacity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Capacity'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[Capacity] ELSE remoteChanges.[Capacity] END
                       ,[SafeFill] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SafeFill'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[SafeFill] ELSE remoteChanges.[SafeFill] END
                       ,[Make] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Make'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[Make] ELSE remoteChanges.[Make] END
                       ,[Model] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Model'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[Model] ELSE remoteChanges.[Model] END
                       ,[Year] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Year'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[Year] ELSE remoteChanges.[Year] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[DeleteFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeleteFlag'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[DeleteFlag] ELSE remoteChanges.[DeleteFlag] END
                       ,[IssPt] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IssPt'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[IssPt] ELSE remoteChanges.[IssPt] END
                       ,[MultiCompartment] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MultiCompartment'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[MultiCompartment] ELSE remoteChanges.[MultiCompartment] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[LookupEquipmentTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupEquipmentTypeIndex'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[LookupEquipmentTypeIndex] ELSE remoteChanges.[LookupEquipmentTypeIndex] END
                       ,[ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END
                       ,[CustomerDesignator] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CustomerDesignator'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[CustomerDesignator] ELSE remoteChanges.[CustomerDesignator] END
                       ,[ServiceTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ServiceTime'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[ServiceTime] ELSE remoteChanges.[ServiceTime] END
                       ,[VolumeUnits] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeUnits'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[VolumeUnits] ELSE remoteChanges.[VolumeUnits] END
                       ,[VolumeDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeDecimalPlaces'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[VolumeDecimalPlaces] ELSE remoteChanges.[VolumeDecimalPlaces] END
                       ,[MassUnits] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassUnits'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[MassUnits] ELSE remoteChanges.[MassUnits] END
                       ,[MassDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassDecimalPlaces'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[MassDecimalPlaces] ELSE remoteChanges.[MassDecimalPlaces] END
                       ,[WingToWingToleranceType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WingToWingToleranceType'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[WingToWingToleranceType] ELSE remoteChanges.[WingToWingToleranceType] END
                       ,[WingToWingToleranceValue] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WingToWingToleranceValue'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[WingToWingToleranceValue] ELSE remoteChanges.[WingToWingToleranceValue] END
                       ,[TankToTankToleranceType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankToTankToleranceType'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[TankToTankToleranceType] ELSE remoteChanges.[TankToTankToleranceType] END
                       ,[TankToTankToleranceValue] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankToTankToleranceValue'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[TankToTankToleranceValue] ELSE remoteChanges.[TankToTankToleranceValue] END
                       ,[FuelServiceToleranceType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelServiceToleranceType'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[FuelServiceToleranceType] ELSE remoteChanges.[FuelServiceToleranceType] END
                       ,[FuelServiceToleranceValue] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelServiceToleranceValue'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[FuelServiceToleranceValue] ELSE remoteChanges.[FuelServiceToleranceValue] END
                       ,[FuelServiceToleranceMaxType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelServiceToleranceMaxType'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[FuelServiceToleranceMaxType] ELSE remoteChanges.[FuelServiceToleranceMaxType] END
                       ,[FuelServiceToleranceMaxValue] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelServiceToleranceMaxValue'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[FuelServiceToleranceMaxValue] ELSE remoteChanges.[FuelServiceToleranceMaxValue] END
                       ,[AllowFuelingByWeight] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AllowFuelingByWeight'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[AllowFuelingByWeight] ELSE remoteChanges.[AllowFuelingByWeight] END
                       ,[LookupCompanyRoleIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupCompanyRoleIndex'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN existingData.[LookupCompanyRoleIndex] ELSE remoteChanges.[LookupCompanyRoleIndex] END

        WHEN NOT MATCHED THEN
            INSERT ([EqTypeName],[EqTypeDescription],[Capacity],[SafeFill],[Make],[Model],[Year],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DeleteFlag],[IssPt],[MultiCompartment],[EquipmentTypeGuid],[SiteGuid],[LookupEquipmentTypeIndex],[ProductGuid],[CustomerDesignator],[ServiceTime],[VolumeUnits],[VolumeDecimalPlaces],[MassUnits],[MassDecimalPlaces],[WingToWingToleranceType],[WingToWingToleranceValue],[TankToTankToleranceType],[TankToTankToleranceValue],[FuelServiceToleranceType],[FuelServiceToleranceValue],[FuelServiceToleranceMaxType],[FuelServiceToleranceMaxValue],[AllowFuelingByWeight],[LookupCompanyRoleIndex])
                VALUES ((CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EqTypeName'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @EqTypeName END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EqTypeDescription'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @EqTypeDescription END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Capacity'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @Capacity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SafeFill'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @SafeFill END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Make'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @Make END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Model'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @Model END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Year'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @Year END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @UpdatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeleteFlag'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @DeleteFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IssPt'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @IssPt END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MultiCompartment'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @MultiCompartment END),@EquipmentTypeGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupEquipmentTypeIndex'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @LookupEquipmentTypeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @ProductGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CustomerDesignator'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @CustomerDesignator END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ServiceTime'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @ServiceTime END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeUnits'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @VolumeUnits END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeDecimalPlaces'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @VolumeDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassUnits'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @MassUnits END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassDecimalPlaces'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @MassDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WingToWingToleranceType'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @WingToWingToleranceType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WingToWingToleranceValue'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @WingToWingToleranceValue END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankToTankToleranceType'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @TankToTankToleranceType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankToTankToleranceValue'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @TankToTankToleranceValue END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelServiceToleranceType'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @FuelServiceToleranceType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelServiceToleranceValue'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @FuelServiceToleranceValue END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelServiceToleranceMaxType'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @FuelServiceToleranceMaxType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelServiceToleranceMaxValue'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @FuelServiceToleranceMaxValue END),@AllowFuelingByWeight,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupCompanyRoleIndex'), @sync_supported_columns_tblEquipmentTypes)) WHEN 0 THEN NULL ELSE @LookupCompanyRoleIndex END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
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
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

