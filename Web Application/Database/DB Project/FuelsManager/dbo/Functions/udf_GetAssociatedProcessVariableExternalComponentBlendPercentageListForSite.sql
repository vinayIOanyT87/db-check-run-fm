CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableExternalComponentBlendPercentageListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableExternalComponentBlendPercentageList TABLE
(
	[ProcessVariableProductToPresetExternalComponentGuid] [uniqueidentifier]
	,[ProductToPresetExternalComponentGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableExternalComponentBlendPercentage_CTE ([ProcessVariableProductToPresetExternalComponentGuid],[ProductToPresetExternalComponentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProcessVariableProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableExternalComponentBlendPercentage]
				INNER JOIN (SELECT [ProductToPresetExternalComponentGuid],[ProductGuid],[TankGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProductToPresetExternalComponentListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProductToPresetExternalComponentGuid] = data1.[ProductToPresetExternalComponentGuid]
				LEFT JOIN [dbo].[tblProcessVariableExternalComponentBlendPercentage] B
					ON [dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProcessVariableProductToPresetExternalComponentGuid] = B.[ProcessVariableProductToPresetExternalComponentGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableExternalComponentBlendPercentage].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableExternalComponentBlendPercentageList SELECT [ProcessVariableProductToPresetExternalComponentGuid],[ProductToPresetExternalComponentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableExternalComponentBlendPercentage_CTE

	RETURN;
END

