/*
  DROP PROCEDURE [Staging].[usp_SetTransactionDateTimeKeyReferences]
  EXEC [staging].[usp_SetTransactionDateTimeKeyReferences]
	
*/
CREATE PROCEDURE [staging].[usp_SetTransactionDateTimeKeyReferences]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetTransactionDateTimeKeyReferences]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Set all references to table dimDate and table dimTime.
  -- Notes:
  -- 1. In the case of records artificially added by the ETL, the Level 0 Date/Time references might have been set already and should not be reset.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    --TransactionHeader
    UPDATE a
    SET a.CreatedDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.CreatedDate)
    WHERE a.CreatedDateSKey IS NULL

    UPDATE a
    SET a.CreatedTimeSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimTime b
      ON b.ElapsedSeconds = DATEPART(HOUR, a.CreatedDate) * 60 * 60
      + DATEPART(MINUTE, a.CreatedDate) * 60
      + DATEPART(SECOND, a.CreatedDate)
    WHERE a.CreatedTimeSKey IS NULL

    UPDATE a
    SET a.DispatchedDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.DispatchedDateTime)
    WHERE a.DispatchedDateSKey IS NULL

    UPDATE a
    SET a.DispatchedTimeSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimTime b
      ON b.ElapsedSeconds = DATEPART(HOUR, a.DispatchedDateTime) * 60 * 60
      + DATEPART(MINUTE, a.DispatchedDateTime) * 60
      + DATEPART(SECOND, a.DispatchedDateTime)
    WHERE a.DispatchedTimeSKey IS NULL


    UPDATE a
    SET a.EffectiveDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.EffectiveDate)
    WHERE a.EffectiveDateSKey IS NULL

    UPDATE a
    SET a.ExpirationDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.ExpirationDate)
    WHERE a.ExpirationDateSKey IS NULL

    UPDATE a
    SET a.InventoryDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.InventoryDate)
    WHERE a.InventoryDateSKey IS NULL

    UPDATE a
    SET a.RequestedDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.RequestedDateTime)
    WHERE a.RequestedDateSKey IS NULL

    UPDATE a
    SET a.RequestedDeliveryDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.RequestedDeliveryDate)
    WHERE a.RequestedDeliveryDateSKey IS NULL

    UPDATE a
    SET a.RouteOriginationDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.RouteOriginationDate)
    WHERE a.RouteOriginationDateSKey IS NULL

    UPDATE a
    SET a.ScheduledDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.ScheduledDate)
    WHERE a.ScheduledDateSKey IS NULL


    UPDATE a
    SET a.TimeEndDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.TimeEnd)
    WHERE a.TimeEndDateSKey IS NULL

    UPDATE a
    SET a.TimeEndTimeSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimTime b
      ON b.ElapsedSeconds = DATEPART(HOUR, a.TimeEnd) * 60 * 60
      + DATEPART(MINUTE, a.TimeEnd) * 60
      + DATEPART(SECOND, a.TimeEnd)
    WHERE a.TimeEndTimeSKey IS NULL

    UPDATE a
    SET a.TimeInDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.TimeIn)
    WHERE a.TimeInDateSKey IS NULL

    UPDATE a
    SET a.TimeInTimeSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimTime b
      ON b.ElapsedSeconds = DATEPART(HOUR, a.TimeIn) * 60 * 60
      + DATEPART(MINUTE, a.TimeIn) * 60
      + DATEPART(SECOND, a.TimeIn)
    WHERE a.TimeInTimeSKey IS NULL

    UPDATE a
    SET a.TimeOutDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.TimeOut)
    WHERE a.TimeOutDateSKey IS NULL

    UPDATE a
    SET a.TimeOutTimeSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimTime b
      ON b.ElapsedSeconds = DATEPART(HOUR, a.TimeOut) * 60 * 60
      + DATEPART(MINUTE, a.TimeOut) * 60
      + DATEPART(SECOND, a.TimeOut)
    WHERE a.TimeOutTimeSKey IS NULL


    UPDATE a
    SET a.TransDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.TransDateTime)
    WHERE a.TransDateSKey IS NULL

    UPDATE a
    SET a.TransTimeSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimTime b
      ON b.ElapsedSeconds = DATEPART(HOUR, a.TransDateTime) * 60 * 60
      + DATEPART(MINUTE, a.TransDateTime) * 60
      + DATEPART(SECOND, a.TransDateTime)
    WHERE a.TransTimeSKey IS NULL


    UPDATE a
    SET a.CardExpirationDateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.CardExpiration)
    WHERE a.CardExpirationDateSKey IS NULL

    UPDATE a
    SET a.Date01DateSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimDate b
      ON b.FullDateAKey = CONVERT(date, a.Date01)
    WHERE a.Date01DateSKey IS NULL

    UPDATE a
    SET a.Date01TimeSKey = b.SKey
    FROM staging.tblTransactions a
    INNER JOIN DimTime b
      ON b.ElapsedSeconds = DATEPART(HOUR, a.Date01) * 60 * 60
      + DATEPART(MINUTE, a.Date01) * 60
      + DATEPART(SECOND, a.Date01)
    WHERE a.Date01TimeSKey IS NULL


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
    + 'Procedure Name: [staging].[usp_SetTransactionDateTimeKeyReferences]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END