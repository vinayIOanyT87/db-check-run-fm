/*
	DROP FUNCTION [staging].[udf_GetBaselineDate]

	SELECT [staging].[udf_GetBaselineDate]()
	
*/


CREATE FUNCTION [staging].[udf_GetBaselineDate]()
RETURNS datetimeoffset(7)
AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [dbo].[udf_GetBaselineDate]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Derives the baseline date, the earliest date applicable to the transactions and the entity mappings
	-- Notes:
	-- 1. This function assumes that the minimum TransactionDateTime value and the minHistoricalTimestamp value have already been captured 
	--    into the staging.tblETLTempVariables table.
	--------------
	DECLARE @baselineDate datetimeoffset(7)

	SELECT @baselineDate = MIN(x.MinTimestamp) FROM
		(
			SELECT CONVERT(DateTimeOffset(7), VariableValue) MinTimestamp FROM staging.tblETLTempVariables WHERE VariableKey = 'MinTransactionDateTime' 
			UNION
			SELECT CONVERT(DateTimeOffset(7), VariableValue) MinTimestamp FROM staging.tblETLTempVariables WHERE VariableKey = 'MinHistoricalDateTime' 
		) x
				   
	IF (@baselineDate IS NULL)
	BEGIN
		SET @baselineDate = DATEFROMPARTS(1900, 01, 01)
	END
	
	RETURN @baselineDate

END;