
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleSelectOwnerGroup] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.OwnerGroupToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.OwnerGroupGuid,
		ASSIGNED.*
	FROM 
		[map].[tblOwnerGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblApplicationString] ASSIGNED WITH (NOLOCK)
		ON MAIN.OwnerGroupGuid = ASSIGNED.ApplicationStringGuid
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END