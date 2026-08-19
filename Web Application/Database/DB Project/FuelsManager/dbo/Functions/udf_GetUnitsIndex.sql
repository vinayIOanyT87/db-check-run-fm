
/*
=============================================
Author: Ryan Hill
Create date: 02/01/13
Description:

Using the unit types provided, determine which one we should use 
We use the first non-null non-zero value which appears in the following order:
The product's units
The transacion alias's units
The site's units

Examples of unit types are gallons or pounds.
=============================================
*/
CREATE FUNCTION [dbo].[udf_GetUnitsIndex]
(@ProductUnitIndex INT, @TransactionAliasUnitIndex INT, @SiteUnitIndex INT)
RETURNS INT
AS
BEGIN
	IF @ProductUnitIndex IS NOT NULL AND @ProductUnitIndex <> 0 RETURN @ProductUnitIndex
	IF @TransactionAliasUnitIndex IS NOT NULL AND @TransactionAliasUnitIndex <> 0 RETURN @TransactionAliasUnitIndex

	RETURN @SiteUnitIndex
END