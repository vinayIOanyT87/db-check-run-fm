
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/17/2012
-- Description:	Select all managers of a given rule
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleSelectManagers] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		*, 
		CompanyGuid as ManagerGuid,
		CompanyID as ID 
	FROM [dbo].[vw_AutoDistributionRuleManagers]
	WHERE AutoDistributionRuleGuid = @AutoDistributionRuleGuid
	ORDER BY CompanyID
END