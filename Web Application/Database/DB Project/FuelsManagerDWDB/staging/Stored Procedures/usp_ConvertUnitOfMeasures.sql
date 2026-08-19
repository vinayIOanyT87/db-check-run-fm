/*
  DROP PROCEDURE [staging].[usp_ConvertUnitOfMeasures]

	EXEC [staging].[usp_ConvertUnitOfMeasures]
	
*/
CREATE PROCEDURE [staging].[usp_ConvertUnitOfMeasures]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_ConvertUnitOfMeasures]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Convert the unit of measure field values into the client-specific unit of measures and load the results into the corresponding client-specific unit of measure fields.
  -- Notes:
  -- 1. 
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @volToCubicMetreConvFactorIndex int = 41
    DECLARE @volToLitreConvFactorIndex int = 42    
    DECLARE @volToUSGallonConvFactorIndex int = 46
    DECLARE @massToKgFactorIndex int = 61
    DECLARE @massToLbConvFactorIndex int = 64


    --TransactionLineItem
    UPDATE staging.tblTransactionLineItems
    SET AlternativeGrossVolumeUSGallon = [dbo].[udf_ConvertFromSIUnits](AlternativeGrossVolumeSI, @volToUSGallonConvFactorIndex, 10),
        AlternativeNetVolumeUSGallon = [dbo].[udf_ConvertFromSIUnits](AlternativeNetVolumeSI, @volToUSGallonConvFactorIndex, 10),
        BottomVolumeUSGallon = [dbo].[udf_ConvertFromSIUnits](BottomVolumeSI, @volToUSGallonConvFactorIndex, 10),
        CleanLineDeductQuantityUSGallon = [dbo].[udf_ConvertFromSIUnits](CleanLineDeductQuantitySI, @volToUSGallonConvFactorIndex, 10),
        CleanLinePackQuantityUSGallon = [dbo].[udf_ConvertFromSIUnits](CleanLinePackQuantitySI, @volToUSGallonConvFactorIndex, 10),
        GrossQuantityUSGallon = [dbo].[udf_ConvertFromSIUnits](GrossQuantitySI, @volToUSGallonConvFactorIndex, 10),
        LineFillUSGallon = [dbo].[udf_ConvertFromSIUnits](LineFillSI, @volToUSGallonConvFactorIndex, 10),
        MassQuantityLb = [dbo].[udf_ConvertFromSIUnits](MassQuantitySI, @massToLbConvFactorIndex, 10),
        NetCapacityUSGallon = [dbo].[udf_ConvertFromSIUnits](NetCapacitySI, @volToUSGallonConvFactorIndex, 10),        
        NetQuantityUSGallon = [dbo].[udf_ConvertFromSIUnits](NetQuantitySI, @volToUSGallonConvFactorIndex, 10)

    --TransactionUserData
    UPDATE a
    SET a.UserData4USGallon = TRY_CONVERT(float, a.UserData4),
    a.UserData5USGallon = TRY_CONVERT(float, a.UserData5),
    a.UserData6USGallon = TRY_CONVERT(float, a.UserData6)
    FROM staging.tblTransactionUserData a

    UPDATE a
    SET a.UserData4SI = [dbo].[udf_ConvertToSIUnits](UserData4USGallon, 46),
    a.UserData5SI = [dbo].[udf_ConvertToSIUnits](UserData5USGallon, 46),
    a.UserData6SI = [dbo].[udf_ConvertToSIUnits](UserData6USGallon, 46)
    FROM staging.tblTransactionUserData a


    --TransactionSubLineItem
    UPDATE staging.tblTransactionSubLineItems
    SET BottomVolumeUSGallon = [dbo].[udf_ConvertFromSIUnits](BottomVolumeSI, @volToUSGallonConvFactorIndex, 10),
        CleanLineDeductQuantityUSGallon = [dbo].[udf_ConvertFromSIUnits](CleanLineDeductQuantitySI, @volToUSGallonConvFactorIndex, 10),
        CleanLinePackQuantityUSGallon = [dbo].[udf_ConvertFromSIUnits](CleanLinePackQuantitySI, @volToUSGallonConvFactorIndex, 10),
        GrossQuantityUSGallon = [dbo].[udf_ConvertFromSIUnits](GrossQuantitySI, @volToUSGallonConvFactorIndex, 10),
        LineFillUSGallon = [dbo].[udf_ConvertFromSIUnits](LineFillSI, @volToUSGallonConvFactorIndex, 10),
        MassQuantityLb = [dbo].[udf_ConvertFromSIUnits](MassQuantitySI, @massToLbConvFactorIndex, 10),
        NetCapacityUSGallon = [dbo].[udf_ConvertFromSIUnits](NetCapacitySI, @volToUSGallonConvFactorIndex, 10),        
        NetQuantityUSGallon = [dbo].[udf_ConvertFromSIUnits](NetQuantitySI, @volToUSGallonConvFactorIndex, 10)
    

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
    + 'Procedure Name: [staging].[usp_ConvertUnitOfMeasures]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END