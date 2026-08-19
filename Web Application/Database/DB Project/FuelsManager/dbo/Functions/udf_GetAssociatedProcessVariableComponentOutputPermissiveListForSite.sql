CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableComponentOutputPermissiveListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableComponentOutputPermissiveList TABLE
(
	[ProcessVariableProductToPresetComponentTankOrTankGroupGuid] [uniqueidentifier]
	,[ProductToPresetComponentTankOrTankGroupGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableComponentOutputPermissive_CTE ([ProcessVariableProductToPresetComponentTankOrTankGroupGuid],[ProductToPresetComponentTankOrTankGroupGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableComponentOutputPermissive].[ProcessVariableProductToPresetComponentTankOrTankGroupGuid],[dbo].[tblProcessVariableComponentOutputPermissive].[ProductToPresetComponentTankOrTankGroupGuid],[dbo].[tblProcessVariableComponentOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableComponentOutputPermissive].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableComponentOutputPermissive]
				INNER JOIN (SELECT [ProductToPresetComponentTankOrTankGroupGuid],[ProductGuid],[TankGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProductToPresetComponentTankOrTankGroupListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableComponentOutputPermissive].[ProductToPresetComponentTankOrTankGroupGuid] = data1.[ProductToPresetComponentTankOrTankGroupGuid]
				LEFT JOIN [dbo].[tblProcessVariableComponentOutputPermissive] B
					ON [dbo].[tblProcessVariableComponentOutputPermissive].[ProcessVariableProductToPresetComponentTankOrTankGroupGuid] = B.[ProcessVariableProductToPresetComponentTankOrTankGroupGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableComponentOutputPermissive].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableComponentOutputPermissiveList SELECT [ProcessVariableProductToPresetComponentTankOrTankGroupGuid],[ProductToPresetComponentTankOrTankGroupGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableComponentOutputPermissive_CTE

	RETURN;
END

