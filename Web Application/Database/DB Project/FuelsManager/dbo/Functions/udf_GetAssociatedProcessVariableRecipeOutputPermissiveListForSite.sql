CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableRecipeOutputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableRecipeOutputPermissiveList TABLE
(
	[ProcessVariableProductToPresetRecipeGuid] [uniqueidentifier]
	,[ProductToPresetRecipeGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableRecipeOutputPermissive_CTE ([ProcessVariableProductToPresetRecipeGuid],[ProductToPresetRecipeGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableRecipeOutputPermissive].[ProcessVariableProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[ProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableRecipeOutputPermissive]
				INNER JOIN (SELECT [ProductToPresetRecipeGuid],[ProductGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProductToPresetRecipeListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableRecipeOutputPermissive].[ProductToPresetRecipeGuid] = data1.[ProductToPresetRecipeGuid]
				LEFT JOIN [dbo].[tblProcessVariableRecipeOutputPermissive] B
					ON [dbo].[tblProcessVariableRecipeOutputPermissive].[ProcessVariableProductToPresetRecipeGuid] = B.[ProcessVariableProductToPresetRecipeGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableRecipeOutputPermissive].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableRecipeOutputPermissiveList SELECT [ProcessVariableProductToPresetRecipeGuid],[ProductToPresetRecipeGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableRecipeOutputPermissive_CTE

	RETURN;
END

