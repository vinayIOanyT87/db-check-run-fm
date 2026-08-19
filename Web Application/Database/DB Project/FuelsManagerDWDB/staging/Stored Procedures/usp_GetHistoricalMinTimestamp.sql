/*
	DROP PROCEDURE [Staging].[usp_GetHistoricalMinTimestamp]

	EXEC [staging].[usp_GetHistoricalMinTimestamp]
	
*/
CREATE PROCEDURE [staging].[usp_GetHistoricalMinTimestamp]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_GetHistoricalMinTimestamp]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Determine the minimum timestamp for the Historical entities and capture the value in the 
  --          staging.tblETLTempVariables table.
  -- Notes:
  -- 1. The purpose of this method is to help determine the minimum timestamp to use when arbitrarily resetting the entity timestamp
  --    during the initial load.
  -- 2. This exercise is limited to entities for which historical records are maintained.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

	  DECLARE @minEntityTimestamp datetimeoffset(7)
	  DECLARE @minEntityToSiteTimestamp datetimeoffset(7)
	  DECLARE @minHistoricalTimestamp datetimeoffset(7)

	  SELECT @minEntityTimestamp = MIN(x.MinTimestamp)
	  FROM
	  (
		SELECT MIN(CreatedDate) MinTimestamp FROM staging.tblProducts
		UNION
		SELECT MIN(UpdatedDate) MinTimestamp FROM staging.tblProducts
		UNION
		SELECT MIN(CreatedDate) MinTimestamp FROM staging.tblEquipment
		UNION
		SELECT MIN(UpdatedDate) MinTimestamp FROM staging.tblEquipment
		UNION
		SELECT MIN(CreatedDate) MinTimestamp FROM staging.tblCompanies
		UNION
		SELECT MIN(UpdatedDate) MinTimestamp FROM staging.tblCompanies
		UNION
		SELECT MIN(CreatedDate) MinTimestamp FROM staging.tblProducts
		UNION
		SELECT MIN(UpdatedDate) MinTimestamp FROM staging.tblProducts
		UNION
		SELECT MIN(CreatedDate) MinTimestamp FROM staging.tblTransactionAliases
		UNION
		SELECT MIN(UpdatedDate) MinTimestamp FROM staging.tblTransactionAliases
	  ) x

	  -- For entities that are supported by Record Verioning, also factor in the entity-to-site mapping timestamp
	  SELECT @minEntityToSiteTimestamp = MIN(x.MinTimestamp)
	  FROM
	  (
		SELECT MIN(CreatedDate) MinTimestamp FROM staging.tblEntityProductToSite
		UNION
		SELECT MIN(UpdatedDate) MinTimestamp FROM staging.tblEntityProductToSite
		UNION
		SELECT MIN(CreatedDate) MinTimestamp FROM staging.tblEntityEquipmentToSite
		UNION
		SELECT MIN(UpdatedDate) MinTimestamp FROM staging.tblEntityEquipmentToSite
		UNION
		SELECT MIN(CreatedDate) MinTimestamp FROM staging.tblEntityCompanyToSite
		UNION
		SELECT MIN(UpdatedDate) MinTimestamp FROM staging.tblEntityCompanyToSite
		UNION
		SELECT MIN(CreatedDate) MinTimestamp FROM staging.tblEntityProductToSite
		UNION
		SELECT MIN(UpdatedDate) MinTimestamp FROM staging.tblEntityProductToSite
		UNION
		SELECT MIN(CreatedDate) MinTimestamp FROM staging.tblEntityTransactionAliasToSite
		UNION
		SELECT MIN(UpdatedDate) MinTimestamp FROM staging.tblEntityTransactionAliasToSite
	  ) x

	  SELECT @minHistoricalTimestamp = MIN(MinTimestamp) FROM
	  (
		SELECT @minEntityTimestamp MinTimestamp
		UNION
		SELECT @minEntityToSiteTimestamp MinTimestamp
	  )x 

	UPDATE staging.tblETLTempVariables
	SET VariableValue = CONVERT(VARCHAR(100), @minHistoricalTimestamp)
	WHERE VariableKey = 'MinHistoricalDateTime'

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
    + 'Procedure Name: [staging].[usp_GetEntityMinTimestamp]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END