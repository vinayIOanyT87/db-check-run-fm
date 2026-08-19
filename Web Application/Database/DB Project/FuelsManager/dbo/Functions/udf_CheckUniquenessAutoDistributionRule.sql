

CREATE FUNCTION [dbo].[udf_CheckUniquenessAutoDistributionRule]
(@AutoDistributionRuleGuid uniqueidentifier, @SiteGuid uniqueidentifier, @RuleID nvarchar(50))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblAutoDistributionRule
	IF 0 < (SELECT COUNT(*) FROM tblAutoDistributionRule e
	LEFT JOIN map.tblEntityAutoDistributionRuleToSite em1 ON em1.AutoDistributionRuleGuid = e.AutoDistributionRuleGuid
	RIGHT JOIN map.tblEntityAutoDistributionRuleToSite em2 ON em2.AutoDistributionRuleGuid = @AutoDistributionRuleGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.AutoDistributionRuleGuid <> @AutoDistributionRuleGuid
	AND RuleID = @RuleID)
		SET @Exists = 0

	RETURN @Exists
END
