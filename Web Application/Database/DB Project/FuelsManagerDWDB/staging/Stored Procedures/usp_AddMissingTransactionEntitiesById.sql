/*
    DROP PROCEDURE [staging].[usp_AddMissingTransactionEntitiesById]

	EXEC [staging].[usp_AddMissingTransactionEntitiesById]
	
*/
CREATE PROCEDURE [staging].[usp_AddMissingTransactionEntitiesById]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_AddMissingTransactionEntitiesById]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Artificially adding new entities to support Transaction references to missing entities, by Id,
  -- for entity references that are not covered by a referential foreign key.
  -- Notes:
  -- 1. The process of re-creating target entities is limited to transactions only.
  -- 2. The re-created dummy entities are end-dated at the time of creation since they do not exist anymore.
  -- 3. The operation assumes that the values of the ID fields have been pre-trimmed first. This is required to
  --    prevent the addition of multiple entity records that only differ by leading or trailing whitespaces in
  --    the ID fields, a condition that is likely to lead to duplicate errors when processing the cube.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @dummyEntityLabel varchar(30) = 'Dummy Record Added By the ETL'
    DECLARE @dummyStartDate datetimeoffset(7) = '1/1/1900'
    DECLARE @shortDummyId varchar(4) = '<NA>'


    -- Transaction Header
    -- Transaction Header Sites
    -- No support for missing Site references (because DimSite is used to locate missing entities below)
    -- Assumption: DimSite is fitted with all the relevant sites




    -- Transaction Header Companies

    -- Transaction Header ShipTo Companies
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction records for which the company key references do not exist but the Transaction Company Id field is populated and does not correspond to an existing Company record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.ShipToId, a.SiteKey, 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblCompanies b
        WHERE b.Id = a.ShipToId
    )
    AND a.ShipToId IS NOT NULL
    AND LEN(a.ShipToId) > 0
    AND a.ShipToCompanyKey IS NULL
    AND a.ShipToCompanySKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimCompany a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.CompanyId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblCompanies (CompanyKey, Id, SiteKey, Name, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.CompanyKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.ShipToCompanyKey = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.ShipToID
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.ShipToCompanyKey IS NULL
    AND a.ShipToCompanySKey IS NULL



    -- Transaction Header Supplier Companies
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo
    
    --Transaction records for which the company key references do not exist but the Transaction Company field field is populated and does not correspond to an existing Company record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.SupplierId, a.SiteKey, 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblCompanies b
        WHERE b.Id = a.SupplierId
    )
    AND a.SupplierId IS NOT NULL
    AND LEN(a.SupplierId) > 0
    AND a.SupplierCompanyKey IS NULL
    AND a.SupplierCompanySKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimCompany a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.CompanyId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblCompanies (CompanyKey, Id, SiteKey, Name, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.CompanyKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.SupplierCompanyKey = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.SupplierID
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.SupplierCompanyKey IS NULL
    AND a.SupplierCompanySKey IS NULL



    -- Transaction Header Shipper Companies
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction records for which the company key references do not exist but the Transaction Company field field is populated and does not correspond to an existing Company record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.ShipperId, a.SiteKey, 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblCompanies b
        WHERE b.Id = a.ShipperId
    )
    AND a.ShipperId IS NOT NULL
    AND LEN(a.ShipperId) > 0
    AND a.ShipperCompanyKey IS NULL
    AND a.ShipperCompanySKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimCompany a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.CompanyId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblCompanies (CompanyKey, Id, SiteKey, Name, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.CompanyKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.ShipperCompanyKey = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.ShipperID
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.ShipperCompanyKey IS NULL
    AND a.ShipperCompanySKey IS NULL


    -- Transaction Header Owner Companies
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction records for which the company key references do not exist but the Transaction Company field field is populated and does not correspond to an existing Company record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.OwnerId, a.SiteKey, 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblCompanies b
        WHERE b.Id = a.OwnerId
    )
    AND a.OwnerId IS NOT NULL
    AND LEN(a.OwnerId) > 0
    AND a.OwnerCompanyKey IS NULL
    AND a.OwnerCompanySKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimCompany a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.CompanyId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblCompanies (CompanyKey, Id, SiteKey, Name, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.CompanyKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.OwnerCompanyKey = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.OwnerID
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.OwnerCompanyKey IS NULL
    AND a.OwnerCompanySKey IS NULL



    -- Transaction Header Manager Companies
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction records for which the company key references do not exist but the Transaction Company field field is populated and does not correspond to an existing Company record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.ManagerId, a.SiteKey, 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblCompanies b
        WHERE b.Id = a.ManagerId
    )
    AND a.ManagerId IS NOT NULL
    AND LEN(a.ManagerId) > 0
    AND a.ManagerCompanyKey IS NULL
    AND a.ManagerCompanySKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimCompany a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.CompanyId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblCompanies (CompanyKey, Id, SiteKey, Name, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.CompanyKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.ManagerCompanyKey = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.ManagerID
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.ManagerCompanyKey IS NULL
    AND a.ManagerCompanySKey IS NULL



    -- Transaction Header Carrier Companies
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction records for which the company key references do not exist but the Transaction Company field field is populated and does not correspond to an existing Company record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.CarrierId, a.SiteKey, 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblCompanies b
        WHERE b.Id = a.CarrierId
    )
    AND a.CarrierId IS NOT NULL
    AND LEN(a.CarrierId) > 0
    AND a.CarrierCompanyKey IS NULL
    AND a.CarrierCompanySKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimCompany a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.CompanyId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblCompanies (CompanyKey, Id, SiteKey, Name, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.CompanyKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.CarrierCompanyKey = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.CarrierID
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.CarrierCompanyKey IS NULL
    AND a.CarrierCompanySKey IS NULL



    -- Transaction Header BillTo Companies
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction records for which the company key references do not exist but the Transaction Company field field is populated and does not correspond to an existing Company record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.BillToId, a.SiteKey, 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblCompanies b
        WHERE b.Id = a.BillToId
    )
    AND a.BillToId IS NOT NULL
    AND LEN(a.BillToId) > 0
    AND a.BillToCompanyKey IS NULL
    AND a.BillToCompanySKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimCompany a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.CompanyId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblCompanies (CompanyKey, Id, SiteKey, Name, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.CompanyKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.BillToCompanyKey = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.BillToID
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.BillToCompanyKey IS NULL
    AND a.BillToCompanySKey IS NULL



    -- Transaction Header StationKeys (FinalStationKey, PreviousStationKey, NextStationKey, OriginStationKey) not supported in a separate dimension or mapping table.


    -- Transaction Header Equipments
    -- Transaction DestinationEquipment1 Equipments
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction records for which the equipment key references do not exist but the Transaction Equipment field field is populated and does not correspond to an existing Equipment record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.DestinationCompanyEquipmentID1, a.SiteKey, 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblEquipment b
        WHERE b.Id = a.DestinationCompanyEquipmentID1
    )
    AND a.DestinationCompanyEquipmentID1 IS NOT NULL
    AND LEN(a.DestinationCompanyEquipmentID1) > 0
    AND a.DestinationEquipment1Key IS NULL
    AND a.DestinationEquipment1SKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimEquipment a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.EquipmentId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblEquipment (EquipmentKey, Id, SiteKey, Description, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.EquipmentKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.DestinationEquipment1Key = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.DestinationCompanyEquipmentID1
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.DestinationEquipment1Key IS NULL
    AND a.DestinationEquipment1SKey IS NULL



    --Transaction SourceEquipment1 Equipments
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction records for which the equipment key references do not exist but the Transaction Equipment field field is populated and does not correspond to an existing Equipment record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.SourceCompanyEquipmentID1, a.SiteKey, 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblEquipment b
        WHERE b.Id = a.SourceCompanyEquipmentID1
    )
    AND a.SourceCompanyEquipmentID1 IS NOT NULL
    AND LEN(a.SourceCompanyEquipmentID1) > 0
    AND a.SourceEquipment1Key IS NULL
    AND a.SourceEquipment1SKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimEquipment a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.EquipmentId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblEquipment (EquipmentKey, Id, SiteKey, Description, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.EquipmentKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.SourceEquipment1Key = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.SourceCompanyEquipmentID1
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.SourceEquipment1Key IS NULL
    AND a.SourceEquipment1SKey IS NULL



    -- Transaction Header FuelCard not supported in a separate dimension or mapping table.

    --Transaction Operator Personnel
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction records for which the Operator key reference is missing but the Transaction.OperatorId field is populated and does not correspond to an existing Operator record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.OperatorId, a.SiteKey, 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblPersonnel b
        WHERE b.PersonId = a.OperatorId
    )
    AND a.OperatorId IS NOT NULL
    AND LEN(a.OperatorId) > 0
    AND a.OperatorPersonnelKey IS NULL
    AND a.OperatorPersonnelSKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimPersonnel a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.PersonId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblPersonnel (PersonnelKey, PersonId, SiteKey, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.PersonnelKey, INSERTED.PersonId, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.OperatorPersonnelKey = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.OperatorId
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.OperatorPersonnelKey IS NULL
    AND a.OperatorPersonnelSKey IS NULL


   --Transaction TransactionAlias
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction records for which the Operator key reference is missing but the Transaction.AliasName field is populated and does not correspond to an existing TransactionAlias record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, FieldB, IsProcessed)
    SELECT NULL, a.TransactionAliasName, a.SiteKey, MAX(a.TransactionTypeIndex), 0
    FROM staging.tblTransactions a
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblTransactionAliases b
        WHERE b.AliasName = a.TransactionAliasName
    )
    AND a.TransactionAliasName IS NOT NULL
    AND LEN(a.TransactionAliasName) > 0
    AND a.TransactionAliasKey IS NULL
    AND a.TransactionAliasSKey IS NULL
    AND a.IgnoreRecord = 0
    GROUP BY a.TransactionAliasName, a.SiteKey

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimTransactionAlias a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.AliasName
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblTransactionAliases(TransactionAliasKey, AliasName, SiteKey, LookupTransTypeIndex, TransactionTypeSKey, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.TransactionAliasKey, INSERTED.AliasName, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        a.RecordId,        
        a.SiteKey,
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
	  WHERE a.IsProcessed <> 1

    UPDATE a
    SET a.TransactionAliasKey = b.RecordKey
    FROM staging.tblTransactions a
    INNER JOIN staging.tblMissingEntitiesTempTwo b
    ON b.RecordId = a.TransactionAliasName
    AND b.SiteKey = a.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.TransactionAliasKey IS NULL
    AND a.TransactionAliasSKey IS NULL




    -- The Transaction Types are fairly stable and are not expected to be deleted after transactions are created against them.

    -- TransactionLineItem
    -- TransactionLineItem AdditiveProfileKey not supported in a separate dimension or mapping table.

    -- TransactionLineItem Equipments
    -- TransactionLineItem DestinationCompartment Equipments
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction Line Item records for which the equipment key references is missing but the LineItem EquipmentId field is populated and does not correspond to an existing Equipment record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.DestinationCompanyEquipmentID, b.SiteKey, 0
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey    
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblEquipment c
        WHERE c.Id = a.DestinationCompanyEquipmentID
    )
    AND a.DestinationCompanyEquipmentID IS NOT NULL
    AND LEN(a.DestinationCompanyEquipmentID) > 0
    AND a.DestinationEquipmentKey IS NULL
    AND a.DestinationEquipmentSKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimEquipment a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.EquipmentId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblEquipment (EquipmentKey, Id, SiteKey, Description, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.EquipmentKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.DestinationEquipmentKey = c.RecordKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo c
    ON c.RecordId = a.DestinationCompanyEquipmentID
    AND c.SiteKey = b.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.DestinationEquipmentKey IS NULL
    AND a.DestinationEquipmentSKey IS NULL




    -- TransactionLineItem SourceEquipment Equipments
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction Line Item records for which the equipment key references is missing but the LineItem EquipmentId field is populated and does not correspond to an existing Equipment record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.SourceCompanyEquipmentID, b.SiteKey, 0
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey    
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblEquipment c
        WHERE c.Id = a.SourceCompanyEquipmentID
    )
    AND a.SourceCompanyEquipmentID IS NOT NULL
    AND LEN(a.SourceCompanyEquipmentID) > 0
    AND a.SourceEquipmentKey IS NULL
    AND a.SourceEquipmentSKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimEquipment a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.EquipmentId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblEquipment (EquipmentKey, Id, SiteKey, Description, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.EquipmentKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.SourceEquipmentKey = c.RecordKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo c
    ON c.RecordId = a.SourceCompanyEquipmentID
    AND c.SiteKey = b.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.SourceEquipmentKey IS NULL
    AND a.SourceEquipmentSKey IS NULL



    -- TransactionLineItem Product
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction Line Item records for which the product key reference is missing but the LineItem.Product (ProductId) field is populated and does not correspond to an existing Product record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.ProductId, b.SiteKey, 0
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey    
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblProducts c
        WHERE c.ProductId = a.ProductId
    )
    AND a.ProductId IS NOT NULL
    AND LEN(a.ProductId) > 0
    AND a.ProductKey IS NULL
    AND a.ProductSKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimProduct a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.ProductId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblProducts (ProductKey, ProductId, SiteKey, Description, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.ProductKey, INSERTED.ProductId, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.ProductKey = c.RecordKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo c
    ON c.RecordId = a.ProductId
    AND c.SiteKey = b.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.ProductKey IS NULL
    AND a.ProductSKey IS NULL



    -- TransactionSubLineItem Product
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction SubLine Item records for which the product key reference is missing but the LineItem.Product (ProductId) field is populated and does not correspond to an existing Product record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.ProductId, b.SiteKey, 0
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey    
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblProducts c
        WHERE c.ProductId = a.ProductId
    )
    AND a.ProductId IS NOT NULL
    AND LEN(a.ProductId) > 0
    AND a.ProductKey IS NULL
    AND a.ProductSKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimProduct a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.ProductId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblProducts (ProductKey, ProductId, SiteKey, Description, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted, StartDate, EndDate)
    OUTPUT INSERTED.ProductKey, INSERTED.ProductId, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1,
        @dummyStartDate,
        GETDATE()
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.ProductKey = c.RecordKey
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo c
    ON c.RecordId = a.ProductId
    AND c.SiteKey = b.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.ProductKey IS NULL
    AND a.ProductSKey IS NULL



    -- TransactionLineItem Station
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction LineItem records for which the LoacationLoadingStation key reference is missing but the LoadingLocationId field is populated and does not correspond to an existing Station record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.LoadingLocationId, b.SiteKey, 0
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblStations c
        WHERE c.ID = a.LoadingLocationId
    )
    AND a.LoadingLocationId IS NOT NULL
    AND LEN(TRIM(a.LoadingLocationId)) > 0
    AND a.LoadingLocationStationKey IS NULL
    AND a.LoadingLocationStationSKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM dbo.DimStation a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.StationId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblStations (StationKey, Id, SiteKey, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted)
    OUTPUT INSERTED.StationKey, INSERTED.Id, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        1,
        GETDATE(),
        1
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.LoadingLocationStationKey = c.RecordKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo c
    ON c.RecordId = a.LoadingLocationID
    AND c.SiteKey = b.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.LoadingLocationStationKey IS NULL
    AND a.LoadingLocationStationSKey IS NULL


    -- TransactionSubLineItems do not have a Station field. They inherit the Station of the parent LineItem.



    -- TransactionLineItem LoadArm - This section assumes that the above Transaction processing for Stations has been completed, i.e. the StationKey on the Transaction have been set where possible (StationKey will still be null for the case where both the StationKey and the StationId were missing)
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction LineItem records for which the LoadArmkey reference is missing but the LoadArmNumber field is populated and does not correspond to an existing LoadArm record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, FieldA, IsProcessed)
    SELECT DISTINCT a.LoadArmKey, a.LoadArmNumber, b.SiteKey, a.LoadingLocationStationKey, 0
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblLoadArms c
        WHERE 
        (
            ((c.BayAStationKey = a.LoadingLocationStationKey) AND (c.BayAArmNumber = a.LoadArmNumber))
            OR
            ((c.BayBStationKey = a.LoadingLocationStationKey) AND (c.BayBArmNumber = a.LoadArmNumber))
        )
    )
    AND a.LoadArmNumber IS NOT NULL
    AND a.LoadArmNumber > 0
    AND a.LoadArmSKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, FieldA, IsProcessed)
	SELECT DISTINCT a.AKey, d.RecordId, d.SiteKey, b.AKey, 0 FROM dbo.DimLoadArm a 
    INNER JOIN dbo.DimStation b
    ON b.SKey = a.StationSKey
	INNER JOIN staging.tblMissingEntitiesTempOne d
	ON d.RecordId = a.ArmNumber
    WHERE 
    (
        (d.FieldA IS NULL AND b.SKey = 0)  -- Default dummy Station
        OR
        (d.FieldA IS NOT NULL AND d.FieldA = b.AKey)
    )

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblLoadArms(LoadArmKey, BayAArmNumber, BayAStationKey, LoadRackText, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted)
    OUTPUT INSERTED.LoadArmKey, INSERTED.BayAArmNumber, INSERTED.BayAStationKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        CONVERT(int, RecordId),
        FieldA,
        @shortDummyId,
        1,
        GETDATE(),
        1
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.LoadArmKey = c.RecordKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo c
    ON c.RecordId = a.LoadArmNumber
    AND c.SiteKey = b.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.LoadArmKey IS NULL
    AND a.LoadArmSKey IS NULL
    AND a.LoadingLocationStationKey IS NULL
    AND a.LoadingLocationID IS NULL
    AND c.FieldA IS NULL

    UPDATE a
    SET a.LoadArmKey = c.RecordKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo c
    ON c.RecordId = a.LoadArmNumber
    AND c.SiteKey = b.SiteKey
    AND c.FieldA = a.LoadingLocationStationKey
    WHERE a.IgnoreRecord = 0
    AND a.LoadArmKey IS NULL
    AND a.LoadArmSKey IS NULL



    -- TransactionSubLineItem LoadArm - This section assumes that the above Transaction processing for Stations has been completed, i.e. the StationKey on the Transaction have been set where possible (StationKey will still be null for the case where both the StationKey and the StationId were missing)
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction SubLineItem records for which the LoadArmkey reference is missing but the LoadArmNumber field is populated and does not correspond to an existing LoadArm record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, FieldA, IsProcessed)
    SELECT DISTINCT a.LoadArmKey, a.LoadArmNumber, c.SiteKey, b.LoadingLocationStationKey, 0
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactionLineItems b
    ON b.TransactionLineItemKey = a.TransactionLineItemKey
    INNER JOIN staging.tblTransactions c
    ON c.TransactionKey = a.TransactionKey
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblLoadArms d
        WHERE 
        (
            ((d.BayAStationKey = b.LoadingLocationStationKey) AND (d.BayAArmNumber = a.LoadArmNumber))
            OR
            ((d.BayBStationKey = b.LoadingLocationStationKey) AND (d.BayBArmNumber = a.LoadArmNumber))
        )
    )
    AND a.LoadArmNumber IS NOT NULL
    AND a.LoadArmNumber > 0
    AND a.LoadArmSKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, FieldA, IsProcessed)
	SELECT DISTINCT a.AKey, d.RecordId, d.SiteKey, b.AKey, 0 FROM dbo.DimLoadArm a 
    INNER JOIN dbo.DimStation b
    ON b.SKey = a.StationSKey
	INNER JOIN staging.tblMissingEntitiesTempOne d
	ON d.RecordId = a.ArmNumber
    WHERE 
    (
        (d.FieldA IS NULL AND b.SKey = 0)  -- Default dummy Station
        OR
        (d.FieldA IS NOT NULL AND d.FieldA = b.AKey)
    )

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblLoadArms(LoadArmKey, BayAArmNumber, BayAStationKey, LoadRackText, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted)
    OUTPUT INSERTED.LoadArmKey, INSERTED.BayAArmNumber, INSERTED.BayAStationKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        CONVERT(int, RecordId),
        FieldA,
        @shortDummyId,
        1,
        GETDATE(),
        1
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.LoadArmKey = d.RecordKey
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactionLineItems b
    ON b.TransactionLineItemKey = a.TransactionLineItemKey
    INNER JOIN staging.tblTransactions c
    ON c.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo d
    ON d.RecordId = a.LoadArmNumber
    AND d.SiteKey = c.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.LoadArmKey IS NULL
    AND a.LoadArmSKey IS NULL
    AND b.LoadingLocationStationKey IS NULL
    AND b.LoadingLocationID IS NULL
    AND d.FieldA IS NULL

    UPDATE a
    SET a.LoadArmKey = d.RecordKey
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactionLineItems b
    ON b.TransactionLineItemKey = a.TransactionLineItemKey
    INNER JOIN staging.tblTransactions c
    ON c.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo d
    ON d.RecordId = a.LoadArmNumber
    AND d.SiteKey = c.SiteKey
    AND d.FieldA = b.LoadingLocationStationKey
    WHERE a.IgnoreRecord = 0
    AND a.LoadArmKey IS NULL
    AND a.LoadArmSKey IS NULL
    


    -- TransactionLineItem Storage Tank
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction Line Item records for which the Storage Tank key reference is missing but the LineItem.StorageLocationId field is populated and does not correspond to an existing Tank record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.StorageLocationID, b.SiteKey, 0
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey    
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblTanks c
        WHERE c.TankID = a.StorageLocationID
    )
    AND a.StorageLocationID IS NOT NULL
    AND LEN(a.StorageLocationID) > 0
    AND a.StorageLocationTankKey IS NULL
    AND a.StorageLocationTankSKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimTank a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.TankId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblTanks (TankKey, TankId, SiteKey, VesselTypeName, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted)
    OUTPUT INSERTED.ProductKey, INSERTED.TankID, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.StorageLocationTankKey = c.RecordKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo c
    ON c.RecordId = a.StorageLocationID
    AND c.SiteKey = b.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.StorageLocationTankKey IS NULL
    AND a.StorageLocationTankSKey IS NULL



    -- TransactionSubLineItem Tank
    TRUNCATE TABLE staging.tblMissingEntitiesTempOne
    TRUNCATE TABLE staging.tblMissingEntitiesTempTwo

    --Transaction SubLine Item records for which the Storage Tank key reference is missing but the SubLineItem.StorageLocationId field is populated and does not correspond to an existing Tank record.
    INSERT INTO staging.tblMissingEntitiesTempOne (RecordKey, RecordId, SiteKey, IsProcessed)
    SELECT DISTINCT NULL, a.StorageLocationID, b.SiteKey, 0
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey    
    WHERE NOT EXISTS 
    (
        SELECT * FROM staging.tblTanks c
        WHERE c.TankID = a.StorageLocationID
    )
    AND a.StorageLocationID IS NOT NULL
    AND LEN(a.StorageLocationID) > 0
    AND a.StorageLocationTankKey IS NULL
    AND a.StorageLocationTankSKey IS NULL
    AND a.IgnoreRecord = 0

    -- Try to locate the referenced entity akey from the dimension table, if it exists
    INSERT INTO staging.tblMissingEntitiesTempTwo
	(RecordKey, RecordId, SiteKey, IsProcessed)
	SELECT DISTINCT a.AKey, c.RecordId, b.AKey, 0 FROM DimTank a
    INNER JOIN DimSite b
    ON b.SKey = a.SiteSKey
	INNER JOIN staging.tblMissingEntitiesTempOne c
	ON c.RecordId = a.TankId
    AND c.SiteKey = b.AKey

	UPDATE a
	SET a.IsProcessed = 1
	FROM staging.tblMissingEntitiesTempOne a
	WHERE EXISTS 
	(
		SELECT * FROM staging.tblMissingEntitiesTempTwo b
		WHERE b.RecordId = a.RecordId
        AND b.SiteKey = a.SiteKey
	)

	-- For those referenced entities that could not be located from the dimension table, create new dummy records
    INSERT INTO staging.tblTanks (TankKey, TankID, SiteKey, VesselTypeName, IsRecordAddedByETL, RecordUpdatedDate, IsRecordDeleted)
    OUTPUT INSERTED.ProductKey, INSERTED.TankID, INSERTED.SiteKey, NULL, NULL, 0 INTO staging.tblMissingEntitiesTempTwo
      SELECT
        NEWID(),
        RecordId,
        SiteKey,
        @dummyEntityLabel,
        1,
        GETDATE(),
        1
      FROM staging.tblMissingEntitiesTempOne
	  WHERE IsProcessed <> 1

    UPDATE a
    SET a.StorageLocationTankKey = c.RecordKey
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN staging.tblTransactions b
    ON b.TransactionKey = a.TransactionKey
    INNER JOIN staging.tblMissingEntitiesTempTwo c
    ON c.RecordId = a.StorageLocationID
    AND c.SiteKey = b.SiteKey
    WHERE a.IgnoreRecord = 0
    AND a.StorageLocationTankKey IS NULL
    AND a.StorageLocationTankSKey IS NULL


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
    + 'Procedure Name: [staging].[usp_AddMissingTransactionEntitiesById]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END