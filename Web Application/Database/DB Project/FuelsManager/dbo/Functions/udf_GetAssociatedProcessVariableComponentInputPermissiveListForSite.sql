CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableComponentInputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableComponentInputPermissiveList TABLE
(
	[ProcessVariableProductToPresetComponentTankOrTankGroupGuid] [uniqueidentifier]
	,[ProductToPresetComponentTankOrTankGroupGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableComponentInputPermissive_CTE ([ProcessVariableProductToPresetComponentTankOrTankGroupGuid],[ProductToPresetComponentTankOrTankGroupGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableComponentInputPermissive].[ProcessVariableProductToPresetComponentTankOrTankGroupGuid],[dbo].[tblProcessVariableComponentInputPermissive].[ProductToPresetComponentTankOrTankGroupGuid],[dbo].[tblProcessVariableComponentInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableComponentInputPermissive].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableComponentInputPermissive]
				INNER JOIN (SELECT [ProductToPresetComponentTankOrTankGroupGuid],[ProductGuid],[TankGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProductToPresetComponentTankOrTankGroupListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableComponentInputPermissive].[ProductToPresetComponentTankOrTankGroupGuid] = data1.[ProductToPresetComponentTankOrTankGroupGuid]
				LEFT JOIN [dbo].[tblProcessVariableComponentInputPermissive] B
					ON [dbo].[tblProcessVariableComponentInputPermissive].[ProcessVariableProductToPresetComponentTankOrTankGroupGuid] = B.[ProcessVariableProductToPresetComponentTankOrTankGroupGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableComponentInputPermissive].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableComponentInputPermissiveList SELECT [ProcessVariableProductToPresetComponentTankOrTankGroupGuid],[ProductToPresetComponentTankOrTankGroupGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableComponentInputPermissive_CTE

	RETURN;
END

