CREATE FUNCTION [dbo].[udf_GetAssociatedProcessVariableEquipmentListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProcessVariableEquipmentList TABLE
(
	[ProcessVariableEquipmentGuid] [uniqueidentifier]
	,[EquipmentGuid] [uniqueidentifier]
	,[OPCConnectionGuid] [uniqueidentifier]
	,[MessageApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	; WITH ProcessVariableEquipment_CTE ([ProcessVariableEquipmentGuid],[EquipmentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid])
	AS
	(
		SELECT [dbo].[tblProcessVariableEquipment].[ProcessVariableEquipmentGuid],[dbo].[tblProcessVariableEquipment].[EquipmentGuid],[dbo].[tblProcessVariableEquipment].[OPCConnectionGuid],[dbo].[tblProcessVariableEquipment].[MessageApplicationStringGuid],data1.[OwnerSiteGuid]
			FROM [dbo].[tblProcessVariableEquipment]
				INNER JOIN (SELECT [EquipmentToSiteGuid],[EquipmentGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedEquipmentListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblProcessVariableEquipment].[EquipmentGuid] = data1.[EquipmentGuid]
				LEFT JOIN [dbo].[tblProcessVariableEquipment] B
					ON [dbo].[tblProcessVariableEquipment].[ProcessVariableEquipmentGuid] = B.[ProcessVariableEquipmentGuid]
				LEFT JOIN (SELECT [ProcessVariableMessageToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](@sync_context_site_guid)) data2
					ON B.[MessageApplicationStringGuid] = data2.[ApplicationStringGuid]
			WHERE ([dbo].[tblProcessVariableEquipment].[MessageApplicationStringGuid] IS NULL OR data2.[ApplicationStringGuid] IS NOT NULL)
	)
	INSERT INTO @tblProcessVariableEquipmentList SELECT [ProcessVariableEquipmentGuid],[EquipmentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM ProcessVariableEquipment_CTE

	RETURN;
END