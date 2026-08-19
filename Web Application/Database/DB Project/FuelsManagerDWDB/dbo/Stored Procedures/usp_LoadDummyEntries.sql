/*
	DROP PROCEDURE [dbo].[usp_LoadDummyEntries]

	EXEC [dbo].[usp_LoadDummyEntries]
	
*/
CREATE PROCEDURE [dbo].[usp_LoadDummyEntries]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [dbo].[usp_LoadDummyEntries]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Load the Dimension and Fact tables with dummy records to help replace null references with references to the dummy records.
  -- Notes:
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
    DECLARE @shortDummyId varchar(4) = '<NA>'
	DECLARE @veryShortDummyId varchar(2) = 'NA'
    DECLARE @dummyDate datetime = '1/1/1900'
    DECLARE @dummyGuid uniqueidentifier = cast(cast(0 as binary) as uniqueidentifier)
    DECLARE @dummyTransactionTypeSKey int = -100

    --Dummy DimDate entry				
    DECLARE @currentDate datetime
    SET @currentDate = @dummyDate

    DECLARE @dayNumberInMonth int
    DECLARE @calendarMonthName nvarchar(20)
    DECLARE @calendarYear int
    DECLARE @calendarMonthNumberInYear int

    SET @dayNumberInMonth = DATEPART(DAY, @currentDate)
    SET @calendarMonthNumberInYear = DATEPART(MONTH, @currentDate)
    SET @calendarMonthName = DATENAME(mm, @currentDate)
    SET @calendarYear = DATEPART(YEAR, @currentDate)

    DELETE dbo.DimDate
    WHERE SKey = @calendarYear * 10000 + @calendarMonthNumberInYear * 100 + @dayNumberInMonth

    INSERT INTO dbo.DimDate (SKey,
    FullDateAKey,
    FullDateDescription,
    DayNumberOfWeek,
    DayNameOfWeek,
    DayNumberOfMonth,
    DayNumberOfYear,
    WeekNumberOfYear,
    MonthNumberOfYear,
    CalendarWeekNumberInYear,
    CalendarMonthNumberInYear,
    CalendarMonthName,
    CalendarYearMonth,
    CalendarQuarter,
    CalendarYear,
    FiscalWeek,
    FiscalWeekNumberInYear,
    FiscalMonth,
    FiscalMonthNumberInYear,
    FiscalYearMonth,
    FiscalQuarter,
    FiscalYearQuarter,
    FiscalYear)
    VALUES 
    (
        @calendarYear * 10000 + @calendarMonthNumberInYear * 100 + @dayNumberInMonth, 
        @currentDate, 
        CONVERT(varchar(2), @dayNumberInMonth) + ' ' + @calendarMonthName + ' ' + CONVERT(varchar(4), @calendarYear),         
        DATEPART(DW, @currentDate),
        dbo.udf_GetWeekDayName(@currentDate, 0),
        DATEPART(DAY, @currentDate), 
        DATEPART(DY, @currentDate), 
        DATEPART(WEEK, @currentDate), 
        DATEPART(MONTH, @currentDate), 
        DATEPART(WEEK, @currentDate), 
        DATEPART(MONTH, @currentDate), 
        DATENAME(mm, @currentDate), 
        CONVERT(varchar(4), @calendarYear) + '-' + CONVERT(varchar(2), @calendarMonthNumberInYear), 
        DATEPART(qq, @currentDate), 
        DATEPART(YEAR, @currentDate), 
        DATEPART(WEEK, @currentDate), 
        DATEPART(WEEK, @currentDate), 
        DATEPART(MONTH, @currentDate), 
        DATEPART(MONTH, @currentDate), 
        CONVERT(varchar(4), @calendarYear) + '-' + CONVERT(varchar(2), @calendarMonthNumberInYear), 
        DATEPART(qq, @currentDate), 
        DATEPART(qq, @currentDate), 
        DATEPART(YEAR, @currentDate)
    )

    UPDATE DimDate
    SET FullDate = CONVERT(varchar(10), FullDateAKey)
    WHERE SKey = @calendarYear * 10000 + @calendarMonthNumberInYear * 100 + @dayNumberInMonth


    --Dummy DimSite entry
    DELETE dbo.DimSite
    WHERE SKey = 0

    SET IDENTITY_INSERT DimSite ON

    INSERT INTO dbo.DimSite 
    (
        [SKey], 
        [AKey], 
        [SiteId], 
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
        [Enabled],         
        [_DeletedFlag],
        [_RecordUpdatedDate]
    )
    VALUES
    (
        0,
        @dummyId,
        @dummyId,
        0,
        @dummyId,
        @dummyId,
        @dummyId,
        @dummyId,
        @dummyId,
        @shortDummyId,
        @dummyId,
        @dummyId,
        @dummyId,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        GETDATE()
    )

    SET IDENTITY_INSERT DimSite OFF

    --Dummy DimProduct entry
    DELETE dbo.DimProduct
    WHERE SKey = 0

    SET IDENTITY_INSERT dbo.DimProduct ON

    INSERT INTO dbo.DimProduct 
    (
        [SKey],
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [ProductId],
        [ProductCode],
        [Description],
        [ProductTypeName],
        [TrackingProductSKey],
        [TrackingProductId],
        [VolumeDecimalPlaces],
        [AviationFuelFlag],
        [GroundFuel],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],  
        [VarianceTolerance],
        [StartDate], 
        [EndDate]
    )
    VALUES
    (
        0,
        @dummyId,
        @dummyId,
        0,
        @dummyId,
        @dummyId,
        @dummyId,
        @dummyId,
        0,
        @dummyId,
        0,
        0,
        0,
        0,
        @dummyId,
        @dummyDate,
        0,
        @dummyDate,
        NULL
    )

    SET IDENTITY_INSERT dbo.DimProduct OFF


    --Dummy DimCompany entry
    DELETE dbo.DimCompany
    WHERE SKey = 0

    SET IDENTITY_INSERT dbo.DimCompany ON

    INSERT INTO dbo.DimCompany
    (
        [SKey],
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [CompanyId],
        [Name],
        [Code],
        [Address1],
        [Address2],
        [City],
        [State],
        [Zip],
        [Country],
        [Phone],
        [EmergencyContact],
        [EmergencyPhone],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],        
        [StartDate], 
        [EndDate]
    )
    VALUES
    (        
        0,
        @dummyId,
        @dummyId,
        0,
        @dummyId,
        @dummyId,
        @shortDummyId,
        @dummyId,
        @dummyId,
        @dummyId,
        @dummyId,
        @shortDummyId,
        @dummyId,
        @dummyId,
        @dummyId,
        @dummyId,
        0,
        @dummyId,
        @dummyDate,
        @dummyDate,
        NULL
    )
    
    SET IDENTITY_INSERT dbo.DimCompany OFF


    --Dummy DimEquipmentType entry
    DELETE dbo.DimEquipmentType
    WHERE SKey = 0

    SET IDENTITY_INSERT dbo.DimEquipmentType ON

    INSERT INTO dbo.DimEquipmentType
    (
        [SKey],
        [AKey],
        [EquipmentTypeName], 
        [EquipmentTypeDescription],
	    [EquipmentTypeIndex],
	    [EquipmentTypeClass],
	    [Capacity],
	    [Make],
	    [Model],
	    [Year],
        [_DeletedFlag],
        [_RecordUpdatedDate]       
    )
    VALUES
    (
        0,
        @dummyId,
        @dummyId, 
        @dummyId,
        -1,
        @dummyId,
        0,
        @dummyId,
        @dummyId,
        1900,
        0,
        GETDATE()
    )

    SET IDENTITY_INSERT DimEquipmentType OFF


    --Dummy DimEquipment entry
    DELETE dbo.DimEquipment
    WHERE SKey = 0

    SET IDENTITY_INSERT dbo.DimEquipment ON

    INSERT INTO dbo.DimEquipment
    (
        [SKey],
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [EquipmentId],
        [EquipmentTypeSKey],
        [Description],
        [Make],
        [Model],
        [InUse],
        [SerialNumber],
        [StartDate], 
        [EndDate]
    )
    VALUES
    (
        0,
        @dummyId,
        @dummyId,
        0,
        @dummyId,
        0,
        @dummyId,
        @dummyId,
        @dummyId,
        0,
        @shortDummyId,
        @dummyDate,
        NULL
    )

    SET IDENTITY_INSERT DimEquipment OFF


    --Dummy DimPersonnel entry
    DELETE dbo.DimPersonnel
    WHERE SKey = 0

    SET IDENTITY_INSERT dbo.DimPersonnel ON

    INSERT INTO dbo.DimPersonnel 
    (
        [SKey],
        [AKey],
        [MasterRecordKey],
        [SiteSKey],
        [PersonID],
        [FirstName],
        [MiddleName],
        [LastName],
        [LockedOut],
        [LockedOutReason],
        [LockedOutDate],
        [StartDate],
        [EndDate]

    )
    VALUES
    (
        0,
        @dummyId,
        @dummyId,
        0,               
        @dummyId,
        @dummyId,
        @dummyId,
        @dummyId,
        0,
        @dummyId,
        @dummyDate, 
        @dummyDate,
        NULL
    )
    
    SET IDENTITY_INSERT dbo.DimPersonnel OFF


    --Dummy DimTransactionAlias entry
    DELETE dbo.DimTransactionAlias 
    WHERE SKey = 0

    SET IDENTITY_INSERT dbo.DimTransactionAlias ON

    INSERT INTO dbo.DimTransactionAlias 
    (
        [SKey],        
        [AKey],
        [MasterRecordKey],
        [SiteSKey],  
        [AliasName],
	    [TransactionTypeSKey],
        [StartDate], 
        [EndDate]
    )
    VALUES
    (
        0,
        @dummyId,
        @dummyId,
        0,                
        @dummyId,
		@dummyTransactionTypeSKey,
        @dummyDate, 
        NULL
    )
    
    SET IDENTITY_INSERT dbo.DimTransactionAlias OFF


    --Dummy DimAutoDistributionReasonCodes entry
    DELETE dbo.DimAutoDistributionReasonCodes
    WHERE SKey = 0

    SET IDENTITY_INSERT dbo.DimAutoDistributionReasonCodes ON

    INSERT INTO dbo.DimAutoDistributionReasonCodes 
    (
        [SKey],
        [AKey],
        [SiteSKey],
		[ReasonCode],
        [_DeletedFlag],
        [_RecordUpdatedDate]
        
    )
    VALUES
    (
        0,
        @dummyId,
        0,              
        @dummyId,
        0,
        GETDATE()
    )
    
    SET IDENTITY_INSERT dbo.DimAutoDistributionReasonCodes OFF



    --Dummy DimStation entry
    DELETE dbo.DimStation
    WHERE SKey = 0

    SET IDENTITY_INSERT dbo.DimStation ON

    INSERT INTO dbo.DimStation
    (
        [SKey],
        [AKey],
        [SiteSKey],
        [StationId],
		[StationInterfaceTypeCode],
        [_DeletedFlag],
        [_RecordUpdatedDate]
        
    )
    VALUES
    (
        0,
        @dummyId,
        0,   
        @dummyId,
        @dummyId,
        0,
        GETDATE()
    )
    
    SET IDENTITY_INSERT dbo.DimStation OFF



    --Dummy DimLoadArm entry
    DELETE dbo.DimLoadArm
    WHERE SKey = 0

    SET IDENTITY_INSERT dbo.DimLoadArm ON

    INSERT INTO dbo.DimLoadArm
    (
        [SKey],
        [AKey],
        [StationSKey],
        [ArmNumber],
        [SwingArm],
        [LoadRackText],
        [BayId],
        [_DeletedFlag],
        [_RecordUpdatedDate]
        
    )
    VALUES
    (
        0,
        @dummyId,
        0,   
        0,
        0,
        @shortDummyId,
        @shortDummyId,
        0,
        GETDATE()
    )
    
    SET IDENTITY_INSERT dbo.DimLoadArm OFF




    --Dummy DimTank entry
    DELETE dbo.DimTank
    WHERE SKey = 0

    SET IDENTITY_INSERT dbo.DimTank ON

    INSERT INTO dbo.DimTank
    (
        [SKey],
        [AKey],
        [SiteSKey],
        [TankId],
		[VesselTypeName],
        [_DeletedFlag],
        [_RecordUpdatedDate]
        
    )
    VALUES
    (
        0,
        @dummyId,
        0,   
        @dummyId,
        @dummyId,
        0,
        GETDATE()
    )
    
    SET IDENTITY_INSERT dbo.DimTank OFF


    
    --Dummy DimTransactionAttributes entry
    DELETE dbo.DimTransactionAttributes
    WHERE SKey = 0

    SET IDENTITY_INSERT DimTransactionAttributes ON

    INSERT INTO dbo.DimTransactionAttributes 
    (
        [SKey], 
        [DeleteFlag], 
        [ReversalType], 
        [SubType], 
        [TransactionStatusName], 
        [InvalidTerminalTime],
        [GrossQuantitySign],
        [IsRecordDeleted],
        [_DeletedFlag], 
        [_RecordUpdatedDate]
    )
    VALUES (0, 0, @veryShortDummyId, @dummyId, @dummyId, 0, @shortDummyId, 0, 0, GETDATE())

    SET IDENTITY_INSERT dbo.DimTransactionAttributes OFF

    
    --Dummy FactTransaction entry. Entry to provide a DeleteFlag entry of [False], in order to support the setting of the DimTransactionAttributes.DeleteFlag.DefaultMember value during the initial ETL.		
    TRUNCATE TABLE staging.tblTransactions

    INSERT INTO staging.tblTransactions (TransactionKey, DeleteFlag)
    VALUES (@dummyId, 0)

    EXEC [staging].[usp_LoadTransactionHeaders] 0, 0

    TRUNCATE TABLE staging.tblTransactions


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
    + 'Procedure Name: [dbo].[usp_LoadDummyEntries]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END