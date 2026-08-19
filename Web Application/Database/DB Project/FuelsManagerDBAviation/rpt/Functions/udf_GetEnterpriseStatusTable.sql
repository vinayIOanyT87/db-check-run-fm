CREATE FUNCTION [rpt].[udf_GetEnterpriseStatusTable]
(	
	@EnterpriseStatus BIT
)
RETURNS @Statuses TABLE 
(
	[TransactionStatusIndex] INT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[udf_GetEnterpriseStatusTable]
	-- Author:Gregory Lybanon
	-- Version/Date: 1.0.000 / 2018-12-21 
	-- Purpose: Gets the list of transactions statuses to show 
	-- Notes:
	-- 1. @EnterpriseStatus - Boolean determining of only Closed (4) and Enterprise (19) statuses should be returned
	------------------------------------------------------------------------------------------------------

	IF (@EnterpriseStatus = 1)
	BEGIN
		INSERT INTO @Statuses 
		([TransactionStatusIndex])
		(SELECT TransactionStatusIndex FROM lookup.tblTransactionStatus WHERE TransactionStatusName in ('Closed','Enterprise'))
	END
	ELSE
	BEGIN
		INSERT INTO @Statuses 
		([TransactionStatusIndex])
		(SELECT TransactionStatusIndex FROM lookup.tblTransactionStatus)
	END

	RETURN
END
GO