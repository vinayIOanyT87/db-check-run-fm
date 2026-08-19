
/*
=============================================
Author: Ryan Hill
Create date: 02/01/13
Description:

Using the volume unit types provided, determine which one we should use 

Volume is a special case because there are multiple volume settings on the site and transaction alias
One is for additive products, one is for everything else.

We get the first non-null non-zero setting in the following order:
The product's setting
The transaction alias's additive volume setting if the product is an additive, or the alias's regular volume setting otherwise
The site's additive volume setting if the product is an additive, or the site's regular volume setting otherwise
=============================================
*/
CREATE FUNCTION [dbo].[udf_GetVolumeUnitsIndex]
(@LookupProductTypeIndex INT, @ProductUnitIndex INT, @TransactionAliasUnitIndex INT, @SiteUnitIndex INT, @TransactionAliasAdditiveUnitIndex INT, @SiteAdditiveUnitIndex INT)
RETURNS TINYINT
AS
BEGIN
	DECLARE @VolumeUnits TINYINT

	IF @LookupProductTypeIndex = 2 --Is the product an additive?
	BEGIN
		SET @VolumeUnits = dbo.udf_GetDecimalPlaces(@ProductUnitIndex, @TransactionAliasAdditiveUnitIndex, @SiteAdditiveUnitIndex)		
	END
	ELSE 
	BEGIN
		SET @VolumeUnits = dbo.udf_GetDecimalPlaces(@ProductUnitIndex, @TransactionAliasUnitIndex, @SiteUnitIndex)
	END

	RETURN @VolumeUnits
END