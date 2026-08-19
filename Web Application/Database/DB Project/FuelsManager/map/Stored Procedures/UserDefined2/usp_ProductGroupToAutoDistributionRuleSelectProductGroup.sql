
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleSelectProductGroup] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.ProductGroupToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.ProductGroupGuid,
		ASSIGNED.*
	FROM 
		[map].[tblProductGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblApplicationString] ASSIGNED WITH (NOLOCK)
		ON MAIN.ProductGroupGuid = ASSIGNED.ApplicationStringGuid
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END