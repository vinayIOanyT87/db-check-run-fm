

CREATE FUNCTION [map].[udf_CheckUniquenessAutoDistributionRule]
(@AutoDistributionRuleGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @RuleID nvarchar(50)
			, @Exists bit
	SET @Exists = 1

	SET @RuleID = (SELECT RuleID FROM tblAutoDistributionRule e WHERE e.AutoDistributionRuleGuid = @AutoDistributionRuleGuid)
	IF 0 < (SELECT COUNT(*) FROM tblAutoDistributionRule e 
	RIGHT JOIN map.tblEntityAutoDistributionRuleToSite em ON em.SiteGuid = @SiteGuid AND em.AutoDistributionRuleGuid = e.AutoDistributionRuleGuid 
	WHERE e.AutoDistributionRuleGuid <> @AutoDistributionRuleGuid
	AND e.RuleID = @RuleID)
		SET @Exists = 0

	RETURN @Exists
END
