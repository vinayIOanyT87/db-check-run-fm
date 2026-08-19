/*
  DROP PROCEDURE [staging].[usp_SetTransactionHeaderAlternateKeys]

	EXEC [staging].[usp_SetTransactionHeaderAlternateKeys]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionHeaderAlternateKeys]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetTransactionHeaderAlternateKeys]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Set the alternate keys (varchar(50)) on the staging.tblTransactions table.
  -- Notes:
  -- 1. This includes both the original identity key of the record itself, and any foreign key that the record maintains to the identity keys of 
  --    other tables, and that is pertinent/maintained in the OLAP database.
  -- 2. The IdentityKey reflects either the IdentityIndex(int) field as used by FuelsManager 8.0 SP4, or the IdentityGuid (uniqueidentifier) field,
  --    as used by FuelsManager Cirrus.
  -- 3. The alternate key field (e.g. SiteKey, ProductKey, etc.) effectively helps make it transparent for the rest of the OLAP system as to what version of the OLTP 
  --    FuelsManager is being used (FuelsManager 8.0 SP4 or FuelsManager Cirrus).
  -- 4. The Staging Transaction tables have been excluded from this process because of their large sizes. Setting the alternate keys for the staging Transactin tables is handled in separate Store Procedures.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- TransactionHeader
    UPDATE staging.tblTransactions
    SET TransactionKey = TransactionGuid,
        TransactionAliasKey = TransactionAliasGuid,
        SiteKey = SiteGuid,
        ShipToCompanyKey = ShipToCompanyGuid,
        SupplierCompanyKey = SupplierCompanyGuid,
        ShipperCompanyKey = ShipperCompanyGuid,
        OwnerCompanyKey = OwnerCompanyGuid,
        ManagerCompanyKey = ManagerCompanyGuid,
        CarrierCompanyKey = CarrierCompanyGuid,
        BillToCompanyKey = BillToCompanyGuid,
        FinalStationIATAKey = FinalStationIATAGuid,
        PreviousStationIATAKey = PreviousStationIATAGuid,
        NextStationIATAKey = NextStationIATAGuid,
        OriginStationIATAKey = OriginStationIATAGuid,
        DestinationEquipment1Key = DestinationEquipment1Guid,
        DestinationEquipment2Key = DestinationEquipment2Guid,
        DestinationEquipment3Key = DestinationEquipment3Guid,
        ReasonCodeKey = ReasonCodeGuid,
        SourceEquipment1Key = SourceEquipment1Guid,
        SourceEquipment2Key = SourceEquipment2Guid,
        SourceEquipment3Key = SourceEquipment3Guid,
        OperatorPersonnelKey = OperatorPersonnelGuid,
        FuelCardKey = FuelCardGuid,
        TransactionTypeKey = TransactionTypeIndex
    WHERE IgnoreRecord = 0


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
    + 'Procedure Name: [staging].[usp_SetTransactionHeaderAlternateKeys]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO
