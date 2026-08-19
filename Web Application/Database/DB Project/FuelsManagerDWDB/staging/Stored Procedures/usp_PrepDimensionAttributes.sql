/*
	DROP PROCEDURE [staging].[usp_PrepDimensionAttributes]

	EXEC [staging].[usp_PrepDimensionAttributes]
	EXEC [staging].[usp_PrepDimensionAttributes] 0
	EXEC [staging].[usp_PrepDimensionAttributes] 1
	
*/
CREATE PROCEDURE [staging].[usp_PrepDimensionAttributes] (@RecordAddedByETLOnly bit = NULL)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_PrepDimensionAttributes]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Prepare dimension table fields to be used as SSAS Dimension Attributes by resetting null values and trimming whitespaces. 
  --          Reset all null-value fields to a non-null dummy value (e.g. '<NOT AVAILABLE>' for character fields, 0 for numerical fields).
  -- Notes:
  -- 1. @RecordAddedByETLOnly Restrict the change only to those records that have an IsRecordAddedByETL value = 1. Used essentially to 
  --    support adding missing entities found while loading transactions. It allows this operation to ignore dimension records for which 
  --    the operation has already been run.
  -- 2. The occurrence of null values in a field that is used a Dimension attribute can conflict with empty (blank) string occurrences 
  --    of the same field, which leads to a Duplicate Attribute Error when deploying the cube.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY
    DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
    DECLARE @shortDummyId varchar(4) = '<NA>'
    DECLARE @bitDummyValue bit = 0

    DECLARE @isRecordAddedByETL bit = 0
    SET @isRecordAddedByETL = ISNULL(@RecordAddedByETLOnly, 0)


    -- ApplicationString
    UPDATE staging.tblApplicationString
    SET Id = ISNULL(TRIM(Id), @dummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- AutoDistributionReasonCode
    UPDATE staging.tblAutoDistributionReasonCodes
    SET ReasonCode = ISNULL(TRIM(ReasonCode), @dummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- Company
    UPDATE staging.tblCompanies
    SET Address1 = ISNULL(TRIM(Address1), @dummyId),
    Address2 = ISNULL(TRIM(Address2), @dummyId),
    City = ISNULL(TRIM(City), @dummyId),
    Code = ISNULL(TRIM(Code), @shortDummyId),
    Id = ISNULL(TRIM(Id), @dummyId),
    Country = ISNULL(TRIM(Country), @dummyId),
    EmergencyContact = ISNULL(TRIM(EmergencyContact), @dummyId),    
    EmergencyPhone = ISNULL(TRIM(EmergencyPhone), @dummyId),    
    LockedOut = ISNULL(LockedOut, @bitDummyValue),
    LockedOutReason = ISNULL(TRIM(LockedOutReason), @dummyId),
    Name = ISNULL(TRIM(Name), @shortDummyId),
    Phone = ISNULL(TRIM(Phone), @dummyId),   
    State = ISNULL(TRIM(State), @dummyId),
    Zip = ISNULL(TRIM(Zip), @shortDummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- Equipment
    UPDATE staging.tblEquipment
    SET Description = ISNULL(TRIM(Description), @dummyId),
    Id = ISNULL(TRIM(Id), @dummyId),    
    InUse = ISNULL(InUse, @bitDummyValue),
    Make = ISNULL(TRIM(Make), @dummyId),
    Model = ISNULL(TRIM(Model), @shortDummyId),
    SerialNumber = ISNULL(TRIM(SerialNumber), @dummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- Equipment Type
    UPDATE staging.tblEquipmentTypes
    SET Capacity = ISNULL(Capacity, 0),   
    LookupEquipmentTypeName = ISNULL(TRIM(LookupEquipmentTypeName), @dummyId),    
    EqTypeDescription = ISNULL(TRIM(EqTypeDescription), @dummyId),    
    EqTypeName = ISNULL(TRIM(EqTypeName), @dummyId), 
    Make = ISNULL(TRIM(Make), @dummyId),    
    Model = ISNULL(TRIM(Model), @shortDummyId),  
    Year = ISNULL(Year, 1900)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


     -- FMUser
    UPDATE staging.tblUsers
    SET UserId = ISNULL(TRIM(UserId), @dummyId),
    Name = ISNULL(TRIM(Name), @dummyId),    
    EmailAddress = ISNULL(TRIM(EmailAddress), @dummyId),    
    InactivityLockout = ISNULL(InactivityLockout, @bitDummyValue)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- Personnel
    UPDATE staging.tblPersonnel
    SET FirstName = ISNULL(TRIM(FirstName), @dummyId),
    MiddleName = ISNULL(TRIM(MiddleName), @dummyId),
    LastName = ISNULL(TRIM(LastName), @dummyId), 
    LockedOut = ISNULL(LockedOut, @bitDummyValue),
    LockedOutReason = ISNULL(TRIM(LockedOutReason), @dummyId),
    PersonId = ISNULL(TRIM(PersonId), @shortDummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- Product
    UPDATE staging.tblProducts
    SET AviationFuelFlag = ISNULL(AviationFuelFlag, @bitDummyValue),
    Description = ISNULL(TRIM(Description), @dummyId),
    GroundFuel = ISNULL(GroundFuel, @bitDummyValue),
    LockedOut = ISNULL(LockedOut, @bitDummyValue),
    LockedOutReason = ISNULL(TRIM(LockedOutReason), @dummyId),
    ProductCode = ISNULL(TRIM(ProductCode), @dummyId),
    ProductId = ISNULL(TRIM(ProductId), @shortDummyId),
    ProductTypeName = ISNULL(TRIM(ProductTypeName), @dummyId),
    TrackingProductId = ISNULL(TRIM(TrackingProductId), @dummyId),
    VolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- Station
    UPDATE staging.tblStations
    SET Id = ISNULL(TRIM(Id), @dummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- LoadArm
    UPDATE staging.tblLoadArms
    SET BayAArmNumber = ISNULL(BayAArmNumber, -1),
    BayBArmNumber = ISNULL(BayBArmNumber, -1),
    LoadRackText = ISNULL(TRIM(LoadRackText), @shortDummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- Tank
    UPDATE staging.tblTanks
    SET TankID = ISNULL(TRIM(TankID), @dummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))

    UPDATE staging.tblTanks
    SET VesselTypeName = ISNULL(TRIM(VesselTypeName), @dummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- Site
    UPDATE staging.tblSites
    SET Address1 = ISNULL(TRIM(Address1), @dummyId),
    Address2 = ISNULL(TRIM(Address2), @dummyId),
    City = ISNULL(TRIM(City), @dummyId),
    Contact1Name = ISNULL(TRIM(Contact1Name), @dummyId),
    Country = ISNULL(TRIM(Country), @dummyId),
    Phone = ISNULL(TRIM(Phone), @dummyId),
    Id = ISNULL(TRIM(Id), @dummyId),
    State = ISNULL(TRIM(State), @dummyId),
    TimeZone = ISNULL(TRIM(TimeZone), @dummyId),
    Zip = ISNULL(TRIM(Zip), @shortDummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    -- TransactionAlias
    UPDATE staging.tblTransactionAliases
    SET AliasName = ISNULL(TRIM(AliasName), @dummyId)
    WHERE IgnoreRecord = 0
    AND ((@RecordAddedByETLOnly IS NULL)
    OR (IsRecordAddedByETL = @isRecordAddedByETL))


    --Transaction Type
    --Set in [dbo].[usp_LoadTransactionTypeDimension]

   
  --Transaction (For DimTransaction attributes)
  --Set during Transaction loading


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
    + 'Procedure Name: [staging].[usp_ResetNullDimensionAttributes]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END