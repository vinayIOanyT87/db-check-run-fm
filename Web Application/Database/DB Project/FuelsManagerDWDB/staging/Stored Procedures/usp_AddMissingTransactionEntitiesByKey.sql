/*
    DROP PROCEDURE [staging].[usp_AddMissingTransactionEntitiesByKey]

	EXEC [staging].[usp_AddMissingTransactionEntitiesByKey]
	
*/
CREATE PROCEDURE [staging].[usp_AddMissingTransactionEntitiesByKey]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_AddMissingTransactionEntitiesByKey]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Artificially adding new entities to support Transaction references to missing entities, by Key,
  -- for entity references that are not covered by a referential foreign key.
  -- This operation is only relevant for the initial OLTP extraction, before the enforcement of the CDC.
  -- This procedure creates a dummy entity entry for each Transaction reference to an external entity that cannot be found.
  -- Notes:
  -- 1. The process of re-creating target entities is limited to transactions only.
  -- 2. Since this procedure is intended only to be used during the initial extraction of OLTP data, and not
  --    for OLTP data changes made after the CDC is in place, the existence of the target entities are only
  --	  verified against the staging tables and not against the dimension tables.
  -- 3. The re-created dummy entities are end-dated at the time of creation since they do not exist anymore.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyEntityLabel varchar(30)
    DECLARE @dummyEntityIDPrefix varchar(30)
    
    SELECT @dummyEntityLabel = 'Dummy Record Added By the ETL'
    SELECT @dummyEntityIDPrefix = 'DummyRecord_'

    DECLARE @runningKey int


    DECLARE @dummyStartDate datetimeoffset(7)
    SELECT
      @dummyStartDate = DATEFROMPARTS(1900, 01, 01)


    -- Transaction Header
    -- Transaction Header Sites
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne


    -- Transaction Header Companies
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne

    --Transaction records for which the company key references exist but do not point to an existing entity
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT x.CompanyKey, x.CompanyId, x.SiteKey, 0
    FROM 
    (
        SELECT
        a.ShipToCompanyKey CompanyKey,
        a.ShipToID CompanyId,
        a.SiteKey,
        IgnoreRecord
        FROM staging.tblTransactions a
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblCompanies b
            WHERE b.CompanyKey = a.ShipToCompanyKey
        )
        AND a.ShipToCompanyKey IS NOT NULL
        UNION
        SELECT
        a.SupplierCompanyKey CompanyKey,
        a.SupplierID CompanyId,
        a.SiteKey,
        IgnoreRecord
        FROM staging.tblTransactions a
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblCompanies b
            WHERE b.CompanyKey = a.SupplierCompanyKey
        )
        AND a.SupplierCompanyKey IS NOT NULL
        UNION
        SELECT
        a.ShipperCompanyKey CompanyKey,
        a.ShipperID CompanyId,
        a.SiteKey,
        IgnoreRecord
        FROM staging.tblTransactions a
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblCompanies b
            WHERE b.CompanyKey = a.ShipperCompanyKey
        )
        AND a.ShipperCompanyKey IS NOT NULL
        UNION
        SELECT
        a.OwnerCompanyKey CompanyKey,
        a.OwnerID CompanyId,
        a.SiteKey,
        IgnoreRecord
        FROM staging.tblTransactions a
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblCompanies b
            WHERE b.CompanyKey = a.OwnerCompanyKey
        )
        AND a.OwnerCompanyKey IS NOT NULL
        UNION
        SELECT
        a.ManagerCompanyKey CompanyKey,
        a.ManagerID CompanyId,
        a.SiteKey,
        IgnoreRecord
        FROM staging.tblTransactions a
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblCompanies b
            WHERE b.CompanyKey = a.ManagerCompanyKey
        )
        AND a.ManagerCompanyKey IS NOT NULL
        UNION
        SELECT
        a.CarrierCompanyKey CompanyKey,
        a.CarrierID CompanyId,
        a.SiteKey,
        IgnoreRecord
        FROM staging.tblTransactions a
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblCompanies b
            WHERE b.CompanyKey = a.CarrierCompanyKey
        )
        AND a.CarrierCompanyKey IS NOT NULL
        UNION
        SELECT
        a.BillToCompanyKey CompanyKey,
        a.BillToID CompanyId,
        a.SiteKey,
        IgnoreRecord
        FROM staging.tblTransactions a
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblCompanies b
            WHERE b.CompanyKey = a.BillToCompanyKey
        )
        AND a.BillToCompanyKey IS NOT NULL
    ) x
    WHERE x.IgnoreRecord = 0

    INSERT INTO staging.tblCompanies (CompanyKey, SiteKey, Id, Name, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    SELECT
        RecordKey,
        SiteKey,
        ISNULL(RecordId, @dummyEntityIDPrefix + RIGHT(RecordKey, 6)),
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
    FROM staging.tblMissingEntitiesTempOne

    -- Transaction Header StationKeys (FinalStationKey, PreviousStationKey, NextStationKey, OriginStationKey) not supported in a separate dimension or mapping table.

    -- Transaction Header Equipments
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne

    --Transaction records for which the equipment key references exist but do not point to an existing entity
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT
    x.EquipmentKey,
    x.EquipmentId,
    x.SiteKey,
    0
    FROM 
    (
        SELECT
        a.DestinationEquipment1Key EquipmentKey,
        a.DestinationCompanyEquipmentID1 EquipmentId,
        a.SiteKey,
        IgnoreRecord
        FROM staging.tblTransactions a
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblEquipment b
            WHERE b.EquipmentKey = a.DestinationEquipment1Key
        )
        AND a.DestinationEquipment1Key IS NOT NULL
        UNION
        SELECT
        a.SourceEquipment1Key EquipmentKey,
        a.SourceCompanyEquipmentID1 EquipmentId,
        a.SiteKey,
        IgnoreRecord
        FROM staging.tblTransactions a
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblEquipment b
            WHERE b.EquipmentKey = a.SourceEquipment1Key
        )
        AND a.SourceEquipment1Key IS NOT NULL
    ) x
    WHERE x.IgnoreRecord = 0

    INSERT INTO staging.tblEquipment (EquipmentKey, SiteKey, Id, Description, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    SELECT
        RecordKey,
        SiteKey,
        ISNULL(RecordId, @dummyEntityIDPrefix + RIGHT(RecordKey, 6)),
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
    FROM staging.tblMissingEntitiesTempOne

    -- Transaction FuelCardKey not supported in a separate dimension or mapping table.

    -- Transaction Header Operator
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne

    --Transaction records for which the Operator key reference exists but does not point to an existing entity
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT
    a.OperatorPersonnelKey,
    a.OperatorID,
    a.SiteKey,
    0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblPersonnel b
        WHERE b.PersonnelKey = a.OperatorPersonnelKey
    )
    AND a.OperatorPersonnelKey IS NOT NULL
    AND a.IgnoreRecord = 0

    INSERT INTO staging.tblPersonnel (PersonnelKey, SiteKey, PersonId, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    SELECT
        RecordKey,
        SiteKey,
        ISNULL(RecordId, @dummyEntityIDPrefix + RIGHT(RecordKey, 6)),
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
    FROM staging.tblMissingEntitiesTempOne


    -- Transaction Header TransactionAlias
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne

    --Transaction records for which the TransactionAlias key reference exists but does not point to an existing entity
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, FieldB, IsProcessed)
    SELECT 
    a.TransactionAliasKey,
    a.TransactionAliasName,    
    a.SiteKey,
    MAX(a.TransactionTypeIndex),
    0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblTransactionAliases b
        WHERE b.TransactionAliasKey = a.TransactionAliasKey
    )
    AND a.TransactionAliasKey IS NOT NULL
    AND a.IgnoreRecord = 0
    GROUP BY a.TransactionAliasKey, a.TransactionAliasName, a.SiteKey

    INSERT INTO staging.tblTransactionAliases(TransactionAliasKey, SiteKey, AliasName, LookupTransTypeIndex, TransactionTypeSKey, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    SELECT
        a.RecordKey,
        a.SiteKey,
        ISNULL(a.RecordId, @dummyEntityIDPrefix + RIGHT(RecordKey, 6)),
        a.FieldB,
        b.SKey,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
    FROM staging.tblMissingEntitiesTempOne a
    LEFT OUTER JOIN DimTransactionType b
    ON b.AKey = CONVERT(nvarchar(50), a.FieldB)




    -- The Transaction Types are fairly stable and are not expected to be deleted after transactions are created against them.

    -- TransactionLineItem

    -- TransactionLineItem AdditiveProfileKey not supported in a separate dimension or mapping table.

    -- TransactionLineItem Equipments

    TRUNCATE TABLE staging.tblMissingEntitiesTempOne

    --Transaction records for which the equipment key references exist but do not point to an existing entity
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT
    x.EquipmentKey,
    x.EquipmentId,
    x.SiteKey,
    0
    FROM 
    (
        SELECT
        a.DestinationCompartmentKey EquipmentKey,
        a.DestinationCompartmentID EquipmentId,
        b.SiteKey,
        a.IgnoreRecord
        FROM staging.tblTransactionLineItems a
        INNER JOIN staging.tblTransactions b
        ON b.TransactionKey = a.TransactionKey
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblEquipment c
            WHERE c.EquipmentKey = a.DestinationCompartmentKey
        )
        AND a.DestinationCompartmentKey IS NOT NULL
        UNION
        SELECT
        a.DestinationEquipmentKey EquipmentKey,
        a.DestinationCompanyEquipmentID EquipmentId,
        b.SiteKey,
        a.IgnoreRecord
        FROM staging.tblTransactionLineItems a
        INNER JOIN staging.tblTransactions b
        ON b.TransactionKey = a.TransactionKey
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblEquipment c
            WHERE c.EquipmentKey = a.DestinationEquipmentKey
        )
        AND a.DestinationEquipmentKey IS NOT NULL
        UNION
        SELECT
        a.SourceCompartmentKey EquipmentKey,
        a.SourceCompartmentID EquipmentId,
        b.SiteKey,
        a.IgnoreRecord
        FROM staging.tblTransactionLineItems a
        INNER JOIN staging.tblTransactions b
        ON b.TransactionKey = a.TransactionKey
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblEquipment c
            WHERE c.EquipmentKey = a.SourceCompartmentKey
        )
        AND a.SourceCompartmentKey IS NOT NULL
        UNION
        SELECT
        a.SourceEquipmentKey EquipmentKey,
        a.SourceCompanyEquipmentID EquipmentId,
        b.SiteKey,
        a.IgnoreRecord
        FROM staging.tblTransactionLineItems a
        INNER JOIN staging.tblTransactions b
        ON b.TransactionKey = a.TransactionKey
        WHERE NOT EXISTS 
        (
            SELECT * FROM staging.tblEquipment c
            WHERE c.EquipmentKey = a.SourceEquipmentKey
        )
        AND a.SourceEquipmentKey IS NOT NULL
    ) x
    WHERE x.IgnoreRecord = 0

    INSERT INTO staging.tblEquipment (EquipmentKey, SiteKey, Id, Description, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    SELECT
        RecordKey,
        SiteKey,
        ISNULL(RecordId, @dummyEntityIDPrefix + RIGHT(RecordKey, 6)),
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
    FROM staging.tblMissingEntitiesTempOne


    -- TransactionLineItem Product
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne

    --Transaction LineItem records for which the product key reference exists but does not point to an existing entity
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT
    a.ProductKey,
    a.ProductId,
    b.SiteKey,
    0
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblProducts c
        WHERE c.ProductKey = a.ProductKey
    )
    AND a.ProductKey IS NOT NULL
    AND a.IgnoreRecord = 0    

    INSERT INTO staging.tblProducts (ProductKey, SiteKey, ProductId, Description, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    SELECT
        RecordKey,
        SiteKey,
        ISNULL(RecordId, @dummyEntityIDPrefix + RIGHT(RecordKey, 6)),
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
    FROM staging.tblMissingEntitiesTempOne


    -- TransactionSubLineItem Product
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne

    --Transaction SubLineItem records for which the product key reference exists but does not point to an existing entity
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT
    a.ProductKey,
    a.ProductId,
    b.SiteKey,
    0
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblProducts c
        WHERE c.ProductKey = a.ProductKey
    )
    AND a.ProductKey IS NOT NULL
    AND a.IgnoreRecord = 0    

    INSERT INTO staging.tblProducts (ProductKey, SiteKey, ProductId, Description, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    SELECT
        RecordKey,
        SiteKey,
        ISNULL(RecordId, @dummyEntityIDPrefix + RIGHT(RecordKey, 6)),
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
    FROM staging.tblMissingEntitiesTempOne



    -- TransactionLineItem Station
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne

    --Transaction LineItem records for which the Station key reference exists but does not point to an existing entity
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT
    a.LoadingLocationStationKey,
    a.LoadingLocationID,
    b.SiteKey,
    0
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblStations c
        WHERE c.StationKey = a.LoadingLocationStationKey
    )
    AND a.LoadingLocationStationKey IS NOT NULL
    AND a.IgnoreRecord = 0    

    INSERT INTO staging.tblStations (StationKey, SiteKey, Id, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted)
    SELECT
        RecordKey,
        SiteKey,
        ISNULL(RecordId, @dummyEntityIDPrefix + RIGHT(RecordKey, 6)),
        1,
        GETDATE(),
        1
    FROM staging.tblMissingEntitiesTempOne



-- TransactionLineItem Tank
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne

    --Transaction LineItem records for which the Storage Location Tank key reference exists but does not point to an existing entity
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT
    a.StorageLocationTankKey,
    a.StorageLocationID,
    b.SiteKey,
    0
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblTanks c
        WHERE c.TankKey = a.StorageLocationTankKey
    )
    AND a.StorageLocationTankKey IS NOT NULL
    AND a.IgnoreRecord = 0    

    INSERT INTO staging.tblTanks (TankKey, SiteKey, TankId, VesselTypeName, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted)
    SELECT
        RecordKey,
        SiteKey,
        ISNULL(RecordId, @dummyEntityIDPrefix + RIGHT(RecordKey, 6)),
        @dummyEntityLabel,
        1,
        GETDATE(),
        1
    FROM staging.tblMissingEntitiesTempOne



    -- TransactionSubLineItem Tank
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne

    --Transaction SubLineItem records for which the Storage Location Tank key reference exists but does not point to an existing entity
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT
    a.StorageLocationTankKey,
    a.StorageLocationID,
    b.SiteKey,
    0
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblTanks c
        WHERE c.TankKey = a.StorageLocationTankKey
    )
    AND a.StorageLocationTankKey IS NOT NULL
    AND a.IgnoreRecord = 0    

    INSERT INTO staging.tblTanks (TankKey, SiteKey, TankId, VesselTypeName, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted)
    SELECT
        RecordKey,
        SiteKey,
        ISNULL(RecordId, @dummyEntityIDPrefix + RIGHT(RecordKey, 6)),
        @dummyEntityLabel,
        1,
        GETDATE(),
        1
    FROM staging.tblMissingEntitiesTempOne



    -- TransactionSubLineItem does not have a separate Station Key, but inherits the Station Key from the parent LineItem.

    -- TransactionSubLineItem does not support a LoadArmKey on the source table (the LoadArmKey field in staging.tblLoadArms is introduced only to support the ETL).


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
    + 'Procedure Name: [staging].[usp_AddMissingTransactionEntitiesByKey]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END