CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableAdditiveOutputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableAdditiveOutputPermissiveList TABLE
(
	[ProcessVariableProductToPresetInjectorGuid] [uniqueidentifier]
	,[ProductToPresetInjectorGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableAdditiveOutputPermissive_CTE ([ProcessVariableProductToPresetInjectorGuid],[ProductToPresetInjectorGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableAdditiveOutputPermissive].[ProcessVariableProductToPresetInjectorGuid],[dbo].[tblProcessVariableAdditiveOutputPermissive].[ProductToPresetInjectorGuid],[dbo].[tblProcessVariableAdditiveOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableAdditiveOutputPermissive].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableAdditiveOutputPermissive]
				INNER JOIN (SELECT [ProductToPresetInjectorGuid],[ProductGuid],[TankGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProductToPresetInjectorListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableAdditiveOutputPermissive].[ProductToPresetInjectorGuid] = data1.[ProductToPresetInjectorGuid]
				LEFT JOIN [dbo].[tblProcessVariableAdditiveOutputPermissive] B
					ON [dbo].[tblProcessVariableAdditiveOutputPermissive].[ProcessVariableProductToPresetInjectorGuid] = B.[ProcessVariableProductToPresetInjectorGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableAdditiveOutputPermissive].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableAdditiveOutputPermissiveList SELECT [ProcessVariableProductToPresetInjectorGuid],[ProductToPresetInjectorGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableAdditiveOutputPermissive_CTE

	RETURN;
END

