
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleSelectManagerGroup] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.ManagerGroupToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.ManagerGroupGuid,
		ASSIGNED.*
	FROM 
		[map].[tblManagerGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblApplicationString] ASSIGNED WITH (NOLOCK)
		ON MAIN.ManagerGroupGuid = ASSIGNED.ApplicationStringGuid
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END