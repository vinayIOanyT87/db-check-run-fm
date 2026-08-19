
/*
=============================================
Author: Ryan Hill
Create date: 02/01/13
Description:

Using the decimal place settings provided, determine which one we should use 
We use the first non-null non-zero value which appears in the following order:
The product's decimals
The transacion alias's decimals
The site's decimals
=============================================
*/
CREATE FUNCTION [dbo].[udf_GetDecimalPlaces]
(@ProductDecimalPlaces TINYINT, @TransactionAliasDecimalPlaces TINYINT, @SiteDecimalPlaces TINYINT)
RETURNS TINYINT
AS
BEGIN
	IF @ProductDecimalPlaces IS NOT NULL AND @ProductDecimalPlaces <> 0 RETURN @ProductDecimalPlaces
	IF @TransactionAliasDecimalPlaces IS NOT NULL AND @TransactionAliasDecimalPlaces <> 0 RETURN @TransactionAliasDecimalPlaces

	RETURN @SiteDecimalPlaces
END