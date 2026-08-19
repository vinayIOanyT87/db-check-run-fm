
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/17/2012
-- Description:	Select all products of a given rule
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleSelectProducts] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	SELECT * FROM [dbo].[vw_AutoDistributionRuleProducts]
	WHERE AutoDistributionRuleGuid = @AutoDistributionRuleGuid
	ORDER BY ProductID
END