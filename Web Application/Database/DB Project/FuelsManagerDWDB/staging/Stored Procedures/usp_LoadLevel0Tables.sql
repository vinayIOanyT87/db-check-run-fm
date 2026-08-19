/*
	DROP PROCEDURE [Staging].[usp_LoadLevel0Tables]

	EXEC [staging].[usp_LoadLevel0Tables]
	
*/
CREATE PROCEDURE [staging].[usp_LoadLevel0Tables]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadLevel0Tables]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads records from staging into level 0 tables in the OLAP database.
  -- Notes:
  -- 1. Level 0 tables are those tables that do not have any foreign key dependency to any other tables, e.g. dimSites.
  -- 2. Level 0 tables can be safely loaded from the data from staging, without first having to update the foreign key field values in staging.
  -- 3. The values of the ID fields are trimmed first before insertion because those ID fields are used when trying to identify the correct entities
  --    for transactions for which the entity id is available but the entity key is missing. In this case trimming avoids the insertion of new entity
  --    records that only differ by prefix or suffix whitespaces, a condition that is likely to lead to duplicate errors when processing the cube.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    --AutoDistributionReasonCodes
    MERGE dbo.DimAutoDistributionReasonCodes AS tgt
    USING (SELECT
        [AutoDistributionReasonCodeKey],
        [ReasonCode],
        [IsRecordDeleted],
        [CombinedUpdatedDate]
    FROM staging.tblAutoDistributionReasonCodes
    WHERE AutoDistributionReasonCodeKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.AKey = src.AutoDistributionReasonCodeKey
    WHEN NOT MATCHED THEN
    INSERT ([AKey], [ReasonCode], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[AutoDistributionReasonCodeKey], src.[ReasonCode], src.[IsRecordDeleted], src.[CombinedUpdatedDate])
    WHEN MATCHED AND src.CombinedUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[ReasonCode] = src.[ReasonCode],
        tgt.[_DeletedFlag] = src.[IsRecordDeleted],
        tgt.[_RecordUpdatedDate] = src.[CombinedUpdatedDate];



    --EquipmentType
    MERGE dbo.DimEquipmentType AS tgt
    USING (SELECT
        [EquipmentTypeKey],
	    [EqTypeName],
	    [EqTypeDescription],
	    [LookupEquipmentTypeIndex],
        [LookupEquipmentTypeName],
        [Capacity],
        [Make],
        [Model],
        [Year],
        [IsRecordDeleted],
        [CombinedUpdatedDate]
    FROM staging.tblEquipmentTypes
    WHERE EquipmentTypeKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.AKey = src.EquipmentTypeKey
    WHEN NOT MATCHED THEN
    INSERT ([AKey], [EquipmentTypeName], [EquipmentTypeDescription], [EquipmentTypeIndex], [EquipmentTypeClass], [Capacity], [Make], [Model], [Year], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[EquipmentTypeKey], src.[EqTypeName], src.[EqTypeDescription], src.[LookupEquipmentTypeIndex], src.[LookupEquipmentTypeName], src.[Capacity], src.[Make], src.[Model], src.[Year], src.[IsRecordDeleted], src.[CombinedUpdatedDate])
    WHEN MATCHED AND src.CombinedUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[EquipmentTypeName] = src.[EqTypeName], 
        tgt.[EquipmentTypeDescription] = src.[EqTypeDescription],
        tgt.[EquipmentTypeIndex] = src.[LookupEquipmentTypeIndex],
        tgt.[EquipmentTypeClass] = src.[LookupEquipmentTypeName], 
        tgt.[Capacity] = src.[Capacity], 
        tgt.[Make] = src.[Make], 
        tgt.[Model] = src.[Model], 
        tgt.[Year] = src.[Year],
        tgt.[_DeletedFlag] = src.[IsRecordDeleted],
        tgt.[_RecordUpdatedDate] = src.[CombinedUpdatedDate];



    --Site
    MERGE dbo.DimSite AS tgt
    USING (SELECT
        [SiteKey],
        Trim([ID]) [ID],
        [SiteGroupFlag],
        [Contact1Name],
        [Address1],
        [Address2],
        [City],
        [State],
        [Zip],
        [Country],
        [Phone],
        [TimeZone],
        [TemperatureDecimalPlaces],
        [TemperatureUnitIndex],
        [DensityDecimalPlaces],
        [DensityUnitIndex],
        [VolumeDecimalPlaces],
        [VolumeUnitIndex],
        [IsRecordDeleted],
        [CombinedUpdatedDate]
    FROM staging.tblSites
    WHERE SiteKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.AKey = src.SiteKey
    WHEN NOT MATCHED THEN
    INSERT ([AKey], [SiteId], [SiteGroupFlag], [Contact1Name], [Address1], [Address2], [City], [State], [Zip], [Country], [Phone], [TimeZone], [TemperatureDecimalPlaces], [TemperatureUnitIndex], [DensityDecimalPlaces],  [DensityUnitIndex], [VolumeDecimalPlaces], [VolumeUnitIndex], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[SiteKey], src.[ID], src.[SiteGroupFlag], src.[Contact1Name], src.[Address1], src.[Address2], src.[City], src.[State], src.[Zip], src.[Country], src.[Phone], src.[TimeZone], src.[TemperatureDecimalPlaces], src.[TemperatureUnitIndex], src.[DensityDecimalPlaces],  src.[DensityUnitIndex], src.[VolumeDecimalPlaces], src.[VolumeUnitIndex], src.[IsRecordDeleted], src.[CombinedUpdatedDate])
    WHEN MATCHED AND src.CombinedUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[SiteId] = src.[ID],
    tgt.[SiteGroupFlag] = src.[SiteGroupFlag],
    tgt.[Contact1Name] = src.[Contact1Name],
    tgt.[Address1] = src.[Address1],
    tgt.[Address2] = src.[Address2],
    tgt.[City] = src.[City],
    tgt.[State] = src.[State],
    tgt.[Zip] = src.[Zip],
    tgt.[Country] = src.[Country],
    tgt.[Phone] = src.[Phone],
    tgt.[TimeZone] = src.[TimeZone],
    tgt.[TemperatureDecimalPlaces] = src.[TemperatureDecimalPlaces],
    tgt.[TemperatureUnitIndex] = src.[TemperatureUnitIndex],
    tgt.[DensityDecimalPlaces] = src.[DensityDecimalPlaces],
    tgt.[DensityUnitIndex] = src.[DensityUnitIndex],
    tgt.[VolumeDecimalPlaces] = src.[VolumeDecimalPlaces],
    tgt.[VolumeUnitIndex] = src.[VolumeUnitIndex],
    tgt.[_DeletedFlag] = src.[IsRecordDeleted],
    tgt.[_RecordUpdatedDate] = src.[CombinedUpdatedDate];


    --User
    MERGE dbo.DimFMUser AS tgt
    USING (SELECT
        [UserKey],
	    [UserID],
	    [Name],
	    [EmailAddress],
	    [InactivityLockout],
        [IsRecordDeleted],
        [CombinedUpdatedDate]
    FROM staging.tblUsers
    WHERE UserKey IS NOT NULL
    AND IgnoreRecord = 0) AS src
    ON tgt.AKey = src.UserKey
    WHEN NOT MATCHED THEN
    INSERT ([AKey], [FMUserID], [Name], [EmailAddress], [InactivityLockout], [_DeletedFlag], [_RecordUpdatedDate])
    VALUES (src.[UserKey], src.[UserID], src.[Name], src.[EmailAddress], src.[InactivityLockout], src.[IsRecordDeleted], src.[CombinedUpdatedDate])
    WHEN MATCHED AND src.CombinedUpdatedDate > tgt._RecordUpdatedDate THEN
    UPDATE SET tgt.[FMUserId] = src.[UserId], 
        tgt.[Name] = src.[Name],
        tgt.[EmailAddress] = src.[EmailAddress],
        tgt.[InactivityLockout] = src.[InactivityLockout],
        tgt.[_DeletedFlag] = src.[IsRecordDeleted],
        tgt.[_RecordUpdatedDate] = src.[CombinedUpdatedDate];

  END TRY
  BEGIN CATCH
    DECLARE @_ErrMessage nvarchar(2048),
            @_ErrNumber int,
            @_ErrProcName nvarchar(126),
            @_ErrLineNumber int;
    SET @_ErrMessage = ERROR_MESSAGE();
    SET @_ErrNumber = ERROR_NUMBER();
    SET @_ErrProcName = ERROR_PROCEDURE();
    SET @_ErrLineNumber = ERROR_LINE();
    SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
    + 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
    + 'Procedure Name: [staging].[usp_LoadLevel0Tables]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END