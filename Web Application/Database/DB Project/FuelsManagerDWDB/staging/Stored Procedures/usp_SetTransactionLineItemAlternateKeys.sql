/*
    DROP PROCEDURE [staging].[usp_SetTransactionLineItemAlternateKeys]

	EXEC [staging].[usp_SetTransactionLineItemAlternateKeys]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionLineItemAlternateKeys]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetTransactionLineItemAlternateKeys]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Set the alternate keys (varchar(50)) on the staging.tblTransactionLineItems table.
  -- Notes:
  -- 1. This includes both the original identity key of the record itself, and any foreign key that the record maintains to the identity keys of 
  --    other tables, and that is pertinent/maintained in the OLAP database.
  -- 2. The IdentityKey reflects either the IdentityIndex(int) field as used by FuelsManager 8.0 SP4, or the IdentityGuid (uniqueidentifier) field,
  --    as used by FuelsManager Cirrus.
  -- 3. The alternate key field (e.g. SiteKey, ProductKey, etc.) effectively helps make it transparent for the rest of the OLAP system as to what version of the OLTP 
  --    FuelsManager is being used (FuelsManager 8.0 SP4 or FuelsManager Cirrus).
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- TransactionLineItem
    UPDATE staging.tblTransactionLineItems
    SET TransactionLineItemKey = TransactionLineItemGuid,
        TransactionKey = TransactionGuid,
        AdditiveProfileKey = AdditiveProfileGuid,
        DestinationCompartmentKey = DestinationCompartmentGuid,
        DestinationEquipmentKey = DestinationEquipmentGuid,
        LoadingLocationStationKey = LoadingLocationStationGuid,
        OperatorPersonnelKey = OperatorPersonnelGuid,
        OrderReferenceTransactionLineItemKey = OrderReferenceTransactionLineItemGuid,
        ProductKey = ProductGuid,
        SourceCompartmentKey = SourceCompartmentGuid,
        SourceEquipmentKey = SourceEquipmentGuid,
        StorageLocationTankKey = StorageLocationTankGuid
    WHERE IgnoreRecord = 0


    --The LineItem does not have a Guid reference to tblLoardArms. That reference has to be constructed from the StationGuid and ArmNumber information.
    UPDATE a
    SET a.LoadArmKey = b.LoadArmKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblLoadArms b
    ON b.BayAStationKey = a.LoadingLocationStationKey
    AND b.BayAArmNumber = a.LoadArmNumber
    WHERE a.IgnoreRecord = 0
    AND a.LoadingLocationStationKey IS NOT NULL
    AND a.LoadArmNumber IS NOT NULL
    AND a.LoadArmKey IS NULL

    UPDATE a
    SET a.LoadArmKey = b.LoadArmKey
    FROM staging.tblTransactionLineItems a
    INNER JOIN staging.tblLoadArms b
    ON b.BayBStationKey = a.LoadingLocationStationKey
    AND b.BayBArmNumber = a.LoadArmNumber
    WHERE a.IgnoreRecord = 0
    AND a.LoadingLocationStationKey IS NOT NULL
    AND a.LoadArmNumber IS NOT NULL
    AND a.LoadArmKey IS NULL


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
    + 'Procedure Name: [staging].[usp_SetTransactionLineItemAlternateKeys]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO