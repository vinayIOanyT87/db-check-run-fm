CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariablePresetInjectorListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariablePresetInjectorList TABLE
(
	[ProcessVariablePresetInjectorGuid] [uniqueidentifier]
	,[ProductToPresetInjectorGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariablePresetInjector_CTE ([ProcessVariablePresetInjectorGuid],[ProductToPresetInjectorGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariablePresetInjector].[ProcessVariablePresetInjectorGuid],[dbo].[tblProcessVariablePresetInjector].[ProductToPresetInjectorGuid],[dbo].[tblProcessVariablePresetInjector].[OPCConnectionGuid],[dbo].[tblProcessVariablePresetInjector].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariablePresetInjector]
				INNER JOIN (SELECT [ProductToPresetInjectorGuid],[ProductGuid],[TankGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProductToPresetInjectorListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariablePresetInjector].[ProductToPresetInjectorGuid] = data1.[ProductToPresetInjectorGuid]
				LEFT JOIN [dbo].[tblProcessVariablePresetInjector] B
					ON [dbo].[tblProcessVariablePresetInjector].[ProcessVariablePresetInjectorGuid] = B.[ProcessVariablePresetInjectorGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariablePresetInjector].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariablePresetInjectorList SELECT [ProcessVariablePresetInjectorGuid],[ProductToPresetInjectorGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariablePresetInjector_CTE

	RETURN;
END

