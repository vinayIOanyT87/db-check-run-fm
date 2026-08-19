/*

	DROP FUNCTION [fmcdc].[udf_GetTransactionRecordCount]

	SELECT [fmcdc].[udf_GetTransactionRecordCount]('1/1/2021', 0)

*/
CREATE FUNCTION [fmcdc].[udf_GetTransactionRecordCount]
(@cutoffdate DateTime, @extractByInventoryDate bit)
RETURNS INT
AS
BEGIN 
	DECLARE @count int
	SELECT @count = COUNT(*) FROM dbo.tblTransactions a
	WHERE ((@extractByInventoryDate = 0) AND a.UpdatedDate >= @cutoffdate)
	OR ((@extractByInventoryDate = 1) AND cast(a.InventoryDate as datetime) >= @cutoffdate)

	RETURN @count
END