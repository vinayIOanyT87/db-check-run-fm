CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableRecipeInputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableRecipeInputPermissiveList TABLE
(
	[ProcessVariableProductToPresetRecipeGuid] [uniqueidentifier]
	,[ProductToPresetRecipeGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableRecipeInputPermissive_CTE ([ProcessVariableProductToPresetRecipeGuid],[ProductToPresetRecipeGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableRecipeInputPermissive].[ProcessVariableProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeInputPermissive].[ProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableRecipeInputPermissive].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableRecipeInputPermissive]
				INNER JOIN (SELECT [ProductToPresetRecipeGuid],[ProductGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProductToPresetRecipeListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableRecipeInputPermissive].[ProductToPresetRecipeGuid] = data1.[ProductToPresetRecipeGuid]
				LEFT JOIN [dbo].[tblProcessVariableRecipeInputPermissive] B
					ON [dbo].[tblProcessVariableRecipeInputPermissive].[ProcessVariableProductToPresetRecipeGuid] = B.[ProcessVariableProductToPresetRecipeGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableRecipeInputPermissive].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableRecipeInputPermissiveList SELECT [ProcessVariableProductToPresetRecipeGuid],[ProductToPresetRecipeGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableRecipeInputPermissive_CTE

	RETURN;
END

