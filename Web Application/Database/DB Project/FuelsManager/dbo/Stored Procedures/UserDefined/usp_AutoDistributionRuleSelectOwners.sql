

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/17/2012
-- Description:	Select all owners of a given rule
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleSelectOwners] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	SELECT 
		*, 
		CompanyGuid as OwnerGuid,
		CompanyID as ID 
	FROM [dbo].[vw_AutoDistributionRuleOwners]
	WHERE AutoDistributionRuleGuid = @AutoDistributionRuleGuid
	ORDER BY CompanyID
END