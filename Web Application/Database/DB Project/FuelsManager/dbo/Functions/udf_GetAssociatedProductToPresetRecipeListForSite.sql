CREATE FUNCTION [dbo].[udf_GetAssociatedProductToPresetRecipeListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProductToPresetRecipeList TABLE
(
	[ProductToPresetRecipeGuid] [uniqueidentifier]
	,[ProductGuid] [uniqueidentifier]
	,[AssignedToLoadArmGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	-- ProductGuid and AssignedToLoadArmGuidother are required field so we will INNER JOIN these columns.
	-- TankGuid, TankGroupApplicationStringGuid and AssignedToMeterGuid MAY OR MAY NOT BE POPULATED.  
	-- IF THEY ARE, they must point to a valid record that is assigned to the current site guid, otherwise they shouldn't participate in the query (join)
	-- IF any of these guids point to an entity that is NOT assigned to the current site, this product mapping entry should be filtered out since the foreign key record would not
	-- be synchronized to the remote node.  This would generate a missing foreign key constraint error.  There's no point in retrieving records that can't be inserted.
	--
	; WITH ProductToPresetRecipe_CTE ([ProductToPresetRecipeGuid],[ProductGuid],[AssignedToLoadArmGuid],[OwnerSiteGuid])
	AS (
		SELECT [map].[tblProductToPresetRecipe].[ProductToPresetRecipeGuid]
				,[map].[tblProductToPresetRecipe].[ProductGuid]
				,[map].[tblProductToPresetRecipe].[AssignedToLoadArmGuid]
				,data1.[OwnerSiteGuid]
			FROM [map].[tblProductToPresetRecipe]
				INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data1
					ON [map].[tblProductToPresetRecipe].[ProductGuid] = data1.[ProductGuid]
	)
	INSERT INTO @tblProductToPresetRecipeList SELECT [ProductToPresetRecipeGuid],[ProductGuid],[AssignedToLoadArmGuid],[OwnerSiteGuid] FROM ProductToPresetRecipe_CTE

	RETURN;
END

