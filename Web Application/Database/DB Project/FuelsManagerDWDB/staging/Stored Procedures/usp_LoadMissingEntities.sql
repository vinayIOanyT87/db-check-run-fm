/*
	DROP PROCEDURE [Staging].[usp_LoadMissingEntities]

	EXEC [staging].[usp_LoadMissingEntities]
	
*/
CREATE PROCEDURE [staging].[usp_LoadMissingEntities]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_LoadMissingEntities]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads records that have been found missing (while loading Transactions) from staging into Entity/Dimension tables in the OLAP database.
  -- Notes:
  -- 1. Those corresponds to records that have likely been deleted from the system, after they have been tied to transactions.
  -- 2. Missing entities are mostly expected on the initial ETL run, i.e. before the implementation of fmcdc which would automatically capture deleted entities.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY    

    DECLARE @dummyDate datetimeoffset(7) = '1/1/1900'
    DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
    DECLARE @shortDummyId varchar(4) = '<NA>'

    --EquipmentType
    INSERT INTO dbo.DimEquipmentType ([AKey], [EquipmentTypeName], [EquipmentTypeDescription], [Capacity], [Make], [Model], [Year], [EquipmentTypeIndex], [EquipmentTypeClass], [_DeletedFlag], [_RecordUpdatedDate], [_IsRecordAddedByETL])
    SELECT
    a.[EquipmentTypeKey],
    a.[EqTypeName],
    a.[EqTypeDescription],
    a.[Capacity],
    a.[Make],
    a.[Model],
    a.[Year],
    a.[LookupEquipmentTypeIndex],
    a.[LookupEquipmentTypeName],
    a.[IsRecordDeleted],
    a.[CombinedUpdatedDate],
    a.[IsRecordAddedByETL]
    FROM staging.tblEquipmentTypes a    
    WHERE a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM DimEquipmentType b
        WHERE b.AKey = a.EquipmentTypeKey
    )


    --Product
    INSERT INTO dbo.DimProduct ([AKey], [MasterRecordKey], [SiteSKey], [ProductId], [ProductCode], [Description], [ProductTypeName], [TrackingProductSKey], [TrackingProductId], [VolumeDecimalPlaces], [AviationFuelFlag], [GroundFuel], [LockedOut], [LockedOutReason], [LockedOutDate], [VarianceTolerance], [StartDate], [EndDate], [_IsRecordAddedByETL])
    SELECT
    a.[ProductKey],
    a.[ProductKey],
    b.[SKey],
    a.[ProductId],    
    ISNULL(a.[ProductCode], @dummyId),
    a.[Description],
    a.[ProductTypeName],
    ISNULL(a.[TrackingProductSKey], 0), 
	ISNULL(a.[TrackingProductID], @dummyId), 
    a.[VolumeDecimalPlaces], 
    ISNULL(a.[AviationFuelFlag], 0),
    ISNULL(a.[GroundFuel], 0),
    ISNULL(a.[LockedOut], 0),
    a.[LockedOutReason], 
    a.[LockedOutDate],
    ISNULL(a.[VarianceTolerance], 0),
    a.[StartDate],
    a.[EndDate],
    a.[IsRecordAddedByETL]
    FROM staging.tblProducts a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.EndDate IS NOT NULL
    AND a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM DimProduct c
        WHERE c.AKey = a.ProductKey
    )

    INSERT INTO map.tblEntityProductToSite 
    ([ProductToSiteKey], [ProductKey], [AssignedFromSiteSKey], [SiteSKey], [CreatedDate])
    SELECT
    NULL,
    a.[ProductKey],
    b.[SKey],
    b.[SKey],
    @dummyDate
    FROM staging.tblProducts a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.EndDate IS NOT NULL
    AND a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM map.tblEntityProductToSite c
        WHERE c.ProductKey = a.ProductKey
    )


    --Company
    INSERT INTO dbo.DimCompany ([AKey], [MasterRecordKey], [SiteSKey], [CompanyID], [Name], [Code], [Address1], [Address2], [City], [State], [Zip], [Country], [Phone], [EmergencyContact], [EmergencyPhone], [LockedOut], [LockedOutReason], [LockedOutDate], [StartDate], [EndDate], [_IsRecordAddedByETL])
    SELECT
    a.[CompanyKey],
    a.[CompanyKey],
    b.[SKey],
    a.[ID],
    a.[Name],
    a.[Code],
    a.[Address1],
    a.[Address2],
    a.[City],
    a.[state],
    a.[Zip],
    a.[Country],
    a.[Phone], 
    a.[EmergencyContact], 
    a.[EmergencyPhone], 
    ISNULL(a.[LockedOut], 0),
    a.[LockedOutReason], 
    a.[LockedOutDate],
    a.[StartDate],
    a.[EndDate],
    a.[IsRecordAddedByETL]
    FROM staging.tblCompanies a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.EndDate IS NOT NULL
    AND a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM DimCompany c
        WHERE c.AKey = a.CompanyKey
    )

    INSERT INTO map.tblEntityCompanyToSite 
    ([CompanyToSiteKey], [CompanyKey], [AssignedFromSiteSKey], [SiteSKey], [CreatedDate])
    SELECT
    NULL,
    a.[CompanyKey],
    b.[SKey],
    b.[SKey],
    @dummyDate
    FROM staging.tblCompanies a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.EndDate IS NOT NULL
    AND a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM map.tblEntityCompanyToSite c
        WHERE c.CompanyKey = a.CompanyKey
    )


    --Equipment
    -- Insert all end-dated, historical records in the order the change history was recorded in the OLTP database
    INSERT INTO dbo.DimEquipment ([AKey], [MasterRecordKey], [SiteSKey], [EquipmentId], [EquipmentTypeSKey], [Description], [Make], [Model], [InUse], [SerialNumber], [StartDate], [EndDate], [_IsRecordAddedByETL])
    SELECT
    a.[EquipmentKey],
    a.[EquipmentKey],
    b.[SKey],
    a.[ID],        
    ISNULL(a.[EquipmentTypeSKey], 0),
    a.[Description],
    a.[Make],
    a.[Model],
    ISNULL(a.[InUse], 0),
    a.[SerialNumber],
    a.[StartDate],
    a.[EndDate],
    a.[IsRecordAddedByETL]
    FROM staging.tblEquipment a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.EndDate IS NOT NULL
    AND a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM DimEquipment c
        WHERE c.AKey = a.EquipmentKey
    )

    INSERT INTO map.tblEntityEquipmentToSite 
    ([EquipmentToSiteKey], [EquipmentKey], [AssignedFromSiteSKey], [SiteSKey], [CreatedDate])
    SELECT
    NULL,
    a.[EquipmentKey],
    b.[SKey],
    b.[SKey],
    @dummyDate
    FROM staging.tblEquipment a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.EndDate IS NOT NULL
    AND a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM map.tblEntityEquipmentToSite c
        WHERE c.EquipmentKey = a.EquipmentKey
    )


    --Personnel
    INSERT INTO dbo.DimPersonnel ([AKey], [MasterRecordKey], [SiteSKey], [PersonID], [FirstName], [MiddleName], [LastName], [LockedOut], [LockedOutReason], [LockedOutDate], [StartDate], [EndDate], [_IsRecordAddedByETL])
    SELECT
    a.[PersonnelKey],
    a.[PersonnelKey],
    b.[SKey],
    a.[PersonID], 
    a.[FirstName], 
    a.[MiddleName],
    a.[LastName], 
    ISNULL([LockedOut], 0),
    a.[LockedOutReason], 
    a.[LockedOutDate],
    a.[StartDate],
    a.[EndDate],
    a.[IsRecordAddedByETL]
    FROM staging.tblPersonnel a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.EndDate IS NOT NULL
    AND a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM dbo.DimPersonnel c
        WHERE c.AKey = a.PersonnelKey
    )

    INSERT INTO map.tblEntityPersonnelToSite 
    ([PersonnelToSiteKey], [PersonnelKey], [AssignedFromSiteSKey], [SiteSKey], [CreatedDate])
    SELECT
    NULL,
    a.[PersonnelKey],
    b.[SKey],
    b.[SKey],
    @dummyDate
    FROM staging.tblPersonnel a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.EndDate IS NOT NULL
    AND a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM map.tblEntityPersonnelToSite c
        WHERE c.PersonnelKey = a.PersonnelKey
    )



    --TransactionAlias
    INSERT INTO dbo.DimTransactionAlias ([AKey], [MasterRecordKey], [SiteSKey], [AliasName], [TransactionTypeSKey], [StartDate], [EndDate], [_IsRecordAddedByETL])
    SELECT
    a.[TransactionAliasKey],
    a.[TransactionAliasKey],
    b.[SKey],
    a.[AliasName], 
    ISNULL(a.[TransactionTypeSKey], 0) [TransactionTypeSKey],
    a.[StartDate],
    a.[EndDate],
    a.[IsRecordAddedByETL]
    FROM staging.tblTransactionAliases a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.EndDate IS NOT NULL
    AND a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM dbo.DimTransactionAlias c
        WHERE c.AKey = a.TransactionAliasKey
    )

    INSERT INTO map.tblEntityTransactionAliasToSite 
    ([TransactionAliasToSiteKey], [TransactionAliasKey], [AssignedFromSiteSKey], [SiteSKey], [CreatedDate])
    SELECT
    NULL,
    a.[TransactionAliasKey],
    b.[SKey],
    b.[SKey],
    @dummyDate
    FROM staging.tblTransactionAliases a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.EndDate IS NOT NULL
    AND a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM map.tblEntityTransactionAliasToSite c
        WHERE c.TransactionAliasKey = a.TransactionAliasKey
    )


    --Station
    INSERT INTO dbo.DimStation (
    [AKey], [SiteSKey], [StationId], [StationInterfaceTypeCode], [_RecordUpdatedDate], [_DeletedFlag], [_IsRecordAddedByETL])
    SELECT
    a.[StationKey],
    b.[SKey],
    a.[Id],    
    a.[StationInterfaceTypeCode],
    a.[CombinedUpdatedDate],
    a.[IsRecordDeleted],
    a.[IsRecordAddedByETL]
    FROM staging.tblStations a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM DimStation c
        WHERE c.AKey = a.StationKey
    )

    -- Update LoadArm Station references - Missing LoadArm Station references are only added to the BayAStation (not to the BayBStation)
	UPDATE a 
	SET a.BayAStationSKey = b.SKey
	FROM staging.tblLoadArms a
	INNER JOIN dbo.DimStation b
	ON b.AKey = a.BayAStationKey
	WHERE a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1


    --LoadArm
    INSERT INTO dbo.DimLoadArm (
    [AKey],	[StationSKey],	[ArmNumber], [SwingArm], [LoadRackText], [BayId], [_RecordUpdatedDate], [_DeletedFlag], [_IsRecordAddedByETL])
    SELECT
    a.LoadArmKey,
    a.[BayAStationSKey],
    a.[BayAArmNumber],
    ISNULL(a.[SwingArm], 0) SwingArm,    
    a.[LoadRackText],
    'BayA' BayId,
    a.[CombinedUpdatedDate],
    a.[IsRecordDeleted],
    a.[IsRecordAddedByETL]
    FROM staging.tblLoadArms a
    WHERE a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM DimLoadArm c
        WHERE c.AKey = a.LoadArmKey
    )


    -- Tank
    INSERT INTO dbo.DimTank (
    [AKey], [SiteSKey], [TankId], [VesselTypeName], [_RecordUpdatedDate], [_DeletedFlag], [_IsRecordAddedByETL])
    SELECT
    a.[TankKey],
    b.[SKey],
    a.[TankID],    
    a.[VesselTypeName],
    a.[CombinedUpdatedDate],
    a.[IsRecordDeleted],
    a.[IsRecordAddedByETL]
    FROM staging.tblTanks a
    INNER JOIN dbo.DimSite b
    ON b.AKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.IsRecordAddedByETL = 1
    AND NOT EXISTS 
    (
        SELECT * FROM DimStation c
        WHERE c.AKey = a.TankKey
    )



    --TransactionAlias
    -- Do not expect a Transaction to be pointing to a missing Transaction Aliases

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
    + 'Procedure Name: [staging].[usp_LoadMissingEntities]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END