/*
  DROP PROCEDURE [staging].[usp_SetMissingTransactionEntityKeyReferencesById]

	EXEC [staging].[usp_SetMissingTransactionEntityKeyReferencesById]
	
*/
CREATE PROCEDURE [staging].[usp_SetMissingTransactionEntityKeyReferencesById] (@IgnoreDateMismatch bit)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetMissingTransactionEntityKeyReferencesById]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Attempt to find and set the missing/null Transactions entity key references when the entity reference id field is not null,
  -- for entity references not enforced by referential integrity.
--   E.g. A Transaction with a ShipToId of W91FR7 but with a ShipToKey of NULL.
  -- Notes:
  -- 1. The process of setting missing key references is limited to transactions only.
  -- 2. @IgnoreDateMismatch: 
  --		0 : For historical tables, strictly use the StartDate-EndDate range of entity records for identifying the foreign keys.
  --			Raise an exception if relationships cannot be resolved because of date range mismatch.
  --		1 : For historical tables, use the StartDate-EndDate range of entity records for identifying the foreign keys where possible. 
  --			For those relationships that cannot be resolved because of a data range mismatch, force the relationships by ignoring the 
  --			date range mismatch.
  -- 3. The operation assumes that the values of the ID fields have been pre-trimmed first, before comparison against the respective 
  --    entity tables, in order to account for entity ID field values that only differ by leading or trailing whitespaces.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @openEndedDate datetimeoffset(7)
    SELECT
      @openEndedDate = DATEADD(YEAR, 100, GETDATE())

    -- Transaction Header

    -- Transaction Header Companies
    UPDATE a
    SET a.BillToCompanySKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimCompany b
      ON b.CompanyId = a.BillToId
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.BillToCompanyKey IS NULL
    AND a.BillToID IS NOT NULL
    AND LEN(a.BillToID) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.BillToCompanySKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT CompanyId, MAX(SKey) SKey
        FROM dbo.DimCompany
        GROUP BY CompanyId
      ) b
      ON b.CompanyId = a.BillToId
      WHERE a.IgnoreRecord = 0
      AND a.BillToCompanyKey IS NULL
      AND a.BillToID IS NOT NULL
      AND LEN(a.BillToID) > 0
    END

    UPDATE a
    SET a.CarrierCompanySKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimCompany b
      ON b.CompanyId = a.CarrierID
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.CarrierCompanyKey IS NULL
    AND a.CarrierID IS NOT NULL
    AND LEN(a.CarrierID) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.CarrierCompanySKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT CompanyId, MAX(SKey) SKey
        FROM dbo.DimCompany
        GROUP BY CompanyId
      ) b
      ON b.CompanyId = a.CarrierId
      WHERE a.IgnoreRecord = 0
      AND a.CarrierCompanyKey IS NULL
      AND a.CarrierID IS NOT NULL
      AND LEN(a.CarrierID) > 0
    END


    UPDATE a
    SET a.ManagerCompanySKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimCompany b
      ON b.CompanyId = a.ManagerId
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.ManagerCompanyKey IS NULL
    AND a.ManagerID IS NOT NULL
    AND LEN(a.ManagerID) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.ManagerCompanySKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT CompanyId, MAX(SKey) SKey
        FROM dbo.DimCompany
        GROUP BY CompanyId
      ) b
      ON b.CompanyId = a.ManagerId
      WHERE a.IgnoreRecord = 0
      AND a.ManagerCompanyKey IS NULL
      AND a.ManagerID IS NOT NULL
      AND LEN(a.ManagerID) > 0
    END

    UPDATE a
    SET a.OwnerCompanySKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimCompany b
      ON b.CompanyId = a.OwnerId
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.OwnerCompanyKey IS NULL
    AND a.OwnerID IS NOT NULL
    AND LEN(a.OwnerID) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.OwnerCompanySKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT CompanyId, MAX(SKey) SKey
        FROM dbo.DimCompany
        GROUP BY CompanyId
      ) b
      ON b.CompanyId = a.OwnerId
      WHERE a.IgnoreRecord = 0
      AND a.OwnerCompanyKey IS NULL
      AND a.OwnerID IS NOT NULL
      AND LEN(a.OwnerID) > 0
    END

    UPDATE a
    SET a.ShipperCompanySKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimCompany b
      ON b.CompanyId = a.ShipperId
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.ShipperCompanyKey IS NULL
    AND a.ShipperID IS NOT NULL
    AND LEN(a.ShipperID) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.ShipperCompanySKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT CompanyId, MAX(SKey) SKey
        FROM dbo.DimCompany
        GROUP BY CompanyId
      ) b
      ON b.CompanyId = a.ShipperId
      WHERE a.IgnoreRecord = 0
      AND a.ShipperCompanyKey IS NULL
      AND a.ShipperID IS NOT NULL
      AND LEN(a.ShipperID) > 0
    END

    UPDATE a
    SET a.ShipToCompanySKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimCompany b
      ON b.CompanyId = a.ShipToId
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.ShipToCompanyKey IS NULL
    AND a.ShipToID IS NOT NULL
    AND LEN(a.ShipToID) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.ShipToCompanySKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT CompanyId, MAX(SKey) SKey
        FROM dbo.DimCompany
        GROUP BY CompanyId
      ) b
      ON b.CompanyId = a.ShipToId
      WHERE a.IgnoreRecord = 0
      AND a.ShipToCompanyKey IS NULL
      AND a.ShipToID IS NOT NULL
      AND LEN(a.ShipToID) > 0
    END

    UPDATE a
    SET a.SupplierCompanySKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimCompany b
      ON b.CompanyId = a.SupplierId
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.SupplierCompanyKey IS NULL
    AND a.SupplierID IS NOT NULL
    AND LEN(a.SupplierID) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.SupplierCompanySKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT CompanyId, MAX(SKey) SKey
        FROM dbo.DimCompany
        GROUP BY CompanyId
      ) b
      ON b.CompanyId = a.SupplierId
      WHERE a.IgnoreRecord = 0
      AND a.SupplierCompanyKey IS NULL
      AND a.SupplierID IS NOT NULL
      AND LEN(a.SupplierID) > 0
    END

    -- Transaction Header Equipments	
    UPDATE a
    SET a.DestinationEquipment1SKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimEquipment b
      ON b.EquipmentId = COALESCE(a.DestinationCompanyEquipmentID1, a.DestinationRegistrationID1)
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.DestinationEquipment1Key IS NULL
    AND (a.DestinationCompanyEquipmentID1 IS NOT NULL OR a.DestinationRegistrationID1 IS NOT NULL)

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.DestinationEquipment1SKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT EquipmentId, MAX(SKey) SKey
        FROM dbo.DimEquipment
        GROUP BY EquipmentId
      ) b
      ON b.EquipmentId = COALESCE(a.DestinationCompanyEquipmentID1, a.DestinationRegistrationID1)
      WHERE a.IgnoreRecord = 0
      AND a.DestinationEquipment1Key IS NULL
      AND (a.DestinationCompanyEquipmentID1 IS NOT NULL OR a.DestinationRegistrationID1 IS NOT NULL)
    END


    UPDATE a
    SET a.DestinationEquipment2SKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimEquipment b
      ON b.EquipmentId = COALESCE(a.DestinationCompanyEquipmentID2, a.DestinationRegistrationID2)
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.DestinationEquipment2Key IS NULL
    AND (a.DestinationCompanyEquipmentID2 IS NOT NULL OR a.DestinationRegistrationID2 IS NOT NULL)

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.DestinationEquipment2SKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT EquipmentId, MAX(SKey) SKey
        FROM dbo.DimEquipment
        GROUP BY EquipmentId
      ) b
      ON b.EquipmentId = COALESCE(a.DestinationCompanyEquipmentID2, a.DestinationRegistrationID2)
      WHERE a.IgnoreRecord = 0
      AND a.DestinationEquipment2Key IS NULL
      AND (a.DestinationCompanyEquipmentID2 IS NOT NULL OR a.DestinationRegistrationID2 IS NOT NULL)
    END


    UPDATE a
    SET a.DestinationEquipment3SKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimEquipment b
      ON b.EquipmentId = COALESCE(a.DestinationCompanyEquipmentID3, a.DestinationRegistrationID3)
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.DestinationEquipment3Key IS NULL
    AND (a.DestinationCompanyEquipmentID3 IS NOT NULL OR a.DestinationRegistrationID3 IS NOT NULL)

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.DestinationEquipment3SKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT EquipmentId, MAX(SKey) SKey
        FROM dbo.DimEquipment
        GROUP BY EquipmentId
      ) b
      ON b.EquipmentId = COALESCE(a.DestinationCompanyEquipmentID3, a.DestinationRegistrationID3)
      WHERE a.IgnoreRecord = 0
      AND a.DestinationEquipment3Key IS NULL
      AND (a.DestinationCompanyEquipmentID3 IS NOT NULL OR a.DestinationRegistrationID3 IS NOT NULL)
    END


    UPDATE a
    SET a.SourceEquipment1SKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimEquipment b
      ON b.EquipmentId = COALESCE(a.SourceCompanyEquipmentID1, a.SourceRegistrationID1)
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.SourceEquipment1Key IS NULL
    AND (a.SourceCompanyEquipmentID1 IS NOT NULL OR a.SourceRegistrationID1 IS NOT NULL)

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.SourceEquipment1SKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT EquipmentId, MAX(SKey) SKey
        FROM dbo.DimEquipment
        GROUP BY EquipmentId
      ) b
      ON b.EquipmentId = COALESCE(a.SourceCompanyEquipmentID1, a.SourceRegistrationID1)
      WHERE a.IgnoreRecord = 0
      AND a.SourceEquipment1Key IS NULL
      AND (a.SourceCompanyEquipmentID1 IS NOT NULL OR a.SourceRegistrationID1 IS NOT NULL)
    END


    UPDATE a
    SET a.SourceEquipment2SKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimEquipment b
      ON b.EquipmentId = COALESCE(a.SourceCompanyEquipmentID2, a.SourceRegistrationID2)
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.SourceEquipment2Key IS NULL
    AND (a.SourceCompanyEquipmentID2 IS NOT NULL OR a.SourceRegistrationID2 IS NOT NULL)

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.SourceEquipment2SKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT EquipmentId, MAX(SKey) SKey
        FROM dbo.DimEquipment
        GROUP BY EquipmentId        
      ) b
      ON b.EquipmentId = COALESCE(a.SourceCompanyEquipmentID2, a.SourceRegistrationID2)
      WHERE a.IgnoreRecord = 0
      AND a.SourceEquipment2Key IS NULL
      AND (a.SourceCompanyEquipmentID2 IS NOT NULL OR a.SourceRegistrationID2 IS NOT NULL)
    END


    UPDATE a
    SET a.SourceEquipment3SKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimEquipment b
      ON b.EquipmentId = COALESCE(a.SourceCompanyEquipmentID3, a.SourceRegistrationID3)
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.SourceEquipment3Key IS NULL
    AND (a.SourceCompanyEquipmentID3 IS NOT NULL OR a.SourceRegistrationID3 IS NOT NULL)

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.SourceEquipment3SKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT EquipmentId, MAX(SKey) SKey
        FROM dbo.DimEquipment
        GROUP BY EquipmentId
      ) b
      ON b.EquipmentId = COALESCE(a.SourceCompanyEquipmentID3, a.SourceRegistrationID3)
      WHERE a.IgnoreRecord = 0
      AND a.SourceEquipment3Key IS NULL
      AND (a.SourceCompanyEquipmentID3 IS NOT NULL OR a.SourceRegistrationID3 IS NOT NULL)
    END


    -- Transaction Header Operator
    UPDATE a
    SET a.OperatorPersonnelSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimPersonnel b
      ON b.PersonId = a.OperatorID
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.OperatorPersonnelKey IS NULL
    AND a.OperatorID IS NOT NULL
    AND LEN(a.OperatorID) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.OperatorPersonnelSKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT PersonId, MAX(SKey) SKey
        FROM dbo.DimPersonnel
        GROUP BY PersonId
      ) b
      ON b.PersonID = a.OperatorID
      WHERE a.IgnoreRecord = 0
      AND a.OperatorPersonnelKey IS NULL
      AND a.OperatorID IS NOT NULL
      AND LEN(a.OperatorID) > 0
    END


    -- Transaction Header TransactionAlias
    UPDATE a
    SET a.TransactionAliasSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN dbo.DimTransactionAlias b
      ON b.AliasName = a.TransactionAliasName
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.TransactionAliasKey IS NULL
    AND a.TransactionAliasName IS NOT NULL
    AND LEN(a.TransactionAliasName) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.TransactionAliasSKey = b.SKey
      FROM staging.tblTransactions a
      INNER JOIN 
      (
        SELECT AliasName, MAX(SKey) SKey
        FROM dbo.DimTransactionAlias
        GROUP BY AliasName
      ) b
      ON b.AliasName = a.TransactionAliasName
      WHERE a.IgnoreRecord = 0
      AND a.TransactionAliasKey IS NULL
      AND a.TransactionAliasName IS NOT NULL
      AND LEN(a.TransactionAliasName) > 0
    END




    -- TransactionLineItem references			
    UPDATE a
    SET a.DestinationCompartmentSKey = b.SKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN dbo.DimEquipment b
      ON b.EquipmentId = a.DestinationCompartmentId
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.DestinationCompartmentKey IS NULL
    AND a.DestinationCompartmentID IS NOT NULL
    AND LEN(a.DestinationCompartmentID) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.DestinationCompartmentSKey = b.SKey
      FROM staging.tblTransactionLineItems a
      INNER JOIN 
      (
        SELECT EquipmentId, MAX(SKey) SKey
        FROM dbo.DimEquipment
        GROUP BY EquipmentId
      ) b
      ON b.EquipmentId = a.DestinationCompartmentId
      WHERE a.IgnoreRecord = 0
      AND a.DestinationCompartmentKey IS NULL
      AND a.DestinationCompartmentID IS NOT NULL
      AND LEN(a.DestinationCompartmentID) > 0
    END


    UPDATE a
    SET a.DestinationEquipmentSKey = b.SKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN dbo.DimEquipment b
      ON b.EquipmentId = COALESCE(a.DestinationCompanyEquipmentID, a.DestinationRegistrationID)
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.DestinationEquipmentKey IS NULL
    AND (a.DestinationCompanyEquipmentID IS NOT NULL OR a.DestinationRegistrationID IS NOT NULL)

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.DestinationEquipmentSKey = b.SKey
      FROM staging.tblTransactionLineItems a
      INNER JOIN 
      (
        SELECT EquipmentId, MAX(SKey) SKey
        FROM dbo.DimEquipment
        GROUP BY EquipmentId
      ) b
      ON b.EquipmentId = COALESCE(a.DestinationCompanyEquipmentID, a.DestinationRegistrationID)
      WHERE a.IgnoreRecord = 0
      AND a.DestinationEquipmentKey IS NULL
      AND (a.DestinationCompanyEquipmentID IS NOT NULL OR a.DestinationRegistrationID IS NOT NULL)
    END


    UPDATE a
    SET a.ProductSKey = b.SKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN dbo.DimProduct b
      ON b.ProductId = a.ProductId
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.ProductKey IS NULL
    AND a.ProductId IS NOT NULL
    AND LEN(a.ProductId) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.ProductSKey = b.SKey
      FROM staging.tblTransactionLineItems a
      INNER JOIN 
      (
        SELECT ProductId, MAX(SKey) SKey
        FROM dbo.DimProduct
        GROUP BY ProductId
      ) b
      ON b.ProductId = a.ProductId
      WHERE a.IgnoreRecord = 0
      AND a.ProductKey IS NULL
      AND a.ProductId IS NOT NULL
      AND LEN(a.ProductId) > 0
    END


    UPDATE a
    SET a.SourceCompartmentSKey = b.SKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN dbo.DimEquipment b
      ON b.EquipmentId = a.SourceCompartmentId
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.SourceCompartmentKey IS NULL
    AND a.SourceCompartmentID IS NOT NULL
    AND LEN(a.SourceCompartmentID) > 0


    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.SourceCompartmentSKey = b.SKey
      FROM staging.tblTransactionLineItems a
      INNER JOIN 
      (
        SELECT EquipmentId, MAX(SKey) SKey
        FROM dbo.DimEquipment
        GROUP BY EquipmentId
      ) b
      ON b.EquipmentId = a.SourceCompartmentId
      WHERE a.IgnoreRecord = 0
      AND a.SourceCompartmentKey IS NULL
      AND a.SourceCompartmentID IS NOT NULL
      AND LEN(a.SourceCompartmentID) > 0
    END


    UPDATE a
    SET a.SourceEquipmentSKey = b.SKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN dbo.DimEquipment b
      ON b.EquipmentId = COALESCE(a.SourceCompanyEquipmentID, a.SourceRegistrationID)
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.SourceEquipmentKey IS NULL
    AND (a.SourceCompanyEquipmentID IS NOT NULL OR a.SourceRegistrationID IS NOT NULL)

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.SourceEquipmentSKey = b.SKey
      FROM staging.tblTransactionLineItems a
      INNER JOIN 
      (
        SELECT EquipmentId, MAX(SKey) SKey
        FROM dbo.DimEquipment
        GROUP BY EquipmentId
      ) b
      ON b.EquipmentId = COALESCE(a.SourceCompanyEquipmentID, a.SourceRegistrationID)
      WHERE a.IgnoreRecord = 0
      AND a.SourceEquipmentKey IS NULL
      AND (a.SourceCompanyEquipmentID IS NOT NULL OR a.SourceRegistrationID IS NOT NULL)
    END


    UPDATE a
    SET a.OperatorPersonnelSKey = b.SKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN dbo.DimPersonnel b
      ON b.PersonId = a.OperatorID
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.OperatorPersonnelKey IS NULL
    AND a.OperatorID IS NOT NULL
    AND LEN(a.OperatorID) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.OperatorPersonnelSKey = b.SKey
      FROM staging.tblTransactionLineItems a
      INNER JOIN 
      (
        SELECT PersonId, MAX(SKey) SKey
        FROM dbo.DimPersonnel
        GROUP BY PersonId
      ) b
      ON b.PersonID = a.OperatorID
      WHERE a.IgnoreRecord = 0
      AND a.OperatorPersonnelKey IS NULL
      AND a.OperatorID IS NOT NULL
      AND LEN(a.OperatorID) > 0
    END


    --TransactionSubLineItem references
    UPDATE a
    SET a.ProductSKey = b.SKey
    FROM staging.tblTransactionSubLineItems a
    INNER JOIN dbo.DimProduct b
      ON b.ProductId = a.ProductId
    WHERE a.IgnoreRecord = 0
    AND a.CombinedUpdatedDate BETWEEN b.StartDate AND ISNULL(b.EndDate, @openEndedDate)
    AND a.ProductKey IS NULL
    AND a.ProductId IS NOT NULL
    AND LEN(a.ProductId) > 0

    IF (@IgnoreDateMismatch = 1)
    BEGIN
      UPDATE a
      SET a.ProductSKey = b.SKey
      FROM staging.tblTransactionSubLineItems a
      INNER JOIN 
      (
        SELECT ProductId, MAX(SKey) SKey
        FROM dbo.DimProduct
        GROUP BY ProductId
      ) b
      ON b.ProductId = a.ProductId
      WHERE a.IgnoreRecord = 0
      AND a.ProductKey IS NULL
      AND a.ProductId IS NOT NULL
      AND LEN(a.ProductId) > 0
    END



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
    + 'Procedure Name: [staging].[usp_SetMissingTransactionEntityKeyReferencesById]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END