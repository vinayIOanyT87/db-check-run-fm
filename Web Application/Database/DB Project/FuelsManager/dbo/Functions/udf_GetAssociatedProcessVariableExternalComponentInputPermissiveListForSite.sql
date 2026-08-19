CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableExternalComponentInputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableExternalComponentInputPermissiveList TABLE
(
	[ProcessVariableProductToPresetExternalComponentGuid] [uniqueidentifier]
	,[ProductToPresetExternalComponentGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableExternalComponentInputPermissive_CTE ([ProcessVariableProductToPresetExternalComponentGuid],[ProductToPresetExternalComponentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableExternalComponentInputPermissive].[ProcessVariableProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[ProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableExternalComponentInputPermissive]
				INNER JOIN (SELECT [ProductToPresetExternalComponentGuid],[ProductGuid],[TankGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProductToPresetExternalComponentListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableExternalComponentInputPermissive].[ProductToPresetExternalComponentGuid] = data1.[ProductToPresetExternalComponentGuid]
				LEFT JOIN [dbo].[tblProcessVariableExternalComponentInputPermissive] B
					ON [dbo].[tblProcessVariableExternalComponentInputPermissive].[ProcessVariableProductToPresetExternalComponentGuid] = B.[ProcessVariableProductToPresetExternalComponentGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableExternalComponentInputPermissive].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableExternalComponentInputPermissiveList SELECT [ProcessVariableProductToPresetExternalComponentGuid],[ProductToPresetExternalComponentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableExternalComponentInputPermissive_CTE

	RETURN;
END

