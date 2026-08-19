CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableAdditiveInputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableAdditiveInputPermissiveList TABLE
(
	[ProcessVariableProductToPresetInjectorGuid] [uniqueidentifier]
	,[ProductToPresetInjectorGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableAdditiveInputPermissive_CTE ([ProcessVariableProductToPresetInjectorGuid],[ProductToPresetInjectorGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableAdditiveInputPermissive].[ProcessVariableProductToPresetInjectorGuid],[dbo].[tblProcessVariableAdditiveInputPermissive].[ProductToPresetInjectorGuid],[dbo].[tblProcessVariableAdditiveInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableAdditiveInputPermissive].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableAdditiveInputPermissive]
				INNER JOIN (SELECT [ProductToPresetInjectorGuid],[ProductGuid],[TankGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProductToPresetInjectorListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableAdditiveInputPermissive].[ProductToPresetInjectorGuid] = data1.[ProductToPresetInjectorGuid]
				LEFT JOIN [dbo].[tblProcessVariableAdditiveInputPermissive] B
					ON [dbo].[tblProcessVariableAdditiveInputPermissive].[ProcessVariableProductToPresetInjectorGuid] = B.[ProcessVariableProductToPresetInjectorGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableAdditiveInputPermissive].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableAdditiveInputPermissiveList SELECT [ProcessVariableProductToPresetInjectorGuid],[ProductToPresetInjectorGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableAdditiveInputPermissive_CTE

	RETURN;
END

