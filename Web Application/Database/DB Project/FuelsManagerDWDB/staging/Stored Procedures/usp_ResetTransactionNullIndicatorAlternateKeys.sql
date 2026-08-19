/*
    DROP PROCEDURE [staging].[usp_ResetTransactionNullIndicatorAlternateKeys]

	EXEC [staging].[usp_ResetTransactionNullIndicatorAlternateKeys]
	
*/
CREATE PROCEDURE [staging].[usp_ResetTransactionNullIndicatorAlternateKeys]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_ResetTransactionNullIndicatorAlternateKeys]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: When a foreign key is enforced, the key can either be null or needs to point to a valid record in the foreign table. 
  --          In Pre-Cirrus FuelsManager version, where foreign keys are not enforced, the reference is also at times set to zero, -1
  --			or an empty string to denote a non-set reference. This procedure converts those invalid references in the Transaction tables 
  --			to null when there is no valid record with a zero-key or a -1-key in the foreign tables.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @hasValidZeroSiteguid bit = 0
    DECLARE @hasValidZeroProductguid bit = 0
    DECLARE @hasValidZeroCompanyguid bit = 0
    DECLARE @hasValidZeroEquipmentguid bit = 0
    DECLARE @hasValidZeroPersonnelguid bit = 0
    DECLARE @hasValidZeroTransactionAliasguid bit = 0
    DECLARE @hasValidZeroEquipmentTypeGuid bit = 0
    DECLARE @hasValidZeroUserguid bit = 0
    DECLARE @hasValidZeroUserToUserGroupGuid bit = 0
    DECLARE @hasValidZeroFuelCardGuid bit = 0
    DECLARE @hasValidZeroTransactionTypeIndex bit = 0

    DECLARE @hasValidMinusOneSiteguid bit = 0
    DECLARE @hasValidMinusOneProductguid bit = 0
    DECLARE @hasValidMinusOneCompanyguid bit = 0
    DECLARE @hasValidMinusOneEquipmentguid bit = 0
    DECLARE @hasValidMinusOnePersonnelguid bit = 0
    DECLARE @hasValidMinusOneTransactionAliasguid bit = 0
    DECLARE @hasValidMinusOneEquipmentTypeGuid bit = 0
    DECLARE @hasValidMinusOneUserguid bit = 0
    DECLARE @hasValidMinusOneUserToUserGroupGuid bit = 0
    DECLARE @hasValidMinusOneFuelCardGuid bit = 0
    DECLARE @hasValidMinusOneTransactionTypeIndex bit = 0

    IF EXISTS 
    (
        SELECT * FROM staging.tblSites
        WHERE Siteguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimSite
        WHERE AKey = '0'
    )
    BEGIN
      SET @hasValidZeroSiteguid = 1
    END
    
    IF EXISTS 
    (
        SELECT * FROM staging.tblSites
        WHERE Siteguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimSite
        WHERE AKey = '-1'
    )
    BEGIN
      SET @hasValidMinusOneSiteguid = 1
    END



    IF EXISTS 
    (
        SELECT * FROM staging.tblProducts
        WHERE Productguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimProduct
        WHERE AKey = '0'
    )
    BEGIN
      SET @hasValidZeroProductguid = 1
    END

    IF EXISTS 
    (
        SELECT * FROM staging.tblProducts
        WHERE Productguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimProduct
        WHERE AKey = '-1'
    )
    BEGIN
      SET @hasValidMinusOneProductguid = 1
    END


    IF EXISTS 
    (
        SELECT * FROM staging.tblCompanies
        WHERE Companyguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimCompany
        WHERE AKey = '0'
    )
    BEGIN
      SET @hasValidZeroCompanyguid = 1
    END

    IF EXISTS 
    (
        SELECT * FROM staging.tblCompanies
        WHERE Companyguid IS NULL        
    )
    OR EXISTS 
    (
        SELECT * FROM DimCompany
        WHERE AKey = '-1'
    )
    BEGIN
      SET @hasValidMinusOneCompanyguid = 1
    END


    IF EXISTS 
    (
        SELECT * FROM staging.tblEquipment
        WHERE Equipmentguid IS NULL     
    )
    OR EXISTS 
    (
        SELECT * FROM DimEquipment
        WHERE AKey = '0'
    )
    BEGIN
      SET @hasValidZeroEquipmentguid = 1
    END

    IF EXISTS 
    (
        SELECT * FROM staging.tblEquipment
        WHERE Equipmentguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimEquipment
        WHERE AKey = '-1'
    )
    BEGIN
      SET @hasValidMinusOneEquipmentguid = 1
    END


    IF EXISTS 
    (
        SELECT * FROM staging.tblPersonnel
        WHERE Personnelguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimPersonnel
        WHERE AKey = '0'
    )
    BEGIN
      SET @hasValidZeroPersonnelguid = 1
    END

    IF EXISTS 
    (
        SELECT * FROM staging.tblPersonnel
        WHERE Personnelguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimPersonnel
        WHERE AKey = '-1'
    )
    BEGIN
      SET @hasValidMinusOnePersonnelguid = 1
    END


    IF EXISTS 
    (
        SELECT * FROM staging.tblTransactionAliases
        WHERE TransactionAliasguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimTransactionAlias
        WHERE AKey = '0'
    )
    BEGIN
      SET @hasValidZeroTransactionAliasguid = 1
    END

    IF EXISTS 
    (
        SELECT * FROM staging.tblTransactionAliases
        WHERE TransactionAliasguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimTransactionAlias
        WHERE AKey = '-1'
    )
    BEGIN
      SET @hasValidMinusOneTransactionAliasguid = 1
    END


    IF EXISTS 
    (
        SELECT * FROM staging.tblEquipmentTypes
        WHERE EquipmentTypeGuid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimEquipmentType
        WHERE AKey = '0'
    )
    BEGIN
      SET @hasValidZeroEquipmentTypeGuid = 1
    END

    IF EXISTS 
    (
        SELECT * FROM staging.tblEquipmentTypes
        WHERE EquipmentTypeGuid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimEquipmentType
        WHERE AKey = '-1'      
    )
    BEGIN
      SET @hasValidMinusOneEquipmentTypeGuid = 1
    END


    IF EXISTS 
    (
        SELECT * FROM staging.tblUsers
        WHERE Userguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimFMUser
        WHERE AKey = '0'
    )
    BEGIN
      SET @hasValidZeroUserguid = 1
    END

    IF EXISTS 
    (
        SELECT * FROM staging.tblUsers
        WHERE Userguid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimFMUser
        WHERE AKey = '-1'
    )
    BEGIN
      SET @hasValidMinusOneUserguid = 1
    END

    IF EXISTS 
    (
        SELECT * FROM staging.tblUserToUserGroup
        WHERE UserToUserGroupGuid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM map.tblUserToUserGroup
        WHERE UserGroupKey = '0'
    )
    BEGIN
      SET @hasValidZeroUserToUserGroupGuid = 1
    END

    IF EXISTS 
    (
        SELECT * FROM staging.tblUserToUserGroup
        WHERE UserToUserGroupGuid IS NULL    
    )
    OR EXISTS 
    (
        SELECT * FROM map.tblUserToUserGroup
        WHERE UserGroupKey = '-1'
    )
    BEGIN
      SET @hasValidMinusOneUserToUserGroupGuid = 1
    END


    /*
    IF EXISTS 
    (
        SELECT * FROM staging.tblFuelCards
        WHERE FuelCardGuid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimFuelCard
        WHERE AKey = '0'
    )
    BEGIN
      SET @hasValidZeroFuelCardGuid = 1
    END

    IF EXISTS 
    (
        SELECT * FROM staging.tblFuelCards
        WHERE FuelCardGuid IS NULL
    )
    OR EXISTS 
    (
        SELECT * FROM DimFuelCard
        WHERE AKey = '-1'
    )
    BEGIN
      SET @hasValidMinusOneFuelCardGuid = 1
    END
    */



    IF EXISTS 
    (
        SELECT * FROM DimTransactionType
        WHERE AKey = '0'
    )
    BEGIN
      SET @hasValidZeroTransactionTypeIndex = 1
    END

    IF EXISTS 
    (
        SELECT * FROM DimTransactionType
        WHERE AKey = '-1'
    )
    BEGIN
      SET @hasValidMinusOneTransactionTypeIndex = 1
    END


    -- Transaction Header
    UPDATE staging.tblTransactions
    SET ConjoinTransID = NULL
    WHERE LEN(ConjoinTransId) = 0

    IF (@hasValidZeroSiteguid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET SiteKey = NULL
      WHERE SiteKey = '0'
      AND IgnoreRecord = 0
    END
    IF (@hasValidMinusOneSiteguid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET SiteKey = NULL
      WHERE SiteKey = '-1'
      AND IgnoreRecord = 0
    END
    IF (@hasValidZeroCompanyguid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET ShipToCompanyKey = NULL
      WHERE ShipToCompanyKey = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET SupplierCompanyKey = NULL
      WHERE SupplierCompanyKey = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET ShipperCompanyKey = NULL
      WHERE ShipperCompanyKey = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET OwnerCompanyKey = NULL
      WHERE OwnerCompanyKey = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET ManagerCompanyKey = NULL
      WHERE ManagerCompanyKey = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET CarrierCompanyKey = NULL
      WHERE CarrierCompanyKey = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET BillToCompanyKey = NULL
      WHERE BillToCompanyKey = '0'
      AND IgnoreRecord = 0
    END
    IF (@hasValidMinusOneCompanyguid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET ShipToCompanyKey = NULL
      WHERE ShipToCompanyKey = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET SupplierCompanyKey = NULL
      WHERE SupplierCompanyKey = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET ShipperCompanyKey = NULL
      WHERE ShipperCompanyKey = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET OwnerCompanyKey = NULL
      WHERE OwnerCompanyKey = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET ManagerCompanyKey = NULL
      WHERE ManagerCompanyKey = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET CarrierCompanyKey = NULL
      WHERE CarrierCompanyKey = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET BillToCompanyKey = NULL
      WHERE BillToCompanyKey = '-1'
      AND IgnoreRecord = 0
    END

    -- StationKeys (FinalStationKey, PreviousStationKey, NextStationKey, OriginStationKey) not supported in a separate dimension or mapping table.

    IF (@hasValidZeroEquipmentguid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET DestinationEquipment1Key = NULL
      WHERE DestinationEquipment1Key = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET DestinationEquipment2Key = NULL
      WHERE DestinationEquipment2Key = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET DestinationEquipment3Key = NULL
      WHERE DestinationEquipment3Key = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET SourceEquipment1Key = NULL
      WHERE SourceEquipment1Key = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET SourceEquipment2Key = NULL
      WHERE SourceEquipment2Key = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET SourceEquipment3Key = NULL
      WHERE SourceEquipment3Key = '0'
      AND IgnoreRecord = 0
    END
    IF (@hasValidMinusOneEquipmentguid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET DestinationEquipment1Key = NULL
      WHERE DestinationEquipment1Key = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET DestinationEquipment2Key = NULL
      WHERE DestinationEquipment2Key = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET DestinationEquipment3Key = NULL
      WHERE DestinationEquipment3Key = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET SourceEquipment1Key = NULL
      WHERE SourceEquipment1Key = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET SourceEquipment2Key = NULL
      WHERE SourceEquipment2Key = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactions
      SET SourceEquipment3Key = NULL
      WHERE SourceEquipment3Key = '-1'
      AND IgnoreRecord = 0
    END

    IF (@hasValidZeroPersonnelguid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET OperatorPersonnelKey = NULL
      WHERE OperatorPersonnelKey = '0'
      AND IgnoreRecord = 0
    END
    IF (@hasValidMinusOnePersonnelguid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET OperatorPersonnelKey = NULL
      WHERE OperatorPersonnelKey = '-1'
      AND IgnoreRecord = 0
    END

    IF (@hasValidZeroFuelCardGuid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET FuelCardKey = NULL
      WHERE FuelCardKey = '0'
      AND IgnoreRecord = 0
    END
    IF (@hasValidMinusOneFuelCardGuid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET FuelCardKey = NULL
      WHERE FuelCardKey = '-1'
      AND IgnoreRecord = 0
    END

    IF (@hasValidZeroTransactionAliasguid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET TransactionAliasKey = NULL
      WHERE TransactionAliasKey = '0'
      AND IgnoreRecord = 0
    END
    IF (@hasValidMinusOneTransactionAliasguid = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET TransactionAliasKey = NULL
      WHERE TransactionAliasKey = '-1'
      AND IgnoreRecord = 0
    END

    IF (@hasValidZeroTransactionTypeIndex = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET TransactionTypeKey = NULL
      WHERE TransactionTypeKey = '0'
      AND IgnoreRecord = 0
    END
    IF (@hasValidMinusOneTransactionTypeIndex = 0)
    BEGIN
      UPDATE staging.tblTransactions
      SET TransactionTypeKey = NULL
      WHERE TransactionTypeKey = '-1'
      AND IgnoreRecord = 0
    END

    -- TransactionLineItem
    -- AdditiveProfileKey not supported in a separate dimension or mapping table.

    IF (@hasValidZeroEquipmentguid = 0)
    BEGIN
      UPDATE staging.tblTransactionLineItems
      SET DestinationCompartmentKey = NULL
      WHERE DestinationCompartmentKey = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactionLineItems
      SET DestinationEquipmentKey = NULL
      WHERE DestinationEquipmentKey = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactionLineItems
      SET SourceCompartmentKey = NULL
      WHERE SourceCompartmentKey = '0'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactionLineItems
      SET SourceEquipmentKey = NULL
      WHERE SourceEquipmentKey = '0'
      AND IgnoreRecord = 0
    END
    IF (@hasValidMinusOneEquipmentguid = 0)
    BEGIN
      UPDATE staging.tblTransactionLineItems
      SET DestinationCompartmentKey = NULL
      WHERE DestinationCompartmentKey = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactionLineItems
      SET DestinationEquipmentKey = NULL
      WHERE DestinationEquipmentKey = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactionLineItems
      SET SourceCompartmentKey = NULL
      WHERE SourceCompartmentKey = '-1'
      AND IgnoreRecord = 0

      UPDATE staging.tblTransactionLineItems
      SET SourceEquipmentKey = NULL
      WHERE SourceEquipmentKey = '-1'
      AND IgnoreRecord = 0
    END

    IF (@hasValidZeroProductguid = 0)
    BEGIN
      UPDATE staging.tblTransactionLineItems
      SET ProductKey = NULL
      WHERE ProductKey = '0'
      AND IgnoreRecord = 0
    END
    IF (@hasValidMinusOneProductguid = 0)
    BEGIN
      UPDATE staging.tblTransactionLineItems
      SET ProductKey = NULL
      WHERE ProductKey = '-1'
      AND IgnoreRecord = 0
    END
    -- LoadingLocationKey, StorageLocationKey not supported in a separate dimension or mapping table.


    -- TransactionSubLineItem

    -- StorageLocationKey not supported in a separate dimension or mapping table.

    IF (@hasValidZeroProductguid = 0)
    BEGIN
      UPDATE staging.tblTransactionSubLineItems
      SET ProductKey = NULL
      WHERE ProductKey = '0'
      AND IgnoreRecord = 0
    END
    IF (@hasValidMinusOneProductguid = 0)
    BEGIN
      UPDATE staging.tblTransactionSubLineItems
      SET ProductKey = NULL
      WHERE ProductKey = '-1'
      AND IgnoreRecord = 0
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
    + 'Procedure Name: [staging].[usp_ResetTransactionNullIndicatorAlternateKeys]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO

