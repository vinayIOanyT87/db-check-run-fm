CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableExternalComponentOutputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableExternalComponentOutputPermissiveList TABLE
(
	[ProcessVariableProductToPresetExternalComponentGuid] [uniqueidentifier]
	,[ProductToPresetExternalComponentGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableExternalComponentOutputPermissive_CTE ([ProcessVariableProductToPresetExternalComponentGuid],[ProductToPresetExternalComponentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableExternalComponentOutputPermissive].[ProcessVariableProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[ProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableExternalComponentOutputPermissive]
				INNER JOIN (SELECT [ProductToPresetExternalComponentGuid],[ProductGuid],[TankGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProductToPresetExternalComponentListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableExternalComponentOutputPermissive].[ProductToPresetExternalComponentGuid] = data1.[ProductToPresetExternalComponentGuid]
				LEFT JOIN [dbo].[tblProcessVariableExternalComponentOutputPermissive] B
					ON [dbo].[tblProcessVariableExternalComponentOutputPermissive].[ProcessVariableProductToPresetExternalComponentGuid] = B.[ProcessVariableProductToPresetExternalComponentGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableExternalComponentOutputPermissive].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableExternalComponentOutputPermissiveList SELECT [ProcessVariableProductToPresetExternalComponentGuid],[ProductToPresetExternalComponentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableExternalComponentOutputPermissive_CTE

	RETURN;
END

