/*
	DROP FUNCTION [fmcdc].[udf_GetTransactionBaseCount]

	SELECT [fmcdc].[udf_GetTransactionBaseCount] ()

*/
CREATE FUNCTION [fmcdc].[udf_GetTransactionBaseCount] ()
RETURNS int
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [fmcdc].[udf_GetTransactionBaseCount]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Get a base record count to be used for fmcdc extraction batching, using the highest record count
	--			from the main transaction tables.
	
	------------------------------------------------------------------------------------------------------
	DECLARE @result int
	SET @result = 0
	
		SELECT @result = MAX(x.n)
		FROM 
		( 
			SELECT COUNT(*) n FROM fmcdc.tblTransactions
			UNION
			SELECT COUNT(*) n FROM fmcdc.tblTransactionLineItems
			UNION
			SELECT COUNT(*) n FROM fmcdc.tblTransactionSubLineItems
			UNION
			SELECT COUNT(*) n FROM fmcdc.tblTransactionUserData
			UNION
			SELECT COUNT(*) n FROM fmcdc.tblTransactionLineItemUserData
		) x

	SET @result = ISNULL(@result, 0);
	RETURN @result;
END
GO